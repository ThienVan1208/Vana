using UnityEngine;
using UnityEngine.UI;

public class Player : PlayerBase
{
    [SerializeField] private GameObject _handHolderPrefab;
    [SerializeField] private GameObject _playButtonPrefab;
    private Vector2 _initCardHolderPos = new Vector2(-146, 85);

    protected override void InitCardHolder()
    {
        base.InitCardHolder();

        // Create cardHolder.
        cardHolder = Instantiate(_handHolderPrefab
                        , _initCardHolderPos
                        , Quaternion.identity).GetComponent<HandHolder>();

        cardHolder.gameObject.transform.SetParent(mainCanvas.gameObject.transform as RectTransform);

        Vector2 anchorPos = new Vector2(0.5f, 0f);
        (cardHolder.gameObject.transform as RectTransform).anchorMin = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchorMax = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchoredPosition = _initCardHolderPos;

        // Create play-card button.
        Vector2 buttonPos = new Vector2(-146f, 15f);

        GameObject button = Instantiate(_playButtonPrefab, buttonPos, Quaternion.identity);
        button.transform.SetParent(mainCanvas.gameObject.transform, false);

        (button.transform as RectTransform).anchorMin = anchorPos;
        (button.transform as RectTransform).anchorMax = anchorPos;
        (button.transform as RectTransform).anchoredPosition = buttonPos;
        
        button.GetComponent<Button>().onClick.AddListener((cardHolder as HandHolder).PlayCard);
    }
    public override void AddCards(Card card)
    {
        card.SetCardHolder(cardHolder);
        cardHolder.AddCard(card);
    }
}
