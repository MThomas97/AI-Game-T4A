using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Threading;

public static class PathFinding
{
	//Costs for moving straight and diagonal
    private static int MOVE_STRIAGHT_COST = 10;
    private static int MOVE_DIAGONAL_COST = 20;

	public static bool DebugCorners = true;
	public static bool ToggleHeapOptimisation = true;

	private static volatile bool isDone = false;

	//Gets all the neighbouring nodes that are walkable from the current node
	private static List<Node> GetNeighbours(Node currentNode)
    {
		List<Node> neighbours = new List<Node>();

        for (int x = -1; x < 2; x++)
        {	
            for (int y = -1; y < 2; y++)
            {
				if (!World.IsPositionWalkable(new Vector2Int(x + currentNode.pos.x, y + currentNode.pos.y)))
					continue;
				if (x == 0 && y == 0)
					continue;

				Node newNeighbour = new Node(new Vector2Int(x + currentNode.pos.x, y + currentNode.pos.y));

				if (DebugCorners && x != 0 && y != 0)
				{
					bool adjacentWall = false;

					for (int v = -1; v < 2; v++)
					{
						if (v == 0) continue;

						if (!World.IsPositionWalkable(newNeighbour.pos + new Vector2Int(0, v))) adjacentWall = true;
					}

					for (int h = -1; h < 2; h++)
					{
						if (h == 0) continue;

						if (!World.IsPositionWalkable(newNeighbour.pos + new Vector2Int(h, 0))) adjacentWall = true;
					}

					if (!adjacentWall) neighbours.Add(newNeighbour);
				}
				else
				{
					neighbours.Add(newNeighbour);
				}
			}
        }
		return neighbours;
    }

	public static int GetDistance(Vector2Int nodeA, Vector2Int nodeB)
	{
		int distX = Mathf.Abs(nodeA.x - nodeB.x);
		int distY = Mathf.Abs(nodeA.y - nodeB.y);

		if (distX > distY)
			return MOVE_DIAGONAL_COST * distY + MOVE_STRIAGHT_COST * (distX - distY);
		return MOVE_DIAGONAL_COST * distX + MOVE_STRIAGHT_COST * (distY - distX);
	}

	public static bool PathComplete()
	{
		if (isDone)
			return true;
		else
			return false;
	}

	//Takes in two vector3 positions to calulate path
    public static Node CalculatePath(Vector3 startPos, Vector3 targetPos, out string output)
    {
		return CalculatePath(new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)), new Vector2Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)), out output);
	}

    public static Node CalculatePath(Vector2Int startPos, Vector2Int targetPos, out string output, bool pathDone = false)
	{
		return ToggleHeapOptimisation ? CalculateOptimisedPath(startPos, targetPos, out output) : CalculateSlowPath(startPos, targetPos, out output);
    }

    private static Node CalculateSlowPath(Vector2Int startPos, Vector2Int targetPos, out string output)
	{
        output = "";

		Stopwatch sw = new Stopwatch();
		sw.Start();

		//Firstly checks if the target and start position are not walkable then its out of bounds
		if (!World.IsPositionWalkable(targetPos) || !World.IsPositionWalkable(startPos))
		{
            output += ("Target is out of bounds.\n");
			return null;
		}
		Node startNode = new Node(startPos);
		Node targetNode = new Node (targetPos);

		Dictionary<Vector2Int, Node> OPEN = new Dictionary<Vector2Int, Node>();
		Dictionary<Vector2Int, Node> CLOSED = new Dictionary<Vector2Int, Node>();
		Dictionary<Vector2Int, Node> Operations = new Dictionary<Vector2Int, Node>();

		OPEN.Add(startNode.pos, startNode);

		while (OPEN.Count > 0) 
		{
			Dictionary<Vector2Int, Node>.Enumerator currNode = OPEN.GetEnumerator();
			currNode.MoveNext();

			Node currentNode = currNode.Current.Value;
			while(currNode.MoveNext())
			{
				if (currNode.Current.Value.fCost < currentNode.fCost || currNode.Current.Value.fCost == currentNode.fCost && currNode.Current.Value.gCost < currentNode.gCost) 
				{
					currentNode = currNode.Current.Value;
				}
			}

			OPEN.Remove(currentNode.pos);
			CLOSED.Add(currentNode.pos, currentNode);

			if (currentNode.pos == targetNode.pos) 
			{
				sw.Stop();
				output += "Unoptimised Path: " + sw.ElapsedMilliseconds + "ms" + "\n";
				output += "Operations: " + Operations.Count + "\n";
				output += "Path Cost: " + currentNode.gCost + "\n";
				OPEN.Clear();
				CLOSED.Clear();
				return RetracePath(startNode, currentNode);
			}

			//Checks all the neighbours nodes
			foreach (Node neighbour in GetNeighbours(currentNode)) 
			{ //If the node has already been visited skip that node
				if (CLOSED.ContainsKey(neighbour.pos))
					continue;
				neighbour.gCost = GetDistance(neighbour.pos, startNode.pos);
				
				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode.pos, neighbour.pos);
				//Checks if the newMovementCostToNeighbour gCost is less than the neighbour gCost and isn't in OPEN list
				if (newMovementCostToNeighbour < neighbour.gCost && !OPEN.ContainsKey(neighbour.pos)) 
				{
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour.pos, targetNode.pos);
					neighbour.parent = currentNode;
					OPEN.Add(neighbour.pos, neighbour);
					Operations.Add(neighbour.pos, neighbour);
				}
			}
		}

        output += "Didn't hit target.\n";

		OPEN.Clear();
		CLOSED.Clear();
		return null;

    }

	private static Node CalculateOptimisedPath(Vector2Int startPos, Vector2Int targetPos, out string output)
	{
        output = "";

        Stopwatch sw = new Stopwatch();
		sw.Start();

		if (!World.IsPositionWalkable(targetPos) || !World.IsPositionWalkable(startPos))
		{
            output += "Target is out of bounds.\n";
			return null;
		}
		Node startNode = new Node(startPos);
		Node targetNode = new Node(targetPos);

		Heap<Node> OPEN = new Heap<Node>(World.WorldMaxSize);
		Dictionary<Vector2Int, Node> Operations = new Dictionary<Vector2Int, Node>();
		Dictionary<Vector2Int, Node> CLOSED = new Dictionary<Vector2Int, Node>();

		OPEN.Add(startNode);

		Node currentNode = new Node(startPos);

		while (OPEN.Count > 0)
		{
			currentNode = OPEN.RemoveFirst();

			if(!CLOSED.ContainsKey(currentNode.pos))
				CLOSED.Add(currentNode.pos, currentNode);

			//Once the current node = the target node retrace path and display stats
			if (currentNode.pos == targetNode.pos)
			{
				sw.Stop();
				output += "Optimised Path: " + sw.ElapsedMilliseconds + "ms" + "\n";
				output += "Operations: " + Operations.Count + "\n";
				output += "Path Cost: " + currentNode.gCost + "\n";
                CLOSED.Clear();
				Operations.Clear();
                return RetracePath(startNode, currentNode);
			}

			//Checks all the neighbours nodes
			foreach (Node neighbour in GetNeighbours(currentNode))
			{ //If the node has already been visited skip that node
				if (CLOSED.ContainsKey(neighbour.pos))
					continue;
				neighbour.gCost = GetDistance(neighbour.pos, startNode.pos);
	
				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode.pos, neighbour.pos);
				//Checks if the newMovementCostToNeighbour gCost is less than the neighbour gCost and isn't in OPEN list
				if (newMovementCostToNeighbour < neighbour.gCost || !Operations.ContainsKey(neighbour.pos))
				{
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour.pos, targetNode.pos);
					neighbour.parent = currentNode;
					OPEN.Add(neighbour);
					Operations.Add(neighbour.pos, neighbour);
				}
			}
		}

        output += "Couldn't find path.\n";
        CLOSED.Clear();
		Operations.Clear();
        return null;
	}



	static Node RetracePath(Node startNode, Node targetNode)
	{ //This function retraces the path back to the start position
        List<Node> path = new List<Node>();
        Node previousNode = null;
		Node currentNode = targetNode;
		int distance = currentNode.gCost;

		//Loops through and traces node back to start position
		while(currentNode != startNode)
		{
			Vector3 pos = new Vector3(currentNode.pos.x, currentNode.pos.y, 0);

			//Set the nextNode of currentNode parent
            Node nextNode = currentNode.parent;
			//Sets the parent of the targetNode to null since you've reached the desination
            currentNode.parent = previousNode;
			//Saves the current node before it gets set to its parent
            previousNode = currentNode;
			//Sets the current node to its parent so it retraces back to the start node
            currentNode = nextNode;
		}

        currentNode.parent = previousNode;
		currentNode.distance = distance;
        return SimplifyPath(currentNode); //current node will have a parent and that node will have a parent that will eventually lead to the targetNode all in one node
	}

	static Node SimplifyPath(Node path)
	{ //Simplifies the path by finding what nodes are all in a straight line
        Node currentNode = path;
		 //Checks the node if its null and checks its future nodes if they're null too
		while (currentNode != null && currentNode.parent != null && currentNode.parent.parent != null)
		{
			//Looks at the parent and the parent of parent to see if the direction in x&y have changed
            Node nextNode = currentNode.parent;
            Node nextNode2 = currentNode.parent.parent;
			Vector3 directionCurrentToNext = Vector3.Normalize(new Vector3(currentNode.pos.x - nextNode.pos.x, currentNode.pos.y - nextNode.pos.y, 0));
            Vector3 directionNext = Vector3.Normalize(new Vector3(nextNode.pos.x - nextNode2.pos.x, nextNode.pos.y - nextNode2.pos.y, 0));
			//If both directions are the same then set the parent to the nextNode2 since the pathfinding is going in the same direction
			if (directionCurrentToNext == directionNext)
			{
                currentNode.parent = nextNode2;
                continue;
			}
            currentNode = currentNode.parent;
		}
		return path;
	}
}
