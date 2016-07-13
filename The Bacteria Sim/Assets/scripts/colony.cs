using UnityEngine;
using System.Collections;

public class colony : MonoBehaviour {

	public GameObject bacteria;
	public Color color;
	public int size = 1;
	public Vector2 position;
	public float growthSpeed = 1;
	public float movementSpeed = 0.2f;
	public float maxPosChange = 1;
	private Vector2 direction;
	private bool isWaiting;
	private Vector3 startScale;
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
		GetComponent<SpriteRenderer>().color = color;
		growthSpeed *= 10;
		direction = randomDirection(maxPosChange);
		isWaiting = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.localScale.x < 30){
			grow(growthSpeed);
		}
		move(movementSpeed);
	}

	void grow (float speed){
		transform.localScale += new Vector3(0.001f,0.001f,0);
		size += 1;
	} 

	void move(float speed){
		if (Vector2.Distance(transform.position, direction) <= 1) {
            direction = randomDirection(maxPosChange);
        }
		transform.position = Vector2.MoveTowards(transform.position, direction, Time.deltaTime * speed);
	}

	Vector2 randomDirection(float maxPosChange){
		float x, y;
        x = Random.Range (transform.position.x - maxPosChange, transform.position.x + maxPosChange);
        y = Random.Range (transform.position.y - maxPosChange, transform.position.y + maxPosChange);
        return new Vector2 (x, y);
	}

    IEnumerator WaitFor(float seconds)
    {
    	isWaiting = true;
        yield return new WaitForSeconds(seconds);
        isWaiting = false;
    }

    void OnParticleCollision(GameObject other) {
        size = 10;
        transform.localScale = startScale;
    }
}
