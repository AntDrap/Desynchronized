using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadBehavior : MonoBehaviour
{
    public GameObject levelHolder;

    private void Start()
    {
        Invoke("lateInactive", 0.25f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!levelHolder.activeSelf&& other.tag == "Player")
        {
            levelHolder.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (levelHolder.activeSelf && other.tag == "Player")
        {
            levelHolder.SetActive(false);
        }
    }

    public void lateInactive()
    {
        levelHolder.SetActive(false);
    }
}
