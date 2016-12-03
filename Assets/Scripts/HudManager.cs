using UnityEngine;

public class HudManager : MonoBehaviour
{
	private GameObject player;
	private WeaponsUIPanel weaponsPanel;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<HardpointManager>().UpdateHud = () => UpdateHud();
		weaponsPanel = GetComponentInChildren<WeaponsUIPanel>();
	}

	private void UpdateHud()
	{
		weaponsPanel.UpdateWeapons(player);
	}
}