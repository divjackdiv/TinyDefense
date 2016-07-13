using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	float tempX;
	float tempY;
	public GameObject gameManager;
	public bool isMacro = true;
	public float leftLimit = -20;
	public float rightLimit = 20;
	public float downLimit = -20;
	public float upLimit = 20;

	public float startFOV = 300;
	public float macroMaxFOV = 300;
	public float macroMinFOV = 30;
	public float microMaxFOV = 8;
	public float microMinFOV = 1;
	private float fov;

	public float zoomSpeed = 2;

	private GameObject currentColony;
	void Start () 
	{
		fov = startFOV;
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
			transform.position += (Vector3.right* fov)/60;
		}
		if(Input.mousePosition.y < Screen.height / 15)
		{
			transform.position += (Vector3.down * fov)/60;
		}
		if(Input.mousePosition.y > (Screen.height * 14) / 15)
		{
			transform.position += (Vector3.up * fov)/60;
		}
			/*tempX = transform.position.x;
			tempY = transform.position.y;
			tempX = Mathf.Clamp(tempX, leftLimit, rightLimit);
			tempY = Mathf.Clamp(tempY, downLimit, upLimit);
			transform.position = new Vector3(tempX, tempY, -10);*/

//Zoom et Dezoom
		if(Input.GetAxis("Mouse ScrollWheel") != 0  )
		{

			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				if (isMacro && fov == macroMinFOV){
            		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            		RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            		if (hit){
                		if (hit.collider != null){
                			Camera.main.orthographicSize = microMaxFOV;
                			fov = 8;
                			gameManager.GetComponent<gameManager>().changeToMicro(hit.collider.gameObject);
                			currentColony = hit.collider.gameObject;
							isMacro = false;	
                		}
                	}
					
				}
				else if((fov > macroMinFOV && isMacro) || (fov >microMinFOV && !isMacro) ){
					Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					zoomTowards(mousePos, -1, isMacro);
				}
			}
			else if(Input.GetAxis("Mouse ScrollWheel") < 0)
			{	
				if(!isMacro && fov == microMaxFOV){
					Camera.main.orthographicSize = macroMaxFOV;
					fov = 300;
                	gameManager.GetComponent<gameManager>().changeToMacro(currentColony);
					isMacro = true;
				}
				else if((fov < macroMaxFOV && isMacro) || (fov < microMaxFOV && !isMacro) ){
					Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					zoomTowards(mousePos, 1, isMacro);
				}
			}  
		}
	}
	void zoomTowards(Vector3 pos, float direction, bool isMacro){
		if(isMacro){
			Camera.main.orthographicSize += direction*10;
			fov += direction * 10;
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, macroMinFOV, macroMaxFOV);			
			float multiplier = (10f / Camera.main.orthographicSize);		
			transform.position += (pos - transform.position) * multiplier;
		}
		else{
			Camera.main.orthographicSize += direction;
			fov += direction;
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, microMinFOV, microMaxFOV);
			float multiplier = (1f/ Camera.main.orthographicSize);					
			transform.position += (pos - transform.position) * multiplier;
		}
	}
}
