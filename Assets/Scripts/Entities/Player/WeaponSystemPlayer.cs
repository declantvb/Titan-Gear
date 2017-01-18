using UnityEngine;

public class WeaponSystemPlayer : MonoBehaviour
{
	[SerializeField]
	private bool MouseLook;

	private WeaponSystem weapons;
	private Transform turret;
	private Transform arms;
	private Camera mainCamera;
	private bool togglingLook;

	private void Start()
	{
		weapons = GetComponentInChildren<WeaponSystem>();
		turret = weapons.transform;
		arms = turret.Find("arms").transform;
		mainCamera = Camera.main;
	}

	public void Update()
	{
		var enemies = GameObject.FindGameObjectsWithTag("Enemy");

		if (enemies.Length > 0)
		{
			var viewRect = new Rect(0, 0, 1, 1);
			var midPoint = new Vector2(0.5f, 0.5f);

			var minDist = 2f;
			Transform minEnemy = enemies[0].transform;

			foreach (var enemy in enemies)
			{
				var viewPos = mainCamera.WorldToViewportPoint(enemy.transform.position);
				var screenPos = new Vector2(viewPos.x, viewPos.y);
				var dist = Vector2.Distance(screenPos, midPoint);
				if (viewRect.Contains(screenPos) && dist < minDist)
				{
					minDist = dist;
					minEnemy = enemy.transform;
				}
			}

			weapons.missileLock = minEnemy;
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