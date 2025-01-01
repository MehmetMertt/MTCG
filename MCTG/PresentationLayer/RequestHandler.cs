using System.Text;
using MTCG.BusinessLayer;
using MTCG.Classes;
using MTCG.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using MTCG.Interfaces;
using System.Reflection.PortableExecutable;
using MTCG.Classes.CardStructure;

namespace MTCG.PresentationLayer
{
    public class RequestHandler
    {
        private readonly UserRepository _userRepository = UserRepository.Instance;
        private readonly CardRepository _cardRepository = CardRepository.Instance;




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
                } else if (request == "/cards")
                {
                    return getCards(jsonBody);
                } else if (request == "/deck")
                {
                    return getDeck(jsonBody);
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
                } else if (request == "/packages")
                {
                    return BuyPackages(jsonBody);
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

        private HttpResponse getDeck(string jsonBody)
        {
            JObject jsonObject = JObject.Parse(jsonBody);
            string? token = jsonObject["token"]?.ToString();
            if (token is null)
            {
                return new HttpResponse(401, "Token is empty");
            }

            int? id = _userRepository.getUserIDFromToken(token);
            if (id == null)
            {
                return new HttpResponse(401, "Token not valid");

            }

            var userCards = _cardRepository.getDeckFromUser(id.Value);
            var cardDtos = userCards.Select(card =>
            {
                var cardDto = new CardDto
                {
                    Name = card.Name,
                    Damage = card.Damage,
                    ElementTyp = card.ElementTyp,
                };

                if (card is MonsterCards monsterCard)
                {
                    cardDto.MonsterType = monsterCard.MonsterType;
                }

                return cardDto;
            }).ToList();

        }


        private HttpResponse getCards(string jsonBody)
        {
            JObject jsonObject = JObject.Parse(jsonBody);
            string? token = jsonObject["token"]?.ToString();
            if (token is null)
            {
                return new HttpResponse(401, "Token is empty");
            }

            int? id = _userRepository.getUserIDFromToken(token);
            if (id == null) 
            {
                return new HttpResponse(401, "Token not valid");

            }

            var userCards = _cardRepository.GetAllCardsFromUserID(id.Value); 
            var cardDtos = userCards.Select(card =>
            {
                var cardDto = new CardDto
                {
                    Name = card.Name,
                    Damage = card.Damage,
                    ElementTyp = card.ElementTyp, 
                };

                if (card is MonsterCards monsterCard)
                {
                    cardDto.MonsterType = monsterCard.MonsterType;
                }

                return cardDto;
            }).ToList();

            var content = JsonConvert.SerializeObject(cardDtos);
            defaultHeader["Content-Length"] = content.Length.ToString();
            return new HttpResponse(200, content, defaultHeader);
        }


        private HttpResponse BuyPackages(string jsonBody)
        {
            JObject jsonObject = JObject.Parse(jsonBody);
            string? token = jsonObject["token"]?.ToString();
            if (token is null)
            {
                throw new ArgumentException("Token is empty");
            }

            User u = _userRepository.getUserFromToken(token);
            if (u is null)
            {
                throw new ArgumentException("User doesnt exist");
            }



            List<Card> boughtSuccessfully = u.BuyPackage();
            if (boughtSuccessfully is not null)
            {
                _userRepository.UpdateUserCoins(token,u.Coins - 5);
                foreach (var c in boughtSuccessfully)
                {
                    _cardRepository.Add(c, token);
                }
                return new HttpResponse(201, "Successfully bought package");
            }
            else
            {
                return new HttpResponse(400, "not enough money");

            }

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
                    if (_userRepository.UpdateUserToken(login.Authentication.Username, login.Authentication.getToken()))
                    { return new HttpResponse(200, "Successfully logged in", headers);
                    }
                }
               
                return new HttpResponse(400, "Invalid credentials");
                
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
