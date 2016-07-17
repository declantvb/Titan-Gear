using UnityEngine;

public class WheeledVehiclePlayer : WheeledVehicle
{
	public override float GetSteering()
	{
		return Input.GetAxis("Horizontal");
	}

	public override float GetThrottle()
	{
		return Input.GetAxis("Vertical");
	}
}