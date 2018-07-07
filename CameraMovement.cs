using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	[SerializeField]
	private float cameraSpeed = 0;

	private float xMax;
	private float yMin;

	// touch screen
	private Touch initTouch = new Touch();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	private void Update () {
		GetInput ();
		if (!GameManager.Instance.OnClickEvent) {
			GetTouch ();
		}
	}

	private void GetInput() {
		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Vector3.up * cameraSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Vector3.left * cameraSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.Translate (Vector3.down * cameraSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Vector3.right * cameraSpeed * Time.deltaTime);
		}

		transform.position = new Vector3 (Mathf.Clamp (transform.position.x, 0, xMax), Mathf.Clamp (transform.position.y, yMin, 0), -10);
	}

	private void GetTouch() {
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				initTouch = touch;
			} 
			else if (touch.phase == TouchPhase.Moved) {
				// swiping
				float deltaX = initTouch.position.x - touch.position.x;
				float deltaY = initTouch.position.y - touch.position.y;

				if (deltaY < 0) {
					transform.Translate (Vector3.up * cameraSpeed * Time.deltaTime);
				}
				if (deltaX > 0) {
					transform.Translate (Vector3.left * cameraSpeed * Time.deltaTime);
				}
				if (deltaY > 0) {
					transform.Translate (Vector3.down * cameraSpeed * Time.deltaTime);
				}
				if (deltaX < 0) {
					transform.Translate (Vector3.right * cameraSpeed * Time.deltaTime);
				}
			}
			else if (touch.phase == TouchPhase.Ended) {
				initTouch = new Touch();
			}
			transform.position = new Vector3 (Mathf.Clamp (transform.position.x, 0, xMax), Mathf.Clamp (transform.position.y, yMin, 0), -10);
		}
	}

	public void SetLimits(Vector3 maxTile) {
		Vector3 wp = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0));

		xMax = maxTile.x - wp.x;
		yMin = maxTile.y - wp.y;
	}
}
