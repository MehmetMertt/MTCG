using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTG.Interfaces;
using MCTG.Classes.CardStructure;


namespace MCTG.Classes
{

    internal class Stack : ICardContainer, IChangeableContainer
    {
        public List<Card> Container { get; set; }

        public Stack()
        {
            this.Container = new List<Card>();
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
