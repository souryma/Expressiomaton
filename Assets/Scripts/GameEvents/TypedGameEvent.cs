
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


    public abstract class TypedGameEvent<T> : ScriptableObject
    {
        private readonly List<ITypedGameEventListener<T>> m_eventListeners = new();

        public void RaiseEvent(T item)
        {
            for (int i = m_eventListeners.Count - 1; i >= 0; i--)
            {
                m_eventListeners[i].OnEventRaised(item);
            }
        }

        public void RegisterListener(ITypedGameEventListener<T> p_listener)
        {
            if (!m_eventListeners.Contains(p_listener))
            {
                m_eventListeners.Add(p_listener);
            }
        }

        public void UnregisterListener(ITypedGameEventListener<T> p_listener)
        {
            if (m_eventListeners.Contains(p_listener))
            {
                m_eventListeners.Remove(p_listener);
            }
        }
    }


