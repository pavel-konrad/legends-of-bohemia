using UnityEngine;

/// <summary>
/// Central UI click audio. Attach once to the scene.
/// Wire buttons OnClick() directly to PlayClick / PlayConfirm / PlayCancel.
/// </summary>
public class UIClickAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _click;
    [SerializeField] private AudioClip _confirm;
    [SerializeField] private AudioClip _cancel;

    public void PlayClick()   => _audioSource.PlayOneShot(_click);
    public void PlayConfirm() => _audioSource.PlayOneShot(_confirm);
    public void PlayCancel()  => _audioSource.PlayOneShot(_cancel);
}
