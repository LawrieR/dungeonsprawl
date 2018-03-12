using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsScript : MonoBehaviour {

    public GameObject mob1;
    public GameObject mob2;
    public float distance = 4.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject g = Instantiate(mob2, new Vector3(hit.point.x, hit.point.y, 0f), transform.rotation);
                Room r = hit.transform.gameObject.GetComponent<RoomSprite>().room;
                g.GetComponent<BaseMob>().currentRoom = r;
                g.GetComponent<BaseMob>().pickTarget();
                g.GetComponent<BaseMob>().player = false;
                
                r.mobs.Add(g);

            }
            
        }

        if (Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject g = Instantiate(mob1, new Vector3(hit.point.x, hit.point.y, 0f), transform.rotation);
                Room r = hit.transform.gameObject.GetComponent<RoomSprite>().room;
                g.GetComponent<BaseMob>().currentRoom = r;
                g.GetComponent<BaseMob>().pickTarget();
                g.GetComponent<BaseMob>().player = true;
                r.mobs.Add(g);

            }

        }
    }
}
