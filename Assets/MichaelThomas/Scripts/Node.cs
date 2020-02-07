using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	public Vector2Int pos;

	public int gCost; //Walking cost from start node
	public int hCost; //Distance cost to reach end node
	public Node parent;

	public Node(Vector2Int position)
	{
		pos = position;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}
}
