using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellSlotView : MonoBehaviour
{
    private void Awake()
    {
        Hide();
    }

    [SerializeField] private Image _icon;
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _timerText;

    public void Show(SpellData data)
    {
        _icon.sprite = data.Icon;
        _background.color = data.Color;
        gameObject.SetActive(true);
    }
    public void UpdateTimer(float seconds, Color color)
    {
        _timerText.text = seconds.ToString("F1");
        _timerText.color = color;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}