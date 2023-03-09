using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendingMachineBehavior : MonoBehaviour
{
    public GameObject[] inventory;
    public GameObject[] display;
    public Transform spawnPoint;
    public GameObject shopDisplay;
    public TextMeshProUGUI idleDisplay;
    bool open;
    float time;
    PlayerBehavior pb;

    private void Start()
    {
        pb = FindObjectOfType<PlayerBehavior>();

        updateScreen();

        ToggleVendingMachine(false);
    }

    private void Update()
    {
        if(open)
        {
            time -= Time.deltaTime;

            if(time <= 0)
            {
                ToggleVendingMachine(false);
            }
        }
    }

    public void ToggleVendingMachine(bool toggle)
    {
        if(!toggle || LightBehavior.powerOn)
        {
            shopDisplay.SetActive(toggle);
            open = toggle;

            if (toggle)
            {
                updateScreen();
                time = 10f;
            }
        }
    }

    public void BuyItem(int item)
    {
        time = 10f;
        ConsumableBehvaior cb = inventory[item].GetComponent<ConsumableBehvaior>();
        if (pb.moneyCount >= cb.price)
        {
            pb.moneyCount -= cb.price;
            GameObject g = Instantiate(inventory[item], spawnPoint.position, Random.rotation);

            if (cb.modNum != "")
            {
                inventory[item] = null;
            }

            g.GetComponent<ConsumableBehvaior>().giveName();

            updateScreen();
        }
    }

    public void updateScreen()
    {
        for (int i = 0; i < display.Length; i++)
        {
            if (i < inventory.Length && inventory[i])
            {
                ConsumableBehvaior cb = inventory[i].GetComponent<ConsumableBehvaior>();

                if(cb.modNum != "")
                {
                    WeaponBehavior wb = pb.equipment[int.Parse(cb.modNum[0].ToString()) - 1].GetComponent<WeaponBehavior>();

                    if(wb.modOne && cb.modNum[1] == '0')
                    {
                        display[i].SetActive(false);
                        return;
                    }

                    if (wb.modTwo && cb.modNum[1] == '1')
                    {
                        display[i].SetActive(false);
                        return;
                    }
                }

                display[i].SetActive(true);
                display[i].transform.GetChild(0).GetComponent<Image>().sprite = cb.shopIcon;
                display[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cb.itemName;
                display[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = cb.itemDescription;
                display[i].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ξ " + cb.price.ToString();
            }
            else
            {
                display[i].SetActive(false);
            }
        }
    }
}