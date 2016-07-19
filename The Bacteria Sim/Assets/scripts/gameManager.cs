using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class gameManager : MonoBehaviour {
    public GameObject debug;
    private GameObject lastdebug;
    public Vector2 origin;
    public float widthOfWorld;
    public float heightOfWorld;
    public int widthOfGrid;
    public int heightOfGrid;
    float Xstep;
    float Ystep;
    public Camera camera;
    public Vector3 cameraMacroPos;
    public List<GameObject> basicTowers;
    private GameObject currentGameObject;
    private GameObject draggedObj;
    bool isDragging;
    bool wasHoldingDown;
    Vector2 oldPos;
    Vector2 defaultNullPos;
    List<Dictionary<int, GameObject>> worldGrid;
    public void Start(){
        Xstep = widthOfWorld /widthOfGrid;
        Ystep = heightOfWorld / heightOfGrid;
        worldGrid = new List<Dictionary<int, GameObject>>();
        Application.targetFrameRate = 30;
        setupWorld();
        wasHoldingDown = false;
        isDragging = false;
        defaultNullPos = new Vector2(-1,-1);
        oldPos = defaultNullPos;
    }
    
    void LateUpdate () {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Drag an already existing GameObject
        if (Input.GetButton("Fire1") && !isDragging){
            if (currentGameObject != null){
                currentGameObject.transform.position = mousePos;
            } 
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit){
                if (hit.collider != null){
                    if (hit.collider.tag == "Turret"){
                        wasHoldingDown = true;
                        currentGameObject = hit.collider.gameObject;
                        if (oldPos == defaultNullPos){
                            oldPos = currentGameObject.transform.position;
                        }
                    }

                }
            }
        }
        //DROP
        else if(wasHoldingDown && currentGameObject != null){
            if (!isTaken(mousePos)){
                currentGameObject.transform.position = nearestPoint(mousePos);
                int x = (int)(Mathf.Round(mousePos.x)/Xstep);
                int y = (int)(Mathf.Round(mousePos.y)/Ystep);
                if(lastdebug != null) Destroy(lastdebug);
                lastdebug = (GameObject) Instantiate(debug, mousePos, Quaternion.identity);
                worldGrid[x][y] = currentGameObject;
                x = (int)(Mathf.Ceil(oldPos.x)/Xstep);
                y = (int)(Mathf.Ceil(oldPos.y)/Ystep);
                worldGrid[x][y] = null;
                oldPos = defaultNullPos;
            }
            else {
                currentGameObject.transform.position = oldPos;
                currentGameObject = null;
                oldPos = defaultNullPos;
            }
            wasHoldingDown = false;
        }
        else if (Input.GetButton("Fire2") && !isDragging){
            if (currentGameObject != null) objLookAt(currentGameObject, mousePos);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit){
                if (hit.collider != null){
                    if (hit.collider.tag == "Turret"){
                        currentGameObject = hit.collider.gameObject;
                    }

                }
            }
        }
        else {
            if(currentGameObject != null) currentGameObject = null;
        }    
    }

    public void setupWorld(){
        createGrid(widthOfGrid, heightOfGrid, widthOfWorld, heightOfWorld);
    }

    public void createGrid(int x, int y, float worldX, float worldY){
        for (int i = 0; i < x; i++){
            worldGrid.Add(new Dictionary<int, GameObject>());
            for (int j = 0; j < y; j++){
                worldGrid[i].Add(j,null);
            }
        }
    }

    public void OnDrag (int i){ 
        isDragging = true;
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (draggedObj == null ){
            draggedObj = createTower(i);
            MonoBehaviour[] scripts = draggedObj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts) script.enabled = false;
        }
        draggedObj.transform.position = mousePosition;
        //Should probably play some wiggling animation
    }

    public void OnDrop(){
        isDragging = false;
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (mousePosition.x < 0 || mousePosition.y < 0)  Destroy(draggedObj);
        Vector2 p = nearestPoint(mousePosition);
        if (isTaken(p))
        {
            if(draggedObj != null) Destroy(draggedObj);
        }
        else{
            int x = (int)(Mathf.Ceil(p.x)/Xstep);
            int y = (int)(Mathf.Ceil(p.y)/Ystep);
            draggedObj.transform.position =  p;
            worldGrid[x][y] = draggedObj;
            MonoBehaviour[] scripts = draggedObj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts) script.enabled = true;
        }
        draggedObj = null;
    }

    bool isTaken(Vector2 mousePosition){
        int x = (int)(Mathf.Ceil(mousePosition.x/Xstep));
        int y = (int)(Mathf.Ceil(mousePosition.y/Ystep));
        if(worldGrid[x][y] != null)return true; 
        return false;
    }

    Vector2 nearestPoint(Vector2 v){
        int x = (int) ((Mathf.Round(v.x/Xstep))*Xstep);
        int y = (int) ((Mathf.Round(v.y/Ystep))*Ystep); 
        return new Vector2(x,y);
    }

    GameObject createTower(int i){
        GameObject tower = Instantiate(basicTowers[i]);
        tower.GetComponent<Rigidbody2D>().isKinematic = true;
        return tower;
    }
    void objLookAt(GameObject g, Vector3 pos){
        pos.x = pos.x - g.transform.position.x;
        pos.y = pos.y - g.transform.position.y;
        float angle = (int) (Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
        g.transform.rotation  = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)),1);
    }
}
