using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    // public static GameManager Instance;
    // private void Awake()
    // {
    //     if(Instance == null){
    //         Instance = this;
    //     }
    //     else{
    //         Destroy(gameObject);
    //     }
    // }
    [SerializeField] private List<Card> _chosenCards = new List<Card>();
    [SerializeField] private ChosenCardEventSO _chosenCardEventSO;
    private void PlayCards(List<Card> chosenCards){
        _chosenCards = chosenCards;
    }

}
