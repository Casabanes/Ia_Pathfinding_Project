using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBackToPatroolRoute : IState
{
    private List<Node> waypoints = new List<Node>();
    private int index;
    private Vector3 direction;
    private float _minDistance = 0.1f;
    private Agent _agent;
    private Pathfinding _pf;
    private StateMachine _fsm;
    public GoBackToPatroolRoute(StateMachine fsm, Agent agent)
    {
        _agent = agent;
        _fsm = fsm;
        _pf = new Pathfinding();
    }

    public void OnEnter()
    {
        index = 0;
        SetPath(_agent.GetWaypoints());
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        Patrol();
    }

    void Patrol()
    {
        if (_agent.GetPlayerIsInSight())
        {
            _fsm.ChangeState(AgentStates.Pursuit);
        }
            direction = waypoints[index].transform.position - _agent.transform.position;
            _agent.transform.forward = direction.normalized;
            Movment();
        if (direction.magnitude < _minDistance)
        {
            index++;
            if (index >= waypoints.Count)
            {
                _fsm.ChangeState(AgentStates.Patrol);
            }
        }
    }
    private void Movment()
    {
        _agent.transform.position += direction.normalized * _agent.GetSpeed() * Time.deltaTime;
    }
    public void SetPath(List<Node> path)
    {
        if (path == null)
        {
            return;
        }
        FindPathToThePath(path);
    }
    private void FindPathToThePath(List<Node> path)
    {
        waypoints = _pf.ClosestPathToPatrolRoute(path, _agent.transform.position);
        if (waypoints.Count < 1)
        {
            _fsm.ChangeState(AgentStates.Patrol);
        }
    }
}
