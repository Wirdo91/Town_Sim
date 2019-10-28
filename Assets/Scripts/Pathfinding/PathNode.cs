using UnityEngine;
using System.Collections;

public class PathNode {
	public Node Node { get; private set; }
	
	public PathNode ParentNode;
	public PathNode EndNode;
	
	public float TotalCost;
	public float DirectCost;
	
	public bool Walkable = true;
	
	public PathNode(PathNode parentNode, 
	                            PathNode endNode, 
	                            Node node, 
	                			float cost)
	{
		ParentNode = parentNode;
		EndNode = endNode;
		Node = node;
		DirectCost = cost;
		
		if (endNode!= null) TotalCost = DirectCost + Distance(endNode);
	}
	
	public float Distance(PathNode otherNode)
	{
		//TODO find distance
		return Vector3.Distance(this.Node._graphPos, otherNode.Node._graphPos);
	}
	
	public bool EqualsPathNode (PathNode otherNode)
	{
		return (otherNode.Node == this.Node);
	}

	public override bool Equals (object obj)
	{
		return EqualsPathNode(obj as PathNode);
	}

	public override int GetHashCode ()
	{
		return Node.GetHashCode();
	}
}
