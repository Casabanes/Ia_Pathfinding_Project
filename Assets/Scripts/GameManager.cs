using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LayerMask nodeLayer;
    public LayerMask wallLayer;
    public LayerMask layerPlayer;
    public static GameManager instance;
    public List<Node> _whitePath;
    public List<Node> _redPath;
    public List<Node> _lightbluePath;
    public List<Node> _yellowPath;
    public Color32 whiteColor;
    public Color32 redColor;
    public Color32 lightblueColor;
    public Color32 yellowColor;
    public List<Agent> allAgents;
    public Player player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
