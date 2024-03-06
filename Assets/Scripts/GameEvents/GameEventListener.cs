using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GameEventListener : MonoBehaviour
{
    public GameEvent @event;
    [FormerlySerializedAs("response")] public UnityEvent m_response;

    private void OnEnable()
    {
        @event.RegisterListener(this);
    }
    private void OnDisable()
    {
        @event.UnregisterListener(this);
    }
    public void OnEventRaised()
    {
        m_response.Invoke();
    }
}
