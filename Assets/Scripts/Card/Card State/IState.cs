using UnityEngine;

public enum CardState{
    Idle,
    Drag,
    Hover,
    Click
}
public interface IState
{
    public void OnEnter();
    public void OnUpdate();
    public void OnExit();
}
