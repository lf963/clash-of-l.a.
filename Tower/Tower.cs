using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

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
				StartCoroutine (SpeedUp(target));
			}
		}
	}

	public void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "USCAthlete") {
			target = null;
		}
	}

	private IEnumerator SpeedUp(Monster target) {
		target.Speed += 2;
//		target.gameObject.GetComponentInChildren<GameObject>().find ("Speedup").SetActive (false);
		target.transform.Find("Speedup").gameObject.SetActive(true);

		yield return new WaitForSeconds (5f);
		target.Speed -= 2;
		// Debug.Log ("unbuffered");
		target.transform.Find("Speedup").gameObject.SetActive(false);
		target.Buffered = false;
	}
}
