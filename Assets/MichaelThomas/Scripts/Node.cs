using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
	public Vector2Int pos;

	public int gCost; //Walking cost from start node
	public int hCost; //Distance cost to reach end node
	public int distance;
	public Node parent;
	int heapIndex;

	public Node(Vector2Int position)
	{
		pos = position;
	}

	public int fCost { //Total cost of gCost and hCost
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

    public Vector3 GetVector3Position()
    {
        return new Vector3(pos.x, pos.y);
    }

	
	public int CompareTo(Node nodeToCompare)
	{//Compare the fCost between two nodes with fCost and gCost
		int compare = fCost.CompareTo(nodeToCompare.fCost);

		if(compare == 0)
			compare = gCost.CompareTo(nodeToCompare.gCost);

		return -compare;
	}
}
