using UnityEngine;

public class WeaponSystemAI : MonoBehaviour
{
	public float rotationSpeed = 0.05f;
	public Transform Target;
	public float FireDistance;
	public bool ShootingEnabled;

	private WeaponSystem weapons;
	private Transform turret;

	void Start()
	{
		weapons = GetComponentInChildren<WeaponSystem>();
		turret = weapons.transform;
	}

	void FixedUpdate()
	{
		HandleLook();

		if (ShootingEnabled && Target != null && Vector3.Distance(transform.position, Target.position) < FireDistance)
		{
			weapons.FireWeapon();
		}
	}

	public void HandleLook()
	{
		if (Target != null)
		{
			var targetPos = Target.position; //todo lead target

			var currentDir = turret.forward;
			var targetDiff = targetPos - turret.position;
			var targetDir = targetDiff.normalized;
			var moveDir = Vector3.RotateTowards(currentDir, targetDir, rotationSpeed, 1);
			var direction = Mathf.Sign(Vector3.Dot(Vector3.Cross(currentDir, targetDir), turret.up));
			var angle = Vector3.Angle(currentDir, moveDir) * direction;

			turret.localRotation = Quaternion.Euler(0, turret.localRotation.eulerAngles.y + angle, 0);
		}
	}
}