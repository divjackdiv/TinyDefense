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

    private Vector2 microBottomLeftPosition;
    private int microGameX;
    private int microGameY;

    public void Start(){
        microBottomLeftPosition = GetComponent<microGameSetup>().microBottomLeftPosition;
        microGameX = GetComponent<microGameSetup>().microGameX;
        microGameY = GetComponent<microGameSetup>().microGameY;

        GetComponent<macroGameSetup>().setupMacroLevel();
        GetComponent<microGameSetup>().setupMicroLevel();
        userInterfaceManager.GetComponent<userInterface>().setupUi();
    }
    public void changeToMicro(GameObject colony){
        Color colonyColor = colony.GetComponent<colony>().color;
        GameObject bacteria = colony.GetComponent<colony>().bacteria;
        canvas.SetActive(false);
        camera.transform.position = cameraMicroPos;
        int size = colony.GetComponent<colony>().size;
        if (size > 50) size = 50;
        for (int i = 0; i < size; i++){
            float x = Random.Range(microBottomLeftPosition.x, microBottomLeftPosition.x + microGameX);
            float y = Random.Range(microBottomLeftPosition.y, microBottomLeftPosition.y + microGameY);                
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


}
