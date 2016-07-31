using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class colony : MonoBehaviour {

	public int level;
	public Vector3 minScale;
	public float startingLifePoints = 150;
	float lifePoints;
	public float hpPerLevel = 2;
	public float maxHP = 1500;
	public List<bool> resistances;

	float startSpeed;
	public float speed = 1;
	public float speedPerLevel = 0.2f;
	public float maxSpeed = 25;

	public int amount = 5;
	public int amountPerLevel = 3;		
	public int maxGroupNb = 10;
	public float damageTakenByParticles = 0.5f;
	public int money = 2;
	public int bonusForGroup;
	public int chanceToMutate;
	private bool hasMutated;

	GameObject gameManager;
	GameObject userInterface;
	GameObject center;
	GameObject waveSpawner;
    List<Color> colors;
	// Use this for initialization
	public void startingStats(int lvl){
		level = lvl;
		lifePoints = startingLifePoints + (hpPerLevel * level);
		speed += (speedPerLevel * level);
		if(speed > maxSpeed) speed = maxSpeed;
		if(lifePoints > maxHP) transform.localScale = minScale + new Vector3(maxHP/100,maxHP/100,1);
		else transform.localScale = minScale + new Vector3(lifePoints/100,lifePoints/100,1);		
		int waveNumber = waveSpawner.GetComponent<waveSpawner>().waveNumber;
		mutate(waveNumber);
	}
	
	public void setVariables(GameObject gm, GameObject c, GameObject ws, GameObject ui, List<Color> cols){
		gameManager = gm;
		center = c;
		waveSpawner = ws;
		userInterface = ui;
		colors = cols;
	}
	// Update is called once per frame
	void Update () {
		if (lifePoints < startingLifePoints/5 && !hasMutated){
			int m = Random.Range(0, chanceToMutate);
			if (m >= chanceToMutate - 1){
				int waveNumber = waveSpawner.GetComponent<waveSpawner>().waveNumber;
				mutate(waveNumber-1);
				hasMutated = true;
			} 
		}
		if (lifePoints <= 0.5 || transform.localScale.x < minScale.x || transform.localScale.y < minScale.y){
			waveSpawner.GetComponent<waveSpawner>().destroy(gameObject, true);
			userInterface.GetComponent<UserInterface>().showMoney(money, transform.position);
		}
		objLookAt(gameObject, center.transform.position);
		transform.Translate(Vector2.right * Time.deltaTime * speed);
	}

	void mutate(int waveNumber){
		int i;
		if(waveNumber > 10){
			i = Random.Range(0, 4);
		} 
		else if(waveNumber > 6){
			i = Random.Range(0, 3);
		}
		else if(waveNumber > 1){
			i = Random.Range(0, 2);
		}
		else i = 1;
		for(int j = 0; j < resistances.Count; j++){
			if(j == i) resistances[j] = true;
			else resistances[j] = false;
		}
		GetComponent<SpriteRenderer>().color = colors[i];
	}

    void objLookAt(GameObject g, Vector3 pos){
        pos.x = pos.x - g.transform.position.x;
        pos.y = pos.y - g.transform.position.y;
        float angle = (int) (Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
        g.transform.rotation  = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)),1);
    }

    void OnParticleCollision(GameObject other) {
		List<bool> turretRes = other.transform.parent.GetComponent<Turret>().resistances;
		for(int t = 0; t<turretRes.Count; t++){
			if(turretRes[t] == true && resistances[t] != true){
		    	lifePoints -= damageTakenByParticles;
			    transform.localScale -= new Vector3(damageTakenByParticles/100,damageTakenByParticles/100,0);
		    	return;
		    }
		}
    }
    void OnCollisionEnter2D(Collision2D collision) {
		if(collision.transform.tag == "Center"){
			waveSpawner.GetComponent<waveSpawner>().destroy(gameObject, false);
		}     
    }

}
