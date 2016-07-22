using UnityEngine;

public class WheeledVehiclePlayer : MonoBehaviour
{
	private WheeledVehicle vehicle;

	public void Start()
	{
		vehicle = GetComponent<WheeledVehicle>();

		vehicle.GetSteering = () => Input.GetAxis("Horizontal");
		vehicle.GetThrottle = () => Input.GetAxis("Vertical");
	}
}