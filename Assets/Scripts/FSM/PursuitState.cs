using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState : IState
{
    private StateMachine _agentFSM;
    private Agent _agent;
    private Vector3 _velocity;
    private Player player;
    private Vector3 _lastPosition;
    public PursuitState (StateMachine fsm, Agent agent)
    {
        _agentFSM = fsm;
        _agent = agent;
    }

    public void OnEnter()
    {
        player=_agent.GetPlayer();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        Pursuit();
    }
    public void Pursuit()
    {
        if(player == null)
        {
            return;
        }
        if (_agent.GetPlayerIsInSight())
        {
            float distance = Vector3.Distance(player.transform.position, _agent.transform.position);
            if (distance < _agent.GetMinDistanceToPursuit())
            {
                //ponerle y en 0
                Vector3 dir = player.transform.position + player.GetVelocity() - _agent.transform.position;
                dir.y = 0;
                _agent.transform.forward = dir.normalized;
                return;
            }
            Vector3 futurePosition = player.transform.position + player.GetVelocity();
            Vector3 desired = (futurePosition - _agent.transform.position);
            _lastPosition = player.transform.position;
            if (distance< player.GetVelocity().magnitude)
            {
                desired = player.transform.position - _agent.transform.position;
            }
            desired = desired.normalized * _agent.GetSpeed();
            desired.y = 0;
            Vector3 steering = Vector3.ClampMagnitude
                (desired - _velocity, _agent.GetMaxForcePursuit() / _agent.GetmaxForceDivisorPursuit());
            ApplyForce(steering);
            _agent.transform.position += _velocity * Time.deltaTime;
            _agent.transform.forward = _velocity;
            if (_velocity == Vector3.zero)
            {
                _agent.transform.forward = desired;
            }
        }
        else
        {
            _agent.NotifyGoToPlace(_lastPosition);
            _agentFSM.ChangeState(AgentStates.GoToPlace);
        }
    }
    private void ApplyForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _agent.GetSpeed());
    }
}
