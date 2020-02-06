using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private const int MOVE_STRIAGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

	private List<Node> OPEN = new List<Node>(); //Nodes to be checked
	private List<Node> CLOSED = new List<Node>(); //Nodes that have already been checked

	private Node currentNode = new Node(new Vector2Int(5,5));
	private Node targetNode = new Node (new Vector2Int (9, 5));

    private World m_world;

    // Start is called before the first frame update
    void Start()
    {
        //hCost = ((targetNode.pos.x - currentNode.pos.x) * 10) + ((targetNode.pos.y - currentNode.pos.y) * 10);
        //print(hCost);
        m_world = GetComponent<World>();
        Debug.Log(m_world.worldTiles[new Vector2Int(0,4)].mTileObject.transform.position);
        Debug.Log(m_world.IsPositionWalkable(new Vector2Int(0, 2)));
		CLOSED.Add (currentNode);
        GetNeighbours();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetNeighbours()
    {
        //for (int x = -1; x < 1; x++)
        //{
        //    for (int y = -1; y < 1; y++)
        //    {
        //        if (x == 0 && y == 0) continue;

                
        //    }
        //}
        for (int y = currentNode.pos.y - 1; y <= currentNode.pos.y + 1; y++)
        {
            if (y < -1)
                return;
            for (int x = currentNode.pos.x - 1; x <= currentNode.pos.x + 1; x++)
            {
                if (!m_world.IsPositionWalkable(new Vector2Int(x,y)))
                    return;

                if (x < -1)
                    return;

				foreach (Node inClosedList in CLOSED)
                {
                    if(inClosedList.pos.x + inClosedList.pos.y == x+y)
                    {
                        Debug.Log("is already in closed list");
                        return;
                    }
                }

				foreach (Node inOpenList in OPEN)
                {
                    if(inOpenList.pos.x + inOpenList.pos.y == x+y)
                    {
                        Debug.Log("is already in open list");
                        return;
                    }
                }
				Node newNeighbour = new Node(new Vector2Int(x, y));
				OPEN.Add(newNeighbour);
            }
        }

    }

	int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs (nodeA.pos.x - nodeB.pos.x);
		int distY = Mathf.Abs (nodeA.pos.y - nodeB.pos.y);

		if (distX > distY)
			return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);
	}

    public void CalculatePath()
    {
		if (currentNode == targetNode)
            return;


        //Calculate path in here
    }
}
