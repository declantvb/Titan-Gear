using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
	public float MaxHealth;
	public float CurrentHealth;

	[Header("Healthbar Display")]
	public bool DisplayInWorld = true;

	public float healthbarWorldOffset = 3f;
	public int healthBarTextHeight = 20;
	public int healthbarHeight = 24;
	public int healthbarWidth = 70;
	public int screenspaceOffsetHeight = 50;

	public GameObject HealthSlider;

	private GUIStyle HealthBarBackground;
	private GUIStyle HealthBarForeground;
	private Player player;
	private Enemy enemy;

	// Use this for initialization
	private void Start()
	{
		CurrentHealth = MaxHealth;

		var redTex = new Texture2D(1, 1);
		redTex.SetPixel(0, 0, Color.red);
		redTex.Apply();
		redTex.wrapMode = TextureWrapMode.Repeat;
		HealthBarBackground = new GUIStyle();
		HealthBarBackground.normal.background = redTex;

		var greenTex = new Texture2D(1, 1);
		greenTex.SetPixel(0, 0, Color.green);
		greenTex.Apply();
		greenTex.wrapMode = TextureWrapMode.Repeat;
		HealthBarForeground = new GUIStyle();
		HealthBarForeground.normal.background = greenTex;

		player = GetComponent<Player>();
		enemy = GetComponent<Enemy>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (CurrentHealth <= 0)
		{
			//destroy
			if (player != null)
			{
				SceneManager.LoadScene(0);
			}

			if (enemy != null)
			{
				enemy.Die();
			}
		}

		if (HealthSlider != null)
		{
			HealthSlider.GetComponent<Slider>().value = CurrentHealth / MaxHealth;
		}
	}

	public void OnGUI()
	{
		//show healthbar
		if (DisplayInWorld)
		{
			var screenHeight = Camera.main.pixelHeight;

			var pos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, healthbarWorldOffset, 0));
			var heading = transform.position - Camera.main.transform.position;
			if (Vector3.Dot(Camera.main.transform.forward, heading) > 0)
			{
				var name = "Unknown";
				if (enemy != null)
				{
					name = enemy.Name;
				}
				else if (player != null)
				{
					name = player.Name;
				}

				var healthPercent = CurrentHealth / MaxHealth;

				var hbPos = new Vector2(pos.x - healthbarWidth / 2, screenHeight - pos.y - screenspaceOffsetHeight);
				var hbSize = new Vector2(healthbarWidth, healthbarHeight);
				GUI.Label(new Rect(hbPos.x, hbPos.y - healthBarTextHeight, hbSize.x, healthBarTextHeight), name);
				GUI.BeginGroup(new Rect(hbPos.x, hbPos.y, hbSize.x, hbSize.y));
				GUI.Box(new Rect(0, 0, hbSize.x, hbSize.y), "", HealthBarBackground);
				GUI.BeginGroup(new Rect(0, 0, hbSize.x * healthPercent, hbSize.y));
				GUI.Box(new Rect(0, 0, hbSize.x, hbSize.y), "", HealthBarForeground);
				GUI.EndGroup();
				GUI.EndGroup();
			}
		}
	}

	internal void YaGotShot(float damage)
	{
		CurrentHealth -= damage;
	}
}