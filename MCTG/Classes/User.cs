using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Classes
{
    internal class User
    {

        public Authentication Authentication { get; set; }

        /* ändern! */

        public int Coins { get; private set; } = 20;
        public int MoneySpent { get; private set; } = 0;

        public int Wins { get; private set; } = 0;

        public int Looses { get; private set; } = 0;
        public int Draws { get; private set; } = 0;
        public int ELO { get; private set; } = 100;

        public Stack Stack { get; set; }
        public Deck Deck { get; set; }

        public User(string username, string password)
        {
            this.Authentication = new Authentication(username, password);
            this.Stack = new Stack();
            this.Deck = new Deck();
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

        public void BuyPackage()
        {
            if (this.Coins < 5)
            {
                Console.WriteLine("You need atleast 5 Coins for buying a package");
                return;
            }
            Package p = new Package();
            foreach (var card in p.Container)
            {
                this.Stack.AddCard(card);
            }
            this.Coins =- 5;
            Console.WriteLine("You successfully bought a package");
        }




    }


}
