using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


/*Select which level you want*/

public class LevelSelect : MonoBehaviour {
	static protected string btnLevel;	//which button has been clicked, use static to pass it to LevelManager.cs
	public void LoadScene(string sceneName){
		if (EventSystem.current.currentSelectedGameObject == null) {
			btnLevel = "BackToMap";//EventSystem.current.currentSelectedGameObject.name;	//which button has been clicked
		} else {
			btnLevel = EventSystem.current.currentSelectedGameObject.name;
		}
		SceneManager.LoadScene (sceneName);

	}

}
