using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	float tempX;
	float tempY;
	public float leftLimit = -20;
	public float rightLimit = 20;
	public float downLimit = -20;
	public float upLimit = 20;

	float futureFOV;
	public float startFOV = 20;
	public float maxFOV = 30;
	public float minFOV = 1;
	private float fov;

	public float zoomSpeed = 2;
	void Start () 
	{
		if (startFOV < maxFOV || startFOV > minFOV ) futureFOV = startFOV;
		else futureFOV = maxFOV;
		fov = futureFOV;
		Camera.main.orthographicSize = fov;
	}

	void Update () 
	{
//Horizontal et Vertical
		if(Input.mousePosition.x < Screen.width / 15)
		{
			transform.position += (Vector3.left * fov)/60;// 
		}
		if(Input.mousePosition.x > (Screen.width * 14) / 15)
		{
			transform.position += (Vector3.right* fov)/60;;
		}
		if(Input.mousePosition.y < Screen.height / 15)
		{
			transform.position += (Vector3.down * fov)/60;;
		}
		if(Input.mousePosition.y > (Screen.height * 14) / 15)
		{
			transform.position += (Vector3.up * fov)/60;;
		}
		tempX = transform.position.x;
		tempY = transform.position.y;
		tempX = Mathf.Clamp(tempX, leftLimit, rightLimit);
		tempY = Mathf.Clamp(tempY, downLimit, upLimit);
		transform.position = new Vector3(tempX, tempY, -10);

//Zoom et Dezoom
		if(Input.GetAxis("Mouse ScrollWheel") != 0  )
		{
			if(Input.GetAxis("Mouse ScrollWheel") > 0  && fov > minFOV)
			{
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				zoomTowards(mousePos, -1);
			}
			else if(Input.GetAxis("Mouse ScrollWheel") < 0 && fov < maxFOV)
			{	
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				zoomTowards(mousePos, 1);
			}  
		}
	}
	void zoomTowards(Vector3 pos, int direction){
		Camera.main.orthographicSize += direction;
		fov += direction;
		float multiplier = (1f / Camera.main.orthographicSize);					
		transform.position += (pos - transform.position) * multiplier;
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minFOV, maxFOV);
	}
}
