using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 using UnityEngine.UI;
/*
		
*/
public class waveSpawner : MonoBehaviour {

	public GameObject center;
	public GameObject gameManager;
	public GameObject userInterface;
	public GameObject canvas;
	public GameObject restartGameButton;
	public GameObject antibios;
    public GameObject waveTextBox;
    public AudioSource soundManager;
    public List<AudioClip> sounds;  // 0 is ennemy death;  1 is new wave ; 2 is bonus money
    float widthOfGridRatio;
    float heightOfGridRatio;

    public int waveCount;
    public int startWave;
    public int waveNumber;
    int ennemyLevel;
    int typesOfEnnemies;
    static Dictionary<string, int> bonusMoney;
    public List<GameObject> ennemies;
    public List<GameObject> bosses;
    private int currentBoss;

    public List<Color> colors;
    static public Dictionary<string , List<GameObject>> objectPool;
    static public Dictionary<string, List<GameObject>> createdPool;
	// Use this for initialization
	void Start () {
		waveNumber = startWave;
		//Setup the Object Pool
		widthOfGridRatio = transform.localScale.x/gameManager.GetComponent<gameManager>().widthOfWorld;
		heightOfGridRatio = transform.localScale.y/gameManager.GetComponent<gameManager>().heightOfWorld;
		objectPool = new Dictionary<string, List<GameObject>>();
		createdPool = new Dictionary<string, List<GameObject>>();
		bonusMoney = new Dictionary<string, int>();
		for (int i = 0; i < ennemies.Count; i++){
			objectPool.Add(ennemies[i].tag, new List<GameObject>());
			createdPool.Add(ennemies[i].tag, new List<GameObject>());
			bonusMoney.Add(ennemies[i].tag, 0);
			setupPool(ennemies[i], 8);
		}
		if(waveNumber < 2) antibios.transform.GetChild(1).GetComponent<Image>().color = Color.black;
		if(waveNumber < 7) antibios.transform.GetChild(2).GetComponent<Image>().color = Color.black;
		if(waveNumber < 11) antibios.transform.GetChild(3).GetComponent<Image>().color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
		if (waveCount <= 0) {
			createWave();
			waveTextBox.GetComponent<Text>().text = "Wave " + waveNumber;
			userInterface.GetComponent<UserInterface>().waveNumber = waveNumber;
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
		float displacement = (float) waveNumber;
		if (mainDir == 0){
			if (displacement > xScale/heightOfGridRatio) displacement = xScale/heightOfGridRatio;
			x = Random.Range(transform.position.x - (xScale/widthOfGridRatio), transform.position.x + (yScale/widthOfGridRatio));
			y = transform.position.x - (xScale/heightOfGridRatio) - displacement;
		}
		//east
		else if (mainDir == 1){
			if (displacement > yScale/widthOfGridRatio) displacement = yScale/widthOfGridRatio;
			x = transform.position.x + (yScale/widthOfGridRatio) + displacement;
			y = Random.Range(transform.position.y - (yScale/xScale), transform.position.y + (xScale/heightOfGridRatio));
		}
		//north
		else if (mainDir == 2){
			if (displacement > xScale/heightOfGridRatio) displacement = xScale/heightOfGridRatio;
			x = Random.Range(transform.position.x - (yScale/widthOfGridRatio), transform.position.x + (xScale/widthOfGridRatio));
			y = transform.position.y + (xScale/heightOfGridRatio) + displacement;
		}
		//west
		else if (mainDir == 3){
			if (displacement > yScale/widthOfGridRatio) displacement = yScale/3;
			x = transform.position.y - (yScale/widthOfGridRatio) - displacement;
			y = Random.Range(transform.position.y - (xScale/heightOfGridRatio), transform.position.y + (yScale/xScale));
		}
		return new Vector2(x,y);
	}

	void createWave(){
		typesOfEnnemies = (int) Mathf.Ceil(waveNumber/3.0f);
    	if (typesOfEnnemies > ennemies.Count) typesOfEnnemies = ennemies.Count; 
    	ennemyLevel = (int) Mathf.Ceil(waveNumber/5);
    	List<int> groupRes = new List<int>();
		for (int i = 0; i < typesOfEnnemies; i++){
			int amount = ennemies[i].GetComponent<colony>().amount;			
			int maxGroupNb = ennemies[i].GetComponent<colony>().maxGroupNb;
			amount += (ennemies[i].GetComponent<colony>().amountPerLevel * (ennemyLevel));
			Vector2 randPos = getRandSpawnPoint();
			GameObject createdEnnemy;

			for (int j = 0; j < amount; j++){
				if (j % maxGroupNb == 0 && j > 0){
					randPos = getRandSpawnPoint();
				}
				createdEnnemy = createEnnemy(ennemies[i], new Vector2 (randPos.x+(j*0.1f), randPos.y+(j*0.1f)));
				if (j % maxGroupNb == 0) userInterface.GetComponent<UserInterface>().showWarning(createdEnnemy, false);
				if(j == 0) bonusMoney[ennemies[i].tag] = createdEnnemy.GetComponent<colony>().bonusForGroup;
			}
		}
		if(waveNumber%5 == 0){
			int amount = bosses[currentBoss].GetComponent<colony>().amount;			
			int maxGroupNb = bosses[currentBoss].GetComponent<colony>().maxGroupNb;
			amount += (bosses[currentBoss].GetComponent<colony>().amountPerLevel * (ennemyLevel));
			Vector2 randPos = getRandSpawnPoint();
			GameObject createdEnnemy;
			for (int j = 0; j < amount; j++){
				if (j % maxGroupNb == 0 && j > 0) randPos = getRandSpawnPoint();
				GameObject gO = (GameObject)Instantiate(bosses[currentBoss], new Vector2 (randPos.x+(j*0.1f), randPos.y+(j*0.1f)), Quaternion.identity);
		        gO.GetComponent<colony>().setVariables(gameManager, center, gameObject, userInterface, colors);
				gO.GetComponent<colony>().startingStats(ennemyLevel);
				userInterface.GetComponent<UserInterface>().showWarning(gO, true);
			}
			currentBoss++;
			if(currentBoss>= bosses.Count) currentBoss = 0;
		}

		if(waveNumber > 10){
			antibios.transform.GetChild(3).GetComponent<Image>().color = colors[3];
		} 
		else if(waveNumber > 6){
			antibios.transform.GetChild(2).GetComponent<Image>().color = colors[2];
		}
		else if(waveNumber > 1){
			antibios.transform.GetChild(1).GetComponent<Image>().color = colors[1];
		}
    	soundManager.PlayOneShot(sounds[1]);
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
		objectPool[g.tag].RemoveAt(0);
		ennemy.transform.position = pos;
		MonoBehaviour[] scripts = ennemy.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = true;
		ennemy.GetComponent<colony>().startingStats(ennemyLevel);
		ennemy.SetActive(true);
		createdPool[ennemy.tag].Add(ennemy);
		return ennemy;
	}

    public GameObject populate(GameObject g){
        Vector2 pos = new Vector2(0,0);
        GameObject gO = (GameObject)Instantiate(g, pos, Quaternion.identity);
        gO.SetActive(false);
        gO.GetComponent<colony>().setVariables(gameManager, center, gameObject, userInterface, colors);
        objectPool[g.tag].Add(gO);
        return gO;
    }

    public void destroy(GameObject g, bool killedByPlayer){
    	if (killedByPlayer){
    		soundManager.PlayOneShot(sounds[0]);
    		gameManager.GetComponent<gameManager>().money += g.GetComponent<colony>().money;
    		if(createdPool.ContainsKey(g.tag)){
    			if(createdPool[g.tag].Count <= 1){
	    			soundManager.PlayOneShot(sounds[2]);
	    			gameManager.GetComponent<gameManager>().money += bonusMoney[g.tag];
    			} 
    		}
     	}
    	else {
    		if(createdPool.ContainsKey(g.tag))
    			bonusMoney[g.tag] = 0;
    	}
        MonoBehaviour[] scripts = g.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = false;
    	g.SetActive(false);
    	if(objectPool.ContainsKey(g.tag)){
    		objectPool[g.tag].Add(createdPool[g.tag][0]);
    	}
    	else Destroy(g);
    	if(createdPool.ContainsKey(g.tag)){
    		createdPool[g.tag].RemoveAt(0);
    	}
    	waveCount--;
    }
}
