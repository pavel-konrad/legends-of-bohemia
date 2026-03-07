using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
	[SerializeField] private GridSystem _grid;
	[SerializeField] private GameObject _prefab;

	void Start()
	{
		for (var x = 0; x < _grid.Width; x ++)
		{
			for (var y = 0; y < _grid.Height; y ++)
			{
				Instantiate(_prefab, _grid.GridToWorld(new Vector2Int(x,y)), Quaternion.identity, transform);
			}
		}
	}
	
}