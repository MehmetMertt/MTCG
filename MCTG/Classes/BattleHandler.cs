using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MTCG.Classes.CardStructure;
using MTCG.DAL;

namespace MTCG.Classes
{

    public class BattleResult
    {
        public User Player1 { get; }
        public User Player2 { get; }
        public User? Winner { get; }
        public List<string> BattleLog { get; }
        public User? Loser => Winner == Player1 ? Player2 : Winner == Player2 ? Player1 : null;
        public BattleResult(User player1, User player2, User? winner, List<string> battleLog)
        {
            Player1 = player1;
            Player2 = player2;
            Winner = winner;
            BattleLog = battleLog;
        }

    }

    public sealed class BattleHandler
    {
        private static BattleHandler _instance;
        private static readonly object padlock = new object();

        BattleHandler()
        {
        }

        public static BattleHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance is null)
                {
                    _instance = new BattleHandler();
                }

                return _instance;
                }
            }
        }

        public BattleResult StartBattle(User player1, User player2)
        {
            List<String> battleLog = new List<String>();
            var player1Deck = player1.Deck.getCards();
            var player2Deck = player2.Deck.getCards();
            const int maxrounds = 100;
            int roundCount = 0;
            while(player1Deck.Any() && player2Deck.Any() && roundCount < 100)
            {
                roundCount++;
                var r = new Random();
                Card Player1Card = player1Deck.ElementAt(r.Next(0, player1Deck.Count()));
                Card Player2Card = player2Deck.ElementAt(r.Next(0, player2Deck.Count()));

                int Player1Damage = Player1Card.GetEffectiveDamage(Player2Card);
                int Player2Damage = Player2Card.GetEffectiveDamage(Player1Card);
                if (Player1Damage > Player2Damage)
                {
                    battleLog.Add($"{Player2Card.Name} (Dmg: {Player2Damage}) gets attacked by {Player1Card.Name} (Dmg: {Player1Damage}) | {player1.Authentication.Username} wins the round {roundCount}!");
                    player1.Deck.AddCard([Player2Card]);
                    player2.Deck.RemoveCard(Player2Card);
                }
                else if (Player2Damage > Player1Damage)
                {
                    battleLog.Add($"{Player1Card.Name} (Dmg: {Player1Damage}) gets attacked by {Player2Card.Name} (Dmg: {Player2Damage}) | {player2.Authentication.Username} wins the round {roundCount}!");
                    player2.Deck.AddCard([Player1Card]);
                    player1.Deck.RemoveCard(Player1Card);
                }
                else
                {
                    battleLog.Add($"Round {roundCount} is a draw");
                }
            }

            User winner = null;
            if (!player1Deck.Any())
            {
                winner = player2;
                player2.Win();
                player1.Loss();
                battleLog.Add($"{player2.Authentication.Username} won the battle!");
            } else if (!player2Deck.Any())
            {
                player1.Win();
                player2.Loss();
                battleLog.Add($"{player1.Authentication.Username} won the battle!");
            }
            else
            {
                winner = player1;
                player1.Draw();
                player2.Draw();
                player1.Deck.Container = player1Deck;
                player2.Deck.Container = player2Deck;
                battleLog.Add($"The battle is a draw");
            }

            return new BattleResult(player1,player2,winner,battleLog);
        }
    }
}
