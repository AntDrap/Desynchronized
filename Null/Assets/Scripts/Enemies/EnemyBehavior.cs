using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : TriggerBehavior
{
    public float health, meleeDamage, wanderRange, randomTimer, speed;
    public GameObject player, hitBox;
    public bool isAggressive;
    public Vector3 randomPosition;
    public ParticleSystem death;
    public NavMeshAgent nAgent;

    public virtual void Start()
    {
        if(hitBox)
        {
            hitBox.GetComponent<EnemyHitbox>().damage = meleeDamage;
            hitBox.SetActive(false);
        }

        print(name);
        player = FindObjectOfType<PlayerBehavior>().gameObject;
        nAgent.destination = FindObjectOfType<PlayerBehavior>().transform.position;
    }

    public virtual void Update()
    {
        if (!player) { player = FindObjectOfType<PlayerBehavior>().gameObject; return; }
    }

    public virtual void ChangeHealth(float amount)
    {
        if (health <= 0)
        {
            return;
        }

        health -= amount;

        if (health <= 0)
        {
            death.Play();
            nAgent.velocity = Vector3.zero;
            nAgent.speed = 0;
            nAgent.angularSpeed = 0;
        }
    }

    public Vector3 RandomNavmeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRange, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    public void turnOffHitbox()
    {
        hitBox.SetActive(false);
    }
}