using UnityEngine;

public class PartSlot : MonoBehaviour
{
	public GameObject currentPart;
	public GameObject swapPart;
	private GameObject partInstance;

	// Use this for initialization
	private void Start()
	{
		partInstance = (GameObject)Instantiate(currentPart, transform.position, transform.rotation);
		partInstance.transform.SetParent(transform);
	}

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			var temp = currentPart;
			ChangePart(swapPart);
			swapPart = temp;
		}
	}

	public void ChangePart(GameObject newPart)
	{
		if (newPart != currentPart)
		{
			//need to copy over some things?

			Destroy(partInstance);

			partInstance = (GameObject)Instantiate(newPart, transform.position, transform.rotation);
			partInstance.transform.SetParent(transform);

			currentPart = newPart;
		}
	}
}