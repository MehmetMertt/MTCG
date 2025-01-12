using MTCG.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Classes
{
    internal class Stats
    {

        private readonly UserRepository _userRepository;

        public Stats(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public StatsEntry GetStats(int userId)
        {
            var user = _userRepository.Get(userId);
            if (user is null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return new StatsEntry
            {
                Username = user.Authentication.Username,
                Wins = user.Wins,
                Losses = user.Looses,
                Elo = user.ELO
            };
        }


    }

    public class StatsEntry
    {
        public string Username { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Elo { get; set; }
    }
}

