using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct neighbour
{
    int gCost;
    int hCost;
    int fCost;
}

public class PathFinding : MonoBehaviour
{
    private const int MOVE_STRIAGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<Vector2Int> OPEN = new List<Vector2Int>(); //Nodes to be checked
    private List<Vector2Int> CLOSED = new List<Vector2Int>(); //Nodes that have already been checked

    private int gCost = 0; //Walking cost from start node
    private int hCost = 0; //Distance cost to reach end node
    private int fCost = 0; //G + H

    private Vector2Int currentNode;
    private Vector2Int targetNode;

    private World m_world;

    // Start is called before the first frame update
    void Start()
    {
        currentNode = new Vector2Int(5, 5);
        targetNode = new Vector2Int(9, 5);
        hCost = ((targetNode.x - currentNode.x) * 10) + ((targetNode.y - currentNode.y) * 10);
        print(hCost);
        m_world = GetComponent<World>();
        Debug.Log(m_world.worldTiles[new Vector2Int(0,4)].mTileObject.transform.position);
        Debug.Log(m_world.IsPositionWalkable(new Vector2Int(0, 2)));
        CheckNeighbours();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckNeighbours()
    {
        //for (int x = -1; x < 1; x++)
        //{
        //    for (int y = -1; y < 1; y++)
        //    {
        //        if (x == 0 && y == 0) continue;

                
        //    }
        //}
        for (int y = currentNode.y - 1; y <= currentNode.y + 1; y++)
        {
            if (y < -1)
                return;
            for (int x = currentNode.x - 1; x <= currentNode.x + 1; x++)
            {
                if (!m_world.IsPositionWalkable(new Vector2Int(x,y)))
                    return;

                if (x < -1)
                    return;

                foreach (Vector2Int inClosedList in CLOSED)
                {
                    if(inClosedList.x + inClosedList.y == x+y)
                    {
                        Debug.Log("is already in closed list");
                        return;
                    }
                }

                foreach (Vector2Int inOpenList in OPEN)
                {
                    if(inOpenList.x + inOpenList.y == x+y)
                    {
                        Debug.Log("is already in open list");
                        return;
                    }
                }
            }
        }

    }

    public void CalculatePath()
    {
        if (CLOSED.Count == 0)
            return;


        //Calculate path in here
    }
}
