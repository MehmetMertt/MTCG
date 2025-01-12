using MTCG.Classes.CardStructure;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Classes;
using MTCG.BusinessLayer;
using Newtonsoft.Json.Linq;

namespace MTCG.DAL
{
    public class TradingShopRepository
    {
        private static TradingShopRepository _instance;

        private readonly string _connectionString;
        private NpgsqlTransaction? _transaction;

        public TradingShopRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void InitDb(string connectionString)
        {

            var repo = new TradingShopRepository(connectionString);
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
                                        CREATE TABLE IF NOT EXISTS tradingshop (
                                            id SERIAL PRIMARY KEY,         
                                            cardid INT NOT NULL,
                                            minimumDamage INT NOT NULL,
                                            monsterCard boolean,
                                            elementType VARCHAR(11) DEFAULT NULL,
                                            created_at TIMESTAMP DEFAULT NOW(), 
                                            FOREIGN KEY (cardid) REFERENCES cards (id) ON DELETE CASCADE
                                        );
                                        ";
                    cmd.ExecuteNonQuery();
                }
            }

            _instance = repo;
        }



        public void SetTransaction(NpgsqlTransaction transaction)
        {
            _transaction = transaction;
        }

        public static TradingShopRepository Instance
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

        public int Insert(int cardid, int minimumDamage, bool monsterCard, ElementTypes? elementTyp)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    command.CommandText =
                        "INSERT INTO tradingshop (cardid, minimumDamage, monsterCard, elementType) VALUES (@cardid, @minimumDamage, @monsterCard, @elementType) RETURNING id";
                    command.Parameters.AddWithValue("cardid", cardid);
                    command.Parameters.AddWithValue("minimumDamage", minimumDamage);
                    command.Parameters.AddWithValue("monsterCard", monsterCard);
                    command.Parameters.AddWithValue("elementType", elementTyp.ToString());
                    return Convert.ToInt32(command.ExecuteScalar());
                }


            }
        }

        public void DeleteOffer(int offerId, int userId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM tradingshop
                                               USING cards
                                               WHERE cards.id = tradingshop.cardid
                                                  AND cards.ownerid = @ownerid
                                                  AND tradingshop.id = @id;
                                                ";
                        command.Parameters.AddWithValue("id", offerId);
                        command.Parameters.AddWithValue("ownerid", userId);
                        command.Transaction = _transaction;
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception($"Offer with ID {offerId} does not exist.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteOffer(int offerId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM tradingshop
                                               USING cards
                                               WHERE cards.id = tradingshop.cardid
                                                  AND tradingshop.id = @id;
                                                ";
                        command.Parameters.AddWithValue("id", offerId);
                        command.Transaction = _transaction;
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception($"Offer with ID {offerId} does not exist.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static List<Card> _shopitems = new List<Card>
        {

        };

        public List<ShopItem> Get()
        {
            List<ShopItem> result = new List<ShopItem>();
            try
            {
                Console.WriteLine("Starting Get() method...");
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection opened.");
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    SELECT cardid, minimumDamage, monsterCard, t.elementType, c.name, c.damage, c.elementtype, c.monstertype, t.id, c.ownerid
                    FROM tradingshop t
                    JOIN cards c ON t.cardid = c.id";

                        Console.WriteLine($"Query: {command.CommandText}");
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int cardid = reader.GetInt32(0);
                                int minimumDamage = reader.GetInt32(1);
                                bool monsterCard = reader.GetBoolean(2);
                                ElementTypes? element = null;
                                bool elementTypeSuccess =
                                    Enum.TryParse(reader.GetString(3), out ElementTypes elementTyp);
                                if (!elementTypeSuccess)
                                {
                                    element = null;
                                }

                                string cardName = reader.GetString(4);
                                int cardDamage = reader.GetInt32(5);
                                bool cardElementSuccess =
                                    Enum.TryParse(reader.GetString(6), out ElementTypes cardElement);
                                if (!cardElementSuccess)
                                {
                                    throw new Exception("Elementtype is not defined");
                                }

                                MonsterTypes? cardMonsterTypeEnum = null;

                                string cardMonsterType = reader.IsDBNull(7) ? "null" :
                                    Enum.TryParse(reader.GetString(7), out MonsterTypes parsedMonsterType)
                                        ? (cardMonsterTypeEnum = parsedMonsterType).ToString()
                                        : "null";

                                int offerId = reader.GetInt32(8);
                                Console.WriteLine($"Read OfferId from database: {offerId}");
                                int ownerId = reader.GetInt32(9);

                                Card card;
                                if (cardMonsterType == "null")
                                {
                                    card = new SpellCards(cardName, cardDamage, cardElement, cardid, ownerId);
                                }
                                else
                                {
                                    card = new MonsterCards(cardName, cardDamage, cardElement, cardMonsterTypeEnum.Value, cardid, ownerId);
                                }

                                var requirement = new CardRequirement(monsterCard, element, minimumDamage);
                                var shopItem = new ShopItem(offerId, card, requirement);
                                // Map row to ShopItem
                                Console.WriteLine($"Reading offer: {reader.GetInt32(8)}");
                                result.Add(shopItem);
                            }
                        }
                    }
                }

                Console.WriteLine($"Get() completed. Found {result.Count} offers.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Get(): {ex.Message}");
            }

            return result; // Return empty list if no data
        }

    }


}
