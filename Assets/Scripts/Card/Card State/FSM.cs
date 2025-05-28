using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM
{
    private StateBase _curState;
    private StateBase _defaultState;
    /* 
    - Key is type of statebase, value is list of statebase that key state can change to.
    - Ex: DragState(Key) can change to [ClickState, IdleState, HoverState](Value).
    - When state needs changing to next state, just search in the list [ClickState, IdleState, HoverState].
    */
    private Dictionary<Type, List<StateBase>> _transition = new Dictionary<Type, List<StateBase>>();

    // Used to get value of @_transition dic for searching next state easily.
    private List<StateBase> _curTransitionList = new List<StateBase>();
    public void Update()
    {
        _curState?.OnUpdate();
    }
    public void InitFSM()
    {
        ChangeState(_defaultState, isForce: true, isExit: false);
    }
    public void ChangeState(StateBase state, bool isForce = false, bool isExit = true, bool isEnter = true)
    {
        if (!isForce && !_curState.isComplete) return;

        if (isExit) _curState.OnExit();
        _curState = state;
        if (isEnter) _curState.OnEnter();
    }
    public void SetDefaultState(StateBase state)
    {
        _defaultState = state;
    }

    public void AddTransit(StateBase from, StateBase to)
    {

        // If transition List of @from is not add to @_transition -> create new list and then add that list.
        if (_transition.TryGetValue(from.GetType(), out var transForThisType) == false)
        {
            transForThisType = new List<StateBase>();
            _transition[from.GetType()] = transForThisType;
        }
        transForThisType.Add(to);
    }

    public void RequestChangeState(StateBase to = null)
    {
        // If @to != null, find @to in Value of @_transition[@_curState].
        if (to != null)
        {
            // If there is no transition for @_curState -> create new transition and then add @to to that transition.
            if (_transition.TryGetValue(_curState.GetType(), out _curTransitionList) == false)
            {
                AddTransit(_curState, to);
                _curTransitionList = _transition[_curState.GetType()];
            }

            // If @_curState has transition list -> find @to to change state.
            foreach (var state in _curTransitionList)
            {
                if (state == to)
                {
                    ChangeState(to, isForce: true);
                    return;
                }
            }
            // If there is no @to in transition list -> error.
            Debug.LogError("Can not find state " + to.GetType() + "in transition of current state " + _curState.GetType() + "!!!");
        }

        // If @to = null, change to the first state in transition list of @_curState.
        else
        {
            // If there is no transition for @_curState -> create new transition and then add @_defaultState to that transition.
            if (_transition.TryGetValue(_curState.GetType(), out _curTransitionList) == false)
            {
                AddTransit(_curState, _defaultState);
                ChangeState(_defaultState);
                
                return;
            }
            // Change to the first state in transition list of @_curState.
            ChangeState(_curTransitionList[0], isForce: true);
            
        }
    }
}
