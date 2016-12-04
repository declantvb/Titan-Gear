using UnityEngine;

public class HudManager : MonoBehaviour
{
	private GameObject player;
	private WeaponsUIPanel weaponsPanel;
	private Speedometer speedometer;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<HardpointManager>().UpdateHud = () => UpdateHud();
		weaponsPanel = GetComponentInChildren<WeaponsUIPanel>();
		speedometer = GetComponentInChildren<Speedometer>();
	}

	private void UpdateHud()
	{
		weaponsPanel.UpdateWeapons(player);
		speedometer.UpdateWith(player);
	}
}