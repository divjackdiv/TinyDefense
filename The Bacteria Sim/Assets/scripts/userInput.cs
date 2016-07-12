using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//Script for user input that is not managed by the interface
public class userInput : MonoBehaviour {

    //general settings
    public bool isMacro = true;

	//particle system
	public GameObject gas;



    void Update()
    {
    	if (!isMacro) return;
		if (Input.GetButton("Gas")){
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
				useGas (mousePosition);
			}
		} 
		else {
			if (gas.GetComponent<ParticleSystem> ().isPlaying) {
				gas.GetComponent<ParticleSystem>().Stop(true);
			}
		}
    }

	public void	useGas(Vector2 pos){
		gas.transform.position = pos; 
		gas.GetComponent<ParticleSystem>().Play(false);
	}

}
