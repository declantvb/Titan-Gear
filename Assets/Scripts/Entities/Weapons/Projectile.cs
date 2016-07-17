using System.Linq;
using UnityEngine;

public class Projectile : Bullet
{
	public GameObject ExplosionPrefab;
	public float DamageModifier = 50f;
	public float BlastRadius = 2f;

	public override void DoDamage(Collider other)
	{
		var explosion = (GameObject)Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
		var hits = Physics.OverlapSphere(transform.position, BlastRadius);
		var enemyHits = hits.Select(x => x.GetComponentInParent<Health>()).Distinct();

		foreach (var enemy in enemyHits)
		{
			if (enemy != null)
			{
				var dist = (transform.position - enemy.transform.position).magnitude;
				var effect = 1 - (dist / BlastRadius);
				print(effect);
				enemy.YaGotShot(Mathf.Max(0, effect) * DamageModifier);
			}
		}
		Destroy(explosion, 5f);
	}
}