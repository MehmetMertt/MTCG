using System.Text;
using MTCG.BusinessLayer;
using MTCG.Classes;
using MTCG.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using MTCG.Interfaces;
using System.Reflection.PortableExecutable;

namespace MTCG.PresentationLayer
{
    public class RequestHandler
    {
        private readonly UserRepository _userRepository = UserRepository.Instance;
        



        public Dictionary<string, string> defaultHeader = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
            { "Content-Length", "0" }
        };

        public HttpResponse HandleRequest(string request, string httpMethod, string? jsonBody)
        {
            Console.Write(jsonBody);

            if (httpMethod == "GET")
            {
                if (request == "/users") //get All Users
                {
                    return GetUsers();
                }
                //toGetHandler
            }
            else if (httpMethod == "POST")
            {

                if (request == "/users") //Register new user
                {
                    return HandleUserRegister(jsonBody);
                }
                else if (request == "/sessions")
                {
                    return LoginUser(jsonBody);
                }
            } else if (httpMethod == "PUT")
            {
                if (request == "/users")
                {
                    return HandleUpdate(jsonBody);
                }
            }

            Console.WriteLine(request);
            Console.WriteLine(httpMethod);

            //return "Unknown request.";
            return new HttpResponse(404, "Unknown request");
        }

        private string HandleAnimalPost(string? jsonBody, string speciesType)
        {
            /*
            if (string.IsNullOrWhiteSpace(jsonBody))
            {
                return "Error: No JSON body provided.";
            }

            var newAnimalDto = JsonSerializer.Deserialize<AnimalDto>(jsonBody);

            if (string.IsNullOrWhiteSpace(newAnimalDto.Name))
            {
                return "Error: 'Name' field is required.";
            }

            ElementType elementType = ParseElementType(newAnimalDto.Element);
            IMovementBehavior movementBehavior = ParseMovementBehavior(newAnimalDto.Movement);
            DateTime birth = string.IsNullOrWhiteSpace(newAnimalDto.BirthDate)
                ? DateTime.Now
                : DateTime.Parse(newAnimalDto.BirthDate);

            if (speciesType == "cat")
            {
                _animalRepository.AddAnimal(new Cat(newAnimalDto.Name, birth, movementBehavior, elementType));
                return $"Cat '{newAnimalDto.Name}' added successfully.";
            }
            else
            {
                _animalRepository.AddAnimal(new Dog(newAnimalDto.Name, birth, movementBehavior, elementType));
                return $"Dog '{newAnimalDto.Name}' added successfully.";
            }
            */
            throw new NotImplementedException();
        }

        // Hilfsfunktion zur Extraktion von Query-Parametern aus der URL
        private string? ExtractQueryParam(string request, string param)
        {
            var uri = new Uri("http://localhost:8080" + request);
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query.Get(param);
        }

        // Methode zur Ausgabe aller Tiere
        private HttpResponse GetUsers()
        {
            var users = _userRepository.GetAll();

            var userDtos = users.Select(user => new UserDto
            {
                UserName = user.Authentication.Username,
                Coins = user.Coins,
                Draws = user.Draws,
                ELO = user.ELO,
                Looses = user.Looses,
                MoneySpent = user.MoneySpent,
                Wins = user.Wins
            }).ToList();

            var content = JsonConvert.SerializeObject(userDtos);
            defaultHeader["Content-Length"] = content.Length.ToString();
            return new HttpResponse(200, content, defaultHeader);
        }

        private HttpResponse HandleUpdate(string jsonBody)
        {
            try
            {
                Console.WriteLine(jsonBody);
                if (string.IsNullOrEmpty(jsonBody))
                {
                    return new HttpResponse(400, "Invalid request: Body cannot be null or empty.");
                }
                JObject jsonObject = JObject.Parse(jsonBody);
                    string? username = jsonObject["username"]?.ToString();
                    if (username == null)
                    {
                        throw new ArgumentException("Please fill every field");
                    }

                    var update = _userRepository.Update(username);
                    if (update)
                    {
                        return new HttpResponse(200, "Successfully changed username");

                    }
                    return new HttpResponse(404, "Cannot changed username");

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private HttpResponse LoginUser(string jsonBody)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonBody);
                string? username = jsonObject["username"]?.ToString();
                string? password = jsonObject["password"]?.ToString();
                if (username == null || password == null)
                {
                    throw new ArgumentException("Please fill every field");
                }

                //Console.WriteLine("Password from Userinput: " + Authentication.sha512(password));
                
                var login = _userRepository
                    .GetAll()
                    .FirstOrDefault(u => u.Authentication.Username == username &&
                                         u.Authentication.Password == Authentication.sha512(password));
                if (login is not null)
                {
                    login.Authentication.GenerateToken();
                    Dictionary<string, string> headers = new Dictionary<string, string>()
                    {
                        {
                            "Authorization",
                            new StringBuilder().Append("Bearer: ").Append(value: login.Authentication.getToken()).ToString()
                        }
                    };
                    return new HttpResponse(200, "Successfully logged in", headers);
                }
                else
                {
                    return new HttpResponse(400, "Invalid credentials");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //bool login = _userRepository.GetUsers().Any(u =>
            //    u.Authentication.Username == username &&
            //    u.Authentication.Password == Authentication.StringToSHA512(password);
            
        }

        private HttpResponse HandleUserRegister(string jsonBody)
        {
            try
            {

                if (string.IsNullOrEmpty(jsonBody))
                {
                    return new HttpResponse(400, "Invalid request: Body cannot be null or empty.");
                }
                User u = JsonConvert.DeserializeObject<User>(jsonBody);
                bool success = _userRepository.Add(u);
                if (success)
                {
                    return new HttpResponse(201, "Successfully registered");
                }
                else
                {
                    return new HttpResponse(404, "User already exists");
                }
            }
            catch (JsonException e)
            {
                return new HttpResponse(400, "Invalid  format");
            }

        }


    }

}
