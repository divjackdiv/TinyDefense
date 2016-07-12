using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//For now this UI script + GENERAL script so creates everything, takes care of user control
public class ui : MonoBehaviour {

    //general settings
    int currentSim = 0;
    public int timeScales = 1;
    public bool isMacro = true;

    //Canvas/ui
    public int currentObject; //default object to create
    GameObject currentGameObject;

    public Canvas canvas;
    private GameObject bottomBar;

	//particle system
	public GameObject gas;


    //Everything to do with food
    public GameObject food;
    public int foodXQuantity = 50;
    public int foodYQuantity = 50;
    public float foodSpacing = 3;
    public int foodRespawnTime = 20;
    public Vector2 foodOrigin = new Vector2(-50, -50); // Where food starts (the 0,0 of the food matrix)

    //objectPooling
    public List<GameObject> objects;
    public Dictionary<string, List<GameObject>> objectPool;
    public int maxSize = 10;
    Dictionary<string, int> created;


    void Start () {
        setupLevel();
    }

    public void setupLevel()
    {
        currentObject = 1;
        created = new Dictionary<string, int>();

        //Object pool and canvas        
        objectPool = new Dictionary<string, List<GameObject>>();
        Vector3 defaultPos = new Vector3(0, 0, 0);
        for (int i = 0; i < objects.Count; i++)
        {
            //Set canvas sprites
            canvas.transform.GetChild(0).GetChild(i).GetComponent<Image>().sprite = objects[i].GetComponent<SpriteRenderer>().sprite;
			canvas.transform.GetChild (0).GetChild (i).GetComponent<Image>().preserveAspect = true;
            canvas.transform.GetChild (0).GetChild (i).GetComponent<Image>().color = objects[i].GetComponent<SpriteRenderer>().color;
            //Populate object pool with the prefabs set and set them not active
            objectPool.Add(objects[i].tag, new List<GameObject>());
            created[objects[i].tag] = 0;
            for (int j = 0; j < maxSize; j++)
            {
                if (objects[i].tag == "food"){
                    create(objects[i], defaultPos, false, true);
                }
                else{
                    create(objects[i], defaultPos, false, true);                    
                }
            }
        }

        //Populate object pool with food prefab and set them active
        if(!objectPool.ContainsKey(food.tag)){   
            objectPool.Add(food.tag, new List<GameObject>());
        }
        for (int i = 0; i < foodXQuantity; i++)
        {
            for (int j = 0; j < foodYQuantity; j++)
            {
                float x = (float)Random.Range(foodSpacing * 7, foodSpacing * 13) / 10;
                float y = (float)Random.Range(foodSpacing * 7, foodSpacing * 13) / 10;
                x = foodOrigin.x + (x * (1 + i));
                y = foodOrigin.y + (y * (1 + j));
                create(food, new Vector3(x, y, 0), true, true);
            }
        }
    }

    void Update()
    {
    	if (!isMacro) return;
		if (Input.GetButton("Gas")){
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
				useGas (mousePosition);
			}
		} 
		else {
			if (gas.GetComponent<ParticleSystem> ().isPlaying) {
				gas.GetComponent<ParticleSystem>().Stop(true);
			}
		}
		/*if (Input.GetButtonDown("DragDrop"))
        {
			if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                create(objects[currentObject], mousePosition, true, false);
            }
        }*/
        if (Input.GetButtonDown("Fire2"))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    destroy(hit.transform.gameObject);
                }
            }
        }
    }

    //returns true if something is created/ or an object is reinstated through object pooling, false otherwise
    public GameObject create(GameObject g, Vector2 pos, bool active, bool isPopulating)
    {
        if (isPopulating) {
            GameObject gO = (GameObject)Instantiate(g, pos, Quaternion.identity);
            if (gO.GetComponent<GeneralAi>() != null)
            {
                gO.GetComponent<GeneralAi>().GameManager = gameObject;
            }
            gO.SetActive(active);
            if (active == false)
            {
                objectPool[gO.tag].Add(gO);
            }
            return gO;
        }
        else if (objectPool[g.tag].Count <= 0) return null;
        else
        {
            GameObject pooledObj = objectPool[g.tag][0];
            pooledObj.transform.position = pos;
            pooledObj.transform.rotation = Quaternion.identity;
            pooledObj.SetActive(active);
            objectPool[g.tag].RemoveAt(0);
            created[g.tag]++;
            return pooledObj;
        }
	}

    public void destroy(GameObject g)
    {
        int s = 0;
        for (int i = 0 ; i < objects.Count; i++)
        {
           if (g.tag == objects[i].tag) s = i;
        }
        g.SetActive(false);
        objectPool[g.tag].Add(g);
    }

    //This doesn't actually destroy an object, it just disables it for a fixed period of time, then "respawns" it in the same space
    public void falseDestroy(GameObject g)
    {
        g.SetActive(false);
        StartCoroutine(WaitAndRespawn(foodRespawnTime,g));
    }

    IEnumerator WaitAndRespawn(int respawnTime, GameObject g)
    {
        yield return new WaitForSeconds(respawnTime);
        g.SetActive(true);
    }
    public void OnDrag(){ 
    	print("dragging");
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
		if (currentGameObject == null ){
			currentGameObject = create(objects[currentObject], mousePosition, true, false);
			MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour script in scripts) script.enabled = false;
		}
        currentGameObject.transform.position = mousePosition;
        //Should probably play some wiggling animation
    }
    public void OnDrop(){
    	print("sort of dropping");
        if (EventSystem.current.IsPointerOverGameObject())
        {
        	if(currentGameObject != null) destroy(currentGameObject);
        }
        MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour script in scripts) script.enabled = true;
        currentGameObject = null;
    }

    public void changeObject(int i)
    {
        if (i > objects.Count - 1) return;
        if (i == 1)
        {
            Sprite temp = canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
            Color col = canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;

            canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite;
            canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().color;

            canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite;
            canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().color;

            canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = temp;
            canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = col;
            currentObject = (currentObject + 1) % 3; 
        }
        else if (i == -1)
        {
            Sprite temp = canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite;
            Color col = canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().color;

            canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite; 
            canvas.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().color;

            canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite; 
            canvas.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;

            canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = temp; 
            canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = col;
            currentObject = currentObject - 1;
            if (currentObject < 0) currentObject = objects.Count - 1;
        }
    }
    public void slowTime()
    {
        if (timeScales >= 16) timeScales = 1;
        else timeScales *= 2;
        Time.timeScale = timeScales;
    }
	public void	useGas(Vector2 pos){
		gas.transform.position = pos; 
		gas.GetComponent<ParticleSystem>().Play(false);
	}

}
