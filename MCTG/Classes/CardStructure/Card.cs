namespace MCTG.Classes.CardStructure
{

    enum ElementTypes
    {
        Water,
        Fire,
        Normal
    }

    internal abstract class Card
    {
        public string Name { get; private set; }
        public int Damage { get; private set; }

        public ElementTypes ElementTyp { get; private set; }

        public abstract void Attack();

        public Card(string name, int damage, ElementTypes elementTyp)
        {
            Name = name;
            ElementTyp = elementTyp;
            Damage = damage;
        }

    }
}
