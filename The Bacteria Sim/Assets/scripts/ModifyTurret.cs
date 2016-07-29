using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifyTurret : MonoBehaviour {

    private GameObject currentObj;
    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetButton("Fire1")){
            if (currentObj != null) currentObj.transform.position = mousePos;
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit){
                if (hit.collider != null){
                    if (hit.collider.tag == "Turret"){
                        currentObj = hit.collider.gameObject;
                    }

                }
            }
        }
        else if (Input.GetButton("Fire2")){
            if (currentObj != null) objLookAt(currentObj, mousePos);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit){
                if (hit.collider != null){
                    if (hit.collider.tag == "Turret"){
                        currentObj = hit.collider.gameObject;
                    }

                }
            }
        }
        else {
            if(currentObj != null) currentObj = null;
        }

        
    }
    void objLookAt(GameObject g, Vector3 pos){
        pos.x = pos.x - g.transform.position.x;
        pos.y = pos.y - g.transform.position.y;
        float angle = (int) (Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
        g.transform.rotation  = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle-90)),1);
    }
}
