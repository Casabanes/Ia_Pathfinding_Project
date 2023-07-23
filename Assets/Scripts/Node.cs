using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]
    private List<Node> neighbors = new List<Node>();
    [SerializeField]
    private int _cost=1;
    private void Start()
    {
        if (neighbors.Count<1)
        {
            neighbors=GetNeighbors();
        }
    }
    public float GetCost()
    {
        return _cost;
    }
    public List<Node> GetNeighbors()
    {
        if (neighbors.Count > 0)
        {
            return neighbors;
        }
        RaycastHit hit;
        float radius = Mathf.Infinity;
        Collider[] go;
        go= Physics.OverlapSphere(transform.position, radius
            , GameManager.instance.nodeLayer);
        List<Node> findedNeighbors = new List<Node>();

        foreach (var item in go)
        {
            Vector3 direction = item.transform.position - transform.position;
            if(!Physics.Raycast(transform.position,direction
                , out hit, direction.magnitude,GameManager.instance.wallLayer))
            {
                if(Physics.Raycast(transform.position, direction
                    ,out hit, direction.magnitude, GameManager.instance.nodeLayer))
                {
                    findedNeighbors.Add(item.GetComponent<Node>());
                }
            }
        }
        neighbors = findedNeighbors;
        return neighbors;
    }
}
