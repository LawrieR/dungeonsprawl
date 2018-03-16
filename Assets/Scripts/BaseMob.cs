using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMob : MonoBehaviour {

    public Room currentRoom;
    public float hitpoints = 100f;
    float startingHitPoints = 100f;
    float walkSpeed = 0.5f;
    float defaultWalkSpeed = 0.5f;
    float turnSpeed = 0.1f;
    public bool canMove = true;
    public GameObject hitTarget;
    public GameObject walkTarget;
    float distanceToWalkTarget = 0f;
    float minWalkDist = 0.21f;
    float rotationOffset = 90f;
    public bool player = false;
    public GameObject healthSlider;
    public Image damageImage;
    public float damagedFlashSpeed = 5f;
    public Color damageFlashColour = new Color(1f, 0f, 0f, 0.1f);
    public bool damaged;
    public float healthBarHeight = 0.2f;


    // Use this for initialization
    void Start () {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.5f);
        walkSpeed = defaultWalkSpeed;
        startingHitPoints = hitpoints;
        
    }
	
	// Update is called once per frame
	virtual public void Update () {
        updateMovement();
        updateHealth();
        updateHealthBarPosition(); //Should be always updated due to moving camera/canvas
    }

    public void pickTarget()
    {
        bool targetFound = false;//used in disabled code below
        if (!player)
        {
            if (currentRoom != null)
            {
                if (currentRoom.playerOwned)
                {//if the current room isn't owned by mobs, mobs should try take it back
                    walkTarget = currentRoom.spawner;
                    targetFound = true;
                }
                else if (currentRoom.parentRoom != null)
                {

                    walkTarget = currentRoom.parentRoom.spawner;
                    targetFound = true;

                }
                {
                    /*if (currentRoom.playerOwned == false && !targetFound)
                    {//if the mobs own the room, move to next room

                        List<Room> rooms = new List<Room>();
                        for (int x = 0; x < currentRoom.AdjacentRooms.Count; x++)
                        {
                            //Find suitable rooms
                            if (currentRoom.AdjacentRooms[x].playerOwned == true)
                            {
                                rooms.Add(currentRoom.AdjacentRooms[x]);
                            }
                        }

                        //immediately adjecent rooms are not owned by the player
                        if(rooms.Count == 0)
                        {
                            addPotentialRooms(rooms, currentRoom);
                        }

                        int roomIndex = Random.Range(0, rooms.Count);
                        walkTarget = rooms[roomIndex].spawner;
                    }*/
                }
            }
        }
        else
        {
            if (currentRoom != null)
            {
                if (!currentRoom.playerOwned)
                {//if the current room isn't owned by mobs, mobs should try take it back
                    walkTarget = currentRoom.spawner;
                    targetFound = true;
                }
                else if (currentRoom.AdjacentRooms.Count > 0)
                {
                    float dist = 1000000000f;
                    float minDist = 1000000000f;
                    for(int x = 0; x < currentRoom.AdjacentRooms.Count; x++)
                    {
                        //Calculate distance to spawner
                        dist = Vector3.Distance(new Vector3(currentRoom.AdjacentRooms[x].spawner.transform.position.x,
                                                                            currentRoom.AdjacentRooms[x].spawner.transform.position.y, 0f),
                                                    new Vector3(transform.position.x, transform.position.y, 0f));
                        if(dist < minDist)
                        {
                            walkTarget = currentRoom.AdjacentRooms[x].spawner;
                            targetFound = true;
                            minDist = dist;
                        }
                    }
                }
            }
        }
    }

    private void addPotentialRooms(List<Room> _r, Room _room)
    {
        if (_room.playerOwned)
        {
            _r.Add(_room);
        }
        if (_room.parentRoom.playerOwned)
        {
            _r.Add(_room.parentRoom);
        }
        else if (_room.parentRoom != null)
        {
            
            for (int x = 0; x < _room.parentRoom.AdjacentRooms.Count; x++)
            {
                //Find suitable rooms
                if (_room.parentRoom.AdjacentRooms[x].playerOwned == true)
                {
                    _r.Add(_room.parentRoom.AdjacentRooms[x]);
                }
            }

            if (_r.Count == 0)
            {
                Room r = _room.parentRoom.parentRoom;
                if(r != null)
                {
                    addPotentialRooms(_r, r);
                }
            }
        }
    }

    private void updateHealth()
    {
        healthSlider.GetComponentInChildren<Slider>().value = hitpoints/startingHitPoints;
        if (hitpoints <= 0)
        {
            currentRoom.mobs.Remove(this.gameObject);
            //If this mob is a hit target for any other mob, remove this from their hit target
            for(int x=0; x<currentRoom.mobs.Count; x++)
            {
                if(this.gameObject == currentRoom.mobs[x].GetComponent<BaseMob>().hitTarget)
                {
                    currentRoom.mobs[x].GetComponent<BaseMob>().hitTarget = null;
                    currentRoom.mobs[x].GetComponent<BaseMob>().pickTarget();
                }
            }
            for (int y = 0; y < currentRoom.AdjacentRooms.Count; y++)
            {
                for (int x = 0; x < currentRoom.AdjacentRooms[y].mobs.Count; x++)
                {
                    if (this.gameObject == currentRoom.AdjacentRooms[y].mobs[x].GetComponent<BaseMob>().hitTarget)
                    {
                        currentRoom.AdjacentRooms[y].mobs[x].GetComponent<BaseMob>().hitTarget = null;
                        currentRoom.AdjacentRooms[y].mobs[x].GetComponent<BaseMob>().pickTarget();
                    }
                }
            }
            if(currentRoom.parentRoom != null)
            {
                for (int x = 0; x < currentRoom.parentRoom.mobs.Count; x++)
                {
                    if (this.gameObject == currentRoom.parentRoom.mobs[x].GetComponent<BaseMob>().hitTarget)
                    {
                        currentRoom.parentRoom.mobs[x].GetComponent<BaseMob>().hitTarget = null;
                        currentRoom.parentRoom.mobs[x].GetComponent<BaseMob>().pickTarget();
                    }
                }
            }
            GameObject.Destroy(healthSlider);
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            if (damageImage != null)
            {
                if (damaged)
                {
                    damageImage.color = damageFlashColour;
                    damaged = false;
                }
                // Otherwise...
                else
                {
                    damageImage.color = Color.Lerp(damageImage.color, Color.red, damagedFlashSpeed * Time.deltaTime);
                }
            }
        }
        //Could do some health recovery ideas here?
    }

    private Room nearestRoom()
    {
        Room result = null;
        
        float dist = 1000000000f;

        //current room
        float currentRoomDist = Vector3.Distance(new Vector3(currentRoom.spawner.transform.position.x,
                                                                currentRoom.spawner.transform.position.y, 0f),
                                                    new Vector3(transform.position.x, transform.position.y, 0f));
        if (currentRoomDist < dist)
        {
            dist = currentRoomDist;
            result = currentRoom;
        }

        //Adjacent rooms
        for (int x = 0; x<currentRoom.AdjacentRooms.Count; x++)
        {
            float dis = Vector3.Distance(new Vector3(currentRoom.AdjacentRooms[x].spawner.transform.position.x,
                                                                currentRoom.AdjacentRooms[x].spawner.transform.position.y, 0f),
                                                    new Vector3(transform.position.x, transform.position.y, 0f));
            if(dis < dist)
            {
                dist = dis;
                result = currentRoom.AdjacentRooms[x];
            }

        }

        if (currentRoom.parentRoom != null)
        {
            //Parent room
            float parentRoomDist = Vector3.Distance(new Vector3(currentRoom.parentRoom.spawner.transform.position.x,
                                                                    currentRoom.parentRoom.spawner.transform.position.y, 0f),
                                                        new Vector3(transform.position.x, transform.position.y, 0f));
            if (parentRoomDist < dist)
            {
                dist = parentRoomDist;
                result = currentRoom.parentRoom;
            }
        }


        return result;
    }

    public void updateHealthBarPosition()
    {
        healthSlider.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + healthBarHeight, this.transform.position.z);
        healthSlider.gameObject.transform.localScale = (Camera.main.orthographicSize > 2) ? Vector3.zero : Vector3.one;
        
    }

    private void updateMovement()
    {
        currentRoom.mobs.Remove(this.gameObject);
        currentRoom = nearestRoom();
        currentRoom.mobs.Add(this.gameObject);

        if (walkTarget != null)
        {
            distanceToWalkTarget = Vector3.Distance(new Vector3(walkTarget.transform.position.x, walkTarget.transform.position.y, 0f),
                                                    new Vector3(transform.position.x, transform.position.y, 0f));

            if (distanceToWalkTarget > minWalkDist)
            {
                //Check if the current room has just been taken, if it has, turn the mob back around
                if((player && !currentRoom.playerOwned) || (!player && currentRoom.playerOwned))
                {
                    int count = 0;
                    //Count the mobs that are of the opposing type, must be zero to override
                    for (int x = 0; x< currentRoom.mobs.Count; x++)
                    {
                        if(currentRoom.mobs[x].GetComponent<BaseMob>().player != this.player)
                        {
                            count++;
                        }
                    }

                    if (count == 0)
                    {//Ensure the mob isnt trying to kill another mob before getting them to turn around
                        walkTarget = currentRoom.spawner;
                    }
                }
                
                
            }
            else
            {
                if (walkTarget.GetComponent<Spawner>() != null)
                {//If the target was a spawner
                    this.currentRoom = walkTarget.GetComponent<Spawner>().parentRoom;

                    //Change target
                    pickTarget();
                }

            }

            if (canMove && walkTarget != null && distanceToWalkTarget > minWalkDist)
            {
                if (distanceToWalkTarget < walkSpeed + minWalkDist)
                {
                    walkSpeed = distanceToWalkTarget;
                }
                else
                {
                    walkSpeed = defaultWalkSpeed;
                }

                Vector3 targetDir = walkTarget.transform.position - transform.position;
                targetDir = new Vector3(targetDir.x, targetDir.y, 0.5f);
                targetDir.Normalize();
                float rot_z = (Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg) + rotationOffset;
                
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
                gameObject.transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y),
                                                                    new Vector3(walkTarget.transform.position.x, walkTarget.transform.position.y),
                                                                    walkSpeed * Time.deltaTime);
                

            }

        }
    }
}
