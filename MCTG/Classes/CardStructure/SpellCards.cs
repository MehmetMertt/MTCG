namespace MCTG.Classes.CardStructure
{
    internal class SpellCards : Card
    {
        public SpellCards(string name, int damage,ElementTypes elementTypes) : base(name, damage,elementTypes)
        {
            
        }
        public override void Attack()
        {

        }

        public override void PrintInfo()
        {
            Console.WriteLine($"Damage: {this.Damage} | Name: {this.Name} | ElementType: {this.ElementTyp} ");
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
