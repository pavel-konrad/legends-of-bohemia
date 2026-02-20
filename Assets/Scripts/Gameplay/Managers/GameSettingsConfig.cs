using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "Config/GameSettingsConfig")]
public class GameSettingsConfig : ScriptableObject
{
    [Header("Spawn Settings")]
    public int MaxActiveSpells = 5;
    public float SpawnInterval = 10f;
    public float NegativeSpellChance = 0.3f;

    [Header("Spell Weights")]
    public List<SpellWeightEntry> SpellWeights;
}

[System.Serializable]
public class SpellWeightEntry
{
    public SpellType Type;
    public int Weight;
}
