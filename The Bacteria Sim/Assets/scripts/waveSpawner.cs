using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
		
*/
public class waveSpawner : MonoBehaviour {

	public GameObject center;
	public GameObject GameManager;
	float widthOfWorld;
    float heightOfWorld;
    public int waveCount;
    public int waveNumber;
    int ennemyLevel;
    int typesOfEnnemies;
    public List<GameObject> ennemies;

    public Dictionary<GameObject, List<GameObject>> objectPool;
    public Dictionary<GameObject, List<GameObject>> createdPool;
	// Use this for initialization
	void Start () {
		//Setup the Object Pool
		objectPool = new Dictionary<GameObject, List<GameObject>>();
		createdPool = new Dictionary<GameObject, List<GameObject>>();
		for (int i = 0; i < ennemies.Count; i++){
			objectPool.Add(ennemies[i], new List<GameObject>());
			createdPool.Add(ennemies[i], new List<GameObject>());
			setupPool(ennemies[i], 150);
		}

		waveCount = 0;
    	typesOfEnnemies = (int) Mathf.Floor(waveNumber/3);
    	if (typesOfEnnemies > ennemies.Count) typesOfEnnemies = ennemies.Count; 
    	ennemyLevel = (int) Mathf.Floor(waveNumber/5);
		widthOfWorld = GameManager.GetComponent<gameManager>().widthOfWorld;
		heightOfWorld = GameManager.GetComponent<gameManager>().heightOfWorld;
	}
	
	// Update is called once per frame
	void Update () {
		if (waveCount <= 0) {
			upgradeWave();
			createWave(typesOfEnnemies);
			waveNumber++;
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
			x = Random.Range(transform.position.x - (xScale/2), transform.position.x + (widthOfWorld/2));
			y = Random.Range(transform.position.y - (yScale/2), transform.position.x - (heightOfWorld/2));
		}
		//east
		else if (mainDir == 1){
			x = Random.Range(transform.position.x + (widthOfWorld/2), transform.position.x + (xScale/2));
			y = Random.Range(transform.position.y - (yScale/2), transform.position.y + (heightOfWorld/2));
		}
		//north
		else if (mainDir == 2){
			x = Random.Range(transform.position.x - (widthOfWorld/2), transform.position.x + (xScale/2));
			y = Random.Range(transform.position.x + (heightOfWorld/2), transform.position.y + (yScale/2));
		}
		//west
		else if (mainDir == 3){
			x = Random.Range(transform.position.x - (xScale/2), transform.position.y - (widthOfWorld/2));
			y = Random.Range(transform.position.y - (heightOfWorld/2), transform.position.y + (yScale/2));
		}
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
			int maxGroupNb = ennemies[i].GetComponent<colony>().maxGroupNb;
			amount += (ennemies[i].GetComponent<colony>().amountPerLevel * ennemyLevel);
			Vector2 randPos = getRandSpawnPoint();
			for (int j = 0; j < amount; j++){
				if (j % maxGroupNb == 0 && j > 0) randPos = getRandSpawnPoint();
				createEnnemy(ennemies[i], new Vector2 (randPos.x+(j*0.1f), randPos.y+(j*0.1f)));
			}
		}
	}
	void setupPool(GameObject g, int howMany){
		for (int i = 0; i < howMany; i++){
			populate(g);
		}
	}
	void createEnnemy(GameObject g, Vector2 pos){
		waveCount++;
		GameObject ennemy ;
		if(objectPool[g].Count <= 0){
			setupPool(g, 100);
		}
		ennemy = objectPool[g][0];
		ennemy.transform.position = pos;
		objectPool[g].RemoveAt(0);
		ennemy.SetActive(true);
		ennemy.GetComponent<colony>().level = ennemyLevel;
		ennemy.GetComponent<colony>().center = center;
		ennemy.GetComponent<colony>().waveSpawner = gameObject;
		createdPool[g].Add(ennemy);
	}

    public GameObject populate(GameObject g){
        Vector2 pos = new Vector2(0,0);
        GameObject gO = (GameObject)Instantiate(g, pos, Quaternion.identity);
        gO.SetActive(false);
        objectPool[g].Add(gO);
        return gO;
    }

    public void destroy(GameObject g)
    {
        g.SetActive(false);
        objectPool[g].Add(g);
    }

    public void destroyAll(GameObject g)
    {
        for (int i = 0; i < createdPool[g].Count; i++){
            GameObject destroyed = createdPool[g][0];
            destroy(destroyed); 
            createdPool[g].RemoveAt(0);
            objectPool[g].Add(destroyed);
        }
    }
}
