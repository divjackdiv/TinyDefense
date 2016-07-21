using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class gameManager : MonoBehaviour {

    //World Creation
    public Vector2 origin;
    public float widthOfWorld;
    public float heightOfWorld;
    public int widthOfGrid;
    public int heightOfGrid;
    List<Dictionary<int, GameObject>> worldGrid;
    float Xstep;
    float Ystep;

    //UI, Given more time I should probably export the ui to another script
    public GameObject canvas;
    public GameObject moneyTextBox;

    //Related objects in the scene
    public Camera camera;
    public GameObject center;
    public Vector3 cameraMacroPos;

    //Dragging and dropping
    private GameObject currentGameObject;
    private GameObject draggedObj;
    bool isDragging;
    bool wasHoldingDown;
    Vector2 oldPos;
    Vector2 defaultNullPos;

    //Money
    public int money;
    //Tower Creation 
    public List<GameObject> basicTowers;
    public List<Color> colors;
    static int currentColor; 

    public void Start(){
        Application.targetFrameRate = 60;

        Xstep = widthOfWorld /widthOfGrid;
        Ystep = heightOfWorld / heightOfGrid;
        worldGrid = new List<Dictionary<int, GameObject>>();
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
            else{
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
        }
        //DROP
        else if(wasHoldingDown && currentGameObject != null){
            if (!isTaken(mousePos)){
                currentGameObject.transform.position = nearestPoint(mousePos);
                int x = (int)(Mathf.Round(mousePos.x/Xstep));
                int y = (int)(Mathf.Round(mousePos.y/Ystep));
                worldGrid[x][y] = currentGameObject;
                x = (int)(Mathf.Round(oldPos.x/Xstep));
                y = (int)(Mathf.Round(oldPos.y/Ystep));
                worldGrid[x][y] = null;
                oldPos = defaultNullPos;
            }
            else {
                currentGameObject.transform.position = oldPos;
                oldPos = defaultNullPos;
            }
            currentGameObject = null;
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
        for (int i = 0; i <= x; i++){
            worldGrid.Add(new Dictionary<int, GameObject>());
            for (int j = 0; j <= y; j++){
                worldGrid[i].Add(j,null);
            }
        }
        int centerXSize = center.GetComponent<center>().xSize;
        int centerYSize = center.GetComponent<center>().ySize;
        Vector2 centerPos = convertPosToGrid(center.transform.position);
        for (int i = 0; i <= centerXSize*2; i++){
            for (int j = 0; j <= centerYSize*2; j++){
                worldGrid[(int)(centerPos.x)-centerXSize+i][(int)(centerPos.y)-centerYSize+j] = center;
            }
        }
    }

    public void OnDrag (int i){ 
        isDragging = true;
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (draggedObj == null ){
            draggedObj = createTower(i, colors[currentColor]);
        }
        draggedObj.transform.position = mousePosition;
    }

    public void OnDrop(){
        isDragging = false;
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (mousePosition.x < 0 || mousePosition.y < 0)  Destroy(draggedObj);
        Vector2 p = nearestPoint(mousePosition);
        if (isTaken(p) || draggedObj.GetComponent<Turret>().cost > money || EventSystem.current.IsPointerOverGameObject())
        {
            if(draggedObj != null) Destroy(draggedObj);
        }
        else{
            money -= draggedObj.GetComponent<Turret>().cost;
            int x = (int)(Mathf.Round(p.x)/Xstep);
            int y = (int)(Mathf.Round(p.y)/Ystep);
            draggedObj.transform.position = p;
            worldGrid[x][y] = draggedObj;
        }
        draggedObj = null;
    }

    bool isTaken(Vector2 mousePosition){
        int x = (int)(Mathf.Round(mousePosition.x/Xstep));
        int y = (int)(Mathf.Round(mousePosition.y/Ystep));
        if( x < 0 || y < 0 || x >= worldGrid.Count || y >= worldGrid[x].Count || worldGrid[x][y] != null) return true; 
        return false;
    }

    Vector2 nearestPoint(Vector2 v){
        int x = (int) ((Mathf.Round(v.x/Xstep))*Xstep);
        int y = (int) ((Mathf.Round(v.y/Ystep))*Ystep); 
        return new Vector2(x,y);
    }

    Vector2 convertPosToGrid(Vector2 v){
        int x = (int)(Mathf.Round(v.x/Xstep));
        int y = (int)(Mathf.Round(v.y/Ystep));
        return new Vector2(x,y);
    }

    GameObject createTower(int i, Color col){
        GameObject tower = Instantiate(basicTowers[i]);
        tower.GetComponent<SpriteRenderer>().color = col; 
        tower.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = col; 
        tower.GetComponent<Turret>().resistances[currentColor] = true;
        tower.GetComponent<Rigidbody2D>().isKinematic = true;
        return tower;
    }
    void objLookAt(GameObject g, Vector3 pos){
        pos.x = pos.x - g.transform.position.x;
        pos.y = pos.y - g.transform.position.y;
        float angle = (int) (Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
        g.transform.rotation  = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)),1);
    }
    public void changeColor(int i){
        currentColor = i;
    }

    void OnGUI(){
        moneyTextBox.GetComponent<Text>().text = "$$$ " +money;
    }
}
