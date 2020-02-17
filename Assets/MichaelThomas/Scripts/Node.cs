using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
	public Vector2Int pos;

	public int gCost; //Walking cost from start node
	public int hCost; //Distance cost to reach end node
	public Node parent;
	int heapIndex;

	public Node(Vector2Int position)
	{
		pos = position;
	}

	public int fCost {
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

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);

		if(compare == 0)
			compare = hCost.CompareTo(nodeToCompare.hCost);

		return -compare;
	}
}
