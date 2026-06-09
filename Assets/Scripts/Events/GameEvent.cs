using System;

namespace WheelOfFortune.Events
{
    public class GameEvent
    {
        private event Action _onRaised;

        public void Subscribe(Action listener) => _onRaised += listener;
        public void Unsubscribe(Action listener) => _onRaised -= listener;
        public void Raise() => _onRaised?.Invoke();
    }

    public class GameEvent<T>
    {
        private event Action<T> _onRaised;

        public void Subscribe(Action<T> listener) => _onRaised += listener;
        public void Unsubscribe(Action<T> listener) => _onRaised -= listener;
        public void Raise(T arg) => _onRaised?.Invoke(arg);
    }
}
