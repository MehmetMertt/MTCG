namespace MTCG.Classes.CardStructure
{
    internal class SpellCards : Card
    {
        public SpellCards(string name, int damage,ElementTypes elementTypes, int id, int ownerid) : base(name, damage,elementTypes, id,ownerid)
        {
            
        }

        public SpellCards(string name, int damage, ElementTypes elementTypes) : base(name, damage, elementTypes)
        {

        }

        public override void Attack()
        {

        }

        public override string getInfo()
        {
            var infoParts = new List<string>();

            if (!string.IsNullOrEmpty(this.Name))
                infoParts.Add($"Name: {this.Name}");

            if (this.Damage != null)
                infoParts.Add($"Damage: {this.Damage}");

            if (this.ElementTyp != null)
                infoParts.Add($"ElementType: {this.ElementTyp}");

            return string.Join(" | ", infoParts);
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
