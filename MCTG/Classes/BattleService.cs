using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Classes.CardStructure;
using MTCG.DAL;

namespace MTCG.Classes
{
    internal class BattleService : Service<BattleService>
    {
        private readonly BattleHandler _battleHandler = BattleHandler.Instance;

        public BattleService(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public BattleResult StartBattle(
            User player1,
            User player2)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                if (player1 == null || player2 == null)
                    throw new Exception("One or both players not found.");

                player1.Deck.Container = _unitOfWork.DeckRepository.getDeck(player1.id);
                player2.Deck.Container = _unitOfWork.DeckRepository.getDeck(player2.id);

                if (!player1.Deck.Container.Any())
                {
                    throw new Exception($"{player1.Authentication.Username} has not  configured a deck");
                }

                if (!player2.Deck.Container.Any())
                {
                    throw new Exception($"{player2.Authentication.Username} has not  configured a deck");
                }


                var battleResult = _battleHandler.StartBattle(player1, player2);

                if(battleResult.Winner is null)
                {
                    _unitOfWork.Rollback();
                    return battleResult;
                }

                _unitOfWork.UserRepository.UpdateStats(player1);
                _unitOfWork.UserRepository.UpdateStats(player2);


                // Transfer Ownership of Cards
                foreach (Card c in player1.Stack.Container)
                {
                    _unitOfWork.CardRepository.TransferCard(c.CardID, player1.id);
                }

                foreach (Card c in player2.Stack.Container)
                {
                    _unitOfWork.CardRepository.TransferCard(c.CardID, player2.id);
                }



                foreach (var c in battleResult.Winner.Deck.Container)
                {
                    _unitOfWork.DeckRepository.updateCardOwner(null, c.CardID, battleResult.Winner.id); //userid of the owner of the card
                }

                _unitOfWork.Commit();
                return battleResult;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }

}