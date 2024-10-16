using MTCG.Classes.CardStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Classes
{
    internal class CardFactory
    {

        public static T GetRandomEnum<T>() where T : Enum
        {
            Random random = new Random();
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(random.Next(values.Length));
        }


        public static int GetRandomDamage()
        {
            int r = Random.Shared.Next(10, 20);
            return r;
        }



        public static Card GetRandomCard()
        {
            int damage = GetRandomDamage();
            ElementTypes element = GetRandomEnum<ElementTypes>();

            int r = Random.Shared.Next(0, 2);
            switch (r)
            {
                case 0:
                    return new SpellCards("test",damage,element);
                    break;
                case 1:
                    MonsterTypes monster = GetRandomEnum<MonsterTypes>();
                    return new MonsterCards("test",damage,element,monster);
                    break;
                default:
                    Console.WriteLine("HALT STOP");
                    throw new ArgumentException();
            }
        }



    }
}
