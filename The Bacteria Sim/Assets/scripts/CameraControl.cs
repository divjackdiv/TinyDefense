using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	float tempX;
	float tempY;
	public GameObject gameManager;

	public float startFOV = 300;
	public float maxFOV = 300;
	public float minFOV = 30;
	private float fov;

	public float zoomSpeed = 2;

	private GameObject currentColony;
	void Start()
	{
		fov = startFOV;
		Camera.main.orthographicSize = fov;
	}

	void Update () 
	{
//Horizontal et Vertical
		if(Input.mousePosition.x < Screen.width / 25)
		{
			transform.position += (Vector3.left * fov)/60;// 
		}
		if(Input.mousePosition.x > (Screen.width * 24) / 25)
		{
			transform.position += (Vector3.right* fov)/60;
		}
		if(Input.mousePosition.y < Screen.height / 25)
		{
			transform.position += (Vector3.down * fov)/60;
		}
		if(Input.mousePosition.y > (Screen.height * 24) / 25)
		{
			transform.position += (Vector3.up * fov)/60;
		}

	if(Input.GetAxis("Mouse ScrollWheel") != 0  )
		{

			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				if(fov > minFOV){
					Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					zoomTowards(mousePos, -1);
				}
			}
			else if(Input.GetAxis("Mouse ScrollWheel") < 0)
			{	
				if(fov < maxFOV){
					Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					zoomTowards(mousePos, 1);
				}
			}  
		}
	}
	void zoomTowards(Vector3 pos, float direction){
		Camera.main.orthographicSize += direction*10;
		fov += direction * 10;
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minFOV, maxFOV);			
		float multiplier = (10f / Camera.main.orthographicSize);		
		transform.position += (pos - transform.position) * multiplier;
	}
}
