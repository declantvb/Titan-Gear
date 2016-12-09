using UnityEngine;

public class WeaponSystemAI : MonoBehaviour
{
	public float rotationSpeed = 10f;
	public Transform Target;
	public float FireDistance;
	public bool ShootingEnabled;

	private WeaponSystem weapons;
	private Transform turret;
	private Transform arms;

	private void Start()
	{
		weapons = GetComponent<WeaponSystem>();
		FireDistance = weapons.ActiveWeapons[0].WeaponDescriptor.InitialBulletVelocity;
		turret = weapons.transform;
		arms = turret.Find("arms").transform;
	}

	private void FixedUpdate()
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

			var currentDir = arms.forward;
			var targetDiff = targetPos - turret.position;
			var targetDir = targetDiff.normalized;
			var cross = Vector3.Cross(currentDir, targetDir);

			var yawDirection = Mathf.Sign(Vector3.Dot(cross, turret.up));
			var yawAngle = Mathf.Clamp(cross.magnitude * yawDirection, -rotationSpeed, rotationSpeed);

			var pitchDirection = Mathf.Sign(Vector3.Dot(cross, turret.right));
			var pitchAngle = Mathf.Clamp(cross.magnitude * pitchDirection, -rotationSpeed, rotationSpeed);

			turret.localEulerAngles = new Vector3(0, turret.localEulerAngles.y + yawAngle, 0);
			arms.localEulerAngles = new Vector3(arms.localEulerAngles.x + pitchAngle, 0, 0);
		}
	}
}