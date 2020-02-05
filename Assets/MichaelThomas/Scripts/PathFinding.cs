using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private const int MOVE_STRIAGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<Node> OPEN = new List<Node>(); //Nodes to be checked
    private List<Node> CLOSED = new List<Node>(); //Nodes that have already been checked

    private int gCost = 0; //Walking cost from start node
    private int hCost = 0; //Distance cost to reach end node
    private int fCost = 0; //G + H

    private Node currentNode = new Node();
    private Node targetNode = new Node();

    private World m_world;
    private WorldTile[,] worldTiles;

    // Start is called before the first frame update
    void Start()
    {
        m_world = GetComponent<World>();
        worldTiles = m_world.worldTiles;
        Debug.Log(worldTiles[2, 4].mTileObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void CalculatePath()
    {
        if (CLOSED.Count == 0)
            return;


        //Calculate path in here
    }
}
