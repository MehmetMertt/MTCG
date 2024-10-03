using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Classes.Cards
{

    enum ElementTypes
    {
        Water,
        Fire,
        Normal
    }

    internal abstract class Cards
    {
        public string Name { get; private set; }
        public int Damage { get; private set; }

        public ElementTypes ElementTyp { get; private  set; }

        public abstract void Attack();

        protected Cards(string name, int damage, ElementTypes elementTyp)
        {
            this.Name = name;
            this.ElementTyp = elementTyp;
            this.Damage = damage;
        }

    }
}
