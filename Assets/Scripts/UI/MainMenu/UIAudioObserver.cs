using UnityEngine;

/// <summary>
/// Handles UI audio for the character select screen.
/// Race/class sounds are data-driven via RaceData and PlayerData.
/// Panel whoosh sounds are handled separately by PanelSoundHandler via Animation Events.
/// </summary>
public class UIAudioObserver : MonoBehaviour, IObserver<CharacterSelectEvent>
{
    [SerializeField] private CharacterSelectEventManager _eventManager;
    [SerializeField] private PlayerDataRegistry _registry;
    [SerializeField] private AudioSource _audioSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip _click;
    [SerializeField] private AudioClip _confirm;
    [SerializeField] private AudioClip _cancel;

    private void Start()
    {
        _eventManager.RegisterObserver(this);
    }

    private void OnDestroy()
    {
        _eventManager?.UnregisterObserver(this);
    }

    public void OnNotify(CharacterSelectEvent eventData)
    {
        switch (eventData.Event)
        {
            case CharacterSelectEventType.PlayerSlotClicked:
                Play(_click);
                break;
            case CharacterSelectEventType.RaceSelected:
                Play(_registry.GetRaceData(eventData.Race)?.SelectSound ?? _click);
                break;
            case CharacterSelectEventType.ClassSelected:
                Play(_click);
                break;
            case CharacterSelectEventType.PlayerConfirmed:
                Play(_confirm);
                break;
            case CharacterSelectEventType.SelectionReset:
                Play(_click);
                break;
            case CharacterSelectEventType.PanelClosed:
                Play(_click);
                break;
            case CharacterSelectEventType.PlayerReset:
                Play(_cancel);
                break;
        }
    }

    private void Play(AudioClip clip)
    {
        if (clip != null)
            _audioSource.PlayOneShot(clip);
    }
}
