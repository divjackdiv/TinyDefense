using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//For now this UI script + GENERAL script so creates everything, takes care of user control
public class gameManager : MonoBehaviour {

    //general settings
    public int timeScales = 1;
    public bool isMacro = true;


    //Everything to do with food Creation
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
        created = new Dictionary<string, int>();

        //Setup Object pool     
        objectPool = new Dictionary<string, List<GameObject>>();
        Vector3 defaultPos = new Vector3(0, 0, 0);
        for (int i = 0; i < objects.Count; i++)
        {
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


}
