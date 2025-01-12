using MTCG.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Classes
{
    internal class Scoreboard
    {
        private readonly UserRepository _userRepository;

        public Scoreboard(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<ScoreEntry> GetScores()
        {
            var users = _userRepository.GetAll();

            return users
                .Select(user => new ScoreEntry
                {
                    Username = user.Authentication.Username,
                    Wins = user.Wins,
                    Losses = user.Looses,
                    Elo = user.ELO
                })
                .OrderByDescending(entry => entry.Elo)
                .ToList();
        }
    }

    public class ScoreEntry
    {
        public string Username { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Elo { get; set; }
    }
}
