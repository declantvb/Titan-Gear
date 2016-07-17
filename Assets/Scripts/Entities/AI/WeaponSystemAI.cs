using UnityEngine;

public class WeaponSystemAI : WeaponSystem
{
	public float rotationSpeed = 0.05f;
	public Transform Target;
	public float FireDistance;

	public bool ShootingEnabled;

	public override bool FireWeapon()
	{
		if (!ShootingEnabled)
		{
			return false;
		}

		if (Target != null)
		{
			//check angle
			return Vector3.Distance(transform.position, Target.position) < FireDistance;
		}

		return false;
	}

	public override void HandleLook()
	{
		if (Target != null)
		{
			var targetPos = Target.position; //todo lead target

			var currentDir = transform.forward;
			var targetDiff = targetPos - transform.position;
			var targetDir = targetDiff.normalized;
			var moveDir = Vector3.RotateTowards(currentDir, targetDir, rotationSpeed, 1);
			var direction = Mathf.Sign(Vector3.Dot(Vector3.Cross(currentDir, targetDir), transform.up));
			var angle = Vector3.Angle(currentDir, moveDir) * direction;

			transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + angle, 0);
		}
	}

	public override bool SwitchWeapon()
	{
		return false;
	}
}