using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "Config/GameSettingsConfig")]
public class GameSettingsConfig : ScriptableObject
{
    [Header("Spawn Settings")]
    public int MaxActiveSpells = 5;
    public float SpawnInterval = 10f;

    [Header("Player Stats Ranges")]
    public float MaxMoveSpeed = 5f;
    public float MaxAttackPower = 10f;
    public float MaxHealth = 20f;
    public float MaxEnergy = 20f;

    [Header("Spell Weights")]
    public List<SpellWeightEntry> SpellWeights;
}

[System.Serializable]
public class SpellWeightEntry
{
    public SpellType Type;
    public int Weight;
}
