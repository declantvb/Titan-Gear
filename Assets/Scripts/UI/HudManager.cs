using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
	public Slider HealthBar;
	public Slider ArmorBar;
	public Slider ShieldBar;
	public Slider FuelBar;
	public Slider BatteryBar;

	private GameObject player;
	private WeaponsUIPanel weaponsPanel;
	private Speedometer speedometer;
	private Health playerHealth;
	private PowerSystem playerPower;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<HardpointManager>().UpdateHud = () => UpdateHud();
		playerHealth = player.GetComponent<Health>();
		playerPower = player.GetComponent<PowerSystem>();
		weaponsPanel = GetComponentInChildren<WeaponsUIPanel>();
		speedometer = GetComponentInChildren<Speedometer>();
	}

	private void UpdateHud()
	{
		weaponsPanel.UpdateWeapons(player);
		speedometer.UpdateWith(player);

		UpdateHealth();
		UpdateArmor();
		UpdateShields();

		UpdatePowerSystem();
	}

	private void UpdatePowerSystem()
	{
		FuelBar.GetComponent<Slider>().value = playerPower.FuelTank.Stored / playerPower.FuelTank.Capacity;
		BatteryBar.GetComponent<Slider>().value = playerPower.BatteryBank.Stored / playerPower.BatteryBank.Capacity;
	}

	private void UpdateShields()
	{
	}

	private void UpdateArmor()
	{
	}

	private void UpdateHealth()
	{
		HealthBar.value = playerHealth.CurrentHealth / playerHealth.MaxHealth;
	}
}