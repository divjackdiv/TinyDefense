using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
//Under this is to do with game Management of the micro game
public class microGameSetup : MonoBehaviour {

    public int timeScales = 1;

    public Vector2 microBottomLeftPosition = new Vector2(-10,-10);
    public int microGameX = 20;
    public int microGameY = 20; 

    //Everything to do with food Creation
    public GameObject food;
    public int foodXQuantity = 20;
    public int foodYQuantity = 20;
    public float foodSpacing = 1;
    public int foodRespawnTime = 10;
    public Vector2 foodOrigin = new Vector2(-10, -10); // Where food starts (the 0,0 of the food matrix)

    //objectPooling
    public List<GameObject> objects;
    public Dictionary<string, List<GameObject>> objectPool;
    public Dictionary<string, List<GameObject>> createdPool;
    public int maxSize = 100;


    public void setupMicroLevel()
    {

        //Setup Object pool     
        objectPool = new Dictionary<string, List<GameObject>>();
        createdPool = new Dictionary<string, List<GameObject>>();
        Vector3 defaultPos = new Vector3(0, 0, 0);
        for (int i = 0; i < objects.Count; i++)
        {
            //Populate object pool with the prefabs set and set them not active
            if(!objectPool.ContainsKey(objects[i].tag)){
                objectPool.Add(objects[i].tag, new List<GameObject>());
                createdPool.Add(objects[i].tag, new List<GameObject>());
            }
            for (int j = 0; j < maxSize; j++)
            {
                Populate(objects[i], defaultPos);
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
                Populate(food, new Vector3(x, y, 0));
            }
        }
    }

    public GameObject Populate(GameObject g, Vector2 pos){
        GameObject gO = (GameObject)Instantiate(g, pos, Quaternion.identity);
        if (gO.GetComponent<bacteriaAiMicro>() != null)
        {
            gO.GetComponent<bacteriaAiMicro>().GameManager = gameObject;
        }
        gO.SetActive(false);
        objectPool[gO.tag].Add(gO);
        return gO;
    }
    //returns true if something is created/ or an object is reinstated through object pooling, false otherwise
    public GameObject createFromPool(GameObject g, Vector2 pos)
    {
        GameObject pooledObj = objectPool[g.tag][0];
        pooledObj.transform.position = pos;
        pooledObj.transform.rotation = Quaternion.identity;
        pooledObj.SetActive(true);
        objectPool[g.tag].RemoveAt(0);
        createdPool[g.tag].Add(pooledObj);
        return pooledObj;
	}


    public void destroy(GameObject g)
    {
        g.SetActive(false);
        objectPool[g.tag].Add(g);
    }
    public void destroyAll(GameObject g)
    {
        for (int i = 0; i <createdPool[g.tag].Count;){
            GameObject bacteria = createdPool[g.tag][0];
            destroy(bacteria); 
            createdPool[bacteria.tag].RemoveAt(0);
        }
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
