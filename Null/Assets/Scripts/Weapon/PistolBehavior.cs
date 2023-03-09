using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBehavior : WeaponBehavior
{
    public GameObject muzzleFlash;
    public float spread, defaultSpread;

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

        pb.ammoText.fontSize = 36;
        ammo--;
        pb.ammoText.text = ammo.ToString();

        muzzleFlash.SetActive(true);
        muzzleFlash.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        Invoke("DisableMuzzleFlash", 0.1f);
        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(-30, 0, 0));

        RaycastHit hit;
        float tempSpread = spread + (0.025f * pb.bobMod);
        Vector3 direction = Camera.main.transform.forward + new Vector3(Random.Range(-tempSpread, tempSpread), Random.Range(-tempSpread, tempSpread), Random.Range(-tempSpread, tempSpread));
        if (Physics.Raycast(new Ray(Camera.main.transform.position, direction), out hit, 100, layerMask))
        {
            HitThing(hit);
        }

        cooldown = true;
        Invoke("endCooldown", 0.1f);
    }

    public override void AltAttack()
    {
        pb.playerCamera.fieldOfView = Mathf.Lerp(pb.playerCamera.fieldOfView, 50, Time.deltaTime * 6);
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, -0.25f, 0.575f), Time.deltaTime * 8);
        spread = defaultSpread * 0.5f;
    }

    public override void EndAltAttack()
    {
        spread = defaultSpread;
    }

    public void DisableMuzzleFlash()
    {
        muzzleFlash.SetActive(false);
    }

    public override void addMod(int modNum)
    {
        if (modNum == 1)
        {
            maxAmmo += 5;
            modOne = true;
        }
        else
        {
            damage += 0.5f;
            modTwo = true;
        }
    }
}