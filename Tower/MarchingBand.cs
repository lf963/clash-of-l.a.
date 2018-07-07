using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingBand : MonoBehaviour {
	private SpriteRenderer mySpriteRenderer;

	private Monster target;

	// Use this for initialization
	void Start () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {

		//Debug.Log (target);
	}

	public void Select() {
		mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "USCAthlete") {
			target = other.GetComponent<Monster> ();
			if (target.Buffered == false) {
				target.Buffered = true;
				// Debug.Log ("buffered");
				StartCoroutine (AddHealth(target));
			}
		}
	}

	public void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "USCAthlete") {
			target = other.GetComponent<Monster> ();
			if (target.Buffered == false) {
				target.Buffered = true;
				// Debug.Log ("buffered");
				StartCoroutine (AddHealth(target));
			}
		}
	}

	public void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "USCAthlete") {
			target = null;
		}
	}

	private IEnumerator AddHealth(Monster target) {
		target.Health.CurrentVal += 10;
		//Debug.Log (target.Health.CurrentVal);
		yield return new WaitForSeconds (0.5f);

		target.Buffered = false;
	}
}
