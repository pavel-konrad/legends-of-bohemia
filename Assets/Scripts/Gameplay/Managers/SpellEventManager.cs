using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Broadcasts SpellEvents to all registered observers.
/// Attach to the player prefab alongside SpellController.
/// SpellController calls NotifyObservers() — do not call it from other systems.
/// To subscribe: spellEventManager.RegisterObserver(this).
/// Entry point for registration is GameManager.HandlePlayerSpawned.
/// See ARCHITECTURE.md for full event reference and subscription example.
/// </summary>
public class SpellEventManager : MonoBehaviour, ISubject<SpellEvent>
{
	private List<IObserver<SpellEvent>> _spellObservers;

	private void Awake()
	{
		_spellObservers = new List<IObserver<SpellEvent>>();
	}
	public void RegisterObserver(IObserver<SpellEvent> observer)
	{
		_spellObservers.Add(observer);
	}

	public void UnregisterObserver(IObserver<SpellEvent> observer)
	{
		_spellObservers.Remove(observer);
	}
	public void NotifyObservers(SpellEvent eventData)
	{
		foreach (var observer in _spellObservers)
		{
			observer.OnNotify(eventData);
		}
	}
}