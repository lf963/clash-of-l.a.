using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour {
	static public string FilePath;	//save file to this path


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void Save(){
		string jsonString = JsonUtility.ToJson (LevelManager.myPlayer);
		File.WriteAllText (FilePath, jsonString);

	}
	public static void Load(){
		string jsonString = File.ReadAllText (FilePath);

		//If the user has already saved his game progress, we overwrite myPlayer with his game progress
		JsonUtility.FromJsonOverwrite (jsonString, LevelManager.myPlayer);
	}
}
