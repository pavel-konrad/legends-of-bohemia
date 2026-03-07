
/// <summary>
/// Generic observer interface for the observer pattern.
/// Implement this on any system that reacts to events (UI, audio, VFX, animator).
/// Register via the corresponding ISubject (e.g. SpellEventManager.RegisterObserver).
/// Always call UnregisterObserver in OnDestroy.
/// </summary>
/// <typeparam name="T">The event data type (e.g. PlayerEvent, SpellEvent).</typeparam>
public interface IObserver<T>
{
	/// <summary>Called by the subject when an event occurs.</summary>
	public void OnNotify(T eventData);
}