using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

	/// An array of prefabs used to create an object in the gameworld
	[SerializeField]
	private GameObject[] objectPrefabs;

	private List<GameObject> pooledObjects = new List<GameObject>();


	/// Gets an object from the pool
	public GameObject GetObject(string type) {
		foreach (GameObject go in pooledObjects)
		{
			if (go.name == type && !go.activeInHierarchy)
			{
				go.SetActive(true);
				return go;
			}
		}

		//If the pool didn't contain the object, that we needed then we need to create a new one
		for (int i = 0; i < objectPrefabs.Length; i++)
		{
			//If we have a prefab for creating the object
			if (objectPrefabs[i].name == type)
			{
				//We instantiate the prefab of the correct type
				GameObject newObject = Instantiate(objectPrefabs[i]);
				pooledObjects.Add(newObject);
				newObject.name = type;
				return newObject;
			}
		}

		return null;
	}

	public void ReleaseObject(GameObject gameObject)
	{
		gameObject.SetActive(false);
		pooledObjects.Remove (gameObject);
	}
}
