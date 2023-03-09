using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderBehavior : EnemyBehavior
{
    public Animator anim;
    public GameObject lightObj;
    public SkinnedMeshRenderer lightbulb;
    bool freeze, attackCool = true;
    Color targetColor;
    float maxSpeed;

    public override void Start()
    {
        base.Start();
        randomTimer = Random.Range(3, 6);
        maxSpeed = speed;
    }

    public override void Update()
    {
        if(!player) { player = FindObjectOfType<PlayerBehavior>().gameObject; return; }
        lightbulb.material.color = Color.Lerp(lightbulb.material.color, targetColor, 3 * Time.deltaTime);
        lightbulb.material.SetColor("_EmissionColor", lightbulb.material.color);
        lightObj.GetComponent<Light>().color = Color.Lerp(lightObj.GetComponent<Light>().color, targetColor, 3 * Time.deltaTime);


        if (health <= 0)
        {
            targetColor = Color.black;
            lightObj.GetComponent<Light>().intensity = Mathf.Lerp(lightObj.GetComponent<Light>().intensity, 0, Time.deltaTime);
            return;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < 7 && Mathf.Abs(player.transform.position.y - transform.position.y) < 2)
        {
            isAggressive = true;
            randomTimer = 5;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < 3f)
        {
            if (attackCool)
            {
                Invoke("Attack", 0.5f);
                anim.SetTrigger("Attack");
                attackCool = false;
                freeze = true;
            }
        }

        if(freeze || Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            nAgent.velocity = Vector3.zero;
            anim.SetBool("Walking", false);
            anim.speed = 1;
        }
        else
        {
            nAgent.speed = Mathf.Lerp(nAgent.speed, maxSpeed, Time.deltaTime * 3f);
            anim.SetBool("Walking", nAgent.velocity.magnitude > 0);
            anim.speed = (nAgent.velocity.magnitude / maxSpeed);
        }

        if (isAggressive)
        {
            targetColor = Color.red;
            nAgent.destination = FindObjectOfType<PlayerBehavior>().transform.position;
        }
        else
        {
            targetColor = Color.yellow;
            nAgent.destination = randomPosition;

            if (randomTimer <= 0)
            {
                randomPosition = RandomNavmeshLocation();
                randomTimer = Random.Range(3, 6);
            }
        }

        randomTimer -= Time.deltaTime;

        if(randomTimer <= 0)
        {
            isAggressive = false;
        }
    }

    public void Attack()
    {
        if(health <= 0)
        {
            return;
        }

        lightObj.GetComponent<ParticleSystem>().Play();

        hitBox.SetActive(true);

        freeze = false;
        Invoke("turnOffHitbox", 0.25f);
        Invoke("CooldownAttack", 1.5f);
    }

    public override void ChangeHealth(float amount)
    {
        if (health <= 0)
        {
            return;
        }

        isAggressive = true;
        randomTimer = 5;

        health -= amount;

        if (health <= 0)
        {
            death.Play();
            nAgent.velocity = Vector3.zero;
            nAgent.speed = 0;
            nAgent.angularSpeed = 0;
            lightObj.GetComponent<ParticleSystem>().Stop();
            lightObj.GetComponent<ParticleSystem>().Clear();
            GetComponent<Collider>().isTrigger = true;
            anim.SetTrigger("Death");
        }
    }

    public void CooldownAttack()
    {
        lightObj.GetComponent<ParticleSystem>().Stop();
        attackCool = true;
    }
}
