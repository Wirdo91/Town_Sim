using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphHelper {

	public static Graph CurrentGraph
	{
		get 
		{
			return _currentGridGraph;
		}
	}

	private static Graph _currentGridGraph;

	public static Graph GenerateGraph(bool[,] obstructionGrid, float[,] speedModifierGrid, Vector3 startPos, Vector2 tileSize, bool useDiagonalMoves)
	{
		int graphWidth = obstructionGrid.GetLength(0);
		int graphLength = obstructionGrid.GetLength(1);
		Node[,] graph = new Node[graphWidth, graphLength];
		
		for (int x = 0; x < graphWidth; x++)
		{
			for (int y = 0; y < graphLength; y++)
			{
				//TODO Calculate position of node with size of grid and tiles in mind
				graph[x, y] = new Node(0, startPos + new Vector3(x * tileSize.x, 0, y * tileSize.y), null, !obstructionGrid[x, y], speedModifierGrid[x, y]);
			}
		}

		_currentGridGraph = new Graph(graph, startPos, tileSize, useDiagonalMoves);
		return _currentGridGraph;
	}
}
