using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]                                           
  public class EnemyData : ScriptableObject                                                                         
  {                                                                                                               
      [Header("Identity")]                                                                                          
      public string Name;                                                                                           

      [Header("Stats")]
      public float MaxHealth;
      public float AttackPower;
      public float MoveSpeed;
      public float MoveCooldown;
      public float AttackCooldown;
      public int DetectionRange;
      public int AttackRange;
      public bool IsRanged;

      [Header("Audio")]
      public AudioClip[] FootstepSounds;
      public AudioClip[] AttackSounds;
      public AudioClip DeathSound;
      public AudioClip AmbientSound;

      [Header("Prefab")]
      public GameObject EnemyPrefab;
  }
