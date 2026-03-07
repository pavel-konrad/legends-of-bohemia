using UnityEngine;

/// <summary>
/// Attach to each animated panel (PreviewPanel, ClassPanel, StartButton).
/// Call PlayWhooshIn / PlayWhooshOut from Animation Events on SlideIn / SlideOut clips.
/// </summary>
public class PanelSoundHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _whooshIn;
    [SerializeField] private AudioClip _whooshOut;

    public void PlayWhooshIn()  => _audioSource.PlayOneShot(_whooshIn);
    public void PlayWhooshOut() => _audioSource.PlayOneShot(_whooshOut);
}
