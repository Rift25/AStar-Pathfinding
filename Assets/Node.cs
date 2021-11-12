using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	//a bool that tells us if the node is walkable or not
	public bool walkable;
	//the world position of the node
	public Vector3 worldPosition;
	//keeps track of its X position in the grid
	public int gridX;
	//keeps track of its Y position in the grid
	public int gridY;
	//the nodes gCost
	public int gCost;
	//the nodes hCost
	public int hCost;
	public Node parent;

	//the constructor for the node
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
	//get the fCost by calculating the gCost and the hCost
	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
}
