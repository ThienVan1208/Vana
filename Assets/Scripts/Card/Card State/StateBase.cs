using UnityEngine;

public class StateBase : IState
{
    public bool isComplete { get; protected set; }
    protected bool isEnter = false;
    protected bool wait4Transit = false;
    protected Card myCard;
    public StateBase(Card card){
        myCard = card;
    }

    public virtual void OnEnter()
    {
        isEnter = true;
    }

    public virtual void OnExit()
    {
        
    }

    public virtual void OnUpdate()
    {
        if(wait4Transit || !isEnter) return;
    }
}
