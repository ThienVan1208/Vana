using UnityEngine;

[CreateAssetMenu(fileName = "CardInfoSO", menuName = "Card/CardInfoSO")]
public class CardInfoSO : ScriptableObject {
    [SerializeField] public CardRank cardRank;
    [SerializeField] public CardSuit cardSuit;
}
