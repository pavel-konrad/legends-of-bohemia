using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Broadcasts CharacterSelectEvents to all registered observers.
/// Attach to the CharacterSelect scene root object.
/// </summary>
public class CharacterSelectEventManager : MonoBehaviour, ISubject<CharacterSelectEvent>
{
    private List<IObserver<CharacterSelectEvent>> _observers;

    private void Awake()
    {
        _observers = new List<IObserver<CharacterSelectEvent>>();
    }

    public void RegisterObserver(IObserver<CharacterSelectEvent> observer)
    {
        _observers.Add(observer);
    }

    public void UnregisterObserver(IObserver<CharacterSelectEvent> observer)
    {
        _observers.Remove(observer);
    }

    public void NotifyObservers(CharacterSelectEvent eventData)
    {
        foreach (var observer in _observers)
            observer.OnNotify(eventData);
    }
}
