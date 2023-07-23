using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlace : IState
{

    private List<Node> waypoints = new List<Node>();
    private int index;
    private Vector3 direction;
    private float _minDistance = 0.1f;
    private Agent _agent;
    private Pathfinding _pf;
    private Vector3 _placeToGo;
    private StateMachine _fsm;
    public GoToPlace(StateMachine fsm, Agent agent)
    {
        _agent = agent;
        _fsm = fsm;
        _agent.SetGoToPlacelIState(this);
        index = _agent.GetStartingIndexPatrol();
        _pf = new Pathfinding();
    }

    public void OnEnter()
    {
        GoToLook(_agent.GetPlaceToGo());
    }

    public void OnExit()
    {
        _agent.goingToLook = false;
    }

    public void OnUpdate()
    {
        CheckGoingToPlace();
        Patrol();
    }
    private void Patrol()
    {
        if (index >= waypoints.Count)
        {
            if (!PlaceIsInLineOfSight(_placeToGo))
            {
                _fsm.ChangeState(AgentStates.GoBackToPatrol);
            }
            else
            {
                LookForPoint();
            }
            return;
        }
        direction = waypoints[index].transform.position - _agent.transform.position;
        if (direction.magnitude < _minDistance)
        {
            index++;
        }
        Movment();
      
    }
    private void LookForPoint()
    {
        direction = _placeToGo - _agent.transform.position;
        Movment();
        if (direction.magnitude < _minDistance)
        {
            _agent.goingToLook = false;
            _fsm.ChangeState(AgentStates.GoBackToPatrol);
        }
    }
    private void CheckGoingToPlace()
    {
        if (_agent.GetPlayerIsInSight())
        {
            _fsm.ChangeState(AgentStates.Pursuit);
        }
    }
    private void Movment()
    {
        direction.y = 0;
        _agent.transform.position += direction.normalized * _agent.GetSpeed() * Time.deltaTime;
        _agent.transform.forward = direction.normalized;
    }
    public void GoToLook(Vector3 place)
    {
        index = 0;
        _placeToGo = place;
        waypoints = _pf.ConstructPathAStar(_agent.transform.position, _placeToGo);
        if ( (waypoints.Count < 1 && !PlaceIsInLineOfSight(_placeToGo)))
        {
            _agent.goingToLook = false;
            _fsm.ChangeState(AgentStates.GoBackToPatrol);
        }
    }
    public bool PlaceIsInLineOfSight(Vector3 place)
    {
        Vector3 direction = place-_agent.transform.position;
        RaycastHit hit;
        if (!Physics.Raycast(_agent.transform.position, direction,out hit
            , direction.magnitude, GameManager.instance.wallLayer))
        {
                return true;
        }
        else
        {
            if (Vector3.Distance(hit.transform.position, place) < 0.2)
            {
                _placeToGo = hit.transform.position;
                return true;
            }
        }
        return false;
    }
}
