using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer;
using MTCG.Classes.CardStructure;

namespace MTCG.Classes
{
    public class ShopItem
    {
        public Card Offer { get; set; }
        public CardRequirement Requirement { get; set; }

        public int OfferId { get; set; }

        public ShopItem(int offerId, Card offer, CardRequirement requirement)
        {
            OfferId = offerId;
            Offer = offer;
            Requirement = requirement;
        }

        public string toString()
        {
            return $"Player {this.Offer.OwnerId} offers {this.Offer.getInfo()} and wants {this.Offer.getInfo()}";
        }
    }

    public class CardRequirement
    {
        public int minimumDamage { get; set; }
        public bool monsterCard { get; set; }
        public ElementTypes? element { get; set; }

        public CardRequirement(bool monsterCard, ElementTypes? element, int minimumDamage = 0)
        {
            if (minimumDamage == null)
                this.minimumDamage = 0;
            this.element = element;
            this.monsterCard = monsterCard;

        }
    }

    public class TradingHandler
    {
        private static TradingHandler _instance;
        private static readonly object padlock = new object();
        public List<ShopItem> shopItems { get; set; }

        private TradingHandler()
        {
            shopItems = new List<ShopItem>();
        }

        public static TradingHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new TradingHandler();
                    }
                    return _instance;
                }
            }
        }

        public void sentOffer(int offerId, Card offer, CardRequirement requirement)
        {
            var shopitem = new ShopItem(offerId,offer, requirement);
            shopItems.Add(shopitem);
        }

        public bool acceptOffer(int offerId, Card c)
        {


            var shopitem = this.shopItems.FirstOrDefault(shi => shi.OfferId == offerId);
            if (shopitem is null)
            {
                throw new ArgumentOutOfRangeException($"Offer number {offerId} does not exist");
            }

            var requirement = shopitem.Requirement;

            if (c.Damage < requirement.minimumDamage)
            {
                return false;
            }
            if ((requirement.monsterCard && c is not MonsterCards) || (!requirement.monsterCard && c is not SpellCards))
            {
                return false;
            }

            if (requirement.element != null && c.ElementTyp != requirement.element)
            {
                return false;
            }

            return true;

        }

        public bool deleteOffer(int offerId)
        {

            var offer = this.shopItems.FirstOrDefault(shi => shi.OfferId == offerId);
            if (offer is null)
            {
                throw new ArgumentOutOfRangeException($"Offer number {offerId} does not exist");
            }

            shopItems.Remove(offer);
            return true;
        }
    }
}
