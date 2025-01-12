using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interfaces;
using MTCG.Classes.CardStructure;


namespace MTCG.Classes
{
    public class Stack : 
        ICardContainer, IChangeableContainer
    {
        public List<Card> Container;
        

        public Stack()
        {
            this.Container = new List<Card>();
        }

        public  List<Card> getCards()
        {
            return this.Container;
        }

        public void AddCard(Card c)
        {
            Container.Add(c);
        }

        public void RemoveCard(Card c)
        {
            Container.Remove(c);
        }

    }
}
