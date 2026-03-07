using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Broadcasts PlayerEvents to all registered observers.
/// Attach to the player prefab alongside PlayerController.
/// PlayerController calls NotifyObservers() directly — do not call it from other systems.
/// To subscribe: playerController.RegisterObserver(this) or playerEventManager.RegisterObserver(this).
/// </summary>
public class PlayerEventManager : MonoBehaviour, ISubject<PlayerEvent>
{
	private List<IObserver<PlayerEvent>> _playerObservers;

	private void Awake()
	{
		_playerObservers = new List<IObserver<PlayerEvent>>();
	}
	public void RegisterObserver(IObserver<PlayerEvent> observer)
	{
		_playerObservers.Add(observer);
	}

	public void UnregisterObserver(IObserver<PlayerEvent> observer)
	{
		_playerObservers.Remove(observer);
	}
	public void NotifyObservers(PlayerEvent eventData)
	{
		foreach (var observer in _playerObservers)
		{
			observer.OnNotify(eventData);
		}
	}
}