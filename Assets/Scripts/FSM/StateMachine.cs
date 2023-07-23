using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState _currentState;
    public Dictionary<AgentStates, IState> _allStates = new Dictionary<AgentStates, IState>();
    
    public void OnUpdate()
    {
        if (_currentState != null)
        {
            _currentState.OnUpdate();
        }
        else
        {
            if (_allStates[0] != null)
            {
                _currentState = _allStates[0];
            }
            else
            {
                Debug.Log("Estamos fritos");
            }
        }
    }
    public void AddState(AgentStates key, IState state)
    {
        if (_allStates.ContainsKey(key))
        {
            return;
        }
        _allStates.Add(key,state);
    }
    public void ChangeState(AgentStates key)
    {
        if (!_allStates.ContainsKey(key))
        {
            return;
        }
        if (_currentState != null)
        {
            _currentState.OnExit();
            _currentState = _allStates[key];
            _currentState.OnEnter();
        }
    }
}
