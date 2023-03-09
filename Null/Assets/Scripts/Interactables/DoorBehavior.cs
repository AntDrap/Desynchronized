using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorBehavior : MonoBehaviour
{
    public GameObject doorOne, doorTwo;
    public bool open;
    float autoClose;
    Vector3 doorOneStart, doorOneEnd, doorTwoStart, doorTwoEnd;

    void Start()
    {
        doorOneStart = doorOne.transform.localPosition;
        doorTwoStart = doorTwo.transform.localPosition;

        doorOneEnd = doorOne.transform.localPosition + Vector3.right * 2;
        doorTwoEnd = doorTwo.transform.localPosition - Vector3.right * 2;
    }

    void Update()
    {
        if(open)
        {
            if (autoClose <= 0)
            {
                open = false;
            }
            else if (autoClose > 0 && Vector3.Distance(transform.position, FindObjectOfType<PlayerBehavior>().transform.position) > 3)
            {
                autoClose -= Time.deltaTime;
            }

            doorOne.transform.localPosition = Vector3.Lerp(doorOne.transform.localPosition, doorOneEnd, 3 * Time.deltaTime);
            doorTwo.transform.localPosition = Vector3.Lerp(doorTwo.transform.localPosition, doorTwoEnd, 3 * Time.deltaTime);
        }
        else
        {
            doorOne.transform.localPosition = Vector3.Lerp(doorOne.transform.localPosition, doorOneStart, 3 * Time.deltaTime);
            doorTwo.transform.localPosition = Vector3.Lerp(doorTwo.transform.localPosition, doorTwoStart, 3 * Time.deltaTime);
        }

        doorOne.GetComponent<NavMeshObstacle>().carving = !open;
        doorTwo.GetComponent<NavMeshObstacle>().carving = !open;
    }

    public void OpenDoor()
    {
        open = !open;

        if(open)
        {
            autoClose = 2f;
        }
    }
}
