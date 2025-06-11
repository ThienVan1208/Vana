using UnityEngine;

public interface IPlayable
{
    public void BeginTurn();
    public void EndTurn();
    public void AddCards(Card card);
    public void RemoveCards(Card card);
    public void WinGame();
    public void LoseGame();
    public int GetCardNum();
}
