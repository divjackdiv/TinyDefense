using UnityEngine;
using System.Collections;

public class bacteriaAiMicro : MonoBehaviour
{
    public GameObject GameManager;
    public float nbOfFrames = 5; // this script is call every x frames, x being this value, this is for optimization 
    public float speed = 0.20f; // Speed the attached gameObject moves at
    public float eatCountdown = 5; //every xtime a game object should eat
    public int startingFood = 3; // How much food the game object starts with
    public int foodNeededToDivide = 3; //How Much food is needed to divide
    public float secondsTillDeath = 60; //After a certain amount of time, the game object will die of old age no matter what
    public float secondsTillDivide = 20; //every x seconds, if the gameobject has food, it will divide
    public float maxStateTime = 5;
    public float maxPosChange = 5;
    public float rotationSpeed = 3;
	public float foodDetectionRadius = 3;
    float radius;

    Vector3 axis = Vector3.forward;
	Vector2 pos;
    Vector2 direction;
    Quaternion rotation;
    float time;
    float frameT;
    int food;
    bool isTumbling;
	bool foundFood;
    float localEatCountDown;
	private GameObject currentFoodTarget;
    private int m; // TO BE DELETED
    float timeTillNextMovement;
    public Animator animator;
    float frame;
    //FOR NOW ANIMS ARE SET LIKE THIS 
    // 0 : normal move anim
    // 1 : divide anim
    // 2 : attack anim
    // 3 : die anim
    // 4 : tumbling anim


    private Vector2 newpos;
    private Vector2 oldpos;
 
    void Start()
    {
        newpos = transform.position;
        oldpos = newpos;
        setStartingStats();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        frameT += Time.deltaTime;
        frame++;
        if (frame % nbOfFrames == 0) {
            pos = transform.position;
            movementManagment();
            checkHunger();
            checkDivide();
            checkAge(); 
            detectFood();
            frameT = 0;
            //drawTrail();
        }
    }

    void drawTrail()
    {
        newpos = transform.position;
        if (isTumbling)
            Debug.DrawLine(oldpos, newpos, Color.blue, 200);
        else Debug.DrawLine(oldpos, newpos, Color.red, 200);
        oldpos = newpos;
    }
    void setStartingStats() {
        frame = 0;
        localEatCountDown = eatCountdown;
        rotation = transform.rotation;
        food = startingFood;
        time = 0;
        isTumbling = false;
        direction = transform.position;
		pos = transform.position;
        timeTillNextMovement = maxStateTime;
		foundFood = false;
    }
	void detectFood(){
		if (foundFood) {
			if (currentFoodTarget.activeSelf == false) {
				foundFood = false;
			}
		}
		if (foundFood == false) {
			LayerMask layer = 1 << 8;
			Collider2D[] foodAround = Physics2D.OverlapCircleAll (pos, foodDetectionRadius, layer);
			if (foodAround.Length > 0) {
				int whichFood = Random.Range (0, foodAround.Length - 1);
				if (foodAround [whichFood].tag == "food") {
					direction = foodAround [whichFood].transform.position;
					currentFoodTarget = foodAround [whichFood].gameObject;
					foundFood = true;
				}
			} else {
				foundFood = false;
			}
		}
	}
/*	void OnDrawGizmos(){
		Gizmos.color = transform.GetComponent<SpriteRenderer>().color;
		Gizmos.DrawWireSphere(pos, foodDetectionRadius);
	}*/
    void movementManagment()
    {
        //Do moving animation if you have one
        if (animator != null)
        {
            if (isTumbling) animator.SetInteger("state", 4);
            else animator.SetInteger("state", 0);
        }
		//Set Direction if there is no food target
		
        if (!foundFood && Vector2.Distance(transform.position, direction) <= 5) {
            float x, y;
            x = Random.Range (direction.x - maxPosChange, direction.x + maxPosChange);
            y = Random.Range (direction.y - maxPosChange, direction.y + maxPosChange);
            direction = new Vector3 (x, y, 0);
        }

		// Change moving state every random x
		if( time % timeTillNextMovement <= 0.5){
            if (isTumbling)
            {
                isTumbling = false;
            }
            else
            {
                isTumbling = true;
            }  
			timeTillNextMovement = Random.Range (1, maxStateTime);
        }

        Vector3 d = new Vector2(direction.x - transform.position.x , direction.y - transform.position.y);
        float angle = Mathf.Atan2 (d.y, d.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, frameT * rotationSpeed);
        transform.Translate(Vector2.right * frameT * speed);
        //GetComponent<Rigidbody2D>().AddForce(transform.right* Time.deltaTime * speed);
    }



    void divide()
    {
        //Do moving animation if you have one
        if (animator != null)
        {
            animator.SetInteger("state", 1);
        }
        //now Actually create another cell
        bool created = GameManager.GetComponent<microGameSetup>().createFromPool(transform.gameObject, transform.position);
    }

    void die()
    {
        //Do moving animation if you have one
        if (animator != null)
        {
            animator.SetInteger("state", 3);
        }
        //reset Stats in case the prefabs gets reused
        setStartingStats();
        //now Actually create another cell
        GameManager.GetComponent<microGameSetup>().destroy(transform.gameObject);
    }
    void eat(GameObject g)
    {
        food += 1;
		foundFood = false;
        GameManager.GetComponent<microGameSetup>().destroy(g);
    }
    void checkHunger()
    {
        if (time >= localEatCountDown )
        {
            if (food <= 0)
            {
                die();
            }
            else
            {
                food--;
                localEatCountDown += eatCountdown;
            }
        }
    }
    void checkDivide()
    {
        if (time % secondsTillDivide <= 0.5 && food >= foodNeededToDivide)
        {
            divide();
        }
    }

    //This function could be deleted as bacterias dont really die of old age, most of the time they get killed beforehand
    void checkAge()
    {
        if (time > secondsTillDeath)
        {
            die();
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "food")
        {
            eat(collision.gameObject);
        }
        
    }
    void OnParticleCollision(GameObject other) {
        die();
    }
}
