using UnityEngine;
using System.Collections;

public class colony : MonoBehaviour {

	public GameObject center;
	public float lifePoints = 15;
	public float speed = 1;
	public float armor = 0;
	public float amount = 5;
	public float money = 2;

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3(lifePoints/10,lifePoints/10,1);
	}
	
	// Update is called once per frame
	void Update () {
		if (lifePoints <= 0.5) Destroy(gameObject);
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
	    	lifePoints += damage;
	        transform.localScale += new Vector3(damage/10,damage/10,0);
    	}
    }
}
