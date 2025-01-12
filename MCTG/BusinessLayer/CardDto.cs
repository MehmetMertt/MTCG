using MTCG.Classes.CardStructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer
{
    public class CardDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] 
        public int? ID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? OwnerID {
            get;
            set;
        }
        public string Name { get; set; }
        public int Damage { get; set; }

        public string ElementTyp { get;  set; }
        public string MonsterType { get;  set; }

    }
}