using UnityEngine;
using System.Collections;

public class CameraDragger : MonoBehaviour 
{
	public float dragSpeed = 80;
	Vector3 dragOrigin;
	Vector3 oldPos;

	void LateUpdate () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			dragOrigin = Input.mousePosition;
			oldPos = transform.position;
			return;
		}
		if (!Input.GetMouseButton(0)) return;
		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);

//		transform.Translate(move, Space.World);  
		transform.position = new Vector3(oldPos.x + move.x, oldPos.y + move.y, -10);
	}
}
