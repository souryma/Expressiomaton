namespace GameEvents
{
    public interface ITypedGameEventListener<T>
    {
        void OnEventRaised(T item);
    }

}