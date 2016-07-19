using UnityEngine;
using System.Collections;

public class center : MonoBehaviour {

	public int LifePoints = 10;
	public int size = 2; //Number of rows and cols the center takes
	// Use this for initialization

	void OnCollisionEnter2D(Collision2D collision) {
        LifePoints--;
        Destroy(collision.gameObject);
        if (LifePoints <= 0){
        	//Destroy(gameObject);
        	print("GAME OVER");  
        }      
    }
}
