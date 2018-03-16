using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsScript : MonoBehaviour {

    public Canvas mobHealthCanvas;
    public GameObject mobHealthBar;
    public GameObject mob1;
    public GameObject mob2;
    public float distance = 4.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject mob = (Input.GetButtonDown("Fire1") ? mob2 : 
                                 (Input.GetButtonDown("Fire2") ? mob1 : null));
                GameObject g = Instantiate(mob, new Vector3(hit.point.x, hit.point.y, 0f), transform.rotation);
                GameObject hp = Instantiate(mobHealthBar, new Vector3(hit.point.x, hit.point.y, 0f), transform.rotation);
                hp.transform.SetParent(mobHealthCanvas.transform, false);
                Room r = hit.transform.gameObject.GetComponent<RoomSprite>().room;
                BaseMob bm = g.GetComponent<BaseMob>();
                bm.currentRoom = r;
                bm.pickTarget();
                bm.player = (Input.GetButtonDown("Fire1") ? false :
                                 (Input.GetButtonDown("Fire2") ? true : false));
                bm.healthSlider = hp;
                bm.damageImage = bm.healthSlider.gameObject.transform.GetComponentInChildren<Image>();
                bm.updateHealthBarPosition();


                r.mobs.Add(g);

            }
            
        }

       
    }
}
