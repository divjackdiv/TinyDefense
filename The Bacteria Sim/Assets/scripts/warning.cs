using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class warning : MonoBehaviour {

    public GameObject target;
	void Update () {
        if(target == null) Destroy(gameObject);
        if(!target.activeSelf) gameObject.SetActive(false);
        else{
            gameObject.SetActive(true);
            position(target);
        }
	}
   

    void position(GameObject t){
        Vector3 pos = Camera.main.WorldToViewportPoint(t.transform.position);
        if(pos.x<=1 && pos.x >= 0 && pos.y <=1 && pos.y >= 0){
            GetComponent<Image>().enabled = false;
            transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
        else {
            GetComponent<Image>().enabled = true;
            transform.GetChild(0).GetComponent<Image>().enabled = true;
        }
        pos = Camera.main.ViewportToScreenPoint(new Vector3( Mathf.Clamp(pos.x, 0, 1) , Mathf.Clamp(pos.y, 0, 1) , 1));
        transform.position = new Vector3(pos.x, pos.y, 1);
        pos = Camera.main.ScreenToWorldPoint(pos);
        Vector3 rot;
        rot.x = t.transform.position.x - pos.x;
        rot.y = t.transform.position.y - pos.y;
        float angle = (int) (Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg);
        transform.rotation  = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle-90)),1);
    }
    
}
