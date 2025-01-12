namespace MTCG.Classes.CardStructure
{
    public enum MonsterTypes
    {
        Dragon,
        Goblin,
        Knights,
        Orks,
        FireElves,
        Kraken,
    }

    internal class MonsterCards : Card
    {
        public MonsterTypes MonsterType { get; private set; }

        public override void Attack() {

        }

        public override string getInfo()
        {
            var infoParts = new List<string>();

            if (!string.IsNullOrEmpty(this.Name))
                infoParts.Add($"Name: {this.Name}");

            if (this.Damage != null)
                infoParts.Add($"Damage: {this.Damage}");

            if (this.MonsterType != null)
                infoParts.Add($"MonsterType: {this.MonsterType}");

            if (this.ElementTyp != null)
                infoParts.Add($"ElementType: {this.ElementTyp}");

            return string.Join(" | ", infoParts);
        }
        public MonsterCards(string name, int damage,ElementTypes elementTypes,MonsterTypes monsterTypes, int id, int ownerid) : base(name,damage,elementTypes, id, ownerid)
        {
            this.MonsterType = monsterTypes;
        }

        public MonsterCards(string name, int damage, ElementTypes elementTypes, MonsterTypes monsterTypes) : base(name, damage, elementTypes)
        {
            this.MonsterType = monsterTypes;
        }
    }
}
