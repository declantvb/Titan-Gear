using UnityEngine;

public class PartSlot : MonoBehaviour
{
	//todo add name/identifiers
	public SlotType Kind;

	public GameObject currentPartPrefab;
	public GameObject currentPartInstance;

	private void Start()
	{
		ChangePart(currentPartPrefab);
	}

	public void ChangePart(GameObject newPart)
	{
		//need to copy over some things?
		if (currentPartInstance != null)
		{
			currentPartInstance.transform.parent = null;
			Destroy(currentPartInstance);
		}

		if (newPart != null)
		{
			currentPartInstance = (GameObject)Instantiate(newPart, transform.position, transform.rotation);
			currentPartInstance.transform.SetParent(transform);
		}

		currentPartPrefab = newPart;
	}

	public enum SlotType
	{
		Mobility,
		Turret,
		Large,
		Medium,
		Small
	}
}