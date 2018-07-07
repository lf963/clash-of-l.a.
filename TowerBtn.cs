using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour {

	[SerializeField]
	private GameObject towerPrefab;

	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private int price;

	public int Price {
		get {
			return price;
		}
	}

	[SerializeField]
	private Text priceTxt;

	public GameObject TowerPrefab {
		get {
			return towerPrefab;
		}
	}

	public Sprite Sprite {
		get {
			return sprite;
		}
	}

	private void Start() {
		priceTxt.text = price + "$";
	}
}
