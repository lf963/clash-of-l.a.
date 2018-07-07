using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AStar {
	private static Dictionary<Point, Node> nodes;

	private static void CreateNodes() {
		// Instantiates the dictionary
		nodes = new Dictionary<Point, Node> ();

		// Run through all the tiles in the game
		foreach (TileScript tile in LevelManager.Instance.Tiles.Values) {
			// Adds the node to the nodes dictionary
			nodes.Add(tile.GridPosition, new Node(tile));
		}
	}

	public static Stack<Node> GetPath(Point start, Point goal) {
		if (nodes == null) {
			CreateNodes ();
		}

		// Create an open list to be used with the A* algorithm
		// Stephanie: actually we don't need openList, we cannot append two nodes at a time.
		HashSet<Node> openList = new HashSet<Node> ();

		// Create an closed list to be used with the A* algorithm
		HashSet<Node> closedList = new HashSet<Node> ();

		// Stephanie
		Stack<Node> finalPath = new Stack<Node>();

		// Finds the start node and creates a reference to it called current node
		Node currentNode = nodes [start];

		//Stephanie
		Node nextNode = null;

		// 1. Adds the start node to the OpenList
		openList.Add (currentNode);

		//while (openList.Count > 0) {//Step 10
		while (openList.Count > 0) {//Step 10

			//2. Runs through all neighbors 
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					// Stephanie: skip diagonal points
					if (x * y != 0) {
						continue;
					}

					Point neighborPos = new Point (currentNode.GridPosition.X - x, currentNode.GridPosition.Y - y);
					if (LevelManager.Instance.InBounds (neighborPos) && LevelManager.Instance.Tiles [neighborPos].WalkAble && neighborPos != currentNode.GridPosition) {
						// 3. Adds the neighbor to the open list
						Node neighbor = nodes [neighborPos];

						if (!openList.Contains (neighbor) && !closedList.Contains (neighbor)) {
							openList.Add (neighbor);
							// 4. Add Parent
							neighbor.CalcValues (currentNode);
							nextNode = neighbor;
						}
					}
				}
			}

			openList.Remove (currentNode);
			closedList.Add (currentNode);

			//****** Only for debugging ******
			//GameObject.Find ("AStarDebugger").GetComponent<AStarDebugger> ().DebugPath (openList, closedList);

			currentNode = nextNode;
			// Stephanie: XXXX
			//Debug.Log (currentNode.GridPosition.X + " " + currentNode.GridPosition.Y);
			if (currentNode == nodes [goal]) {
				while (currentNode.GridPosition != start) {
					finalPath.Push (currentNode);
					currentNode = currentNode.Parent;
				}
				Debug.Log ("get finalPath");
				return finalPath; 
			}
		}
		return null;
	}
}
