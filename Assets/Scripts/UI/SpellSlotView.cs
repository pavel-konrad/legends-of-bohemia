using UnityEngine;
using UnityEngine.UI;

public class SpellSlotView : MonoBehaviour
{
    private void Awake()
    {
        Hide();
    }

    [SerializeField] private Image _icon;
    [SerializeField] private Image _background;

    public void Show(SpellData data)
    {
        _icon.sprite = data.Icon;
        _background.color = data.Color;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}