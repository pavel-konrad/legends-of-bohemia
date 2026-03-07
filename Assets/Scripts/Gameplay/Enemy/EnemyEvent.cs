public enum EnemyEventType
{
	Moving,
	Standing,
	Attacking,
	TookDamage,
	Died
}

public class EnemyEvent
{
	public EnemyEventType Event;
	public float Value;
	public float MaxValue;
}