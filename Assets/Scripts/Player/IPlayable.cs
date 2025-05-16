using UnityEngine;

public interface IPlayable
{
    public void BeginTurn();
    public void EndTurn();
    public void GetCards(int numCard);
    public void RemoveCards(int numCard);
}
