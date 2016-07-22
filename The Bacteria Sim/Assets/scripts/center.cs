using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class center : MonoBehaviour {

	public GameObject waveSpawner;
	public GameObject restartGameButton;
	public int LifePoints = 10;
	public int xSize = 2; //Number of rows and cols the center takes
	public int ySize = 2;
	private int currentSprite = 0;
	public List<Sprite> sprites;

	// Use this for initialization
	void OnCollisionEnter2D(Collision2D collision) {
        LifePoints--;
        currentSprite++;
        if(sprites.Count > currentSprite){
			GetComponent<SpriteRenderer>().sprite = sprites[currentSprite];
        }
        if (LifePoints <= 0){
        	stopGame();
        	GetComponent<SpriteRenderer>().sprite = null;	
        	print("GAME OVER");  
        }      
    }
    void stopGame(){
    	Time.timeScale = 0;
    	restartGameButton.SetActive(true);
    }
}
