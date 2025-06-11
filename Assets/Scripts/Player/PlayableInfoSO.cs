using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayableInfoSO", menuName = "PlayableInfoSO")]
public class PlayableInfoSO : ScriptableObject
{
    private List<IPlayable> _playableList = new List<IPlayable>();
    public int curPlayerIdx = 0, prevPlayerIdx = -1;
    private int _totalPlayerNum = 0;
    public int GetTotalPlayerNum() { return _totalPlayerNum; }
    public void SetTotalPlayerNum(int num)
    {
        _totalPlayerNum = num;
    }
    public void AddNewPlayer(IPlayable newPlayer)
    {
        _playableList.Add(newPlayer);
        SetTotalPlayerNum(_playableList.Count);
    }
    public IPlayable GetPlayerByIndex(int index)
    {
        if (index >= _playableList.Count)
        {
            Debug.LogWarning("Player index is out of bound.");
            return null;
        }
        return _playableList[index];
    }

    public void ForEeachPlayableList(Action<IPlayable> func)
    {
        foreach (IPlayable playable in _playableList)
        {
            func?.Invoke(playable);
        }
    }
    public IReadOnlyList<IPlayable> GetPlayableList()
    {
        return _playableList.AsReadOnly();
    }
}
