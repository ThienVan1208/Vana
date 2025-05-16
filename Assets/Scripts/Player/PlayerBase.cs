using UnityEngine;

public class PlayerBase : MonoBehaviour, IPlayable
{
    [SerializeField] protected Canvas mainCanvas;
    protected CardHolder cardHolder;
    protected virtual void Awake()
    {
        InitCardHolder();
    }
    protected virtual void PlayCards(){}
    protected virtual void ChooseCard(){}
    public virtual void GetCards(int numCard) { }
    public virtual void RemoveCards(int numCard){}
    public virtual void BeginTurn()
    {
        if (GameManager.FirstTurn)
        {
            GameManager.FirstTurn = false;
            PlayCards();
        }
        else
        {

        }
    }
    public virtual void EndTurn(){}
    
    protected virtual void InitCardHolder(){}
}
