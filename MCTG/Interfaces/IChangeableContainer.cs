using MCTG.Classes.CardStructure;

namespace MCTG.Interfaces;

internal interface IChangeableContainer
{
    public void RemoveCard(Card c);

    public void AddCard(Card c);
}