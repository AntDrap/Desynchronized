using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerBehavior : WeaponBehavior
{
    public float speed = 0.15f;
    public Light flameLight;
    public ParticleSystem ps;

    public override void Start()
    {
        flameLight.intensity = 0;
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

        if (cooldown)
        {
            return;
        }

        flameLight.intensity = 3;
        pb.ammoText.fontSize = 36;

        if(modTwo)
        {
            int chance = Random.Range(0, 6);

            if(chance < 5)
            {
                ammo--;
            }
        }
        else
        {
            ammo--;
        }


        pb.ammoText.text = ammo.ToString();

        cooldown = true;
        Invoke("endCooldown", speed);
    }

    private void Update()
    {
        flameLight.intensity = Mathf.Lerp(flameLight.intensity, 0, 3 * Time.deltaTime);

        if (flameLight.intensity > 2.5f)
        {
            ps.Play();
        }
    }

    public override void Equip()
    {
        base.Equip();
        totalAmmo = pb.equipment[3].GetComponent<DrillBehavior>().totalAmmo;
        pb.totalAmmoText.text = totalAmmo.ToString();
    }

    public override void Reload()
    {
        int tempAmmo = Mathf.Clamp(maxAmmo - ammo, 0, pb.equipment[3].GetComponent<DrillBehavior>().totalAmmo);

        if (tempAmmo > 0)
        {
            pb.ammoText.fontSize = 22;
            pb.totalAmmoText.fontSize = 22;
            pb.reloading = true;
            Invoke("endReload", reloadTime);
        }
    }

    public override void endReload()
    {
        int tempAmmo = Mathf.Clamp(maxAmmo - ammo, 0, totalAmmo);
        ammo += tempAmmo;
        pb.equipment[3].GetComponent<DrillBehavior>().totalAmmo -= tempAmmo;
        totalAmmo = pb.equipment[3].GetComponent<DrillBehavior>().totalAmmo;
        pb.ammoText.fontSize = 36;
        pb.totalAmmoText.fontSize = 36;
        pb.totalAmmoText.text = totalAmmo.ToString();
        pb.ammoText.text = ammo.ToString();
        pb.reloading = false;
        cooldown = false;
    }

    public override void AddAmmo(int amount)
    {
        pb = FindObjectOfType<PlayerBehavior>();

        totalAmmo = pb.equipment[3].GetComponent<DrillBehavior>().totalAmmo;

        if (!pb.currentEquip || !pb.currentEquip.Equals(gameObject))
        {
            return;
        }

        pb.totalAmmoText.text = totalAmmo.ToString();

        if (!pb.reloading)
        {
            pb.totalAmmoText.fontSize = 36;
        }

        if (ammo == 0)
        {
            Reload();
        }
    }

    public override void addMod(int modNum)
    {
        if(modNum == 1)
        {
            maxAmmo = 50;
            modOne = true;
        }
        else
        {
            modTwo = true;
        }
    }
}