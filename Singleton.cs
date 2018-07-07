using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : LevelSelect where T : MonoBehaviour{
	private static T instance; // notice: i as lowercase

	public static T Instance { // notice: i as uppercase
		get {
			if (instance == null) {
				instance = FindObjectOfType<T> ();
			}
			return instance;
		}
	}
}
