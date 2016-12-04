using System.Linq;
using UnityEngine;

public class Projectile : Bullet
{
	public GameObject ExplosionPrefab;
	public float DamageModifier = 50f;
	public float BlastRadius = 2f;

	public override void Hit(Vector3 point, Collider other)
	{
		var explosion = (GameObject)Instantiate(ExplosionPrefab, point, Quaternion.identity);
		var hits = Physics.OverlapSphere(point, BlastRadius);
		var enemyHits = hits.Select(x => x.GetComponentInParent<Health>()).Distinct();

		foreach (var enemy in enemyHits)
		{
			if (enemy != null)
			{
				var dist = (point - enemy.transform.position).magnitude;
				var effect = 1 - (dist / BlastRadius);
				enemy.YaGotShot(Mathf.Max(0, effect) * DamageModifier);
			}
		}
		Destroy(explosion, 5f);
	}
}