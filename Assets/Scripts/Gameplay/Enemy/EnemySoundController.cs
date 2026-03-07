using UnityEngine;

public class EnemySoundController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _footstepAudio;
    [SerializeField] private AudioSource _combatAudio;

    private EnemyData _data;

    public void Initialize(EnemyData data)
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
        if (_data.AttackSounds.Length == 0) return;
        _combatAudio.PlayOneShot(_data.AttackSounds[Random.Range(0, _data.AttackSounds.Length)]);
    }
}
