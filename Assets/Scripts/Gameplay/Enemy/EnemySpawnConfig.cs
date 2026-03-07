using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class EnemyWeightEntry
{
    public EnemyData Data;
    public int Weight;
}

[CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Enemy/EnemySpawnConfig")]
public class EnemySpawnConfig : ScriptableObject
{
    [Header("Spawn Settings")]
    public int MaxActiveEnemies = 3;
    public float SpawnInterval = 15f;

    [Header("Enemy Weights")]
    public List<EnemyWeightEntry> EnemyWeights;
}
