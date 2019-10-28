using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph {

	public Node[,] GraphGrid { get; private set; }
	public List<Node> GraphList;

	public readonly bool usingMatrix = false;
	public readonly bool usingList = false;

	public int Level = 0;

	Vector3 _startPos;
	Vector2 _tileSize;

	public Vector3 minimumPosition => _startPos;
	public Vector3 maximumPosition => _startPos + new Vector3(GraphGrid.GetLength(0), 0, GraphGrid.GetLength(1));

	public Graph(List<Node> graph)
	{
		usingList = true;
		GraphList = graph;
	}

	public Graph(Node[,] graph, Vector3 startPos, Vector2 tileSize, bool useDiagonalMoves)
	{
		usingMatrix = true;

		GraphGrid = graph;

		_startPos = startPos;
		_tileSize = tileSize;

		SetConnectingNodes(useDiagonalMoves);
	}

	public Node GetNode(Vector3 pos)
	{
		if (usingMatrix)
		{
			//TODO Divide with size of tiles
			return GraphGrid[(int)((pos.x - _startPos.x) / _tileSize.x), (int)((pos.z - _startPos.z) / _tileSize.y)];
		}
		else
		{
			return FindNodeFromPoint(pos, GraphList);
		}

	}
	
	private Node FindNodeFromPoint(UnityEngine.Vector3 position, List<Node> nodes) {
		foreach(Node node in nodes)
		{
			if (IsInsideNode(position, node))
				return node;
		}
		return null;
	}
	
	private bool IsInsideNode(UnityEngine.Vector3 position, Node node) {
		UnityEngine.Vector3 v1 = node.Vertices[0];
		UnityEngine.Vector3 v2 = node.Vertices[1];
		UnityEngine.Vector3 v3 = node.Vertices[2];
		
		float planeV1V2 = (v1.x - position.x) * (v2.z - position.z) - (v2.x - position.x) * (v1.z - position.z);
		float planeV2V3 = (v2.x - position.x) * (v3.z - position.z) - (v3.x - position.x) * (v2.z - position.z);
		float planeV3V1 = (v3.x - position.x) * (v1.z - position.z) - (v1.x - position.x) * (v3.z - position.z);
		
		int signV1V2 = (int)(Math.Abs(planeV1V2) / planeV1V2);
		int signV2V3 =  (int)(Math.Abs(planeV2V3) / planeV2V3);
		int signV3V1 = (int)(Math.Abs(planeV3V1) / planeV3V1);
		
		return signV1V2 == signV2V3 && signV2V3 == signV3V1;
	}
	
	private void SetConnectingNodes(bool useDiagonalMoves)
	{
		int width = GraphGrid.GetLength(0);
		int height = GraphGrid.GetLength(1);

		for (int X = 0; X < width; X++)
		{
			for (int Y = 0; Y < height; Y++)
			{		
				Dictionary<Node, float> connectingNodes = new Dictionary<Node, float>();
				
				bool upLeft = true;
				bool upRight = true;
				bool downLeft = true;
				bool downRight = true;
				
				if ((X > 0) && GraphGrid[X - 1, Y].walkable)
				{
					connectingNodes.Add(GraphGrid[X - 1, Y], 1.0f);
				}
				else
				{
					upLeft = false;
					downLeft = false;
				}
				if ((X < (width - 1)) && GraphGrid[X + 1, Y].walkable)
				{
					connectingNodes.Add(GraphGrid[X + 1, Y], 1.0f);
				}
				else
				{
					upRight = false;
					downRight = false;
				}
				if ((Y > 0) && GraphGrid[X, Y - 1].walkable)
				{
					connectingNodes.Add(GraphGrid[X, Y - 1], 1.0f);
				}
				else
				{
					upLeft = false;
					upRight = false;
				}
				if ((Y < (height - 1)) && GraphGrid[X, Y + 1].walkable)
				{
					connectingNodes.Add(GraphGrid[X, Y + 1], 1.0f);
				}
				else
				{
					downLeft = false;
					downRight = false;
				}
				if (useDiagonalMoves)
				{
					if ((upLeft) && GraphGrid[X - 1, Y - 1].walkable)
					{
						connectingNodes.Add(GraphGrid[X - 1, Y - 1], 1.5f);
					}
					if ((upRight) && GraphGrid[X + 1, Y - 1].walkable)
					{
						connectingNodes.Add(GraphGrid[X + 1, Y - 1], 1.5f);
					}
					if ((downLeft) && GraphGrid[X - 1, Y + 1].walkable)
					{
						connectingNodes.Add(GraphGrid[X - 1, Y + 1], 1.5f);
					}
					if ((downRight) && GraphGrid[X + 1, Y + 1].walkable)
					{
						connectingNodes.Add(GraphGrid[X + 1, Y + 1], 1.5f);
					}
				}

				GraphGrid[X,Y].SetConnections(connectingNodes);
			}
		}
	}
}
