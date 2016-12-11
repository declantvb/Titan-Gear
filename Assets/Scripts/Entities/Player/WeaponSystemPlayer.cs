using UnityEngine;

public class WeaponSystemPlayer : MonoBehaviour
{
	[SerializeField]
	private bool MouseLook;

	private WeaponSystem weapons;
	private Transform turret;
	private Transform arms;
	private Transform mainCamera;
	private bool togglingLook;

	private void Start()
	{
		weapons = GetComponentInChildren<WeaponSystem>();
		turret = weapons.transform;
		arms = turret.Find("arms").transform;
		mainCamera = Camera.main.transform;
	}

	public void Update()
	{
		RaycastHit hit;
		if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 500f))
		{
			var enemy = hit.collider.GetComponentInParent<Enemy>();
			if (enemy != null)
			{
				weapons.missileLock = enemy.transform;
			}
		}
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
		if (Input.GetKey(KeyCode.M))
		{
			if (!togglingLook)
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
				togglingLook = true; 
			}
		}
		else
		{
			togglingLook = false;
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