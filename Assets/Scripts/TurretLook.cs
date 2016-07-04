using UnityEngine;
using System.Collections;

public class TurretLook : MonoBehaviour
{
	[SerializeField]
	bool MouseLook = false;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
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
}
