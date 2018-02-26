using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMob : MonoBehaviour {

    public Room currentRoom;
    float hitpoints = 10f;
    float walkSpeed = 0.5f;
    float turnSpeed = 0.1f;
    bool canMove = true;
    public GameObject hitTarget;
    public GameObject walkTarget;
    float distanceToWalkTarget = 0f;
    float minWalkDist = 0.2f;
    float rotationOffset = 90f;
    bool player = false;
    // Use this for initialization
    void Start () {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.5f);
    }
	
	// Update is called once per frame
	void Update () {
        updateMovement();

    }

    private void updateMovement()
    {
        if (walkTarget != null)
        {
            distanceToWalkTarget = Vector3.Distance(new Vector3(walkTarget.transform.position.x, walkTarget.transform.position.y, 0f),
                                                    new Vector3(transform.position.x, transform.position.y, 0f));

            if (canMove && walkTarget != null && distanceToWalkTarget > minWalkDist)
            {
                gameObject.transform.position = new Vector3(
                    gameObject.transform.position.x + Mathf.Cos(gameObject.transform.rotation.eulerAngles.z) * walkSpeed * Time.deltaTime,
                    gameObject.transform.position.y + Mathf.Sin(gameObject.transform.rotation.eulerAngles.z) * walkSpeed * Time.deltaTime,
                    0.5f);
            }

            if (distanceToWalkTarget > minWalkDist)
            {
                Vector3 targetDir = walkTarget.transform.position - transform.position;
                targetDir = new Vector3(targetDir.x, targetDir.y, targetDir.z);
                float rot_z = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z + rotationOffset);
            }
        }
    }
}
