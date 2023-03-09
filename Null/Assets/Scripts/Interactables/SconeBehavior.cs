using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SconeBehavior : ConsumableBehvaior
{
    Vector3 startPos;
    public override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * (Mathf.PingPong(Time.time * 0.125f, 0.25f) - 0.125f);
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }
}
