using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour, IObserver<PlayerEvent>
{
	private Animator _animator;
	private PlayerEventManager _eventManager;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_eventManager = GetComponent<PlayerEventManager>();
		_eventManager.RegisterObserver(this);
	}

	public void OnNotify(PlayerEvent eventData)
	{
		switch (eventData.Event)
		{
			case PlayerEventType.Standing:
				_animator.SetFloat("Speed", 0f);
				break;
			case PlayerEventType.Walking:
				_animator.SetFloat("Speed", 1f);
				break;
			case PlayerEventType.Running:
				_animator.SetFloat("Speed", 2f);
				break;
			case PlayerEventType.Attacking:
				_animator.SetTrigger("Attack");
				break;
		}
	}
}