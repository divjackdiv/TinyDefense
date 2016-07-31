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


    //Related objects in the scene
    public Camera camera;
    public GameObject center;
    public Vector3 cameraMacroPos;
    public GameObject userInterface;

    //Dragging and dropping
    private GameObject currentGameObject;
    private GameObject rotatingObj;
    public int currentTurret;
    public bool creatingTurret;
    public bool wasHoldingDown;
    Vector2 oldPos;
    Vector2 defaultNullPos;

    //Money
    public int startMoney;
    public int money;

    //Tower Creation 
    public List<GameObject> basicTowers;
    public List<Color> colors;
    public int currentColor; 
    List<GameObject> createdTowers;
    int layermask;
    private  GameObject turret;
    //Sound 
    public AudioSource soundManager;
    public List<AudioClip> sounds; //0 is tower death; 1 is tower pick up; 2 is drop; 3 is drop did not work;

    public void Start(){
        layermask = ~(1 << 11);
        createdTowers = new List<GameObject>();
        Application.targetFrameRate = 60;
        Xstep = widthOfWorld /widthOfGrid;
        Ystep = heightOfWorld / heightOfGrid;
        worldGrid = new List<Dictionary<int, GameObject>>();
        setupWorld();

        currentTurret = -1;
        creatingTurret = false;
        wasHoldingDown = false;
        defaultNullPos = new Vector2(-1,-1);
        oldPos = defaultNullPos;
        money = startMoney;
        
    }
    public void restartGame(){
        Time.timeScale = 1;
        userInterface.GetComponent<UserInterface>().changeColor(0);
        Application.LoadLevel(Application.loadedLevel);
    }
    
    void LateUpdate () {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetButtonDown("Fire1")){

            if (creatingTurret){
                creatingTurret = false;
                turret = createTower(currentTurret, colors[currentColor]);
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector2 p = nearestPoint(mousePosition);
                if (isTaken(p) || turret.GetComponent<Turret>().cost > money || EventSystem.current.IsPointerOverGameObject(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject(0)) {
                        soundManager.PlayOneShot(sounds[3]);
                        userInterface.GetComponent<UserInterface>().showError(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    }
                    if(turret != null) Destroy(turret);
                }
                else{
                    money -= turret.GetComponent<Turret>().cost;
                    int x = (int)(Mathf.Round(p.x)/Xstep);
                    int y = (int)(Mathf.Round(p.y)/Ystep);
                    turret.transform.position = p;
                    MonoBehaviour[] scripts = turret.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour script in scripts) script.enabled = true;        
                    worldGrid[x][y] = turret;
                    createdTowers.Add(turret);
                    soundManager.PlayOneShot(sounds[2]); 
                }
            }           
           /* else if (Input.touchCount == 2){
                if (rotatingObj != null){
                    objLookAt(rotatingObj, Input.GetTouch(1).position);
                    camera.GetComponent<androidCamera>().rotating = true;
                } 
                else{
                    RaycastHit2D hit = Physics2D.Raycast(Input.GetTouch(0).position, Vector2.zero, Mathf.Infinity, layermask);
                    if (hit){
                        if (hit.collider != null){
                            if (hit.collider.tag == "Turret"){
                                soundManager.PlayOneShot(sounds[4]);
                                rotatingObj = hit.collider.gameObject;
                            }

                        }
                    }
                    else camera.GetComponent<androidCamera>().rotating = false;
                }     
            }    */
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
                    soundManager.PlayOneShot(sounds[2]);
                }
                else {
                    userInterface.GetComponent<UserInterface>().showError(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    soundManager.PlayOneShot(sounds[3]); 
                    currentGameObject.transform.position = oldPos;
                    oldPos = defaultNullPos;
                }
                currentGameObject = null;
                wasHoldingDown = false;
            }
            else {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layermask);
                if (hit){
                    if (hit.collider != null){
                        if (hit.collider.tag == "Turret"){
                            currentGameObject = hit.collider.gameObject;
                            wasHoldingDown = true;
                            if (oldPos == defaultNullPos){
                                soundManager.PlayOneShot(sounds[1]); 
                                oldPos = currentGameObject.transform.position;
                            }
                        }
                    }
                }
            }
        }
        else if (Input.GetButton("Fire2")){
            if (rotatingObj != null){
                objLookAt(rotatingObj, mousePos);
                camera.GetComponent<androidCamera>().rotating = true;
            } 
            else{
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layermask);
                if (hit){
                    if (hit.collider != null){
                        if (hit.collider.tag == "Turret"){
                            rotatingObj = hit.collider.gameObject;
                        }

                    }
                }
            }           
        }
        else {
            rotatingObj = null;
            camera.GetComponent<androidCamera>().rotating = false;
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
        tower.GetComponent<Turret>().soundManager = soundManager;
        tower.GetComponent<Turret>().dyingSound = sounds[0];       
        tower.GetComponent<Rigidbody2D>().isKinematic = true;
        return tower;
    }
    void objLookAt(GameObject g, Vector3 pos){
        pos.x = pos.x - g.transform.position.x;
        pos.y = pos.y - g.transform.position.y;
        float angle = (int) (Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
        g.transform.rotation  = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle-90)),1);
    }
}
