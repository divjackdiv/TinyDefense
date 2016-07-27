using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	float tempX;
	float tempY;
	public GameObject gameManager;
	public float leftLimit;
	public float rightLimit;
	public float highLimit;
	public float lowLimit;
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
		if(Input.GetAxis("Horizontal") < 0 && transform.position.x > leftLimit)
		{
			transform.position += (Vector3.left * fov)/60;// 
		}
		if(Input.GetAxis("Horizontal") > 0 && transform.position.x < rightLimit)
		{
			transform.position += (Vector3.right* fov)/60;
		}
		if(Input.GetAxis("Vertical") < 0 && transform.position.y > lowLimit)
		{
			transform.position += (Vector3.down * fov)/60;
		}
		if(Input.GetAxis("Vertical") > 0  && transform.position.y < highLimit)
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
		if(pos.x < leftLimit) pos.x = leftLimit;
		else if(pos.x > rightLimit) pos.x = rightLimit;
		if(pos.y < lowLimit) pos.y = lowLimit;
		else if(pos.y > highLimit) pos.y = highLimit;
		transform.position += (pos - transform.position) * multiplier;
	}
}
