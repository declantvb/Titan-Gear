﻿using UnityEngine;

public class AIMover : MonoBehaviour
{
	private float searchRadius = 100f;
	private WheeledVehicleAI vehicle;
	private WeaponSystemAI weapons;
	private Vector3 CurrentMoveTarget;
	[SerializeField]
	private Transform CurrentTransformTarget;
	public float OffsetDistance;
	public float WanderDistance;
	public bool CanShoot;
	private bool init;

	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
		if (!init)
		{
			vehicle = GetComponentInChildren<WheeledVehicleAI>();
			weapons = GetComponentInChildren<WeaponSystemAI>();

			if (vehicle != null && weapons != null)
			{
				init = true;
			}
			return;
		}

		GetTarget();

		HandleMovement();

		HandleWeapons();
	}

	private void GetTarget()
	{
		if (CurrentTransformTarget == null)
		{
			//search
			var res = Physics.OverlapSphere(transform.position, searchRadius);

			foreach (var collider in res)
			{
				var player = collider.GetComponentInParent<Player>();
				if (player != null)
				{
					CurrentTransformTarget = player.transform;
					break;
				}
			}
		}
	}

	private void HandleWeapons()
	{
		if (CurrentTransformTarget != null)
		{
			weapons.Target = CurrentTransformTarget;
			weapons.ShootingEnabled = CanShoot;
		}
	}

	private void HandleMovement()
	{
		if (CurrentTransformTarget != null)
		{
			var targetPos = CurrentTransformTarget.transform.position;
			var offset = (transform.position - targetPos).normalized * OffsetDistance;
			vehicle.CurrentMoveTarget = targetPos + offset;
		}
		//still no target
		else if (CurrentMoveTarget != default(Vector3))
		{
			vehicle.CurrentMoveTarget = CurrentMoveTarget;
			if (Vector3.Distance(transform.position, CurrentMoveTarget) < OffsetDistance)
			{
				CurrentMoveTarget = default(Vector3);
			}
		}
		else
		{
			//wander
			var rand = UnityEngine.Random.insideUnitCircle;
			var wander = rand * WanderDistance;
			CurrentMoveTarget = new Vector3(transform.position.x + wander.x, transform.position.y, transform.position.z + wander.y); // get ground height
		}

		//fix getting stuck
	}
}