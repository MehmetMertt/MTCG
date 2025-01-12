using MTCG.BusinessLayer;
using MTCG.Classes;
using MTCG.Classes.CardStructure;
using MTCG.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using System.Xml;

namespace MTCG.PresentationLayer
{

    public class RequestHandler
    {
        private readonly UserRepository _userRepository = UserRepository.Instance;
        private readonly CardRepository _cardRepository = CardRepository.Instance;
        private readonly DeckRepository _deckRepository = DeckRepository.Instance;
        private readonly UnitOfWork _unitOfWork = UnitOfWork.Instance;
        private readonly ShopService _shopService;

        public RequestHandler(ShopService shopService)
        {
            _shopService = shopService;
        }

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
                else if (request == "/cards")
                {
                    return getCards(jsonBody);
                }
                else if (request == "/deck")
                {
                    return getDeck(jsonBody);
                }
                else if (request == "/scoreboard")
                {
                    return getScoreboard(jsonBody);
                }
                else if (request == "/stats")
                {
                    return getStats(jsonBody);
                } else if (request == "/tradings")
                {
                    return getTradings(jsonBody);
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
                else if (request == "/packages")
                {
                    return BuyPackages(jsonBody);
                }
                else if (request == "/battles")
                {
                    return Battle(jsonBody);
                } else if (request == "/tradings")
                {
                    return CreateTrading(jsonBody);
                }
                else if (request.StartsWith("/tradings/"))
                {
                    // Extract ID from the URL
                    string id = request.Substring("/tradings/".Length);
                    if (!Int32.TryParse(id, out int offerId))
                    {
                        throw new Exception("Invalid offerId");
                    }
                    return AcceptTrading(jsonBody, offerId);
                }
            }
            else if (httpMethod == "PUT")
            {
                if (request == "/users")
                {
                    return HandleUpdate(jsonBody);
                }
                else if (request == "/deck")
                {
                    return ConfigureDeck(jsonBody);
                }
            } else if (httpMethod == "DELETE")
            {
                if (request == "/deck")
                {
                    return DelteDeck(jsonBody);
                }
            }

            Console.WriteLine(request);
            Console.WriteLine(httpMethod);

            //return "Unknown request.";
            return new HttpResponse(404, "Unknown request");
        }

        private HttpResponse AcceptTrading(string? jsonBody, int offerId)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonBody);
                string token = jsonObject["token"]?.ToString();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpResponse(401, "Token is empty");
                int? id = _userRepository.getUserIDFromToken(token);
                if (!id.HasValue)
                    return new HttpResponse(401, "Token not valid");
                string cardIdJson = jsonObject["cardId"]?.ToString();
                if (string.IsNullOrWhiteSpace(cardIdJson))
                    return new HttpResponse(401, "CardId is empty");

                int cardId;

                if (!Int32.TryParse(cardIdJson, out cardId))
                {
                    return new HttpResponse(401, "CardId is not valid");
                }
                Card c = _cardRepository.getCard(cardId, id.Value);
                
                _shopService.AcceptOffer(offerId, c, id.Value);
                return new HttpResponse(201, "Card traded");
            }
            catch (Exception e)
            {
                return new HttpResponse(401, e.Message);
            }

        }

        private HttpResponse DelteDeck(string? jsonBody)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonBody);
                string token = jsonObject["token"]?.ToString();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpResponse(401, "Token is empty");
                int? id = _userRepository.getUserIDFromToken(token);
                if (!id.HasValue)
                    return new HttpResponse(401, "Token not valid");
                _deckRepository.ClearDeck(id.Value);
                return new HttpResponse(200, "Deck is cleared");

            }
            catch (Exception e)
            {
                return new HttpResponse(500, e.ToString());

            }

        }

        private HttpResponse CreateTrading(string? jsonBody)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonBody);
                string token = jsonObject["token"]?.ToString();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpResponse(401, "Token is empty");
                int? id = _userRepository.getUserIDFromToken(token);
                if (!id.HasValue)
                    return new HttpResponse(401, "Token not valid");

                string cardIdJson = jsonObject["cardId"]?.ToString();
                if (string.IsNullOrWhiteSpace(cardIdJson))
                    return new HttpResponse(401, "CardId is empty");

                int cardId;

                if (!Int32.TryParse(cardIdJson, out cardId))
                {
                    return new HttpResponse(401, "CardId is not valid");
                }

                var card = _cardRepository.getCard((int)cardId, id.Value);

                if (card is null)
                {
                    return new HttpResponse(401, "card not found");
                }

                int minimumDamage;
                if (jsonObject.TryGetValue("minimumDamage", out JToken? jtoken))
                {
                    minimumDamage = jtoken.Value<int>();
                }
                else
                {
                    return new HttpResponse(401, "no minimumDamage");
                }

                bool monsterCard;
                if (jsonObject.TryGetValue("type", out JToken? jtokentype))
                {
                    string typ = jtokentype.Value<string>();
                    if (typ == "monster")
                    {
                        monsterCard = true;
                    }
                    else if (typ == "spell")
                    {
                        monsterCard = false;
                    }
                    else
                    {
                        return new HttpResponse(401, "this card typ does not exist");
                    }
                }
                else
                {
                    return new HttpResponse(401, "Please either select mosnter or spell card");

                }

                CardRequirement cr = new CardRequirement(monsterCard, null, minimumDamage);
                if (jsonObject.TryGetValue("element", out JToken? jtokenelement))
                {
                    string typ = jtokenelement.Value<string>();
                    if (Enum.TryParse(typ, true, out ElementTypes element))
                    {
                        cr.element = element;
                    }
                    else
                    {
                        return new HttpResponse(401, "This Element does not exist");
                    }
                }


                _shopService.ListItem(card, cr);
                var content = "Created successfully";
                defaultHeader["Content-Length"] = content.Length.ToString();
                return new HttpResponse(200, content, defaultHeader);
            }
            catch (Exception e)
            {
                var content = e.ToString();
                defaultHeader["Content-Length"] = content.Length.ToString();
                return new HttpResponse(400, content, defaultHeader);
            }
           
        }

        private HttpResponse getTradings(string jsonBody)
        {
            JObject jsonObject = JObject.Parse(jsonBody);
            string token = jsonObject["token"]?.ToString();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpResponse(401, "Token is empty");

            int? id = _userRepository.getUserIDFromToken(token);
            if (!id.HasValue)
                return new HttpResponse(401, "Token not valid");

            var offers = _shopService.GetOffers();
            var content = JsonConvert.SerializeObject(offers);
            defaultHeader["Content-Length"] = content.Length.ToString();
            return new HttpResponse(200, content, defaultHeader);
        }

        private HttpResponse Battle(string? jsonBody)
        {

            JObject jsonObject = JObject.Parse(jsonBody);
            string token = jsonObject["token"]?.ToString();
            if (!jsonObject.TryGetValue("enemy", out var enemyToken) || !int.TryParse(enemyToken?.ToString(), out int enemyId))
            {
                throw new Exception("Invalid or missing 'enemy' value in JSON.");
            }
            if (string.IsNullOrWhiteSpace(token))
                return new HttpResponse(401, "Token is empty");

            int? id = _userRepository.getUserIDFromToken(token);
            if (!id.HasValue)
                return new HttpResponse(401, "Token not valid");



            var battleService = new BattleService(_unitOfWork);
            User player1 = _userRepository.getUserFromToken(token);
            User player2 = _userRepository.Get(enemyId);
            //TODO: schgöner machen
   

            var battleResult = battleService.StartBattle(player1,player2);
            var content = JsonConvert.SerializeObject(battleResult.BattleLog);
            defaultHeader["Content-Length"] = content.Length.ToString();
            return new HttpResponse(200, content, defaultHeader);
        }

        private HttpResponse getStats(string jsonBody)
        {

            JObject jsonObject = JObject.Parse(jsonBody);
            string token = jsonObject["token"]?.ToString();

            if (string.IsNullOrWhiteSpace(token))
                return new HttpResponse(401, "Token is empty");

            int? id = _userRepository.getUserIDFromToken(token);
            if (!id.HasValue)
                return new HttpResponse(401, "Token not valid");
            Stats s = new Stats(_userRepository);
            var stats = s.GetStats(id.Value);
            var content = JsonConvert.SerializeObject(stats);
            defaultHeader["Content-Length"] = content.Length.ToString();
            return new HttpResponse(200, content, defaultHeader);

        }

        private HttpResponse getScoreboard(string jsonBody)
        {

            JObject jsonObject = JObject.Parse(jsonBody);
            string token = jsonObject["token"]?.ToString();

            if (string.IsNullOrWhiteSpace(token))
                return new HttpResponse(401, "Token is empty");

            int? id = _userRepository.getUserIDFromToken(token);

            if (!id.HasValue)
                return new HttpResponse(401, "Token not valid");
            Scoreboard scb = new Scoreboard(_userRepository);
            var scoreboard = scb.GetScores();
            var content = JsonConvert.SerializeObject(scoreboard);
            defaultHeader["Content-Length"] = content.Length.ToString();
            return new HttpResponse(200, content, defaultHeader);
        }



        private HttpResponse ConfigureDeck(string jsonBody)
        {
            //TODO: Bug - User can configure deck multiple times -> 4+ cards possible 
            JObject jsonObject = JObject.Parse(jsonBody);
            string token = jsonObject["token"]?.ToString();

            if (string.IsNullOrWhiteSpace(token))
                return new HttpResponse(401, "Token is empty");

            int? id = _userRepository.getUserIDFromToken(token);

            if (!id.HasValue)
                return new HttpResponse(401, "Token not valid");

            if (jsonObject["cards"] == null)
            {
                return new HttpResponse(400, "Cards is empty");
            }

            if (jsonObject["cards"] is not JArray cardsArray)
            {
                return new HttpResponse(400, "Cards is empty"); ;
            }

            List<string> cards = cardsArray.ToObject<List<string>>();

            // Check if the "cards" array is empty
            if (cards == null || cards.Count <= 3)
            {
                return new HttpResponse(400, "Please provide 4 cards");

            }

            User u = _userRepository.getUserFromToken(token);
            List<Card> cardsToAdd = new List<Card>();
                    int? userID = _userRepository.getUserIDFromToken(token);
                    if (!userID.HasValue)
                        throw new ApplicationException("UserID not found");
            foreach (var card in cards)
            {
                try
                {
                    int cardID = Int32.Parse(card);
                    Card c = _cardRepository.getCard(cardID, userID.Value);
                    cardsToAdd.Append(c);
                    _deckRepository.AddCardToDeck(userID.Value, cardID);
                        
                        

                }
                catch (Exception e)
                {
                    return new HttpResponse(500, e.ToString());
                }

            }

            try
            {
                u.Deck.AddCard(cardsToAdd);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return new HttpResponse(400, e.ToString());
            }



            return new HttpResponse(200, "Deck configured");
        }
        //TODO: überall wo jsonBody zu jsonObject umgewandelt wird -> crasht es wenn jsonBody null ist
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

            var userCards = _deckRepository.getDeck(id.Value);
            var cardDtos = userCards.Select(card =>
            {
                var cardDto = new CardDto
                {
                    ID = card.CardID,
                    Name = card.Name,
                    Damage = card.Damage,
                    ElementTyp = card.ElementTyp.ToString(),
                };

                if (card is MonsterCards monsterCard)
                {
                    cardDto.MonsterType = monsterCard.MonsterType.ToString();
                }

                return cardDto;
            }).ToList();
            var content = JsonConvert.SerializeObject(cardDtos);
            defaultHeader["Content-Length"] = content.Length.ToString();
            return new HttpResponse(200, content, defaultHeader);
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
                    ID = card.CardID,
                    Name = card.Name,
                    OwnerID = card.OwnerId,
                    Damage = card.Damage,
                    ElementTyp = card.ElementTyp.ToString(),
                };

                if (card is MonsterCards monsterCard)
                {
                    cardDto.MonsterType = monsterCard.MonsterType.ToString();
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
                return new HttpResponse(400, "Token is empty");

            }

            User u = _userRepository.getUserFromToken(token);
            if (u is null)
            {
                return new HttpResponse(400, "Token is not valid");

            }



            List<Card> boughtSuccessfully = u.BuyPackage();
            if (boughtSuccessfully is not null)
            {
                _userRepository.UpdateUserCoins(token, u.Coins - 5);
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
                id = user.id,
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
                    {
                        return new HttpResponse(200, "Successfully logged in", headers);
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
