using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointBehavior : TriggerBehavior
{
    public GameObject cube;
    Vector3 start;
    GameObject player;
    ParticleSystem ps;
    float scale = 0.5f;

    private void Start()
    {
        player = FindObjectOfType<PlayerBehavior>().gameObject;
        start = transform.position;
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if(ps.isPlaying)
        {
            if(Vector3.Distance(player.transform.position, transform.position) > 20)
            {
                ps.Stop();
            }

            cube.transform.Rotate(Vector3.up * 45 * Time.deltaTime);
            cube.transform.Rotate(Vector3.forward * 45 * Time.deltaTime);
            cube.transform.Rotate(Vector3.right * 45 * Time.deltaTime);
            scale = Mathf.PingPong(Time.time * 0.5f, 1f);
            cube.transform.localScale = Vector3.one * scale;
            transform.position = start + new Vector3(0, Mathf.PingPong(Time.time * 0.125f, 0.25f) - 0.125f, 0);
        }
        else
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 20)
            {
                ps.Play();
                scale = 0.5f;
            }
        }
    }

    public override void Trigger()
    {
        FindObjectOfType<SavingBehavior>().saveGame(gameObject);
    }
}
