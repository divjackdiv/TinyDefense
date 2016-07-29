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

	public float startFOV = 300;
	public float maxFOV = 300;
	public float minFOV = 30;
	public float leftLimit;
	public float rightLimit;
	public float highLimit;
	public float lowLimit;
	private float fov;
	public bool rotating;
	private GameObject currentColony;

	void Start () 
	{
		fov = startFOV;
		Camera.main.orthographicSize = fov;
	}

	void Update () 
	{
//Horizontal et Vertical
		if(gameManager.GetComponent<gameManager>().creatingTurret || gameManager.GetComponent<gameManager>().wasHoldingDown ) return;
		if (Input.touchCount == 1 ) {
			if (Input.GetTouch(0).phase == TouchPhase.Moved){
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				//float x = Mathf.Clamp(touchDeltaPosition.x * (fov), leftLimit, rightLimit);
            	//float y = Mathf.Clamp(touchDeltaPosition.y * (fov), lowLimit, highLimit);
            	transform.Translate( -touchDeltaPosition.x * (fov)/2 * Time.deltaTime, -touchDeltaPosition.y * (fov)/2 * Time.deltaTime, 0);
			}
        }
//Zoom et Dezoom

		if (Input.touchCount == 2)
        {
            // Store both touches.
            if (!rotating){
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
					if(fov > minFOV){
						zoomTowards(middlePos, -1);
					}
				}
				else if(deltaMagnitudeDiff > 0)
				{	
					if(fov < maxFOV){
						zoomTowards(middlePos, 1);
					}
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
