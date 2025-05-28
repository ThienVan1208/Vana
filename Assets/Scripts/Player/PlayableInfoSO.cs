using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayableInfoSO", menuName = "PlayableInfoSO")]
public class PlayableInfoSO : ScriptableObject
{
    public int curPlayerIdx = 0, prevPlayerIdx = -1;
}
