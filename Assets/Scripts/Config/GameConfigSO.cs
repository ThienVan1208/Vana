using UnityEngine;


[CreateAssetMenu(fileName = "GameConfigSO", menuName = "Config/GameConfigSO")]
public class GameConfigSO : ScriptableObject
{
    public readonly int initCardNum = 10;
    public readonly int minCard2Play = 2;
    public readonly int maxCard2Play = 4;
}