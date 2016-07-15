using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
//Links the scripts together, passes from macro to micro world
public class gameManager : MonoBehaviour {
    public GameObject canvas;
    public Camera camera;
    public GameObject userInterfaceManager;
    public Vector3 cameraMacroPos;
    public Vector3 cameraMicroPos;
    public int MaxNumberOfBacterias;
    float w;
    float h;

    public void Start(){
        Application.targetFrameRate = 30;
        w = Screen.width;
        h = Screen.height;
        GetComponent<microGameSetup>().maxSize = MaxNumberOfBacterias;
        setupWorld();
    }
    public void changeToMicro(GameObject colony){
        Color colonyColor = colony.GetComponent<colony>().color;
        GameObject bacteria = colony.GetComponent<colony>().bacteria;
        canvas.SetActive(false);
        camera.transform.position = cameraMicroPos;
        int size = colony.GetComponent<colony>().size;
        if (size > MaxNumberOfBacterias) size = MaxNumberOfBacterias;
        for (int i = 0; i < size; i++){
            float x = Random.Range(cameraMicroPos.x - (w/2) +2, cameraMicroPos.x + (w/2) -2);
            float y = Random.Range(cameraMicroPos.y - (h/2) +2, cameraMicroPos.y + (h/2) -2);                
            bacteria = GetComponent<microGameSetup>().createFromPool(bacteria, new Vector2(x,y));
            bacteria.GetComponent<SpriteRenderer>().color = colonyColor;
        }
    }

    public void changeToMacro(GameObject colony){
        GameObject bacteria = colony.GetComponent<colony>().bacteria;
        canvas.SetActive(true);
        camera.transform.position = cameraMacroPos;
        GetComponent<microGameSetup>().destroyAll(bacteria);
    }

    public void setupWorld(){
        GetComponent<macroGameSetup>().cameraMacroPos = cameraMacroPos;
        GetComponent<macroGameSetup>().setupMacroLevel();
        GetComponent<microGameSetup>().cameraMicroPos = cameraMicroPos;
        GetComponent<microGameSetup>().setupMicroLevel();
        userInterfaceManager.GetComponent<userInterface>().setupUi();
    }

}
