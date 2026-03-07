using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
	[Header("Audio Sources")]
	[SerializeField] private AudioSource _footstepAudio;
	[SerializeField] private AudioSource _combatAudio;

	private PlayerData _data;

	public void Initialize(PlayerData data)
	{
		_data = data;
	}

	public void OnFootstep()
	{
		if (_data.FootstepSounds.Length == 0) return;
		_footstepAudio.PlayOneShot(_data.FootstepSounds[Random.Range(0, _data.FootstepSounds.Length)]);
	}

	public void OnAttack()
	{
		if (_data.AttackSound.Length == 0) return;
		_combatAudio.PlayOneShot(_data.AttackSound[Random.Range(0, _data.AttackSound.Length)]);
	}
}
