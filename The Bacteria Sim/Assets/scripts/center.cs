using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class center : MonoBehaviour {

	public GameObject waveSpawner;
	public GameObject restartGameButton;
	public int LifePoints = 10;
	public int xSize = 2; //Number of rows and cols the center takes
	public int ySize = 2;
	public int currentSprite = 0;
	public List<Sprite> sprites;
    public AudioSource soundManager;
    public AudioClip looseHealthSound; 

	// Use this for initialization
	void OnCollisionEnter2D(Collision2D collision) {
        soundManager.PlayOneShot(looseHealthSound); 
        LifePoints--;
        currentSprite++;
        if(sprites.Count > currentSprite){
			GetComponent<SpriteRenderer>().sprite = sprites[currentSprite];
            //GetComponent<Collider2D>().Reset();
        }
        if (LifePoints <= 0){
        	stopGame();
        	GetComponent<SpriteRenderer>().sprite = null;	
        }      
    }
    void stopGame(){
    	Time.timeScale = 0;
    	restartGameButton.SetActive(true);
    }
}
