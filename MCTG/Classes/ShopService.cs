using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer;
using MTCG.Classes.CardStructure;

namespace MTCG.Classes
{
    public class ShopService : Service<ShopService>
    {
        public ShopService(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<ShopItem> GetOffers()
        {
            return _unitOfWork.ShopRepository.Get();
        }

        public bool ListItem(Card card, CardRequirement requirement)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                //Überprüfe ob User, die Karte im Deck hat.
                if (_unitOfWork.DeckRepository.getDeck(card.OwnerId).Any(deckCard => deckCard.CardID == card.CardID))
                {
                    throw new Exception("Please take the card out of your deck before listing it");
                }

                if(_unitOfWork.ShopRepository.Get().Exists(c => c.Offer.CardID == card.CardID))
                {
                    throw new Exception("Card is already inserted");
                }
                _unitOfWork.CardRepository.LockCard(card.CardID, true); // Lock the card from beeing used


                //erstelle das Inserat
                int offerId = _unitOfWork.ShopRepository.Insert(card.CardID, requirement.minimumDamage, requirement.monsterCard,
                    requirement.element);

                TradingHandler.Instance.sentOffer(offerId,card, requirement);

                _unitOfWork.Commit();
                return true;

            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw;
            }

        }

        public bool DeleteOffer(int offerId, int userId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var card = _unitOfWork.ShopRepository.Get().FirstOrDefault(c => c.OfferId == offerId);
                if (card is null)
                {
                    throw new Exception("This card does not exist");

                }
                _unitOfWork.ShopRepository.DeleteOffer(offerId,userId);
                _unitOfWork.CardRepository.LockCard(card.Offer.CardID,false);
                _unitOfWork.Commit();
                TradingHandler.Instance.deleteOffer(offerId);
                
                return true;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw;
            }

            return false;
        }

        public bool AcceptOffer(int offerId, Card cardOffer, int userId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                List<ShopItem> offers = _unitOfWork.ShopRepository.Get();
                TradingHandler.Instance.shopItems = offers;
                var offer = offers.FirstOrDefault(c => c.OfferId == offerId);
                if (offer == null)
                {
                    Console.WriteLine("Available offers:");
                    foreach (var o in offers)
                    {
                        Console.WriteLine($"OfferId: {o.OfferId}");
                    }
                    throw new Exception("This offer does not exist");
                }

                if (offer.Offer.OwnerId == userId)
                {
                    throw new Exception("You cannot trade with yourself");
                }
                _unitOfWork.ShopRepository.DeleteOffer(offerId);
                _unitOfWork.CardRepository.TransferCard(offer.Offer.CardID, userId);
                _unitOfWork.CardRepository.LockCard(offer.Offer.CardID, false);
                _unitOfWork.Commit();
                bool clientSuccess = TradingHandler.Instance.acceptOffer(offerId,cardOffer);
                if (!clientSuccess)
                {
                    throw new Exception("Accepting the offer failed on the client-side");
                }
                return true;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        
    }
}
