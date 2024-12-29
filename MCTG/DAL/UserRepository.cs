using MTCG.Classes;
using MTCG.Interfaces;
using Npgsql;
using System.Data;
using MTCG.DAL;


namespace MTCG.DAL
{
    public class UserRepository : IRepository<User>
    {
        private static UserRepository _instance;

        private readonly string _connectionString;

        private static List<User> _users = new List<User>
        {
            new User("Mehmet", "StarkesPassword123"),
            new User("Max", "StaerkeresPassword123")
        };

        public static UserRepository Instance
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

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void InitDb(string connectionString)
        {

            var repo = new UserRepository(connectionString);
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            string dbName = builder.Database;

            builder.Remove("Database");
            string cs = builder.ToString();

            using (IDbConnection connection = new NpgsqlConnection(cs))
            {
                connection.Open();

                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT 1 FROM pg_catalog.pg_database WHERE datname = '{dbName}'";
                    var result = cmd.ExecuteScalar(); // Use ExecuteScalar to check for existence

                    if (result == null)
                    {
                        cmd.CommandText = $"CREATE DATABASE \"{dbName}\"";
                        cmd.ExecuteNonQuery();
                    }
                }


                connection.ChangeDatabase(dbName);


                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            id SERIAL PRIMARY KEY, 
                            username VARCHAR(500) NOT NULL,
                            password VARCHAR(500) NOT NULL,
                            coins INT DEFAULT 20,
                            moneyspent INT DEFAULT 0,
                            wins INT DEFAULT 0,
                            looses INT DEFAULT 0,
                            draws INT DEFAULT 0,
                            ELO INT DEFAULT 100,
                            UNIQUE(username)
                        )
                    ";
                    cmd.ExecuteNonQuery();
                }
            }

            _instance = repo;
        }


        // Gibt alle User zurück
        public IEnumerable<User> GetAll()
        {
            List<User> result = new List<User>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText =
                        @"SELECT username, moneyspent, coins, wins, looses, draws, elo, password FROM users;";
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User u = new(reader.GetString(0), "");
                            u.MoneySpent = (reader.GetInt32(1));
                            u.Coins = (reader.GetInt32(2));
                            u.Wins = (reader.GetInt32(3));
                            u.Looses = (reader.GetInt32(4));
                            u.Draws = (reader.GetInt32(5));
                            u.ELO = (reader.GetInt32(6));
                            u.Authentication.Password = reader.GetString(7);
                            result.Add(u);

                        }
                    }

                }
            }

            return result;
        }


        public User Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(string username)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(
                           @"UPDATE users 
              SET
                  username = @username
              WHERE username = @username", connection))
                {
                   // command.Parameters.Add(new NpgsqlParameter("@coins", DbType.Int32) { Value = t.Coins });
                   // command.Parameters.Add(new NpgsqlParameter("@password", DbType.String) { Value = t.Authentication.Password });
                    command.Parameters.Add(new NpgsqlParameter("@username", DbType.String) { Value = username });

                    try
                    {
                        // Execute the update command
                        int rowsAffected = command.ExecuteNonQuery();

                        // If at least one row was affected, return true
                        return rowsAffected > 0;
                    }
                    catch (NpgsqlException ex)
                    {
                        // Log exception or handle it as needed
                        Console.WriteLine("Database error: " + ex.Message);
                        return false;
                    }
                }
            }
        }






        public bool Add(User user)
        {
            if (_instance.GetAll().Any(user1 => user1.Authentication.Username == user.Authentication.Username) == false)
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", connection))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("@username", user.Authentication.Username));
                        cmd.Parameters.Add(new NpgsqlParameter("@password", user.Authentication.Password));

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Update(User t, string[] parameters)
        {
            throw new NotImplementedException();
        }


        public bool Delete(User u)
        {
            //TODO: Fragne ob gefordert?
            throw new NotImplementedException();
        }
    }
}
