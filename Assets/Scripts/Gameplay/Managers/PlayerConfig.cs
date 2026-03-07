using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Player Settings")]
    public PlayerData SelectedPlayer;
}

