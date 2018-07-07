using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour {

	/// <summary>
	/// The tiles grids position
	/// </summary>
	/// <value>The grid position.</value>
	public Point GridPosition {
		get;
		private set;
	}

	public bool isEmpty {
		get;
		set;
	}

	private Tower myTower;

	// red color
	private Color32 fullColor = new Color32(255, 118, 118, 255);

	// green color
	private Color32 emptyColor = new Color32(96, 255, 90, 255);

	private SpriteRenderer spriteRenderer;

	public int TileIndex {
		get;
		private set;
	}

	public bool WalkAble {
		get;
		set;
	}

	public bool Debugging {
		get;
		set;
	}

	public Vector2 WorldPosition {
		get {
			return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 2));
		}
	}

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		ShowColor ();
	}

	/// <summary>
	/// Sets up the tile, this is an alternative to a constructor
	/// </summary>
	/// <param name="gridPos">Grid position.</param>
	/// <param name="worldPos">World position.</param>
	public void Setup(Point gridPos, Vector3 worldPos, Transform parent, int tileIndex) {
		WalkAble = true;
		isEmpty = true;
		this.TileIndex = tileIndex;
		this.GridPosition = gridPos;
		transform.position = worldPos;
		transform.SetParent (parent);
		LevelManager.Instance.Tiles.Add (gridPos, this);
	}

	private void OnMouseOver() {
		if (!EventSystem.current.IsPointerOverGameObject () && GameManager.Instance.ClickedBtn != null) {
			if (isEmpty && !Debugging) { // Colors the tile green
				ColorTile (emptyColor);
			}
			if (!isEmpty && !Debugging) { // Colors the tile red
				ColorTile (fullColor);
			} else if (Input.GetMouseButtonDown (0)) {
				PlaceTower ();
			}
		} else if (!EventSystem.current.IsPointerOverGameObject () && GameManager.Instance.ClickedBtn == null && Input.GetMouseButtonDown(0)) {
			if (myTower != null) {
				GameManager.Instance.SelectTower (myTower);
			} else {
				GameManager.Instance.DeselectTower ();
			}
		}
	}

	private void OnMouseExit() {
		if (!Debugging) {
			ColorTile (Color.white);
		}
	}

	private void ShowColor() {
		if (GameManager.Instance.ClickedBtn != null) {
			if (isEmpty && !Debugging) { // Colors the tile green
				ColorTile (emptyColor);
			}
			else if (!isEmpty && !Debugging) { // Colors the tile red
				ColorTile (fullColor);
			}
		} else {
			ColorTile (Color.white);
		}
	}

	private void PlaceTower() {
		GameObject tower = (GameObject) Instantiate (GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity);
		tower.GetComponent<SpriteRenderer> ().sortingOrder = GridPosition.Y;

		tower.transform.SetParent (transform);

		this.myTower = tower.transform.GetChild (0).GetComponent<Tower> ();

		isEmpty = false;
		ColorTile (Color.white);

		GameManager.Instance.BuyTower ();

		WalkAble = false;
	}

	/// <summary>
	/// Sets the color on the tile
	/// </summary>
	/// <param name="newColor">New color.</param>
	private void ColorTile(Color newColor) {
		spriteRenderer.color = newColor;
	}
}
