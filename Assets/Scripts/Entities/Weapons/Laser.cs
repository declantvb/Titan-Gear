using UnityEngine;

public class Laser : MonoBehaviour
{
	public float Duration;
	public float DamagePerSecond;
	private LineRenderer line;

	// Use this for initialization
	private void Start()
	{
		line = GetComponent<LineRenderer>();
		Destroy(gameObject, Duration);
	}

	// Update is called once per frame
	private void Update()
	{
		Ray ray = new Ray(transform.position, transform.forward);

		line.SetPosition(0, ray.origin);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100))
		{
			line.SetPosition(1, hit.point);
			var health = hit.collider.GetComponent<Health>();
			if (health != null)
			{
				var damage = DamagePerSecond * Time.deltaTime;
				health.YaGotShot(damage);
			}
		}
		else
		{
			line.SetPosition(1, ray.GetPoint(100));
		}
	}
}