using MTCG.Classes.CardStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer
{
    public class CardDto
    {
        public string Name { get; set; }
        public int Damage { get; set; }

        public ElementTypes ElementTyp { get;  set; }
        public MonsterTypes MonsterType { get;  set; }

    }
}