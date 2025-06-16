using UnityEngine;


[CreateAssetMenu(fileName = "LevelInfoSO", menuName = "LevelInfoSO")]
public class LevelInfoSO : ScriptableObject
{
    [SerializeField] private int _level;
    public void SetLevel(int val)
    {
        _level = val;
    }
    public void GetLevelUp()
    {
        _level++;
    }
    public int GetLevel()
    {
        return _level;
    }
}
