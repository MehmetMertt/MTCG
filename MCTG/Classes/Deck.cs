using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using MTCG.Classes.CardStructure;
using MTCG.Interfaces;

namespace MTCG.Classes
{
    public class Deck
    {
        public List<Card> Container;

        public Deck()
        {
            this.Container = new List<Card>();
        }

        public void AddCard(List<Card> cards)
        {
            foreach (var card in cards)
            {
                Container.Add(card);
            }
        }

        public List<Card> getCards()
        {
            return this.Container;
        }

        public void RemoveCard(Card c)
        {
            Container.Remove(c);
        }
    }
}
