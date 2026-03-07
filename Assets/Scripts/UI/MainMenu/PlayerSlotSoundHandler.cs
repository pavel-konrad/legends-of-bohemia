using UnityEngine;

/// <summary>
/// Attach to Player1Slot.
/// Called from CharacterSelectPresenter on class selection.
/// If a ShowPreview animation clip exists, PlaySelectSound() can also be called via Animation Event.
/// </summary>
public class PlayerSlotSoundHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void PlaySelectSound(AudioClip clip)
    {
        if (clip != null)
            _audioSource.PlayOneShot(clip);
    }
}
