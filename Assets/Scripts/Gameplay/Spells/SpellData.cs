using UnityEngine;

[CreateAssetMenu(fileName = "SpellsConfig", menuName = "Spells/SpellsConfig")]
public class SpellData : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public Color Color;
    public float Duration;
    public float ChargeTime;
    public SpellType Type;
    public float EffectValue;
}