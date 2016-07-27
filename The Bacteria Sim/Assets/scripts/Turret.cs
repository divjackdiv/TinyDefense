using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour {

	public List<bool> resistances;
	public float timeTillPerish;
	private float time;
	public int cost;
	public AudioSource soundManager;
    public AudioClip dyingSound;
	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if(time>timeTillPerish){
			soundManager.PlayOneShot(dyingSound);
			transform.GetChild(0).gameObject.SetActive(false);
			Destroy(gameObject);
		} 
	}
}
