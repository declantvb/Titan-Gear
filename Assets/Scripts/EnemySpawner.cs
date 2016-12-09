using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public int ConcurrentEnemyCount;

	[SerializeField]
	private GameObject enemyPrefab;

	[SerializeField]
	private List<GameObject> currentEnemies;

	[SerializeField]
	private GameObject terrainMaster;

	private TerrainGenerator terrainGenerator;

	// Use this for initialization
	private void Start()
	{
		currentEnemies = new List<GameObject>();
		if (terrainMaster != null)
		{
			terrainGenerator = terrainMaster.GetComponent<TerrainGenerator>();
		}
	}

	// Update is called once per frame
	private void Update()
	{
		var deficit = ConcurrentEnemyCount - currentEnemies.Count;
		for (int i = 0; i < deficit; i++)
		{
			var newPos = Random.insideUnitCircle.ToFlatVector3() * 100;

			var height = 5f;
			if (terrainGenerator != null)
			{
				height = terrainGenerator.GetHeightAt(newPos);
			}

			newPos.y = height;
			var newEnemy = (GameObject)Instantiate(enemyPrefab, newPos, Quaternion.identity);
			currentEnemies.Add(newEnemy);
		}

		currentEnemies = currentEnemies.Where(x => x != null).ToList();
	}
}