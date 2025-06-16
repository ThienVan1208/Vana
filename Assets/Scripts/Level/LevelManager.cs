using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelInfoSO _levelInfoSO;
    [SerializeField] private VoidEventSO _levelUpEventSO;
    private void OnEnable()
    {
        _levelUpEventSO.EventChannel += _levelInfoSO.GetLevelUp;
    }
    private void OnDisable()
    {
        _levelUpEventSO.EventChannel -= _levelInfoSO.GetLevelUp;
    }
}
