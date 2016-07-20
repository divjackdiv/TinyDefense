﻿using UnityEngine;
using System.Collections;

public class colony : MonoBehaviour {

	public GameObject center;
	public GameObject waveSpawner;
	public int level;
	
	public float lifePoints = 15;
	public float hpPerLevel = 2;
	public float maxHP = 150;
	
	float startSpeed;
	public float speed = 1;
	public float speedPerLevel = 0.2f;
	public float maxSpeed = 25;
	
	public float armor = 0;
	public float armorPerLevel = 0.5f;
	public float maxArmor = 20;

	public int amount = 5;
	public int amountPerLevel = 3;		
	public int maxGroupNb = 10;

	public float money = 2;

	// Use this for initialization
	void Start () {
		lifePoints += (hpPerLevel * level);
		if(lifePoints > maxHP) lifePoints = maxHP;
		speed += (speedPerLevel * level);
		if(speed > maxSpeed) speed = maxSpeed;
		armor += (armorPerLevel * level);
		if(armor > maxArmor) armor = maxArmor;
		transform.localScale = new Vector3(lifePoints/10,lifePoints/10,1);
	}
	
	// Update is called once per frame
	void Update () {
		if (lifePoints <= 0.5) die();
		objLookAt(gameObject, center.transform.position);
		transform.Translate(Vector2.right * Time.deltaTime * speed);
	}

    void objLookAt(GameObject g, Vector3 pos){
        pos.x = pos.x - g.transform.position.x;
        pos.y = pos.y - g.transform.position.y;
        float angle = (int) (Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
        g.transform.rotation  = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)),1);
    }

    void OnParticleCollision(GameObject other) {
    	float damage = - 0.1f;
    	if(damage < 0){
    		if(armor <=0){
    			lifePoints += damage;
	        	transform.localScale += new Vector3(damage/10,damage/10,0);
    		}
    		else {
    			armor +=damage;
    		}
    	}
    }
    void die(){
    	Destroy(gameObject);
    	waveSpawner.GetComponent<waveSpawner>().waveCount--;
    }
}
