using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
	//public Transform seeker, target;

    private const int MOVE_STRIAGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

	//private Node currentNode = new Node(new Vector2Int(5,5));
	//private Node targetNode = new Node (new Vector2Int (9, 5));

    private World m_world;

    // Start is called before the first frame update
    void Start()
    {
        //hCost = ((targetNode.pos.x - currentNode.pos.x) * 10) + ((targetNode.pos.y - currentNode.pos.y) * 10);
        //print(hCost);
        m_world = GetComponent<World>();
//        Debug.Log(m_world.worldTiles[new Vector2Int(0,4)].mTileObject.transform.position);
        Debug.Log(m_world.IsPositionWalkable(new Vector2Int(0, 2)));
		//CLOSED.Add (currentNode);
        //GetNeighbours();
		CalculatePath(new Vector2Int(38, 12), new Vector2Int(48, 13));
    }

    // Update is called once per frame
    void Update()
    {
		//CalculatePath (seeker.position, target.position);
    }

	List<Node> GetNeighbours(Node currentNode)
    {
        //for (int x = -1; x < 1; x++)
        //{
        //    for (int y = -1; y < 1; y++)
        //    {
        //        if (x == 0 && y == 0) continue;

                
        //    }
        //}
		List<Node> neighbours = new List<Node>();

        for (int y = currentNode.pos.y - 1; y <= currentNode.pos.y + 1; y++)
        {
			if (y < -1)
				continue;
            for (int x = currentNode.pos.x - 1; x <= currentNode.pos.x + 1; x++)
            {
				if (!m_world.IsPositionWalkable (new Vector2Int (x, y)))
					continue;

				if (x < -1)
					continue;

				Node newNeighbour = new Node(new Vector2Int(x, y));
				neighbours.Add(newNeighbour);
            }
        }
		return neighbours;
    }

	int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs (nodeA.pos.x - nodeB.pos.x);
		int distY = Mathf.Abs (nodeA.pos.y - nodeB.pos.y);

		if (distX > distY)
			return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);
	}

    public void CalculatePath(Vector2Int startPos, Vector2Int targetPos)
	{
		Node startNode = new Node(startPos);
		Node targetNode = new Node (targetPos);
		Debug.Log (startNode);
		List<Node> OPEN = new List<Node>(); //Nodes to be checked
		List<Node> CLOSED = new List<Node>(); //Nodes that have already been checked
		OPEN.Add(startNode);

		while (OPEN.Count > 0) 
		{
			Node currentNode = OPEN [0];
			currentNode.parent = startNode;
			for (int i = 1; i < OPEN.Count; i++) {
				if (OPEN[i].fCost < currentNode.fCost || OPEN[i].fCost == currentNode.fCost && OPEN[i].hCost < currentNode.hCost) 
				{
					currentNode = OPEN[i];
				}
			}

			OPEN.Remove(currentNode);
			CLOSED.Add(currentNode);

			if (currentNode.pos == targetNode.pos) 
			{
				RetracePath (startNode, currentNode);
				return;
			}

			foreach (Node neighbour in GetNeighbours(currentNode)) 
			{
				if (CLOSED.Contains(neighbour))
					continue;
				neighbour.gCost = GetDistance (neighbour, startNode);
				neighbour.hCost = GetDistance(neighbour, targetNode);

				if (!OPEN.Contains(neighbour)) 
				{
					neighbour.parent = currentNode;
					OPEN.Add (neighbour);
				}
			}
			
		}
    

        //Calculate path in here
    }

	/*void OnDrawGizmos()
	{
		OnDrawGizmos.DrawWireCube (transform.position, new Vector3 (100, 1, 100));
		//add gizmos to display gizmos
	}*/

	void RetracePath(Node startNode, Node targetNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;
		while (currentNode.pos != startNode.pos) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		for (int i = 0; i < path.Count; i++) {
			Debug.Log (path [i].pos);
		}
	}
}
