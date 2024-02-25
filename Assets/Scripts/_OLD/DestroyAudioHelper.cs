using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAudioHelper : MonoBehaviour
{
     private AudioSource source;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}
	// Start is called before the first frame update
	void Start()
    {
		StartCoroutine(WaitCoroutine());
    }

	IEnumerator WaitCoroutine()
	{
		yield return new WaitForSeconds(source.clip.length);
		Destroy(gameObject);
	}
}
