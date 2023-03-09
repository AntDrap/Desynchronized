using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonBehavior : TriggerBehavior
{
    public int accessLevel;
    public TextMeshProUGUI display;
    public Material[] materials;
    public MeshRenderer buttonRender;
    public bool electronicLock;
    public GameObject lightObj;

    private void Start()
    {
        if(!electronicLock)
        {
            buttonRender.material = materials[accessLevel];
            display.text = "A" + accessLevel;
        }
        else
        {
            buttonRender.material = materials[materials.Length - 1];
            display.text = "";
        }

        if(lightObj)
        {
            lightObj.SetActive(!electronicLock);
        }
    }

    public override void Trigger()
    {
        if(electronicLock)
        {
            return;
        }

        if(FindObjectOfType<PlayerBehavior>().accessLevel >= accessLevel)
        {
            onTrigger.Invoke();
        }
        else
        {
            // play error noise
        }
    }

    public void Unlock()
    {
        electronicLock = false;
        buttonRender.material = materials[accessLevel];
        display.text = "A" + accessLevel;
        if (lightObj)
        {
            lightObj.SetActive(true);
        }
    }
}
