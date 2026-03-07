/// <summary>
/// Types of player events broadcast by PlayerEventManager.
/// Observers receive these via IObserver&lt;PlayerEvent&gt;.OnNotify().
/// </summary>
public enum PlayerEventType
{
	Walking,
	Running,
	Standing,
	Attacking,
	Collecting,
	HealthChanged,
	EnergyChanged
}

/// <summary>
/// Event data passed to all IObserver&lt;PlayerEvent&gt; observers.
/// See ARCHITECTURE.md – PlayerEvent Reference for full table.
/// </summary>
public class PlayerEvent
{
	public PlayerEventType Event;
	/// <summary>Current value (e.g. CurrentHealth, CurrentSpeed).</summary>
	public float Value;
	/// <summary>Maximum value (e.g. MaxHealth, MoveSpeed). Used to compute normalized ratio.</summary>
	public float MaxValue;
}
