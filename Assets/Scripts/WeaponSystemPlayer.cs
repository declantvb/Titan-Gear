using UnityEngine;

public class WeaponSystemPlayer : WeaponSystem
{
	[SerializeField]
	private bool MouseLook;

	public override void HandleLook()
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

			var current = transform.localEulerAngles;
			var currentY = current.x; // rotation about x axis
			var relativeY = currentY > 180 ? currentY - 360 : currentY; // make negatives negative

			var clampedY = relativeY + mouseY;

			if (clampedY > 90) clampedY = 90;
			else if (clampedY < -90) clampedY = -90;

			var update = new Vector3(clampedY, current.y + mouseX, 0);

			transform.localEulerAngles = update;
			//transform.localEulerAngles += new Vector3(mouseY, mouseX, 0);
		}
	}

	public override bool FireWeapon()
	{
		return MouseLook && Input.GetAxis("Fire1") > 0f;
	}

	public override bool SwitchWeapon()
	{
		return MouseLook && Input.GetAxis("WeaponSwitch") > 0;
	}
}