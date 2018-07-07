using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	//Stephanie
	private int tileIndex;

	public Point GridPosition {
		get;
		private set;
	}

	/// The nodes position in the world, this is more a reference to the tile that the node is connected to
	public Vector2 WorldPosition { get; set; }

	public TileScript TileRef {
		get;
		private set;
	}

	public Node Parent {
		get;
		private set;
	}

	public Node(TileScript tileRef) {
		this.TileRef = tileRef;
		this.GridPosition = tileRef.GridPosition;
		this.WorldPosition = tileRef.WorldPosition;
	}

	public void CalcValues(Node parent) {
		this.Parent = parent;
	}
}
