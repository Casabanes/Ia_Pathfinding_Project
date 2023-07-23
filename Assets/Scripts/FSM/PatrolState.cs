using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{

    private List<Node> waypoints = new List<Node>();
    private int index;
    private Vector3 direction;
    private float _minDistance = 0.1f;
    private Agent _agent;
    private StateMachine _fsm;
    private bool ignoreAfterFirstStart;
    public PatrolState (StateMachine fsm, Agent agent)
    {
        _agent = agent;
        _fsm = fsm;
        waypoints = _agent.GetWaypoints();
        index = _agent.GetStartingIndexPatrol();
    }

    public void OnEnter()
    {
        if (!ignoreAfterFirstStart)
        {
            ignoreAfterFirstStart = true;
            return;
        }
        ActualizeIndexUsingActualNode();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        CheckWaypoints();
        Patrol();
    }

    void Patrol()
    {
        if (_agent.GetPlayerIsInSight())
        {
            _fsm.ChangeState(AgentStates.Pursuit);
        }
        if (_agent.goingToLook)
        {
            _fsm.ChangeState(AgentStates.GoToPlace);
        }
        if (waypoints.Count < 1)
        {
            return;
        }
        if (index + 1< waypoints.Count)
        {
            if (IsNodeInSight(waypoints[index+1], _agent.transform.position))
            {
                index++;
            }
        }
        direction = waypoints[index].transform.position - _agent.transform.position;
        _agent.transform.forward = direction.normalized;
            Movment(); 
        if (direction.magnitude < _minDistance) 
        {
            index++;
            if (index >= waypoints.Count)
            {
                index = 0; 
            }
        }
    }
    private void CheckWaypoints()
    {
        if (waypoints.Count < 1)
        {
            ActualizeIndexUsingActualNode();
        }
    }
    private void Movment()
    {
        _agent.transform.position += direction.normalized * _agent.GetSpeed() * Time.deltaTime;
    }
    private void ActualizeIndexUsingActualNode()
    {
        waypoints = _agent.GetWaypoints();
        if (!AnyNodeIsInSight())
        {
            _fsm.ChangeState(AgentStates.GoBackToPatrol);
        }
    }
    private bool AnyNodeIsInSight()
    {
        index = 0;
        for (int i = waypoints.Count-1; 0<i; i--)
        {
            if (IsNodeInSight(waypoints[i], _agent.transform.position))
            {
                        index = i;
                        if (i == 0)
                        {
                            return true;
                        }    
            }
        }
        if (index == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool IsNodeInSight(Node node,Vector3 origin)
    {
        Vector3 direction = node.transform.position - origin;
        if (!Physics.Raycast(origin, direction
            , direction.magnitude, GameManager.instance.wallLayer))
        {
            if (Physics.Raycast(_agent.transform.position, direction
                , direction.magnitude, GameManager.instance.nodeLayer))
            {
                    return true;
            }
        }
            return false;
    }
}
