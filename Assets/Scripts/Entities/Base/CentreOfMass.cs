using UnityEngine;

public class CentreOfMass : MonoBehaviour
{
	public Vector3 Offset;

	// Use this for initialization
	private void Start()
	{
	}

	private void FixedUpdate()
	{
		GetComponent<Rigidbody>().centerOfMass = Offset;
	}
}