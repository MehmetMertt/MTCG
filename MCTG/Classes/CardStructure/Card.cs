namespace MCTG.Classes.CardStructure
{
    public enum ElementTypes
    {
        Water,
        Fire,
        Normal
    }

    public abstract class Card
    {
        public string Name { get; private set; }
        public int Damage { get; private set; }

        public ElementTypes ElementTyp { get; private set; }

        public abstract void Attack();

        public abstract void PrintInfo();


        public Card(string name, int damage, ElementTypes elementTyp)
        {
            Name = name;
            ElementTyp = elementTyp;
            Damage = damage;
        }
    }
}
