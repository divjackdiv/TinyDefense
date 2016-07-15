using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
//Under this is to do with game Management of the micro game
public class macroGameSetup : MonoBehaviour {

    public List<GameObject> Colonies;
    public Vector2 cameraMacroPos;
    public GameObject Wall;
    public GameObject backgroundImage;

    public void setupMacroLevel(){
        setupWalls();
        float w = Screen.width;
        float h = Screen.height;
        for (int i = 0; i < Colonies.Count; i++){
            float localX = (float) Random.Range(cameraMacroPos.x - (w/2), cameraMacroPos.x + (w/2));
            float localY = (float) Random.Range(cameraMacroPos.y - (h/2), cameraMacroPos.y + (h/2));
            createRandColony(Colonies[i],new Vector2(localX,localY));
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

    public void setupWalls(){
        float w = Screen.width;
        float h = Screen.height;
        GameObject eastWall = (GameObject)Instantiate(Wall, new Vector2(cameraMacroPos.x - (w/2), cameraMacroPos.y), Quaternion.identity);
        eastWall.transform.localScale = new Vector3(5f,h,0);
        GameObject westWall = (GameObject)Instantiate(Wall, new Vector2(cameraMacroPos.x + (w/2), cameraMacroPos.y), Quaternion.identity);
        westWall.transform.localScale = new Vector3(5f,h,0);
        GameObject southWall = (GameObject)Instantiate(Wall, new Vector2(cameraMacroPos.x, cameraMacroPos.y - (h/2)), Quaternion.identity);
        southWall.transform.localScale = new Vector3(w,5f,0);
        GameObject northWall = (GameObject)Instantiate(Wall, new Vector2(cameraMacroPos.x, cameraMacroPos.y + (h/2)), Quaternion.identity);
        northWall.transform.localScale = new Vector3(w,5f,0);
        GameObject img = (GameObject) Instantiate(backgroundImage,new Vector3(cameraMacroPos.x, cameraMacroPos.y,1), Quaternion.identity);
        img.transform.localScale = new Vector3(w,h,1);
    }
}