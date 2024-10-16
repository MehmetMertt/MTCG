using MCTG.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MCTG.DAL
{
    public class UserRepository
    {
        private static UserRepository _instance;

        private static List<User> _users = new List<User>
        {
            new User("Mehmet", "StarkesPassword123"),
            new User("Max", "StaerkeresPassword123")
        };

        public static UserRepository Instance
        {
            get
            {
                {
                    if (_instance == null)
                    {
                        _instance = new UserRepository();
                    }

                    return _instance;
                }
            }
        }


        // Gibt alle User zurück
        public IEnumerable<User> GetUsers()
        {
            return _users;
        }


        public HttpResponse AddUser(User user)
        {
            bool userNameAlreadyResgisterd = _users.Any(u => u.Authentication.Username == user.Authentication.Username);
            string jsonResponse;
            if (!userNameAlreadyResgisterd)
            {
                _users.Add(user);
                jsonResponse = "{\"message\": \"Account successfully created\"}";
                return new HttpResponse(200, jsonResponse, null);
            }
            else if (userNameAlreadyResgisterd)
            {
                jsonResponse = "{\"message\": \"Username already exists\"}";
                return new HttpResponse(400, jsonResponse, null);
            }
            jsonResponse = "{\"message\": \"Unknown error\"}";
            return new HttpResponse(400, jsonResponse, null);
        }


        public bool RemoveUserByName(string name)
        {
            throw new NotImplementedException();
        }

    }
}
