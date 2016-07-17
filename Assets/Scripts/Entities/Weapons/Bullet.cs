using UnityEngine;
using System.Collections;
using System;

public abstract class Bullet : MonoBehaviour
{
	public float TimeToLive = 30f;
	private Rigidbody _rigidbody;

	public void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	public void FixedUpdate()
	{
		TimeToLive -= Time.fixedDeltaTime;
		if (TimeToLive < 0)
		{
			Destroy(gameObject);
		}

		transform.LookAt(transform.position + _rigidbody.velocity);
	}

	public void OnTriggerEnter(Collider other)
	{
		DoDamage(other);
		Destroy(gameObject);
	}

	public abstract void DoDamage(Collider other);
}