using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

	public GameObject gameManager;
	public GameObject waveSpawner;
    public GameObject canvas;
    public GameObject worldCanvas;
    public GameObject moneyTextBox;
    public GameObject turretsCanvas;
    public AudioSource soundManager;
    public GameObject greenMoney;
    public GameObject error;
    public GameObject warning;
    public GameObject bossWarning;
    public List<AudioClip> sounds; // 0 is Cancel ; 1 is changing turret;
    public int waveNumber;

	// Use this for initialization
	void Start () {
		//Set UI prices
		List<GameObject> basicTowers = gameManager.GetComponent<gameManager>().basicTowers;
        for (int i = 0; i < basicTowers.Count; i++){
            turretsCanvas.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text = basicTowers[i].GetComponent<Turret>().cost + "$";
            turretsCanvas.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = basicTowers[i].GetComponent<Turret>().timeTillPerish +"";
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void showMoney(int money, Vector3 position){
        GameObject m = (GameObject) Instantiate(greenMoney, position, Quaternion.identity);
        m.transform.parent = worldCanvas.transform;
        m.GetComponent<Text>().text = "$" + money;
        Destroy(m, 3);
    }

    public void showError(Vector3 position){
        GameObject m = (GameObject) Instantiate(error, position, Quaternion.identity);
        m.transform.position = new Vector3(m.transform.position.x, m.transform.position.y, 1);
        Destroy(m, 1);
    }
    public void showWarning(GameObject g, bool isBoss){
        GameObject m;
        if(isBoss)m = (GameObject) Instantiate(bossWarning);
        else m = (GameObject) Instantiate(warning);
        m.transform.parent = canvas.transform;
        m.GetComponent<warning>().target = g;
        //Destroy(m, 2);
    }

	public void changeColor(int i){
        if(waveNumber < 2 && i > 0) return;
        else if(waveNumber < 7 && i > 1) return;
        else if(waveNumber < 11 && i > 2) return;
        if (gameManager.GetComponent<gameManager>().currentColor == i) return;
        soundManager.PlayOneShot(sounds[0]);
        gameManager.GetComponent<gameManager>().currentColor = i;
    }


    public void changeTurret(int i){
        gameManager.GetComponent<gameManager>().creatingTurret = true;
        soundManager.PlayOneShot(sounds[1]);
        gameManager.GetComponent<gameManager>().currentTurret = i;
    }

    void OnGUI(){
        moneyTextBox.GetComponent<Text>().text = "$ "+ gameManager.GetComponent<gameManager>().money;
        turretsCanvas.GetComponent<Image>().color = gameManager.GetComponent<gameManager>().colors[gameManager.GetComponent<gameManager>().currentColor];
    }
}
