using UnityEngine;
using System.Collections;
using System;

public class Projectile : Bullet
{
	public GameObject ExplosionPrefab;
	public float DamageModifier = 50f;
	public float ExplosionDist = 2f;

	public override void DoDamage(Collider other)
	{
		var explosion = (GameObject)Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
		//var hits = Physics.OverlapSphere(transform.position, ExplosionDist);
		//foreach (var col in hits)
		//{
		//	var enemy = col.GetComponentInParent<EnemyBase>();
		//	if (enemy != null)
		//	{
		//		var dist = (transform.position - enemy.transform.position).magnitude;
		//		var damage = 1 / (dist + 0.5f) * DamageModifier;
		//		enemy.YaGotShot(damage);
		//	}
		//}
		Destroy(explosion, 5f);
	}
}