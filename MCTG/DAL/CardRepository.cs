using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Classes;
using MTCG.Classes.CardStructure;
using MTCG.Interfaces;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace MTCG.DAL
{
    public class CardRepository //: IRepository<Card>
    {

        CardRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        private NpgsqlTransaction? _transaction;
        public void SetTransaction(NpgsqlTransaction transaction)
        {
            _transaction = transaction;
        }

        private static CardRepository _instance;

        private readonly string _connectionString;


        private static List<Card> _cards = new List<Card>
        {

        };

        public static CardRepository Instance
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

            var repo = new CardRepository(connectionString);
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
                                        CREATE TABLE IF NOT EXISTS Cards (
                                            id SERIAL PRIMARY KEY,
                                            name VARCHAR(500) NOT NULL,
                                            deck_id INT,
                                            damage INT NOT NULL,
                                            elementtype VARCHAR(20) NOT NULL,
                                            monstertype VARCHAR(20),
                                            ownerid INT NOT NULL,
                                            locked boolean DEFAULT FALSE,
                                            FOREIGN KEY (ownerid) REFERENCES users(id) ON DELETE CASCADE,
                                            FOREIGN KEY (deck_id) REFERENCES decks(id) ON DELETE CASCADE
                                        );
                                        ";



                    cmd.ExecuteNonQuery();
                }
            }

            _instance = repo;
        }


        public Card getCard(int cardID, int ownerID)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText =
                        @"SELECT name, elementtype, monstertype, damage FROM Cards WHERE ownerid = @ownerID and id = @cardID;";
                    command.Parameters.Add(new NpgsqlParameter("@ownerID", ownerID));
                    command.Parameters.Add(new NpgsqlParameter("@cardID", cardID));

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Card c;
                            string name = reader.GetString(0);
                            bool elementTypeSuccess = Enum.TryParse(reader.GetString(1), out ElementTypes elementTyp);
                            if (!elementTypeSuccess)
                            {
                                throw new Exception("Error while creating card");

                            }

                            int damage = reader.GetInt32(3);

                            if (!reader.IsDBNull(2))
                            {
                                bool monsterTypeSuccess = Enum.TryParse(reader.GetString(2), out MonsterTypes monsterType);
                                if (!monsterTypeSuccess)
                                {
                                    throw new Exception("Error while creating card");
                                }
                                c = new MonsterCards(name, damage, elementTyp, monsterType, cardID,ownerID);
                            }
                            else
                            {
                                c = new SpellCards(name, damage, elementTyp, cardID,ownerID);
                            }

                            if (c == null)
                            {
                                throw new Exception("Error while getting card");
                            }
                            return c;
                        }
                    }
                }
            }

            throw new Exception("Card not found");
        }
        




        public IEnumerable<Card> GetAllCardsFromUserID(int id)
        {
            List<Card> result = new List<Card>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText =
                        @"SELECT id, name, elementtype, monstertype, damage FROM Cards WHERE ownerid = @id;";
                    command.Parameters.Add(new NpgsqlParameter("@id", id));

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Card c;
                            string name = reader.GetString(1);
                            bool elementTypeSuccess = Enum.TryParse(reader.GetString(2), out ElementTypes elementTyp);
                            if (!elementTypeSuccess)
                            {
                                return null;
                            }

                            int cardID = reader.GetInt32(0);
                            int damage = reader.GetInt32(4);

                            if (!reader.IsDBNull(3))
                            {
                                bool monsterTypeSuccess = Enum.TryParse(reader.GetString(3), out MonsterTypes monsterType);
                                if (!monsterTypeSuccess)
                                {
                                    return null;
                                }
                                c = new MonsterCards(name, damage, elementTyp, monsterType,cardID,id);
                            }
                            else
                            {
                                c = new SpellCards(name, damage, elementTyp,cardID,id);
                            }
                            result.Add(c);
                        }
                    }
                }
            }

            return result;
        }


        public Card Get(int id)
        {
            throw new NotImplementedException();
        }



        public bool Add(Card t,string token)
        {

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand("INSERT INTO Cards (name, damage, elementtype, monstertype, ownerid) VALUES (@name, @damage, @elementtype, @monstertype, @ownerid)", connection))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("@name", t.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@damage", t.Damage));
                        cmd.Parameters.Add(new NpgsqlParameter("@elementtype", t.ElementTyp.ToString()));
                        int? id = UserRepository.Instance.getUserIDFromToken(token);
                        if (id == null)
                        {
                            return false;
                        }
                        cmd.Parameters.Add(new NpgsqlParameter("@ownerid", id.Value));
                        if (t is MonsterCards monstercard)
                        {
                            cmd.Parameters.Add(new NpgsqlParameter("@monstertype", monstercard.MonsterType.ToString()));
                        }
                        else
                        {
                            cmd.Parameters.Add(new NpgsqlParameter("@monstertype",(object)DBNull.Value));

                        }

                         cmd.ExecuteNonQuery();
                        return true;
                    }
                }

            return false;
        }

        public bool LockCard(int cardID, bool cardLock)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("UPDATE Cards SET locked=@cardLock WHERE id = @cardid", connection))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@cardid", cardID));
                    cmd.Parameters.Add(new NpgsqlParameter("@cardLock", cardLock));
                    cmd.Transaction = _transaction;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        public bool TransferCard(int cardId, int newOwnerId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                //                using (var cmd = new NpgsqlCommand("UPDATE Cards (name, damage, elementtype, monstertype, ownerid) VALUES (@name, @damage, @elementtype, @monstertype, @ownerid)", connection))

                using (var cmd = new NpgsqlCommand("UPDATE Cards SET ownerid=@ownerid WHERE id = @cardid", connection))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@ownerid", newOwnerId));
                    cmd.Parameters.Add(new NpgsqlParameter("@cardid", cardId));
                    cmd.Transaction = _transaction;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        public bool Delete(Card t)
        {
            throw new NotImplementedException();
        }

    }
}
