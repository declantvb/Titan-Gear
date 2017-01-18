using UnityEngine;
using UnityEngine.UI;

public class MissileLock : MonoBehaviour
{
	private WeaponSystem weaponSystem;
	public Texture2D texture2D;

	public void OnGUI()
	{
		if (weaponSystem.missileLock != null)
		{
			var pos = Camera.main.WorldToScreenPoint(weaponSystem.missileLock.position);
			var heading = weaponSystem.missileLock.position - Camera.main.transform.position;
			if (Vector3.Dot(Camera.main.transform.forward, heading) > 0)
			{
				var screenHeight = Camera.main.pixelHeight;

				GUI.color = Color.red;
				GUI.DrawTexture(new Rect(pos.x - 10, screenHeight - pos.y - 10, 20, 20), texture2D);
				GUI.color = Color.white;
			}
		}
	}

	public void UpdateWith(GameObject player)
	{
		weaponSystem = player.GetComponentInChildren<WeaponSystem>();
	}
}