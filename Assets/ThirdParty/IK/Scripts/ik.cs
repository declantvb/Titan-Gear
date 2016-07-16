using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ik : MonoBehaviour
{
	List<Transform> pLimbs;
 
	public Transform target;
	public Transform pReachLimb;
	public Transform elbowTarget;
	public bool IsEnabled = true;
	public bool debug = true;
	public float transition = 1.0f;

	private List<Quaternion> startRotations;
	private Vector3 targetRelativeStartPosition;
	private Vector3 elbowTargetRelativeStartPosition;
	private GameObject pIk;

	public void Start()
	{

		pLimbs = new List<Transform>();
		pLimbs.Add(transform);
		pLimbs.Add(pReachLimb);
		pLimbs.Add(pReachLimb);
		while (pLimbs[1] != null && pLimbs[1].parent.transform != pLimbs[0])
		{
			pLimbs[1] = pLimbs[1].parent.transform;
		}

		pIk = new GameObject(name + "-ik");
		pIk.transform.parent = null;

		var i = 0;

		startRotations = new List<Quaternion>();
		startRotations.Clear();

		for (i = 0; i < pLimbs.Count; i++)
		{
			startRotations.Add(pLimbs[i].rotation);
		}

		targetRelativeStartPosition = target.position - pLimbs[0].position;
		if (elbowTarget != null)
		{
			elbowTargetRelativeStartPosition = elbowTarget.position - pLimbs[0].position;
		}
		else
		{
			elbowTargetRelativeStartPosition = new Vector3(0, 0, 0);
		}
	}

	public void LateUpdate()
	{
		if (!IsEnabled)
		{
			return;
		}
		CalculateIK2();
	}

	public void CalculateIK2()
	{
		//Lengths and distances.
		var i = 0;
		var lengths = new List<float>();
		lengths.Clear();
		var totalLength = 0f;
		var distance = Vector3.Distance(pLimbs[0].position, pLimbs[pLimbs.Count - 1].position);

		pIk.transform.parent = null;

		pLimbs[0] = transform;
		pLimbs[1] = pReachLimb;
		pLimbs[2] = pReachLimb;
		while (pLimbs[1] != null && pLimbs[1].parent.transform != pLimbs[0])
		{
			pLimbs[1] = pLimbs[1].parent.transform;
		}

		if (pLimbs[2].parent.transform != pLimbs[1])
		{
			if (!pLimbs[1].GetComponent<ik>())
			{
				pLimbs[1].gameObject.AddComponent<ik>();
				pLimbs[1].GetComponent<ik>().pLimbs = new List<Transform>();
			}
			pLimbs[1].GetComponent<ik>().target = target;
			pLimbs[1].GetComponent<ik>().elbowTarget = elbowTarget;
			pLimbs[1].GetComponent<ik>().IsEnabled = IsEnabled;
			pLimbs[1].GetComponent<ik>().debug = debug;
			pLimbs[1].GetComponent<ik>().transition = transition;
			pLimbs[1].GetComponent<ik>().pReachLimb = pReachLimb;
		}

		for (i = 0; i < pLimbs.Count - 1; i++)
		{
			lengths.Add(Vector3.Distance(pLimbs[i].position, pLimbs[i + 1].position));
			totalLength += Vector3.Distance(pLimbs[i].position, pLimbs[i + 1].position);
		}

		distance = Mathf.Min(distance, totalLength - 0.0001f);

		var adj = (Mathf.Pow(lengths[0], 2) - Mathf.Pow(lengths[1], 2) + Mathf.Pow(distance, 2)) / (2 * distance);

		//Debug.Log(adj);

		var ikAng = Mathf.Acos(adj / lengths[0]) * Mathf.Rad2Deg;

		var targetPos = target.position;
		Vector3 elbowPos;

		if (elbowTarget != null)
		{
			elbowPos = elbowTarget.position;
		}
		else
		{
			elbowPos = new Vector3(0, 0, 0);
		}

		//Parents
		var parents = new List<Transform>();
		parents.Clear();

		for (i = 0; i < pLimbs.Count; i++)
		{
			parents.Add(pLimbs[i].parent);
		}

		//Scales
		var scales = new List<Vector3>();
		scales.Clear();

		for (i = 0; i < pLimbs.Count; i++)
		{
			scales.Add(pLimbs[i].localScale);
		}

		//Position
		var positions = new List<Vector3>();
		positions.Clear();

		for (i = 0; i < pLimbs.Count; i++)
		{
			positions.Add(pLimbs[i].localPosition);
		}

		//Position
		var rotations  = new List<Quaternion>();
		rotations.Clear();

		for (i = 0; i < pLimbs.Count; i++)
		{
			rotations.Add(pLimbs[i].rotation);
		}

		target.position = targetRelativeStartPosition + pLimbs[0].position;

		if (elbowTarget != null)
		{
			elbowTarget.position = elbowTargetRelativeStartPosition + pLimbs[0].position;
		}

		for (i = 0; i < pLimbs.Count; i++)
		{
			pLimbs[i].rotation = startRotations[i];
		}

		pIk.transform.position = pLimbs[0].position;
		pIk.transform.LookAt(targetPos, elbowPos - pIk.transform.position);

		var axisCorrections  = new List<GameObject>();
		axisCorrections.Clear();
		for (i = 0; i < pLimbs.Count; i++)
		{
			axisCorrections.Add(new GameObject(pLimbs[i].name + "AxisCorrection"));
		}
		for (i = 0; i < pLimbs.Count; i++)
		{
			axisCorrections[i].transform.position = pLimbs[i].position;
			if (i != pLimbs.Count - 1)
			{
				axisCorrections[i].transform.LookAt(pLimbs[i + 1].position, pIk.transform.root.up);
			}
			if (i == 0)
			{
				axisCorrections[i].transform.parent = pIk.transform;
			}
			else
			{
				axisCorrections[i].transform.parent = axisCorrections[i - 1].transform;
			}
			pLimbs[i].parent = axisCorrections[i].transform;
		}

		target.position = targetPos;
		if (elbowTarget != null)
		{
			elbowTarget.position = elbowPos;
		}

		if (elbowTarget != null)
		{
			axisCorrections[0].transform.LookAt(target, elbowTarget.position - axisCorrections[0].transform.position);
			var euler = axisCorrections[0].transform.eulerAngles;
			euler.x -= ikAng;
			axisCorrections[0].transform.localEulerAngles = euler;
			axisCorrections[1].transform.LookAt(target, elbowTarget.position - axisCorrections[0].transform.position);
			axisCorrections[2].transform.rotation = target.rotation;
		}
		else
		{
			axisCorrections[0].transform.LookAt(target, -axisCorrections[0].transform.position);
			var euler = axisCorrections[0].transform.localEulerAngles;
			euler.x -= ikAng;
			axisCorrections[0].transform.localEulerAngles = euler;
			axisCorrections[1].transform.LookAt(target, -axisCorrections[0].transform.position);
			axisCorrections[2].transform.rotation = target.rotation;
		}

		for (i = 0; i < pLimbs.Count; i++)
		{
			pLimbs[i].parent = parents[i];
			pLimbs[i].localScale = scales[i];
			pLimbs[i].localPosition = positions[i];
		}

		for (i = 0; i < pLimbs.Count; i++)
		{
			Destroy(axisCorrections[i]);
		}

		transition = Mathf.Clamp01(transition);
		for (i = 0; i < pLimbs.Count; i++)
		{
			pLimbs[i].rotation = Quaternion.Slerp(rotations[i], pLimbs[i].rotation, transition);
		}

		pIk.transform.parent = transform;
	}
}
