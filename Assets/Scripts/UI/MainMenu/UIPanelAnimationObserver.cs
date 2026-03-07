using UnityEngine;

/// <summary>
/// Handles all UI panel animations for the character select screen.
/// Attach to a single GameObject in the CharacterSelect scene.
/// Register animators for each panel and react to CharacterSelectEvents.
/// </summary>
public class UIAnimationObserver : MonoBehaviour, IObserver<CharacterSelectEvent>
{
    [SerializeField] private CharacterSelectEventManager _eventManager;

    [Header("Panel Animators")]
    [SerializeField] private Animator _previewPanelAnimator;
    [SerializeField] private Animator _classPanelAnimator;
    [SerializeField] private Animator _playerSlotAnimator;
    [SerializeField] private Animator _startButtonAnimator;

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
                _previewPanelAnimator.SetBool("IsOpen", true);
                _startButtonAnimator.SetBool("IsOpen", false);
                _playerSlotAnimator.SetTrigger("Select");
                break;
            case CharacterSelectEventType.RaceSelected:
                _classPanelAnimator.SetBool("IsOpen", true);
                break;
            case CharacterSelectEventType.ClassSelected:
                _playerSlotAnimator.SetTrigger("ShowPreview");
                break;
            case CharacterSelectEventType.PanelClosed:
                _classPanelAnimator.SetBool("IsOpen", false);
                break;
            case CharacterSelectEventType.PlayerConfirmed:
                _previewPanelAnimator.SetBool("IsOpen", false);
                _startButtonAnimator.SetBool("IsOpen", true);
                _playerSlotAnimator.SetBool("Confirmed", true);
                break;
            case CharacterSelectEventType.SelectionReset:
                _previewPanelAnimator.SetBool("IsOpen", true);
                _startButtonAnimator.SetBool("IsOpen", false);
                _playerSlotAnimator.SetBool("Confirmed", false);
                break;
            case CharacterSelectEventType.PlayerReset:
                _previewPanelAnimator.SetBool("IsOpen", false);
                _classPanelAnimator.SetBool("IsOpen", false);
                _startButtonAnimator.SetBool("IsOpen", false);
                _playerSlotAnimator.SetBool("Confirmed", false);
                _playerSlotAnimator.SetTrigger("Reset");
                break;
        }
    }
}
