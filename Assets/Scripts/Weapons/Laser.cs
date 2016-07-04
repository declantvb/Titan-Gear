using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
	public float Duration;

	LineRenderer line;

	// Use this for initialization
	void Start()
	{
		line = GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		Ray ray = new Ray(transform.position, transform.forward);

		line.SetPosition(0, ray.origin);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100))
		{
			line.SetPosition(1, hit.point);
		}
		else
		{
			line.SetPosition(1, ray.GetPoint(100)); 
		}


		Duration -= Time.deltaTime;
		if (Duration < 0)
		{
			Destroy(gameObject);
		}
	}
}
