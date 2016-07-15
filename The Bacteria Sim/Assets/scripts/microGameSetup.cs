using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
//Under this is to do with game Management of the micro game
public class microGameSetup : MonoBehaviour {

    public int timeScales = 1;

    public Vector2 cameraMicroPos;
    public GameObject backgroundImage;
    public GameObject Wall;
    //Everything to do with food Creation
    public GameObject food;
    public int foodXQuantity = 20;
    public int foodYQuantity = 20;
    public int foodRespawnTime = 10;

    //objectPooling
    public GameObject bacteria;
    public Dictionary<string, List<GameObject>> objectPool;
    public Dictionary<string, List<GameObject>> createdPool;
    public int maxSize;

    float w = Screen.width;
    float h = Screen.height;

    public void setupMicroLevel()
    {
        setupWalls();
        //Setup Object pool     
        objectPool = new Dictionary<string, List<GameObject>>();
        createdPool = new Dictionary<string, List<GameObject>>();
        if(!objectPool.ContainsKey(bacteria.tag)){
            objectPool.Add(bacteria.tag, new List<GameObject>());
            createdPool.Add(bacteria.tag, new List<GameObject>());
        }
        for (int j = 0; j < maxSize; j++)
        {
            Populate(bacteria);
        }

        //Populate object pool with food prefab and set them active
        if(!objectPool.ContainsKey(food.tag)){   
            objectPool.Add(food.tag, new List<GameObject>());
        }
        for (int i = 0; i < foodXQuantity; i++)
        {
            for (int j = 0; j < foodYQuantity; j++)
            {
                float x = (float)Random.Range((cameraMicroPos.x -(w/2)) + i * (w/foodXQuantity) +2, (cameraMicroPos.x -(w/2)) + (i + 1) * (w/foodXQuantity) -2);
                float y = (float)Random.Range((cameraMicroPos.y -(h/2)) + j * (h/foodYQuantity) +2, (cameraMicroPos.y -(h/2)) + (j + 1) * (h/foodYQuantity) -2);
                CreateAndPopulate(food, new Vector3(x, y, 0));
            }
        }
    }

     public GameObject CreateAndPopulate(GameObject g, Vector2 pos){
        GameObject gO = (GameObject)Instantiate(g, pos, Quaternion.identity);
        gO.SetActive(true);
        objectPool[gO.tag].Add(gO);
        return gO;
    }

    public GameObject Populate(GameObject g){
        Vector2 pos = new Vector2(0,0);
        GameObject gO = (GameObject)Instantiate(g, pos, Quaternion.identity);
        if (gO.GetComponent<bacteriaAiMicro>() != null)
        {
            gO.GetComponent<bacteriaAiMicro>().GameManager = gameObject;
        }
        gO.SetActive(false);
        objectPool[gO.tag].Add(gO);
        return gO;
    }

    public void setupWalls(){
        GameObject eastWall = (GameObject)Instantiate(Wall, new Vector2(cameraMicroPos.x - (w/2), cameraMicroPos.y), Quaternion.identity);
        eastWall.transform.localScale = new Vector3(5f,h,0);
        GameObject westWall = (GameObject)Instantiate(Wall, new Vector2(cameraMicroPos.x + (w/2), cameraMicroPos.y), Quaternion.identity);
        westWall.transform.localScale = new Vector3(5f,h,0);
        GameObject southWall = (GameObject)Instantiate(Wall, new Vector2(cameraMicroPos.x, cameraMicroPos.y - (h/2)), Quaternion.identity);
        southWall.transform.localScale = new Vector3(w,5f,0);
        GameObject northWall = (GameObject)Instantiate(Wall, new Vector2(cameraMicroPos.x, cameraMicroPos.y + (h/2)), Quaternion.identity);
        northWall.transform.localScale = new Vector3(w,5f,0);
        GameObject img = (GameObject) Instantiate(backgroundImage,new Vector3(cameraMicroPos.x, cameraMicroPos.y,1), Quaternion.identity);
        img.transform.localScale = new Vector3(w,h,0);
    }

    //returns gameObject if something is created/ or an object is reinstated through object pooling, null otherwise
    public GameObject createFromPool(GameObject g, Vector2 pos)
    {
        if(objectPool[g.tag].Count <= 0) return null;
        GameObject pooledObj = objectPool[g.tag][0];
        pooledObj.transform.position = pos;
        pooledObj.transform.rotation = Quaternion.identity;
        pooledObj.GetComponent<SpriteRenderer>().color = g.GetComponent<SpriteRenderer>().color;
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
