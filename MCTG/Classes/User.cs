using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTG.Classes.Cards;

namespace MCTG.Classes
{
    internal class User
    {
        public string Name { get; private set; }

        private string Username { get; set; }
        private string Password { get; set; }

        /* ändern! */
        public List<MCTG.Classes.Cards.Cards> Deck { get; set; } = new List<MCTG.Classes.Cards.Cards>();
        public List<MCTG.Classes.Cards.Cards> Stack { get; set; } = new List<MCTG.Classes.Cards.Cards>();

        public int Coins { get; private set; } = 20;
        public int MoneySpent { get; private set; } = 0;

        public int Wins { get; private set; } = 0;

        public int Looses { get; private set; } = 0;
        public int Draws { get; private set; } = 0;
        public int ELO { get; private set; } = 100;

        public User(string name,string password, string username)
        {
            this.Name = name;
            this.Password = password;
            this.Username = username;
        }

        public void MangeCards()
        {
        }

        public void Trade()
        {
        }

        public void Battle()
        {
        }

        public void BuyCards()
        {
        }




    }


}
