using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllCardContainerSO", menuName = "Card/AllCardContainerSO")]
public class AllCardContainerSO : ScriptableObject
{
    [SerializeField] private List<GameObject> _allCardsPrefab = new List<GameObject>();
    [SerializeField] private List<GameObject> _inGameCardList = new List<GameObject>();
    public void Init()
    {
        while (_inGameCardList.Count != 0)
        {
            _allCardsPrefab.Add(_inGameCardList[0].gameObject);
            _inGameCardList.RemoveAt(0);
        }   
    }
    public GameObject GetRandomCardPrefab()
    {
        int randomNum = Random.Range(0, _allCardsPrefab.Count);
        GameObject randomCard = _allCardsPrefab[randomNum];
        _inGameCardList.Add(randomCard);
        _allCardsPrefab.Remove(randomCard);
        return randomCard;
    }
}
