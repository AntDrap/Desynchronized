using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    PlayerBehavior pb;
    public Light lightObj;
    public static bool powerOn = false;
    public bool turnedOn;

    private void Awake()
    {
        powerOn = false;
        pb = FindObjectOfType<PlayerBehavior>();
        ToggleLight(turnedOn);
    }

    public void ToggleLight()
    {
        ToggleLight(!turnedOn);
    }

    public void ToggleLight(bool toggle)
    {
        lightObj.gameObject.SetActive(toggle);

        turnedOn = toggle;
    }

    public void ToggleAllLights(bool toggle)
    {
        List<GameObject> allObjects = FindObjectOfType<SavingBehavior>().actuallyFindAllObjects();

        foreach(GameObject g in allObjects)
        {
            if(g.GetComponent<VendingMachineBehavior>())
            {
                g.GetComponent<VendingMachineBehavior>().idleDisplay.text = "Swipe Employee ID to continue";
            }

            if (g.GetComponent<ButtonBehavior>())
            {
                g.GetComponent<ButtonBehavior>().Unlock();
            }

            if (g.GetComponent<LightBehavior>())
            {
                g.GetComponent<LightBehavior>().ToggleLight(true);
            }
        }

        powerOn = true;
    }
}