using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoombaBehavior : EnemyBehavior
{
    public float spinTime;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if (health <= 0)
        {
            if (spinTime > 0)
            {
                transform.Rotate(new Vector3(0, 0, 180) * Time.deltaTime * spinTime);
                spinTime -= Time.deltaTime;
            }

            return;
        }

        nAgent.speed = Mathf.Lerp(nAgent.speed, 2, Time.deltaTime);

        nAgent.destination = randomPosition;

        if (Vector3.Distance(transform.position, randomPosition) < 0.25f || randomTimer <= 0)
        {
            randomPosition = RandomNavmeshLocation();
            randomTimer = Random.Range(3, 6);
        }

        randomTimer -= Time.deltaTime;
    }

    public override void ChangeHealth(float amount)
    {
        if (health <= 0)
        {
            return;
        }

        health -= amount;

        randomPosition = RandomNavmeshLocation();
        randomTimer = 2;
        nAgent.speed = 6;

        if (health <= 0)
        {
            death.Play();
            nAgent.speed = 0;
            nAgent.angularSpeed = 0;
            nAgent.velocity = Vector3.zero;
            spinTime = 3f;
        }
    }
}