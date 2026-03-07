using UnityEngine;

public class PlayerFactory : MonoBehaviour
{
    [SerializeField] private GridSystem _gridSystem;

    public GameObject Create(PlayerData data, Vector2Int spawnPoint)
    {
        GameObject instance = Instantiate(data.PlayerPrefab);
        CharacterBehavior characterBehavior = AddCharacterBehavior(instance, data.Race);
        instance.GetComponent<PlayerController>().Initialize(data, _gridSystem, spawnPoint);
        instance.GetComponent<PlayerSoundController>().Initialize(data);
        characterBehavior.Initialize(_gridSystem, data);
        return instance;
    }

    private CharacterBehavior AddCharacterBehavior(GameObject instance, Race race)
    {
        return race switch
        {
            Race.Undead => instance.AddComponent<UndeadBehavior>(),
            Race.Angel  => instance.AddComponent<AngelBehavior>(),
            Race.Human  => instance.AddComponent<HumanBehavior>(),
            Race.Devil  => instance.AddComponent<DevilBehavior>(),
            _           => instance.AddComponent<CharacterBehavior>()
        };
    }
}