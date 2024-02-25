using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class UpdateObstacles : MonoBehaviour
{
	private AstarPath p;

	private void Awake()
	{
		p = GameObject.Find("A*").GetComponent<AstarPath>();
	}

	public void UpdateMap()
	{
		p.Scan();
	}
}
