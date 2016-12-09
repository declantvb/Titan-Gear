using UnityEngine;

public class WeaponSystemPlayer : MonoBehaviour
{
	[SerializeField]
	private bool MouseLook;

	private WeaponSystem weapons;
	private Transform turret;

	private void Start()
	{
		weapons = GetComponentInChildren<WeaponSystem>();
		turret = weapons.transform;
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

			var current = turret.localEulerAngles;
			var currentY = current.x; // rotation about x axis
			var relativeY = currentY > 180 ? currentY - 360 : currentY; // make negatives negative

			var update = new Vector3(Mathf.Clamp(relativeY + mouseY, -90, 90), current.y + mouseX, 0);

			turret.localEulerAngles = update;
		}
	}
}