using UnityEngine;
using System.Collections;

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
			var mouseY = Input.GetAxis("Mouse Y");

			transform.localEulerAngles += new Vector3(-mouseY, mouseX, 0);
		}
	}

	public override bool FireWeapon()
	{
		return Input.GetAxis("Fire1") > 0f;
	}

	public override bool SwitchWeapon()
	{
		return Input.GetAxis("WeaponSwitch") > 0;
	}
}
