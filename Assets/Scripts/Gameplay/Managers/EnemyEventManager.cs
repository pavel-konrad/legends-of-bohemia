using UnityEngine;
using System.Collections.Generic;

public class EnemyEventManager : MonoBehaviour, ISubject<EnemyEvent>
{
	private List<IObserver<EnemyEvent>> _enemyObservers;

	private void Awake()
	{
		_enemyObservers = new List<IObserver<EnemyEvent>>();
	}
	public void RegisterObserver(IObserver<EnemyEvent> observer)
	{
		_enemyObservers.Add(observer);
	}

	public void UnregisterObserver(IObserver<EnemyEvent> observer)
	{
		_enemyObservers.Remove(observer);
	}
	public void NotifyObservers(EnemyEvent eventData)
	{
		foreach (var observer in _enemyObservers)
		{
			observer.OnNotify(eventData);
		}
	}
}