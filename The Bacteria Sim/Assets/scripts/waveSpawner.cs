using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 using UnityEngine.UI;
/*
		
*/
public class waveSpawner : MonoBehaviour {

	public GameObject center;
	public GameObject gameManager;
	public GameObject canvas;
    public GameObject waveTextBox;
	float widthOfWorld;
    float heightOfWorld;

    public int waveCount;
    public int waveNumber;
    int ennemyLevel;
    int typesOfEnnemies;
    static Dictionary<string, int> bonusMoney;
    public List<GameObject> ennemies;

    public List<Color> colors;

    static public Dictionary<string , List<GameObject>> objectPool;
    static public Dictionary<string, List<GameObject>> createdPool;
	// Use this for initialization
	void Start () {
		//Setup the Object Pool
		objectPool = new Dictionary<string, List<GameObject>>();
		createdPool = new Dictionary<string, List<GameObject>>();
		bonusMoney = new Dictionary<string, int>();
		for (int i = 0; i < ennemies.Count; i++){
			objectPool.Add(ennemies[i].tag, new List<GameObject>());
			createdPool.Add(ennemies[i].tag, new List<GameObject>());
			bonusMoney.Add(ennemies[i].tag, 0);
			setupPool(ennemies[i], 150);
		}
        //setup waves
		waveCount = 0;
    	typesOfEnnemies = (int) Mathf.Ceil(waveNumber/3.0f);
    	if (typesOfEnnemies > ennemies.Count) typesOfEnnemies = ennemies.Count; 
    	ennemyLevel = (int) Mathf.Ceil(waveNumber/5);
		widthOfWorld = gameManager.GetComponent<gameManager>().widthOfWorld;
		heightOfWorld = gameManager.GetComponent<gameManager>().heightOfWorld;
	}
	
	// Update is called once per frame
	void Update () {
		if (waveCount <= 0) {
			upgradeWave();
			createWave(typesOfEnnemies);
			waveTextBox.GetComponent<Text>().text = "Vague " + waveNumber;
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
			amount += (ennemies[i].GetComponent<colony>().amountPerLevel * (ennemyLevel-1));
			Vector2 randPos = getRandSpawnPoint();
			GameObject createdEnnemy;
			for (int j = 0; j < amount; j++){
				if (j % maxGroupNb == 0 && j > 0) randPos = getRandSpawnPoint();
				createdEnnemy = createEnnemy(ennemies[i], new Vector2 (randPos.x+(j*0.1f), randPos.y+(j*0.1f)));
				if(j == 0) bonusMoney[ennemies[i].tag] = createdEnnemy.GetComponent<colony>().bonusForGroup;
			}
		}
	}
	void setupPool(GameObject g, int howMany){
		for (int i = 0; i < howMany; i++){
			populate(g);
		}
	}
	GameObject createEnnemy(GameObject g, Vector2 pos){
		waveCount++;
		GameObject ennemy ;
		if(objectPool[g.tag].Count <= 0){
			setupPool(g, 100);
		}
		ennemy = objectPool[g.tag][0];
		ennemy.transform.position = pos;
		objectPool[g.tag].RemoveAt(0);
		ennemy.SetActive(true);
		ennemy.GetComponent<colony>().level = ennemyLevel;
		ennemy.GetComponent<colony>().center = center;
		ennemy.GetComponent<colony>().waveSpawner = gameObject;
		ennemy.GetComponent<colony>().gameManager = gameManager;
		int i = Random.Range(0, colors.Count);
		ennemy.GetComponent<colony>().resistances[i] = true;
		ennemy.GetComponent<SpriteRenderer>().color = colors[i];
		createdPool[g.tag].Add(ennemy);
		return ennemy;
	}

    public GameObject populate(GameObject g){
        Vector2 pos = new Vector2(0,0);
        GameObject gO = (GameObject)Instantiate(g, pos, Quaternion.identity);
        gO.SetActive(false);
        objectPool[g.tag].Add(gO);
        return gO;
    }

    public void destroy(GameObject g, bool killedByPlayer){
    	if (killedByPlayer){
    		gameManager.GetComponent<gameManager>().money += g.GetComponent<colony>().money;
    		    		print(gameManager.GetComponent<gameManager>().money);
    	}
    	else {
    		bonusMoney[g.tag] = 0;
    	}
    	createdPool[g.tag].RemoveAt(0);
    	if(createdPool[g.tag].Count == 0){
    		gameManager.GetComponent<gameManager>().money += bonusMoney[g.tag];
    	} 
    	g.SetActive(false);
    	objectPool[g.tag].Add(g);
    	waveCount--;
    }

    public void destroyAll(GameObject g)
    {
        for (int i = 0; i < createdPool[g.tag].Count; i++){
            GameObject destroyed = createdPool[g.tag][0];
            destroy(destroyed, false); 
            createdPool[g.tag].RemoveAt(0);
            objectPool[g.tag].Add(destroyed);
        }
    }
}
