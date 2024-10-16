using MTCG.Classes.CardStructure;

namespace MTCG.Interfaces;

internal interface IChangeableContainer
{
    public void RemoveCard(Card c);

    public void AddCard(Card c);
}