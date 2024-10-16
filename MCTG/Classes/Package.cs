using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Classes.CardStructure;
using MTCG.Interfaces;

namespace MTCG.Classes
{
    internal class Package : ICardContainer
    {
        public List<Card> Container { get; set; }
        private readonly int maxCardsInPackage = 5;
        public Package()
        {
            this.Container = new List<Card>();
            for (int i = 0; i < maxCardsInPackage; i++)
            {
                this.Container.Add(CardFactory.GetRandomCard());
            }
        }

    }
}
