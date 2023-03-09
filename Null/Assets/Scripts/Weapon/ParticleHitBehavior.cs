using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHitBehavior : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<EnemyBehavior>())
        {
            other.GetComponent<EnemyBehavior>().ChangeHealth(0.05f);
        }
    }
}