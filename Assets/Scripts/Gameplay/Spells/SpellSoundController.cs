using UnityEngine;

public class SpellSoundController : MonoBehaviour, IObserver<SpellEvent>
{
	[SerializeField] private AudioSource _hitsAudio;
	[SerializeField] private AudioSource _loopAudio;
	private SpellEventManager _spellManager;

	public void SetSpellEventManager(SpellEventManager manager)
	{
		_spellManager = manager;
		_spellManager.RegisterObserver(this);
	}

	private void OnDestroy()
	{
		if (_spellManager != null)
			_spellManager.UnregisterObserver(this);
	}

	public void OnNotify(SpellEvent eventData)
	{
		switch (eventData.Event)
		{
			case SpellEventType.Collected:
				if (eventData.Spell.Data.CollectSound != null)
					_hitsAudio.PlayOneShot(eventData.Spell.Data.CollectSound);
				break;
			case SpellEventType.QueueChanged:
				if (eventData.Queue != null && eventData.Queue.Count > 0)
				{
					ISpell next = eventData.Queue.Peek();
					if (next.Data.CastSound != null)
					{
						_loopAudio.clip = next.Data.CastSound;
						_loopAudio.loop = true;
						_loopAudio.Play();
					}
				}
				else
				{
					_loopAudio.Stop();
				}
				break;
			case SpellEventType.Cast:
				_loopAudio.Stop();
				if (eventData.Spell.Data.EffectSound != null)
				{
					_loopAudio.clip = eventData.Spell.Data.EffectSound;
					_loopAudio.loop = true;
					_loopAudio.Play();
				}
				break;
		}
	}
}