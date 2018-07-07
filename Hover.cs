using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : Singleton<Hover> {

	/// <summary>
	/// A reference to the icon's spriterenderer
	/// </summary>
	private SpriteRenderer spriteRenderer;

	/// <summary>
	/// A reference to the rangecheck on the tower
	/// </summary>
	private SpriteRenderer rangeSpriteRenderer;

	// Use this for initialization
	void Start () {
		this.spriteRenderer = GetComponent<SpriteRenderer>();

		this.rangeSpriteRenderer = transform.GetChild (0).GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		FollowMouse ();
	}

	private void FollowMouse() {
		if (spriteRenderer.enabled) {
			transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		}
	}

	public void Activate(Sprite sprite) {
		this.spriteRenderer.sprite = sprite;
		spriteRenderer.enabled = true;

		rangeSpriteRenderer.enabled = true;
	}

	public void Deactivate() {
		spriteRenderer.enabled = false;

		rangeSpriteRenderer.enabled = false;

		GameManager.Instance.ClickedBtn = null;

	}
}
