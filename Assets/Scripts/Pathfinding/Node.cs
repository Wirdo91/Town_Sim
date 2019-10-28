using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {
	int _level;
	public Vector3 _graphPos;

	public Graph InnerGraph { get; private set; }

	public Dictionary<Node, float> ConnectingNodes { get; private set; }

	public readonly bool walkable;

	public readonly Vector3[] Vertices;

	public float traversalCost => 1 / traverselSpeed;
	private float _traverselSpeed = 1f;
	public float traverselSpeed => _traverselSpeed;

	public Node(int level, Vector3 graphPos, Graph innerGraph, bool _walkable, float traverselSpeed = 1f)
	{
		walkable = _walkable;
		_level = level;
		InnerGraph = innerGraph;
		_graphPos = graphPos;
		_traverselSpeed = traverselSpeed;
	}

	public Node(int level, Vector3 graphPos, Graph innerGraph, bool _walkable, Vector3[] vertices) : 
		this (level, graphPos, innerGraph, _walkable)
	{
		Vertices = vertices;
	}

	public Node(Node copy)
	{
		this.walkable = copy.walkable;
		this._level = copy._level;
		this.InnerGraph = copy.InnerGraph;
		this._graphPos = copy._graphPos;
	}

	public void SetConnections(Dictionary<Node, float> nodes)
	{
		ConnectingNodes = nodes;
	}

	public override bool Equals (object obj)
	{
		if (obj.GetType() == typeof(Node))
		{
			return this._graphPos == ((Node)obj)._graphPos && this._level == ((Node)obj)._level;
		}
		return base.Equals (obj);
	}
	public override int GetHashCode ()
	{
		return (int)(_graphPos.x * 10000 + _graphPos.y);
	}
}
