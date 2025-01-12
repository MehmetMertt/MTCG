using System.Numerics;

namespace MTCG.Classes.CardStructure
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

        public int OwnerId { get; set; }

        public int CardID { get; private set; }

        public ElementTypes ElementTyp { get; private set; }

        public abstract void Attack();

        public abstract string getInfo();

        /*
        public static bool operator ==(Card userHas, Card userWants)
        {
            if (ReferenceEquals(userHas, userWants)) return true;  // Referenz is thew same
            if (userHas is null || userWants is null) return false; // One of the elements is null

            if (userHas is SpellCards spellCardHas && userWants is not SpellCards spellCardWants) return false; //typen sind nicht gleich, daher nicht selbe Karte
            if (userHas is MonsterCards monsterCardHas && userWants is not SpellCards monsterCardWants) return false; //typen sind nicht gleich, daher nicht selbe Karte

            if (userHas.Damage < userWants.Damage) return false; //Damage ist geringer als das Minimum

            if (userHas.ElementTyp != userWants.ElementTyp) return false;
            


            return left.Id == riuserWantsght.Id && left.Name == userWants.Name; // Custom logic
        }

        public static bool operator !=(Card left, Card right)
        {
            return !(left == right);
        }

        */

        public Card(string name, int damage, ElementTypes elementTyp, int id, int ownerID)
        {
            OwnerId = ownerID;
            CardID = id;
            Name = name;
            ElementTyp = elementTyp;
            Damage = damage;
        }

        public Card(string name, int damage, ElementTypes elementTyp)
        {
            Name = name;
            ElementTyp = elementTyp;
            Damage = damage;
        }

        public static int IsPrime(int number)
        {
            if (number <= 1) return 1;
            if (number == 2) return 2;
            if (number % 2 == 0) return 1;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return 1;

            return 2;
        }

        public int GetEffectiveDamage(Card opponent)
        {
            // Goblins are too afraid of Dragons to attack.
            if (Name == "Goblin" && opponent.Name == "Dragon")
                return 0;

            // Wizzard can control Orks so they are not able to damage them
            if (Name == "Wizard" && opponent.Name == "Ork")
                return 0; // Wizard controls Orks

            // The armor of Knights is so heavy that WaterSpells make them drown them instantly.
            if (Name == "Knight" && opponent.ElementTyp == ElementTypes.Water && opponent is SpellCards)
                return int.MaxValue;

            // The Kraken is immune against spells.
            if (Name == "Kraken" && opponent is SpellCards)
                return 0;

            // The FireElves know Dragons since they were little and can evade their attacks.
            if (Name == "FireElf" && opponent.Name == "Dragon")
                return 0;


            // mandatory unique feature: if the cardid is a prime number -> you get luck (double damage)
            if (this is SpellCards || opponent is SpellCards)
            {
                if (this.ElementTyp == ElementTypes.Water && opponent.ElementTyp == ElementTypes.Water)
                    return Damage * 2 * IsPrime(CardID);
                if (this.ElementTyp == ElementTypes.Fire && opponent.ElementTyp == ElementTypes.Water)
                    return (Damage * IsPrime(CardID)) / 2;
                if (this.ElementTyp == ElementTypes.Fire && opponent.ElementTyp == ElementTypes.Normal)
                    return Damage * 2 * IsPrime(CardID);
                if (this.ElementTyp == ElementTypes.Normal && opponent.ElementTyp == ElementTypes.Fire)
                    return (Damage * IsPrime(CardID)) / 2;
                if (this.ElementTyp == ElementTypes.Normal && opponent.ElementTyp == ElementTypes.Water)
                    return (Damage * IsPrime(CardID)) * 2;
                if (this.ElementTyp == ElementTypes.Water && opponent.ElementTyp == ElementTypes.Normal)
                    return (Damage * IsPrime(CardID)) / 2;
            }

            return Damage;
        }

    }

}

