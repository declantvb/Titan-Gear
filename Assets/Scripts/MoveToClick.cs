using UnityEngine;
using System.Collections;

public class MoveToClick : MonoBehaviour
{
	private WheeledVehicleAI vehicle;

	// Use this for initialization
	void Start()
	{
		vehicle = GetComponent<WheeledVehicleAI>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var mousePos = Input.mousePosition;
			var ray = Camera.main.ScreenPointToRay(mousePos);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				vehicle.CurrentMoveTarget = hit.point;
			}
		}
	}
}
