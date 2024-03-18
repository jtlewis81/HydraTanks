using UnityEngine;

/// <summary>
/// 
///		Destroys game objects
///		Used in the OnDead UnityEvent of an object's Damageable script
/// 
/// </summary>

public class DestroyHelper : MonoBehaviour
{
	// Place on root Game Object!

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
