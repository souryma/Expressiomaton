using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameEvents
{
    public abstract class TypedGameEventListener<T, E, UER> : MonoBehaviour, ITypedGameEventListener<T>
        where E : TypedGameEvent<T> where UER : UnityEvent<T>
    {
        [FormerlySerializedAs("gameEvent")] [SerializeField] private E m_gameEvent;

        

        [SerializeField] private UER unityEventResponse;

        private void OnEnable()
        {
            if (m_gameEvent == null)
            {
                return;
            }

            m_gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (m_gameEvent == null)
            {
                return;
            }

            m_gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised(T item)
        {
            if (unityEventResponse != null)
            {
                unityEventResponse.Invoke(item);
            }
        }
    }
}