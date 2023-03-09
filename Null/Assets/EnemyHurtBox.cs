using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtBox : MonoBehaviour
{
    public EnemyBehavior eb;

    public void damage(float amount)
    {
        eb.ChangeHealth(amount);
    }
}
