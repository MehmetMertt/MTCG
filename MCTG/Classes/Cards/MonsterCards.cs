using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Classes.Cards
{
    enum MonsterTypes
    {
        Dragon,
        Goblin,
        Knights,
        Orks,
        WaterSpells,
        FireElves,
        Kraken,
    }

    internal class MonsterCards : Cards
    {
        public MonsterTypes MonsterType { get; set; }

        public override void Attack() {

        }

        public MonsterCards(string name, int damage,ElementTypes elementTypes,MonsterTypes monsterTypes) : base(name,damage,elementTypes)
        {
            this.MonsterType = monsterTypes;
        }
    }
}
