using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
	public float TimeToLive = 30f;
	private Rigidbody _rigidbody;
	private Vector3 previousPos;
	private bool destroyed;
	private LayerMask layerMask;

	public void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		previousPos = transform.position;

		layerMask = LayerMaskExtensions.Create(LayerMask.NameToLayer("Bullets")).Inverse();
	}

	public void FixedUpdate()
	{
		if (destroyed) return;

		TimeToLive -= Time.fixedDeltaTime;
		if (TimeToLive <= 0)
		{
			Destroy(gameObject);
		}

		RaycastHit rayHit;
		if (Physics.Linecast(previousPos, transform.position, out rayHit, layerMask))
		{
			Hit(rayHit.point, rayHit.collider);
			Destroy(gameObject);
			destroyed = true;
		}

		transform.LookAt(transform.position + _rigidbody.velocity);
		previousPos = transform.position;
	}

	public abstract void Hit(Vector3 point, Collider other);
}