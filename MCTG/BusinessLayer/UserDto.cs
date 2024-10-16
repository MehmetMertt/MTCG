using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer
{
    public class UserDto
    {
        public int Coins { get;  set; }
        public int MoneySpent { get;  set; }

        public string UserName { get; set; }
        public int Wins { get; set; }

        public int Looses { get;  set; }
        public int Draws { get;  set; }
        public int ELO { get;  set; }
    }
}
