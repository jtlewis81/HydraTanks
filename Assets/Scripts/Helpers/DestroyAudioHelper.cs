using System.Collections;
using UnityEngine;

/// <summary>
/// 
///		Used to destroy audio clip clone objects after the clip has played
/// 
/// </summary>

public class DestroyAudioHelper : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

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
