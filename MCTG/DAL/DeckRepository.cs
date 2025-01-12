using MTCG.Classes;
using MTCG.Classes.CardStructure;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL
{
    public class DeckRepository
    {
        DeckRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        private static DeckRepository _instance;

        private readonly string _connectionString;
        private NpgsqlTransaction? _transaction;
        public void SetTransaction(NpgsqlTransaction transaction)
        {
            _transaction = transaction;
        }
        private static List<Card> _cards = new List<Card>
        {

        };




        public static DeckRepository Instance
        {
            get
            {
                if (_instance is null)
                {
                    throw new InvalidOperationException("Database not initialized. Call InitDb first.");
                }

                return _instance;

            }
        }

        public static void InitDb(string connectionString)
        {

            var repo = new DeckRepository(connectionString);
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            string dbName = builder.Database;

            builder.Remove("Database");
            string cs = builder.ToString();

            using (IDbConnection connection = new NpgsqlConnection(cs))
            {
                connection.Open();

                connection.ChangeDatabase(dbName);


                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                                        CREATE TABLE IF NOT EXISTS decks (
                                            id SERIAL PRIMARY KEY,         
                                            user_id INT NOT NULL,               
                                            created_at TIMESTAMP DEFAULT NOW(), 
                                            FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
                                        );
                                        ";


                    cmd.ExecuteNonQuery();
                }
            }

            _instance = repo;
        }

        private int EnsureDeckExists(int id)
        {
            int deckID;
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT id FROM decks WHERE user_id = @id LIMIT 1;";
                    command.Parameters.AddWithValue("id", id);
                    using (NpgsqlDataReader reader = command.ExecuteReader()){
                        if (reader.Read())
                        {
                            deckID = reader.GetInt32(0);
                            return deckID;

                        }
                    }


                }

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO decks (user_id) VALUES (@id) RETURNING id";
                    command.Parameters.AddWithValue("id", id);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }


        public bool AddCardToDeck(int userID,int cardID)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                NpgsqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    UpdateCardDeck(connection, cardID, userID);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

            }

            return false;
        }


        public void updateCardOwner(NpgsqlConnection? connection, int cardId,int userID)
        {
            if (connection == null)
            {
                connection = new NpgsqlConnection(_connectionString);
                connection.Open();
            }

            int deckId = EnsureDeckExists(userID);
            var updateCardQuery = @"UPDATE cards 
                            SET deck_id = @DeckId 
                            WHERE id = @CardId";

            using (var command = new NpgsqlCommand(updateCardQuery, connection))
            {
                command.Parameters.AddWithValue("DeckId", deckId);
                command.Parameters.AddWithValue("CardId", cardId);
                command.ExecuteNonQuery();

            }
        }

        public void ClearDeck(int userID)
        {

                NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
                connection.Open();
            
            

            int deckId = EnsureDeckExists(userID);
            var updateCardQuery = @"UPDATE cards 
                            SET deck_id = @DeckId 
                            WHERE ownerid = @userID;";

            using (var command = new NpgsqlCommand(updateCardQuery, connection))
            {
                command.Parameters.AddWithValue("DeckId", DBNull.Value);
                command.Parameters.AddWithValue("userID", userID);
                command.ExecuteNonQuery();

            }
        }


        public void UpdateCardDeck(NpgsqlConnection? connection, int cardId, int userId)
        {
            if (connection == null)
            {
                connection = new NpgsqlConnection(_connectionString);
                connection.Open();
            }

            int deckId = EnsureDeckExists(userId);
            int CardAmount = getDeck(userId).Count;
            if (CardAmount == 4)
            {
                throw new Exception("You have already configured a deck. Please clear your deck");
            }
            else if(CardAmount > 4)
            {
                throw new Exception("You have more than 4 cards in your Deck. Please clear your deck");

            }
            var updateCardQuery = @"UPDATE cards 
                            SET deck_id = @DeckId 
                            WHERE id = @CardId AND ownerid = @userID;";

            using (var command = new NpgsqlCommand(updateCardQuery, connection))
            {
                command.Parameters.AddWithValue("DeckId", deckId);
                command.Parameters.AddWithValue("CardId", cardId);
                command.Parameters.AddWithValue("userID", userId);
                command.ExecuteNonQuery();

            }
        }


        public List<Card> getDeck(int id)
        {
            List<Card> result = new List<Card>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText =
                        @"SELECT c.id, c.name, c.damage, c.elementtype, c.monstertype, c.ownerid
                        FROM cards c
                        JOIN decks d ON c.deck_id = d.id
                        JOIN users u ON d.user_id = u.id
                        WHERE u.id = @id";
                    command.Parameters.Add(new NpgsqlParameter("@id", id));

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Card c;
                            string name = reader.GetString(1);
                            bool elementTypeSuccess = Enum.TryParse(reader.GetString(3), out ElementTypes elementTyp);
                            if (!elementTypeSuccess)
                            {
                                return null;
                            }

                            int cardID = reader.GetInt32(0);
                            int damage = reader.GetInt32(2);
                            int ownerID = reader.GetInt32(5);

                            if (!reader.IsDBNull(4))
                            {
                                bool monsterTypeSuccess = Enum.TryParse(reader.GetString(4), out MonsterTypes monsterType);
                                if (!monsterTypeSuccess)
                                {
                                    return null;
                                }
                                c = new MonsterCards(name, damage, elementTyp, monsterType,cardID, ownerID);
                            }
                            else
                            {
                                c = new SpellCards(name, damage, elementTyp, cardID, ownerID);
                            }
                            result.Add(c);
                        }
                    }
                }
            }

            return result;
        }

    }
}
