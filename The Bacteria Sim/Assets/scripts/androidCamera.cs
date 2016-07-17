using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class androidCamera : MonoBehaviour 
{
	float tempX;
	float tempY;
	//public GameObject dummy;
	//dummy.GetComponent<Text>().text += "at  " + transform.position + " zooming towards " + pos;
	public GameObject gameManager;
	public bool isMacro = true;
	public float speed;

	public float startFOV = 300;
	public float maxFOV = 300;
	public float minFOV = 30;
	private float fov;

	public bool mode = true;
	public GameObject gas;
	private GameObject currentColony;

	void Start () 
	{
		fov = startFOV;
		Camera.main.orthographicSize = fov;
	}

	void Update () 
	{
//Horizontal et Vertical
		if (Input.touchCount == 1 ) {
			if ( mode  && Input.GetTouch(0).phase == TouchPhase.Moved){
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            	transform.Translate(-touchDeltaPosition.x * speed * Time.deltaTime, -touchDeltaPosition.y * speed * Time.deltaTime, 0);
			}
			// Gas
	        else if ( !mode ){
				Vector2 touchDeltaPosition =  Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
				Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	            useGas (touchDeltaPosition, gas);
			} 
        }
        else {
			if (gas.GetComponent<ParticleSystem>().isPlaying) {
				gas.GetComponent<ParticleSystem>().Stop(true);
			}
		}
//Zoom et Dezoom

		if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Vector2 mPos = (touchZeroPrevPos - touchOnePrevPos)/2 + touchOnePrevPos;
            Vector3 middlePos =  Camera.main.ScreenToWorldPoint(mPos);

            if (deltaMagnitudeDiff < 0)
			{
				if (isMacro && fov == minFOV){
            		RaycastHit2D hit = Physics2D.Raycast(middlePos, Vector2.zero);
            		if (hit){
                		if (hit.collider != null){
                			Camera.main.orthographicSize = maxFOV;
                			fov = maxFOV;
                			gameManager.GetComponent<gameManager>().changeToMicro(hit.collider.gameObject);
                			currentColony = hit.collider.gameObject;
							isMacro = false;
                		}
                	}
					
				}
				else if((fov > minFOV && isMacro) || (fov >minFOV && !isMacro) ){
					
					zoomTowards(middlePos, -1);
				}
			}
			else if(deltaMagnitudeDiff > 0)
			{	
				if(!isMacro && fov == maxFOV){
					Camera.main.orthographicSize = maxFOV;
					fov = maxFOV;
                	gameManager.GetComponent<gameManager>().changeToMacro(currentColony);
					isMacro = true;
				}
				else if((fov < maxFOV && isMacro) || (fov < maxFOV && !isMacro) ){
					zoomTowards(middlePos, 1);
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

	public void	useGas(Vector2 pos, GameObject g){
		g.transform.position = pos; 
		g.GetComponent<ParticleSystem>().Play(false);
	}
	
	public void changeMode(){
		if(mode == true) mode = false;
		else mode = true;
	}
}
