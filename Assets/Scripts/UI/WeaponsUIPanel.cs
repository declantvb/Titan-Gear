using UnityEngine;

public class WeaponsUIPanel : MonoBehaviour
{
	public GameObject weaponPanelPrefab;
	public float spacing;
	public float prefabHeight;

	public void Start()
	{
	}

	public void UpdateWeapons(GameObject player)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject.Destroy(transform.GetChild(i).gameObject);
		}

		var rectTransform = gameObject.transform as RectTransform;

		var weapons = player.GetComponentsInChildren<Weapon>();
		rectTransform.sizeDelta = new Vector2(190, weapons.Length * (prefabHeight + spacing) + spacing);

		foreach (var weapon in weapons)
		{
			var panel = GameObject.Instantiate(weaponPanelPrefab);
			panel.transform.SetParent(transform, false);

			var ui = panel.GetComponent<WeaponUIPanel>();
			ui.Weapon = weapon;
			ui.Initialise();
		}
	}
}