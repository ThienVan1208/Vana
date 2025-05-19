using UnityEngine;

public class VirtualPlayer : PlayerBase
{
    [SerializeField] private GameObject _handHolderPrefab;
    private Vector2 _initCardHolderPos = new Vector2(-146f, -50f);
    protected override void InitCardHolder()
    {
        base.InitCardHolder();

        // Create cardHolder.
        cardHolder = Instantiate(_handHolderPrefab
                        , _initCardHolderPos
                        , Quaternion.identity).GetComponent<VirtualHandHolder>();

        cardHolder.gameObject.transform.SetParent(mainCanvas.gameObject.transform as RectTransform);

        Vector2 anchorPos = new Vector2(0.5f, 1f);
        (cardHolder.gameObject.transform as RectTransform).anchorMin = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchorMax = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchoredPosition = _initCardHolderPos;

    }
    public override void AddCards(Card card)
    {
        card.SetCardHolder(cardHolder);
        card.FaceCardDown();
        cardHolder.AddCard(card);
    }
}
