using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobMelee : BaseMob {


    float meleeRange = 0.22f;
    float damagePerTick = 1f;
    float tickRatePerSecond = 1f; //one tick per second = 1f

    float minDistance = 0.15f; //distance between each mob - if lower, then push away
    float mobPushSpeed = 0.2f;
    float timeLeft = 0f;
    public bool hitAll = true;

    // Use this for initialization
    void Start () {
		
	}

    private void resetAttack()
    {
        if (timeLeft < 0)
        {
            timeLeft = 1f / tickRatePerSecond;
        }
    }

    private void damage()
    {
        if(timeLeft < 0)
        {
            base.hitTarget.GetComponent<BaseMob>().hitpoints -= damagePerTick;
            base.hitTarget.GetComponent<BaseMob>().damaged = true;
        }
    }
	
    private void updateHitTarget()
    {
        float dist = 100000f;
        float minDist = 100000f;

        //Melee
        if (base.hitTarget == null)
        {
            //check the current room for opposing mobs
            for (int x = 0; x < base.currentRoom.mobs.Count; x++)
            {
                if (base.currentRoom.mobs[x].GetComponent<BaseMob>() != null)
                {
                    if ((base.player && !base.currentRoom.mobs[x].GetComponent<BaseMob>().player) ||
                    (!base.player && base.currentRoom.mobs[x].GetComponent<BaseMob>().player))
                    {
                        dist = Vector3.Distance(new Vector3(base.currentRoom.mobs[x].transform.position.x,
                                                                                    base.currentRoom.mobs[x].transform.position.y, 0f),
                                                            new Vector3(transform.position.x, transform.position.y, 0f));


                        if (dist < meleeRange)
                        {
                            base.hitTarget = base.currentRoom.mobs[x];
                            if (hitAll)
                            {
                                damage();
                            }
                        }
                        else if (dist < minDist)
                        {
                            base.hitTarget = base.currentRoom.mobs[x];//{revent this line from executing to cause mobs to only hit when passing and in range
                            if (dist < meleeRange)
                            {
                                damage();
                            }
                        }
                    }
                }

            }

        }
        else
        {
            if (!hitAll)
            {
                dist = Vector3.Distance(new Vector3(hitTarget.gameObject.transform.position.x,
                                                                                hitTarget.gameObject.transform.position.y, 0f),
                                                        new Vector3(transform.position.x, transform.position.y, 0f));
                if (dist < meleeRange)
                {
                    damage();
                }
            }
        }
        for (int x = 0; x < base.currentRoom.mobs.Count; x++)
        {
            if (base.gameObject != base.currentRoom.mobs[x])
            {
                dist = Vector3.Distance(new Vector3(base.currentRoom.mobs[x].transform.position.x,
                                                                            base.currentRoom.mobs[x].transform.position.y, 0f),
                                                    new Vector3(transform.position.x, transform.position.y, 0f));
                //Move other mobs away
                if (dist < minDistance)
                {
                    gameObject.transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y),
                        new Vector3(base.currentRoom.mobs[x].transform.position.x, base.currentRoom.mobs[x].transform.position.y),
                        -mobPushSpeed * Time.deltaTime);
                }

                if (dist < meleeRange)
                {
                    if (!hitAll)
                    {
                        if ((base.player && !base.currentRoom.mobs[x].GetComponent<BaseMob>().player) ||
                        (!base.player && base.currentRoom.mobs[x].GetComponent<BaseMob>().player))
                        {
                            base.hitTarget = base.currentRoom.mobs[x];
                            damage();
                        }
                    }
                }
            
            }
        }
        if (hitTarget != null)
        {
            walkTarget = hitTarget;
        }
        resetAttack();
    }

    // Update is called once per frame
    override public void Update()
    {
        updateHitTarget();
        base.Update();
        
        if (timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime * 1000f;
        }

    }
}
