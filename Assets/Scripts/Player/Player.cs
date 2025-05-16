using UnityEngine;

public class Player : PlayerBase
{
    [SerializeField] private GameObject _handHolderPrefab;
    protected override void InitCardHolder()
    {
        base.InitCardHolder();
        // cardHolder = Instantiate(_handHolderPrefab
        //                 , (mainCanvas.gameObject.transform as RectTransform).position
        //                 , Quaternion.identity).GetComponent<HandHolder>();

        // cardHolder.gameObject.transform.SetParent(mainCanvas.gameObject.transform);
        // (cardHolder.gameObject.transform as RectTransform).localPosition = new Vector3(0, 120f, 0f);
    }
}
