using UnityEngine;

[CreateAssetMenu(fileName = "CardInfoSO", menuName = "Card/CardInfoSO")]
public class CardInfoSO : ScriptableObject {
    public CardRank cardRank;
    public CardSuit cardSuit;
}
