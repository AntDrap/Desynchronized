using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableBehvaior : TriggerBehavior
{
    public Sprite shopIcon;
    public int price;
    public string itemName;
    [TextArea]
    public string itemDescription;
    PlayerBehavior pb;
    public string modNum;
    public virtual void Start()
    {
        pb = FindObjectOfType<PlayerBehavior>();
    }

    public override void Trigger()
    {
        pb = FindObjectOfType<PlayerBehavior>();
        onTrigger.Invoke();
    }

    public void AddAmmo(int index)
    {
        WeaponBehavior wb = pb.equipment[index].GetComponent<WeaponBehavior>();
        int tempAmmo = 0;

        switch(index)
        {
            case 1:
                tempAmmo = 10;
                break;
            case 2:
                tempAmmo = 4;
                break;
            case 3:
                tempAmmo = 50;
                break;
        }

        wb.AddAmmo(tempAmmo);
        pb.equipment[4].GetComponent<WeaponBehavior>().AddAmmo(0);
        gameObject.SetActive(false);
    }

    public void HealPlayer()
    {
        if(pb.health >= pb.maxHealth)
        {
            FindObjectOfType<PopUpBehavior>().addWord("Health Full");
            return;
        }

        FindObjectOfType<PopUpBehavior>().addWord("Health Restored");
        pb.ChangeHealth(pb.maxHealth * 0.25f);
        gameObject.SetActive(false);
    }

    public void GainMoney(int count)
    {
        pb.ChangeMoney(count);
        gameObject.SetActive(false);
    }

    public void GainScone()
    {
        FindObjectOfType<PopUpBehavior>().addWord("Time Scone Aquired");
        pb.ChangeScone();
        gameObject.SetActive(false);
    }

    public void GainEquipment(int equipmentIndex)
    {
        FindObjectOfType<PlayerBehavior>().UnlockEquipment(equipmentIndex);

        foreach(ConsumableBehvaior g in FindObjectsOfType<ConsumableBehvaior>())
        {
            if(g != this && g.itemName == itemName)
            {
                g.gameObject.SetActive(false);
            }
        }

        FindObjectOfType<PopUpBehavior>().addWord(itemName + " Aquired");

        gameObject.SetActive(false);
    }

    public void WeaponUpgrade(int upgradeIndex)
    {
        switch(upgradeIndex)
        {
            case 10:
                // Heavy Flashlight
                pb.equipment[0].GetComponent<WeaponBehavior>().addMod(1);
                break;
            case 11:
                // Longer Handle
                pb.equipment[0].GetComponent<WeaponBehavior>().addMod(2);
                break;


            case 20:
                // Pistol Magazine Extension
                pb.equipment[1].GetComponent<WeaponBehavior>().addMod(1);
                break;
            case 21:
                // Rifle Barrel
                pb.equipment[1].GetComponent<WeaponBehavior>().addMod(2);
                break;


            case 30:
                // Shotgun Choke
                pb.equipment[2].GetComponent<WeaponBehavior>().addMod(1);
                break;
            case 31:
                // Packed Shells
                pb.equipment[2].GetComponent<WeaponBehavior>().addMod(2);
                break;


            case 40:
                // Diamond Tip
                pb.equipment[3].GetComponent<WeaponBehavior>().addMod(1);
                break;
            case 41:
                // High Power Motor
                pb.equipment[3].GetComponent<WeaponBehavior>().addMod(2);
                break;


            case 50:
                pb.equipment[4].GetComponent<WeaponBehavior>().addMod(1);
                break;
            case 51:
                pb.equipment[4].GetComponent<WeaponBehavior>().addMod(2);
                break;
        }
        FindObjectOfType<PopUpBehavior>().addWord(itemName + " Aquired");
        gameObject.SetActive(false);
    }

    public void giveName()
    {
        string[] temp = name.Split('-');

        string temp2 = "";

        for (int i = 0; i < temp[0].Length; i++)
        {
            if (temp[0][i] != ' ' && temp[0][i] != ':' && temp[0][i] != '[' && temp[0][i] != ']')
            {
                temp2 += temp[0][i];
            }
        }

        name = temp2.Split('(')[0] + "-" + UnityEngine.Random.Range(0, 1000000);
    }
}
