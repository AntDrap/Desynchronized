using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : TriggerBehavior
{
    public GameObject prize;
    public MeshRenderer marker;
    public float moveRange, moveSpeed;
    public Vector3 startPos;
    public Vector3 moveDirection;
    public bool prizeGiven;
    public bool shot;

    private void Update()
    {
        if(!shot)
        {
            transform.localPosition = startPos + moveDirection * (Mathf.PingPong(Time.time * moveSpeed, moveRange * 2) - moveRange);
        }
    }

    public override void Trigger()
    {
        hitTarget(true);

        onTrigger.Invoke();
    }

    public void resetTarget()
    {
        hitTarget(false);
    }

    public void hitTarget(bool state, bool prizeState = true)
    {
        shot = state;

        if(prize && prizeState && !prizeGiven && shot)
        {
            prize.SetActive(true);
        }

        prizeGiven = prizeState;

        if (state)
        {
            marker.material.SetColor("_EmissionColor", Color.black);
        }
        else
        {
            marker.material.SetColor("_EmissionColor", Color.yellow);
        }
    }
}
