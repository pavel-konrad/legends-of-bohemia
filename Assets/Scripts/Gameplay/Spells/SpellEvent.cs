using System.Collections.Generic;

/// <summary>
/// Types of spell events broadcast by SpellEventManager.
/// See ARCHITECTURE.md – SpellEvent Reference for full table.
/// </summary>
public enum SpellEventType
{
	/// <summary>Player picked up a spell. Spell field is populated.</summary>
	Collected,
	/// <summary>Spell queue changed (add or remove). Queue field is populated.</summary>
	QueueChanged,
	/// <summary>Charging in progress every frame. Value is 0→1 normalized.</summary>
	ChargeTick,
	/// <summary>Spell was cast on target. Spell field is populated.</summary>
	Cast,
	/// <summary>Active effect in progress every frame. Value is 1→0 normalized.</summary>
	DurationTick
}

/// <summary>
/// Event data passed to all IObserver&lt;SpellEvent&gt; observers.
/// Not all fields are populated for every event type — see SpellEventType docs.
/// </summary>
public class SpellEvent
{
	public SpellEventType Event;
	/// <summary>The spell involved. Populated for Collected and Cast.</summary>
	public ISpell Spell;
	/// <summary>Normalized progress value. Populated for ChargeTick (0→1) and DurationTick (1→0).</summary>
	public float Value;
	/// <summary>Current spell queue snapshot. Populated for QueueChanged.</summary>
	public Queue<ISpell> Queue;
}