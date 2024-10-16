using System.Text;
using MCTG.BusinessLayer;
using MCTG.Classes;
using MCTG.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace MCTG.PresentationLayer
{
    public class RequestHandler(UserRepository userRepository)
    {
        private readonly UserRepository _userRepository = UserRepository.Instance;

        public Dictionary<string, string> defaultHeader = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
            { "Content-Length", "0" }
        };

        public HttpResponse HandleRequest(string request, string httpMethod, string? jsonBody)
        {

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
            var users = _userRepository.GetUsers();

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

        private HttpResponse LoginUser(string jsonBody)
        {
            JObject jsonObject = JObject.Parse(jsonBody);
            string? username = jsonObject["username"]?.ToString();
            string? password = jsonObject["password"]?.ToString();
            if (username == null || password == null)
            {
                //error
            }

            //bool login = _userRepository.GetUsers().Any(u =>
            //    u.Authentication.Username == username &&
            //    u.Authentication.Password == Authentication.StringToSHA512(password);
            var login = _userRepository
                .GetUsers()
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
                return new HttpResponse(200, "Successfully logged in",headers);
            }
            else
            {
                return new HttpResponse(400, "Invalid credentials");
            }
        }

        private HttpResponse HandleUserRegister(string jsonBody)
        {
            User u = JsonConvert.DeserializeObject<User>(jsonBody);
            return _userRepository.AddUser(u);
        }


    }

}
