using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarDebugger : MonoBehaviour {
	private TileScript start, goal;

	[SerializeField]
	private Sprite blankTile;

	[SerializeField]
	private GameObject arrowPrefab;

	[SerializeField]
	private GameObject debugTilePrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// (Stephanie) turn off debugger
//		ClickTile ();
//
//		if (Input.GetKeyDown (KeyCode.Space)) {
//			Stack<Node> finalPath = AStar.GetPath (start.GridPosition, goal.GridPosition);
//		}
	}

	private void ClickTile() {
		if (Input.GetMouseButtonDown (1)) {
			// test *****
			Debug.Log(Input.mousePosition);
			Debug.Log (Camera.main.ScreenToWorldPoint (Input.mousePosition));

			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

			if (hit.collider != null) {
				TileScript tmp = hit.collider.GetComponent<TileScript> ();

				if (tmp != null) {
					if (start == null) {
						start = tmp;

						Debug.Log (start.WorldPosition);
						CreateDebugTile (start.WorldPosition, new Color32 (255, 135, 0, 255));
					} else if (goal == null) {
						goal = tmp;
						Debug.Log (goal.WorldPosition);
						CreateDebugTile (goal.WorldPosition, new Color32 (255, 0, 0, 255));
					}
				}
			}
		}
	}

	/// <summary>
	/// Debugs the path, so that we can see what's going on
	/// </summary>
	/// <param name="openList">Open list.</param>
	public void DebugPath(HashSet<Node> openList, HashSet<Node> closedList) {
		foreach (Node node in openList) {
			if (node.TileRef != start) {
				CreateDebugTile (node.TileRef.WorldPosition, Color.cyan);
			}
			// Points at the parent
			PointToParent (node, node.TileRef.WorldPosition);
		}

		foreach (Node node in closedList) {
			if (node.TileRef != start && node.TileRef != goal) {
				CreateDebugTile (node.TileRef.WorldPosition, Color.blue);
			}
		}
	}

	private void PointToParent(Node node, Vector2 position) {
		if (node.Parent != null) {
			GameObject arrow = (GameObject)Instantiate (arrowPrefab, position, Quaternion.identity);
			arrow.GetComponent<SpriteRenderer> ().sortingOrder = 3;

			// Right
			if ((node.GridPosition.X < node.Parent.GridPosition.X) && (node.GridPosition.Y == node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 0);
			}
			// Top Right
			else if ((node.GridPosition.X < node.Parent.GridPosition.X) && (node.GridPosition.Y > node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 45);
			}
			// Top
			else if ((node.GridPosition.X == node.Parent.GridPosition.X) && (node.GridPosition.Y > node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 90);
			}
			// Top Left
			else if ((node.GridPosition.X > node.Parent.GridPosition.X) && (node.GridPosition.Y > node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 135);
			}
			// Left
			else if ((node.GridPosition.X > node.Parent.GridPosition.X) && (node.GridPosition.Y == node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 180);
			}
			// Bottom Left
			else if ((node.GridPosition.X > node.Parent.GridPosition.X) && (node.GridPosition.Y < node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 225);
			}
			// Bottome
			else if ((node.GridPosition.X == node.Parent.GridPosition.X) && (node.GridPosition.Y < node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 270);
			}
			// Bottome Right
			else if ((node.GridPosition.X < node.Parent.GridPosition.X) && (node.GridPosition.Y < node.Parent.GridPosition.Y)) {
				arrow.transform.eulerAngles = new Vector3 (0, 0, 315);
			}
		}
	}

	private void CreateDebugTile(Vector3 worldPos, Color32 color) {
		GameObject debugTile = (GameObject)Instantiate(debugTilePrefab, worldPos, Quaternion.identity);

		debugTile.GetComponent<SpriteRenderer> ().color = color;
	}
}
