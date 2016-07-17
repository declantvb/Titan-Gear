using UnityEngine;

public class Enemy : MonoBehaviour
{
	public string Name;
	public GameObject lootPrefab;

	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}

	internal void Die()
	{
		Instantiate(lootPrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}