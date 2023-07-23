using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum AgentStates
    {
        Patrol,
        Pursuit,
        GoToPlace,
        GoBackToPatrol
    }
public class Agent : MonoBehaviour
{
    public enum ColorState
    {
        White,
        Red,
        Lightblue,
        Yellow
    }
    [SerializeField]
    private ColorState _patrolRoute;
    private ColorState _actualPatrolRoute;

    [SerializeField]
    private Material material;

    [SerializeField]
    private float speed;
    private StateMachine _fsm;
    private List<Node> _waypoints;
    private GoToPlace _goToPlace;

    [SerializeField]
    private int _startingIndexPatrol;

    [SerializeField]
    private float viewRadius;
    [SerializeField]
    private float viewAngle;
    private Vector3 placeToGo;
    [SerializeField]
    private Player player;

    [SerializeField]
    private float maxForcePursuit;
    [SerializeField]
    private float maxForceDivisorPursuit;
    [SerializeField]
    private bool _playerIsInLineOfSight;
    public bool goingToLook;
    [SerializeField]
    private float minDistanceToPursuit;
    private void Start()
    {
        if (material == null)
        {
            material = gameObject.GetComponent<Material>();
        }
        if(player == null)
        {
            player = FindObjectOfType<Player>();
        }
        ChangeColorRoute();
        _fsm = new StateMachine();
        _actualPatrolRoute = _patrolRoute;
        _fsm.AddState(AgentStates.Patrol, new PatrolState(_fsm,this));
        _fsm.AddState(AgentStates.Pursuit, new PursuitState(_fsm,this));
        _fsm.AddState(AgentStates.GoToPlace, new GoToPlace(_fsm,this));
        _fsm.AddState(AgentStates.GoBackToPatrol, new GoBackToPatroolRoute(_fsm, this));
        _fsm.ChangeState(AgentStates.Patrol);
    }
    private void Update()
    {
        FieldOfView();
        CheckColorRoute();
        _fsm.OnUpdate();

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log(_fsm._currentState +"   "  + gameObject.name);
        }
    }
    public void CheckColorRoute()
    {
        if (_actualPatrolRoute != _patrolRoute)
        {
            ChangeColorRoute();
            _actualPatrolRoute = _patrolRoute;
        }
    }
    public void ChangeColorRoute()
    {
        switch (_patrolRoute)
        {
            case (ColorState.White):
                material.color = GameManager.instance.whiteColor;
                _waypoints = GameManager.instance._whitePath;
                break;
            case (ColorState.Red):
                material.color = GameManager.instance.redColor;
                _waypoints = GameManager.instance._redPath;
                break;
            case (ColorState.Lightblue):
                material.color = GameManager.instance.lightblueColor;
                _waypoints = GameManager.instance._lightbluePath;
                break;
            case (ColorState.Yellow):
                material.color = GameManager.instance.yellowColor;
                _waypoints = GameManager.instance._yellowPath;
                break;
        }
    }
    public float GetSpeed()
    {
        return speed;
    }
    public List<Node> GetWaypoints()
    {
        return _waypoints;
    }
    public int GetStartingIndexPatrol()
    {
        return _startingIndexPatrol;
    }
    
    public void SetGoToPlacelIState(GoToPlace g)
    {
        _goToPlace = g;
    }
    public Vector3 GetPlaceToGo()
    {
        return placeToGo;
    }
    public bool GetPlayerIsInSight()
    {
        return _playerIsInLineOfSight;
    }
    public Player GetPlayer()
    {
        return player;
    }
    public float GetMaxForcePursuit()
    {
        return maxForcePursuit;
    }
    public float GetmaxForceDivisorPursuit()
    {
        return maxForceDivisorPursuit;
    }
    public float GetMinDistanceToPursuit()
    {
        return minDistanceToPursuit;
    }
    public void NotifyGoToPlace(Vector3 position)
    {
        if (!_playerIsInLineOfSight)
        {
            goingToLook = true;
        }
        else
        {
            goingToLook = false;
        }
        placeToGo = position;
        if (!goingToLook)
        {
            _goToPlace.GoToLook(position);
        }
    }
    public void FieldOfView()
    {
            Vector3 dir = player.transform.position - transform.position;
            if (dir.magnitude > viewRadius)
            {
                _playerIsInLineOfSight = false;
                return;
            }
            float angle = Vector3.Angle(transform.forward, dir.normalized);
            if (angle <= viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dir, dir.magnitude, GameManager.instance.wallLayer))
                {
                    _playerIsInLineOfSight = true;
                    foreach (var item in GameManager.instance.allAgents)
                    {
                      item.NotifyGoToPlace(player.transform.position);
                    }
                }
                else
                {
                    _playerIsInLineOfSight = false;
                }
            }
            else
            {
                _playerIsInLineOfSight = false;
            }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 lineA = GetVectorFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 lineB = GetVectorFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + lineA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * viewRadius);
    }
    Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
