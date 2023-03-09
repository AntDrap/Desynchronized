using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBehavior : MonoBehaviour
{
    protected PlayerBehavior pb;
    public int ammo, totalAmmo, maxAmmo, layerMask;
    public float reloadTime, damage;
    public GameObject hitMark;
    public bool cooldown, toggleAlt;
    public bool modOne, modTwo, attacking;

    public virtual void Start()
    {
        layerMask = LayerMask.GetMask(new string[] { "Default", "Target", "Interact", "Floor", "Enemy" });
        pb = FindObjectOfType<PlayerBehavior>();
    }

    public virtual void Equip()
    {
        pb = FindObjectOfType<PlayerBehavior>();
        pb.ammoText.fontSize = 36;
        pb.totalAmmoText.fontSize = 36;
    }

    public abstract void Attack();

    public virtual void AltAttack()
    {
        return;
    }

    public virtual void EndAltAttack()
    {
        return;
    }

    public virtual void Reload()
    {
        int tempAmmo = Mathf.Clamp(maxAmmo - ammo, 0, totalAmmo);

        if (tempAmmo > 0)
        {
            pb.ammoText.fontSize = 22;
            pb.totalAmmoText.fontSize = 22;
            pb.reloading = true;
            Invoke("endReload", reloadTime);
        }
    }

    public void HitThing(RaycastHit hit)
    {
        if (hit.collider.GetComponent<TriggerBehavior>())
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if(hit.collider.GetComponent<EnemyBehavior>())
                {
                    hit.collider.GetComponent<EnemyBehavior>().ChangeHealth(damage);
                }
                else if(hit.collider.transform.parent.GetComponent<EnemyBehavior>())
                {
                    hit.collider.transform.parent.GetComponent<EnemyBehavior>().ChangeHealth(damage);
                }
                else if(hit.collider.GetComponent<EnemyHurtBox>())
                {
                    hit.collider.GetComponent<EnemyHurtBox>().damage(damage);
                }

                if(hitMark)
                {
                    GameObject temp = Instantiate(hitMark, hit.point + (hit.normal * 0.05f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                    temp.transform.parent = hit.transform;
                }

            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                hit.collider.GetComponent<TriggerBehavior>().Trigger();
            }
        }
        else
        {
            if (hitMark)
            {
                GameObject temp = Instantiate(hitMark, hit.point + (hit.normal * 0.05f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                temp.transform.parent = hit.transform;
            }
        }
    }

    public virtual void endReload()
    {
        int tempAmmo = Mathf.Clamp(maxAmmo - ammo, 0, totalAmmo);
        ammo += tempAmmo;
        totalAmmo -= tempAmmo;

        pb.ammoText.fontSize = 36;
        pb.totalAmmoText.fontSize = 36;
        pb.totalAmmoText.text = totalAmmo.ToString();
        pb.ammoText.text = ammo.ToString();
        pb.reloading = false;
        cooldown = false;
    }

    public void endCooldown()
    {
        cooldown = false;
    }

    public virtual void AddAmmo(int amount)
    {
        totalAmmo += amount;
        pb = FindObjectOfType<PlayerBehavior>();
        if (pb.currentEquip && pb.currentEquip.Equals(gameObject))
        {
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
    }

    public abstract void addMod(int modNum);
}
