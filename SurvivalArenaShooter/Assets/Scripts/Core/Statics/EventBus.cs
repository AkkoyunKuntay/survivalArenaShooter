using System;
using System.Collections.Generic;

public enum EventType { OnEnemySpawned, OnEnemyDespawned }

public static class EventBus<T>
{
    private static readonly Dictionary<EventType, Action<T>> _events = new();

    public static void Subscribe(EventType type, Action<T> callback)
    {
        if (!_events.ContainsKey(type))
            _events[type] = delegate { };
        _events[type] += callback;
    }

    public static void Unsubscribe(EventType type, Action<T> callback)
    {
        if (_events.ContainsKey(type))
            _events[type] -= callback;
    }

    public static void Invoke(EventType type, T arg)
    {
        if (_events.ContainsKey(type))
            _events[type].Invoke(arg);
    }
}