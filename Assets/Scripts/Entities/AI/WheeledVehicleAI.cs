using UnityEngine;

public class WheeledVehicleAI : MonoBehaviour
{
	public Vector3 CurrentMoveTarget;

	[SerializeField]
	private float currentSteering = 0f;

	[SerializeField]
	private float currentThrottle = 0f;

	[SerializeField]
	private float StopThreshold = 10f;

	[SerializeField]
	private float changedcooldown;

	[SerializeField]
	private bool reversing;

	private WheeledVehicle vehicle;

	public void Start()
	{
		vehicle = GetComponent<WheeledVehicle>();

		vehicle.GetSteering = () => currentSteering;
		vehicle.GetThrottle = () => currentThrottle;
	}

	public void Update()
	{
		var currentDir = transform.forward;
		var targetDiff = CurrentMoveTarget - transform.position;
		var targetDir = targetDiff.normalized;
		var moveDir = Vector3.RotateTowards(currentDir, targetDir, vehicle.maxSteeringAngle * Mathf.Deg2Rad, 1);
		var direction = Mathf.Sign(Vector3.Dot(Vector3.Cross(currentDir, targetDir), transform.up));
		currentSteering = Vector3.Angle(currentDir, moveDir) / vehicle.maxSteeringAngle * direction;

		var dist = targetDiff.magnitude;
		if (dist > StopThreshold)
		{
			currentThrottle = 1f;

			var needToReverse = Vector3.Dot(targetDir, currentDir) < 0;

			if (reversing != needToReverse)
			{
				if (changedcooldown <= 0)
				{
					reversing = needToReverse;
					changedcooldown = 0.2f;
				}
				changedcooldown -= Time.deltaTime;
			}

			if (reversing)
			{
				currentThrottle = -currentThrottle;
				currentSteering = -currentSteering;
			}
		}
		else
		{
			currentThrottle = 0f;
			changedcooldown = 0f;
		}
	}
}