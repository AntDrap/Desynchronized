using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleBehavior : MonoBehaviour
{
    public ParticleSystem[] particles;
    MeshRenderer mr;
    bool fading, played;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        Invoke("StartDecay", 6);
    }

    void Update()
    {
        if(!played && transform.parent)
        {
            if(transform.parent.gameObject.tag == "Metal" || transform.parent.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
            {
                particles[1].Play();
            }
            else
            {
                particles[0].Play();
            }

            played = true;
        }

        if(fading)
        {
            mr.material.color = Vector4.Lerp(mr.material.color, new Vector4(1,1,1,0), Time.deltaTime);

            if(mr.material.color.a <= 0.001)
            {
                Destroy(gameObject);
            }
        }
    }

    void StartDecay()
    {
        fading = true;
    }
}