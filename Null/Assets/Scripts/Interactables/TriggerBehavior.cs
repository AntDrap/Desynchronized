using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;
using UnityEngine;

public class TriggerBehavior : MonoBehaviour
{
    [Serializable]
    public class TriggerEvent : UnityEvent { }

    [FormerlySerializedAs("onTrigger")]
    [SerializeField]
    protected TriggerEvent onTrigger = new TriggerEvent();

    public virtual void Trigger()
    {
        onTrigger.Invoke();
    }
}