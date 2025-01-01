using MTCG.Classes.CardStructure;

namespace MTCG.Classes
{
    public class User
    {

        public Authentication Authentication { get; set; }

        /* ändern! */

        public int Coins { get; set; } = 20; //TODO: Dont forget to change in UserRepository.cs
        public int MoneySpent { get; set; } = 0;

        public int Wins { get; set; } = 0;

        public int Looses { get;  set; } = 0;
        public int Draws { get;  set; } = 0;
        public int ELO { get;  set; } = 100;

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

        public List<Card> BuyPackage()
        {
            List<Card> boughtCards = new List<Card>();


        Console.WriteLine("I have: " + this.Coins);
            if (this.Coins < 5)
            {
                Console.WriteLine("You need atleast 5 Coins for buying a package");
                return null;
            }
            Package p = new Package();
            foreach (var card in p.Container)
            {
                this.Stack.AddCard(card);
                boughtCards.Add(card);
            }

            this.Coins = this.Coins - 5;
            Console.WriteLine("I have: " + this.Coins);
            Console.WriteLine("You successfully bought a package");
            return boughtCards;
        }




    }


}
