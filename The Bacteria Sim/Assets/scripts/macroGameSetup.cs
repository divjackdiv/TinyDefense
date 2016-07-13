using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
//Under this is to do with game Management of the micro game
public class macroGameSetup : MonoBehaviour {

    public List<GameObject> Colonies;
    public Vector2 bottomLeftPosition;
    public float x;
    public float y;

    public void setupMacroLevel(){
        GetComponent<microGameSetup>().objects = new List<GameObject>();
        for (int i = 0; i < Colonies.Count; i++){
            float localX = (float) Random.Range(bottomLeftPosition.x, bottomLeftPosition.x + x);
            float localY = (float) Random.Range(bottomLeftPosition.y, bottomLeftPosition.y + x);
            createRandColony(Colonies[i],new Vector2(localX,localY));
            GetComponent<microGameSetup>().objects.Add(Colonies[i].GetComponent<colony>().bacteria);
        }
    }
    public GameObject createRandColony(GameObject g, Vector2 pos)
    {
        GameObject colony = (GameObject)Instantiate(g, pos, Quaternion.identity);
        Color randColor = new Color(Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f), 1);
        colony.GetComponent<colony>().color = randColor;
        colony.GetComponent<colony>().growthSpeed = Random.Range (0.6f, 1.2f);
        colony.GetComponent<colony>().movementSpeed = Random.Range (1,2) /10;
        return colony;
    }
}