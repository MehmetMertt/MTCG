using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using MCTG.Classes.CardStructure;
using MCTG.Interfaces;

namespace MCTG.Classes
{
    public class Deck : ICardContainer, IChangeableContainer
    {
        public List<Card> Container { get; set; }

        public void AddCard(Card c)
        {
            if (Container.Count > 3)
            {
                // TODO: Maybe Custom Exception handling
                Console.WriteLine("Es sind maximal 4 Karten im Deck erlaubt");
                return;
            }
            Container.Add(c);
        }

        public void RemoveCard(Card c)
        {
            Container.Remove(c);
        }
    }
}
