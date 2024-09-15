using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Classes.Cards
{
    internal class SpellCards : Cards
    {
        public SpellCards(string name, int damage,ElementTypes elementTypes) : base(name, damage,elementTypes)
        {
            
        }
        public override void Attack()
        {

        }

        public int calculateEffectiveness()
        {
            /*
                water -> fire
                fire -> normal
                normal -> water
                    effective (eg: water is effective against fire, so damage is doubled)
                    not effective (eg: fire is not effective against water, so damage is halved)
                    no effect (eg: normal monster vs normal spell, no change of damage, direct
                    comparison between damages)
             
             */
            return 0;
        }
    }
}
