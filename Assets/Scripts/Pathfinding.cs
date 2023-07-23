using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public List<Node> ConstructPathAStar(Vector3 origin, Vector3 goal)
    {
        if (IsPointInSight(origin, goal))
        {
            return new List<Node>();
        }
        Node startingNode;
        Node goalNode;
        if (origin == null || goal == null)
        {
            return null;
        }
        startingNode =GetClosestNodeToPosition(origin);
        goalNode = GetClosestNodeToPosition(goal);
    
        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);

        Dictionary<Node, Node> CameFrom = new Dictionary<Node, Node>();
        CameFrom.Add(startingNode, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(startingNode, 0);

        while(frontier.Count > 0)
        {
            Node current = frontier.Get();
            if(current == goalNode)
            {
                List<Node> path = new List<Node>();
                Node nodeToAdd = current;
                while (nodeToAdd != null)
                {
                    path.Add(nodeToAdd);
                    nodeToAdd = CameFrom[nodeToAdd];
                }
                path.Reverse();
                return path;
            }
            foreach (var next in current.GetNeighbors())
            {
                float dist = Vector3.Distance(goalNode.transform.position, next.transform.position);
                float newCost = costSoFar[current] + next.GetCost();
                float priority = newCost + dist * dist;

                if (!CameFrom.ContainsKey(next))
                {
                    frontier.Put(next, priority);
                    CameFrom.Add(next, current);
                    costSoFar.Add(next, newCost);
                }
                else
                {
                    if(newCost < costSoFar[next])
                    {
                        frontier.Put(next, priority);
                        CameFrom[next] = current;
                        costSoFar[next] = newCost;
                    }
                }
            }
        }
        return null;
    }
    public Node GetClosestNodeToPosition(Vector3 position)
    {
        if(position == null)
        {
            return null;
        }
        List<Node> nearbyNodes = new List<Node>();
        Node findedNode = new Node();
        Collider[] go;
        float radius = Mathf.Infinity;
        go = Physics.OverlapSphere(position, radius
           , GameManager.instance.nodeLayer);
        foreach (var item in go)
        {
            Vector3 direction = item.transform.position - position;
            if (!Physics.Raycast(position, direction
                ,direction.magnitude, GameManager.instance.wallLayer))
            {
                if (Physics.Raycast(position, direction
                    ,direction.magnitude, GameManager.instance.nodeLayer))
                {
                    nearbyNodes.Add(item.GetComponent<Node>());
                }
            }
        }
        if (nearbyNodes.Count < 1)
        {
            foreach (var item in go)
            {
                nearbyNodes.Add(item.GetComponent<Node>());
            }
        }
        float distance = Mathf.Infinity;
            foreach (var node in nearbyNodes)
            {
                float dis = Vector3.Distance(node.transform.position, position);
                if (dis < distance)
                {
                    distance = dis;
                    findedNode = node;
                }
            }
        return findedNode;
    }
    public List<Node> ClosestPathToPatrolRoute(List<Node> pathToFind, Vector3 origin)
    {
        Node startingNode;
        Node goalNode;
        List<Node> auxList=new List<Node>();
        List<Node> path = new List<Node>();
        if (origin == null || pathToFind == null)
        {
            return null;
        }
        for (int i = 0; i < pathToFind.Count; i++)
        {
            startingNode = GetClosestNodeToPosition(origin);
            goalNode = pathToFind[i];

            PriorityQueue frontier = new PriorityQueue();
            frontier.Put(startingNode, 0);

            Dictionary<Node, Node> CameFrom = new Dictionary<Node, Node>();
            CameFrom.Add(startingNode, null);

            Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
            costSoFar.Add(startingNode, 0);

            while (frontier.Count > 0) 
            {
                path.Clear();
                Node current = frontier.Get(); 
                if (current == goalNode)  
                {
                    Node nodeToAdd = current; 
                    while (nodeToAdd != null) 
                    {
                        path.Add(nodeToAdd);   
                        nodeToAdd = CameFrom[nodeToAdd]; 
                    }
                    break; 
                }
                foreach (var next in current.GetNeighbors()) 
                {
                    float dist = Vector3.Distance(goalNode.transform.position, next.transform.position);
                    float newCost = costSoFar[current] + next.GetCost();
                    float priority = newCost + dist * dist;
                    
                    if (!CameFrom.ContainsKey(next)) 
                    {
                        frontier.Put(next, priority);
                        CameFrom.Add(next, current);
                        costSoFar.Add(next, newCost);
                    }
                    else 
                    {
                        if (newCost < costSoFar[next]) 
                        {                              
                            frontier.Put(next, priority); 
                            CameFrom[next] = current;
                            costSoFar[next] = newCost;
                        }
                    }
                }
            }
        }
        if (auxList.Count < path.Count || auxList.Count <= 0)
        {
            auxList = path;
        }
        if (IsPointInSight(origin, auxList[0].transform.position))
        {
            return new List<Node>();
        }
        auxList.Reverse();
        for (int i = 0; i < auxList.Count-1; i++) //aca smootheamos si del origen podemos ver al siguiente entonces sacamos al actual
        {
            if (NodeIsInLineOfSight(origin, auxList[i + 1]) 
                && Vector3.Distance(origin, auxList[i + 1].transform.position)
                <Vector3.Distance(auxList[i].transform.position, auxList[i+1].transform.position))
            {
                auxList.RemoveAt(i);
            }
        }
        for (int i = 0; i < auxList.Count; i++) //aca hacemos varios bucles
        {
            for (int h = 0; h < pathToFind.Count; h++) //para comparar distintas listas
            {
                if (auxList[i] == pathToFind[h]) //y si coinciden limpiamos el resto porque ya llego a
                {
                    for (int j = i+1; j < auxList.Count; j++) // la zona de patrulla 
                    {
                        auxList.RemoveAt(j);
                    }
                }
            }
        }
        return auxList;
    }
    private bool NodeIsInLineOfSight(Vector3 position, Node node)
    {
        Vector3 direction = node.transform.position - position;
        if (!Physics.Raycast(position, direction
            ,direction.magnitude, GameManager.instance.wallLayer))
        {
            if (Physics.Raycast(position, direction
                , direction.magnitude, GameManager.instance.nodeLayer))
            {
                return true;
            }
        }
        return false;

    }
    private bool IsPointInSight(Vector3 origin, Vector3 goal)
    {
        Vector3 direction = goal - origin;
        if (!Physics.Raycast(origin, direction
            , direction.magnitude, GameManager.instance.wallLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
