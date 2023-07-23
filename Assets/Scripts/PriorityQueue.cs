using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    private Dictionary<Node, float> _allNodes = new Dictionary<Node, float>();
    public int Count
    {
        get { return _allNodes.Count; }
    }
    public void Put(Node n, float cost)
    {
        if (_allNodes.ContainsKey(n))
        {
            _allNodes[n] = cost;
        }
        else
        {
            _allNodes.Add(n, cost);
        }
    }
    public Node Get()
    {
        Node node = null;
        float lowestValue = Mathf.Infinity;
        foreach (var item in _allNodes)
        {
            if(item.Value < lowestValue)
            {
                lowestValue = item.Value;
                node = item.Key;
            }
        }
        _allNodes.Remove(node);
        return node;
    }
    public void Clear()
    {
        _allNodes.Clear();
    }
}
