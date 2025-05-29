using UnityEngine;

public class Test : MonoBehaviour
{
    public Card card;
    public RectTransform cardSlot;

    public void GetCardMove()
    {
        card.GetMove(cardSlot);
    }
}
