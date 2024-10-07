using MCTG.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public bool RemoveUserByName(string name)
        {
            throw new NotImplementedException();
        }

    }
}
