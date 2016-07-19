using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class waveSpawner : MonoBehaviour {

	public GameObject center;
	public GameObject GameManager;
	float widthOfWorld;
    float heightOfWorld;
    public int waveCount;
    int waveNumber;
    int ennemyLevel;
    int typesOfEnnemies;
    public List<GameObject> ennemies;
	// Use this for initialization
	void Start () {
		waveCount = 0;
    	waveNumber = 0;
    	typesOfEnnemies = 1; 
    	ennemyLevel = 0;
		widthOfWorld = GameManager.GetComponent<gameManager>().widthOfWorld;
		heightOfWorld = GameManager.GetComponent<gameManager>().heightOfWorld;
	}
	
	// Update is called once per frame
	void Update () {
		if (waveCount <=0) {
			waveNumber++;
			upgradeWave();
			createWave(typesOfEnnemies);
		}
	}

	Vector2 getRandSpawnPoint(){
		int mainDir = Random.Range(0,4);
		float x = 0f;
		float y = 0f;
		float xScale = transform.localScale.x;
		float yScale = transform.localScale.y;
		//south
		if (mainDir == 0){
			x = Random.Range(transform.position.x - (xScale/2), transform.position.x + (xScale/2));
			y = Random.Range(transform.position.y - (yScale/2), transform.position.x - (heightOfWorld/2));
		}
		//east
		else if (mainDir == 1){
			x = Random.Range(transform.position.x - (xScale/2), transform.position.x + (xScale/2));
			y = Random.Range(transform.position.y + (widthOfWorld/2), transform.position.y + (yScale/2));
		}
		//north
		else if (mainDir == 2){
			x = Random.Range(transform.position.x - (xScale/2), transform.position.x + (xScale/2));
			y = Random.Range(transform.position.x + (heightOfWorld/2), transform.position.y + (yScale/2));
		}
		//west
		else if (mainDir == 3){
			x = Random.Range(transform.position.x - (xScale/2), transform.position.x + (xScale/2));
			y = Random.Range(transform.position.y - (yScale/2), transform.position.y - (widthOfWorld/2));
		}
	//	print("mainDir " + mainDir +" x : " + x + " y :" + y);
		return new Vector2(x,y);
	}
	void upgradeWave(){
		if(waveNumber%3 == 0){
			if (typesOfEnnemies < ennemies.Count){
				typesOfEnnemies++;
			}
		}
		if(waveNumber%5 == 1){
			ennemyLevel++;
		}
	}
	void createWave(int typesOfEnnemies){
		for (int i = 0; i < typesOfEnnemies; i++){
			int amount = ennemies[i].GetComponent<colony>().amount;
			amount += (ennemies[i].GetComponent<colony>().amountPerLevel * ennemyLevel);
			for (int j = 0; j < amount; j++){
				createEnnemy(ennemies[i], getRandSpawnPoint());
			}
		}
	}

	void createEnnemy(GameObject g, Vector2 pos){
		waveCount++;
		GameObject ennemy = (GameObject) Instantiate(g,pos, Quaternion.identity);
		ennemy.GetComponent<colony>().level = ennemyLevel;
		ennemy.GetComponent<colony>().center = center;
		ennemy.GetComponent<colony>().waveSpawner = gameObject;
	}
}
