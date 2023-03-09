using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardBehavior : TriggerBehavior
{
    public int accessLevel;
    public Material[] materials;
    private void Start()
    {
        GetComponent<MeshRenderer>().material = materials[accessLevel - 1];
    }

    public override void Trigger()
    {
        FindObjectOfType<PopUpBehavior>().addWord("Access Level Increased To A" + accessLevel);
        FindObjectOfType<PlayerBehavior>().changeAccessLevel(accessLevel);
        gameObject.SetActive(false);
    }
}
