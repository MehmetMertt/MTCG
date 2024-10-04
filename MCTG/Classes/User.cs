﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Classes
{
    internal class User
    {
        public string Name { get; private set; }

        public Authentication Authentication { get; set; }

        /* ändern! */

        public int Coins { get; private set; } = 20;
        public int MoneySpent { get; private set; } = 0;

        public int Wins { get; private set; } = 0;

        public int Looses { get; private set; } = 0;
        public int Draws { get; private set; } = 0;
        public int ELO { get; private set; } = 100;

        public User(string name,string password, string username)
        {
            this.Authentication = new Authentication(username, password);
        }

        public void MangeCards()
        {
        }

        public void Trade()
        {
        }

        public void Battle()
        {
        }

        public void BuyCards()
        {
        }




    }


}
