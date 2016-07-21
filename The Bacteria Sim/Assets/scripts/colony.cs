using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class colony : MonoBehaviour {

	public GameObject gameManager;
	public GameObject center;
	public GameObject waveSpawner;
	public int level;
	public float lifePoints = 150;
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

	// Use this for initialization
	void Start () {
		lifePoints += (hpPerLevel * level);
		speed += (speedPerLevel * level);
		if(speed > maxSpeed) speed = maxSpeed;
		if(lifePoints > maxHP) transform.localScale = new Vector3(maxHP/100,maxHP/100,1);
		else transform.localScale = new Vector3(lifePoints/100,lifePoints/100,1);
	}
	
	// Update is called once per frame
	void Update () {
		if (lifePoints <= 0.5) waveSpawner.GetComponent<waveSpawner>().destroy(gameObject,true);
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
		List<bool> turretRes = other.transform.parent.GetComponent<Turret>().resistances;
		for(int t = 0; t<turretRes.Count-1; t++){
			if(turretRes[t] == true && resistances[t] == false){
		    	lifePoints -= damageTakenByParticles;
			    transform.localScale -= new Vector3(damageTakenByParticles/100,damageTakenByParticles/100,0);
		    	return;
		    }
		}
    }

}
