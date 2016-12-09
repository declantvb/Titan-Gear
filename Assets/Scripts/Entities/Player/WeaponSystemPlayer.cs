using UnityEngine;

public class WeaponSystemPlayer : MonoBehaviour
{
	[SerializeField]
	private bool MouseLook;

	private WeaponSystem weapons;
	private Transform turret;
	private Transform arms;

	private void Start()
	{
		weapons = GetComponentInChildren<WeaponSystem>();
		turret = weapons.transform;
		arms = turret.Find("arms").transform;
	}

	private void FixedUpdate()
	{
		HandleLook();

		if (MouseLook && Input.GetAxis("Fire1") > 0f)
		{
			weapons.FireWeapon();
		}
	}

	public void HandleLook()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			MouseLook = !MouseLook;
			if (MouseLook)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		if (MouseLook)
		{
			var mouseX = Input.GetAxis("Mouse X");
			var mouseY = -Input.GetAxis("Mouse Y"); //todo invert look option

			var currentY = arms.localEulerAngles.x; // rotation about x axis
			var relativeY = arms.localEulerAngles.x > 180 ? currentY - 360 : currentY; // make negatives negative

			var pitch = new Vector3(Mathf.Clamp(relativeY + mouseY, -90, 90), 0, 0);
			var yaw = new Vector3(0, turret.localEulerAngles.y + mouseX, 0);

			turret.localEulerAngles = yaw;
			arms.localEulerAngles = pitch;
		}
	}
}