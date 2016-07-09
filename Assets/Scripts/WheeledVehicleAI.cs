using UnityEngine;

public class WheeledVehicleAI : WheeledVehicle
{
	[SerializeField]
	private float currentSteering = 0f;

	[SerializeField]
	private float currentThrottle = 0f;

	[Header("AI stuff")]
	public Vector3 CurrentMoveTarget;

	public float StopThreshold;

	[SerializeField]
	private float changedcooldown;
	private bool reversing;

	public override void Update()
	{
		var currentDir = transform.forward;
		var targetDiff = CurrentMoveTarget - transform.position;
		var targetDir = targetDiff.normalized;
		var moveDir = Vector3.RotateTowards(currentDir, targetDir, MaxSteeringAngle * Mathf.Deg2Rad, 1);
		var direction = Mathf.Sign(Vector3.Dot(Vector3.Cross(currentDir, targetDir), transform.up));
		currentSteering = Vector3.Angle(currentDir, moveDir) / MaxSteeringAngle * direction;

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

		base.Update();
	}

	public override float GetSteering()
	{
		return currentSteering;
	}

	public override float GetThrottle()
	{
		return currentThrottle;
	}
}