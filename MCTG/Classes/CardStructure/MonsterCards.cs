namespace MCTG.Classes.CardStructure
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

    internal class MonsterCards : Card
    {
        public MonsterTypes MonsterType { get; private set; }

        public override void Attack() {

        }

        public MonsterCards(string name, int damage,ElementTypes elementTypes,MonsterTypes monsterTypes) : base(name,damage,elementTypes)
        {
            this.MonsterType = monsterTypes;
        }
    }
}
