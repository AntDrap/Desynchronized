using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightBehavior : WeaponBehavior
{
    public float range = 2.5f;

    public override void Start()
    {
        base.Start();
    }


    public override void Attack()
    {
        if (pb.reloading || pb.stamina < 0.5f)
        {
            return;
        }

        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, -75, 0));

        RaycastHit hit;

        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit, range, layerMask))
        {
            HitThing(hit);
        }

        pb.ChangeStamina(-0.5f);
        pb.reloading = false;
        cooldown = true;
        Invoke("endReload", reloadTime);
    }

    public override void AltAttack()
    {
        transform.GetChild(3).gameObject.SetActive(!transform.GetChild(3).gameObject.activeSelf);
    }

    public override void Reload()
    {
        return;
    }

    public override void addMod(int modNum)
    {
        if (modNum == 1)
        {
            damage += 0.5f;
            modOne = true;
        }
        else
        {
            range = 4;
            modTwo = true;
        }
    }
}