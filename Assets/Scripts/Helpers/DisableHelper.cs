using UnityEngine;

/// <summary>
/// 
///		Used to disable the muzzle flash effect on tanks
/// 
/// </summary>

public class DisableHelper : MonoBehaviour
{
	public void Disable()
	{
		gameObject.SetActive(false);
	}
}
