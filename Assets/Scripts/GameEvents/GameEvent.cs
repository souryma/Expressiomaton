using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> m_listeners =
        new List<GameEventListener>();

    public void Raise()
    {
        for (int i = m_listeners.Count -1; i >= 0; i--)
        {
            m_listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener p_listener)
    {
        if (m_listeners is not null)
        {
            m_listeners.Add(p_listener);
        }
     
    }

    public void UnregisterListener(GameEventListener p_listener)
    {
        if (m_listeners is not null)
        {
            m_listeners.Remove(p_listener);
        }
    }
}
