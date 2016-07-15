using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// User interface script
public class userInterface : MonoBehaviour {

    public Transform canvasButtons;
    public int nbOfButtons = 3;
    public Transform GameManager;
    int currentObject;
    GameObject currentGameObject;
    List<GameObject> objects;

    public void setupUi(){
        objects =  GameManager.GetComponent<macroGameSetup>().Colonies;
        for (int i = 0; i < 3 /*objects.Count*/; i++)
        {
            canvasButtons.GetChild(i).GetComponent<Image>().sprite = objects[i].GetComponent<SpriteRenderer>().sprite;
            canvasButtons.GetChild(i).GetComponent<Image>().preserveAspect = true;
            canvasButtons.GetChild(i).GetComponent<Image>().color = objects[i].GetComponent<SpriteRenderer>().color;
        }
    }
    public void OnDrag(){ 
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (currentGameObject == null ){
            currentGameObject = GameManager.GetComponent<macroGameSetup>().createRandColony(objects[currentObject], mousePosition);
            MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts) script.enabled = false;
        }
        currentGameObject.transform.position = mousePosition;
        //Should probably play some wiggling animation
    }

    public void OnDrop(){
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if(currentGameObject != null) Destroy(currentGameObject);
        }
        MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = true;
        currentGameObject = null;
    }

    public void changeObject(int i)
    {
        if (i > nbOfButtons - 1) return;
        if (i == 1)
        {
            Sprite temp = canvasButtons.GetChild(0).GetComponent<Image>().sprite;
            Color col = canvasButtons.GetChild(0).GetComponent<Image>().color;

            canvasButtons.GetChild(0).GetComponent<Image>().sprite = canvasButtons.GetChild(1).GetComponent<Image>().sprite;
            canvasButtons.GetChild(0).GetComponent<Image>().color = canvasButtons.GetChild(1).GetComponent<Image>().color;

            canvasButtons.GetChild(1).GetComponent<Image>().sprite = canvasButtons.GetChild(2).GetComponent<Image>().sprite;
            canvasButtons.GetChild(1).GetComponent<Image>().color = canvasButtons.GetChild(2).GetComponent<Image>().color;

            canvasButtons.GetChild(2).GetComponent<Image>().sprite = temp;
            canvasButtons.GetChild(2).GetComponent<Image>().color = col;
            currentObject = (currentObject + 1) % nbOfButtons; 
        }
        else if (i == -1)
        {
            Sprite temp = canvasButtons.GetChild(2).GetComponent<Image>().sprite;
            Color col = canvasButtons.GetChild(2).GetComponent<Image>().color;

            canvasButtons.GetChild(2).GetComponent<Image>().sprite = canvasButtons.GetChild(1).GetComponent<Image>().sprite; 
            canvasButtons.GetChild(2).GetComponent<Image>().color = canvasButtons.GetChild(1).GetComponent<Image>().color;

            canvasButtons.GetChild(1).GetComponent<Image>().sprite = canvasButtons.GetChild(0).GetComponent<Image>().sprite; 
            canvasButtons.GetChild(1).GetComponent<Image>().color = canvasButtons.GetChild(0).GetComponent<Image>().color;

            canvasButtons.GetChild(0).GetComponent<Image>().sprite = temp; 
            canvasButtons.GetChild(0).GetComponent<Image>().color = col;
            currentObject = currentObject - 1;
            if (currentObject < 0) currentObject = nbOfButtons - 1;
        }
    }    
    public void slowTime()
    {
        int timeScales = GameManager.GetComponent<microGameSetup>().timeScales;
        if (timeScales >= 16) timeScales = 1;
        else timeScales *= 2;
        Time.timeScale = timeScales;
        GameManager.GetComponent<microGameSetup>().timeScales = timeScales;
    }

}
