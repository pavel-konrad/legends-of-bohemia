using UnityEngine;
using System.Collections.Generic;
/*
This Factory is for creating spells, base on their type. The SpawnManager.cs manage wchich type will be spawned,
base on the weight of spell. The type of spells is Enum defined in SpellType.cs. This Factory has to be
attached on empty object in scenes and add spells prefab to the list.
*/
public class SpellFactory : MonoBehaviour, IFactory<SpellType, GameObject>
{

    [SerializeField] private List<GameObject> _spells;
    public GameObject Create(SpellType type)
    {
        GameObject prefab = _spells.Find(s => s.GetComponent<SpellBase>().Data.Type == type);
        if(prefab == null) return null;

        return Instantiate(prefab);
    }
}