using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHelper : MonoBehaviour
{
	// Place on root Game Object!

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
