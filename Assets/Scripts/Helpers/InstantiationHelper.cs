using UnityEngine;

/// <summary>
/// 
///		Helper script to attach to VFX GameObject Prefabs.
///		It handles Instantiating the object into the game.
///		Also sets it to have a parent object within the asynchronously loaded scene
///		  so that it gets destroyed when the level is unloaded.
/// 
/// </summary>

public class InstantiationHelper : MonoBehaviour
{
	public GameObject gameObjectPrefab;
	
	public void Create()
	{
		GameObject go = Instantiate(gameObjectPrefab);
		go.transform.SetParent(FindObjectOfType<ObjectHolder>().transform);
        go.transform.position = transform.position;
	}
}
