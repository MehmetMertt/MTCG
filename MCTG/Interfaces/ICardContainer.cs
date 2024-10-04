using MCTG.Classes.CardStructure;

namespace MCTG.Interfaces;

internal interface ICardContainer
{
    public List<Card> Container { get; set; }

    public void RemoveCard(Card c);

    public void AddCard(Card c);


}