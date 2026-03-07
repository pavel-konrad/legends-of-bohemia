
/// <summary>
/// Generic subject interface for the observer pattern.
/// Implement this on manager classes that broadcast events (e.g. PlayerEventManager, SpellEventManager).
/// </summary>
/// <typeparam name="T">The event data type (e.g. PlayerEvent, SpellEvent).</typeparam>
public interface ISubject<T>
{
	public void RegisterObserver(IObserver<T> observer);
	public void UnregisterObserver(IObserver<T> observer);
	public void NotifyObservers(T eventData);
}