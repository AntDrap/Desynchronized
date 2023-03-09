using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillBehavior : WeaponBehavior
{
    public GameObject drillBit;
    public float speed = 0.15f;
    public float spinSpeedMod;
    float spinSpeed;

    public override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        if (ammo <= 0)
        {
            if (totalAmmo <= 0)
            {
                pb.ammoText.fontSize = 36;
                pb.totalAmmoText.fontSize = 36;
            }
            else
            {
                Reload();
            }

            return;
        }

        spinSpeed = 200 * spinSpeedMod;

        if (cooldown)
        {
            return;
        }

        pb.ammoText.fontSize = 36;
        ammo--;
        pb.ammoText.text = ammo.ToString();

        gameObject.transform.localPosition = transform.localPosition - new Vector3(0,0,0.05f);

        RaycastHit hit;
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit, 3, layerMask))
        {
            HitThing(hit);
        }

        cooldown = true;
        Invoke("endCooldown", speed);
    }

    private void Update()
    {
        spinSpeed = Mathf.Lerp(spinSpeed, 30 * spinSpeedMod, 3 * Time.deltaTime);
        drillBit.transform.Rotate(new Vector3(0, spinSpeed, 0) * Time.deltaTime);
    }

    public override void addMod(int modNum)
    {
        if (modNum == 1)
        {
            damage += 0.25f;
            modOne = true;
        }
        else
        {
            speed = 0.1f;
            spinSpeedMod = 1.5f;
            modTwo = true;
        }
    }
}