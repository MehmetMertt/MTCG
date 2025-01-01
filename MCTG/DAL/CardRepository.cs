using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Classes;
using MTCG.Classes.CardStructure;
using MTCG.Interfaces;
using Npgsql;

namespace MTCG.DAL
{
    internal class CardRepository //: IRepository<Card>
    {

        CardRepository(string connectionString)
        {
            this._connectionString = connectionString;
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
                    deck_id INT NOT NULL,
                    damage INT NOT NULL,
                    elementtype VARCHAR(20) NOT NULL,
                    monstertype VARCHAR(20),
                    ownerid INT NOT NULL,
                    FOREIGN KEY (ownerid) REFERENCES Users(id) ON DELETE CASCADE
                    FOREIGN KEY (deck_id) REFERENCES decks (deck_id) ON DELETE CASCADE
                )
            ";

                    cmd.ExecuteNonQuery();
                }
            }

            _instance = repo;
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
                        @"SELECT id, name, elementtype, monstertype, ownerid, damage FROM Cards WHERE ownerid = @id;";
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

                            int ownerID = reader.GetInt32(4);
                            int damage = reader.GetInt32(5);

                            if (!reader.IsDBNull(3))
                            {
                                bool monsterTypeSuccess = Enum.TryParse(reader.GetString(3), out MonsterTypes monsterType);
                                if (!monsterTypeSuccess)
                                {
                                    return null;
                                }
                                c = new MonsterCards(name, damage, elementTyp, monsterType);
                            }
                            else
                            {
                                c = new SpellCards(name, damage, elementTyp);
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

        public bool Update(Card t, string[] parameters)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Card t)
        {
            throw new NotImplementedException();
        }

    }
}
