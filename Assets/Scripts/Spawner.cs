using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Room parentRoom;
    public float spawnAfterMilliseconds = 3000f;
    private float timeLeft = 99f;
    public GameObject[] mobs;
    public bool active = true;
    
    // Use this for initialization
    void Start () {
        timeLeft = spawnAfterMilliseconds;

    }
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            timeLeft -= Time.deltaTime * 1000f;
            if (timeLeft <= 0)
            {
                timeLeft += spawnAfterMilliseconds;
                spawn();
            }
        }
    }

    void spawn()
    {
        int mobType = Random.Range(0, mobs.Length-1);
        GameObject mob = Instantiate(mobs[mobType]);
        mob.transform.position = this.transform.position;
        BaseMob bm = mob.GetComponent<BaseMob>();
        bm.currentRoom = parentRoom;

        int roomIndex = Random.Range(0, parentRoom.AdjacentRooms.Count-1);
        if(roomIndex >= 0 && roomIndex < parentRoom.AdjacentRooms.Count)
        {
            bm.walkTarget = parentRoom.AdjacentRooms[roomIndex].spawner;
        }
        
    }

    public void setParent(Room parent)
    {
        parentRoom = parent;
    }
}
