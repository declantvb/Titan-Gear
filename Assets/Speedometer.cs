using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
	private const float MetersPerSecondToKilometersPerHour = 3.6f;
	private Rigidbody rb;
	private Text text;

	public void Start()
	{
		text = GetComponent<Text>();
	}

	public void UpdateWith(GameObject player)
	{
		rb = player.GetComponent<Rigidbody>();
	}

	public void OnGUI()
	{
		text.text = "Speed:\n" + Mathf.Floor(rb.velocity.magnitude * MetersPerSecondToKilometersPerHour) + " KM/H";
	}
}