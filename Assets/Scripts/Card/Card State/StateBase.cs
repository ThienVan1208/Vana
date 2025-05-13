using UnityEngine;

public class StateBase : IState
{
    public bool isComplete { get; protected set; }
    protected bool _isEnter = false;
    protected bool _wait4Transit = false;
    protected FSM _stateMachine;
    protected Card _myCard;
    public StateBase(FSM statemachine, Card card){
        _stateMachine = statemachine;
        _myCard = card;
    }

    public virtual void OnEnter()
    {
        _isEnter = true;
    }

    public virtual void OnExit()
    {
        
    }

    public virtual void OnUpdate()
    {
        if(_wait4Transit || !_isEnter) return;
    }
}
