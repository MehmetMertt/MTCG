using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Classes;
using MTCG.Classes.CardStructure;

namespace MTCG.BusinessLayer
{
    public class ShopItemDto
    {
        public int id { get; set; }

        public int minimumDamage { get; set; }

        public bool monsterCard { get; set; }

        public string ElementTyp { get; set; }

        public CardDto cardToTrade { get; set; }



    }
}
