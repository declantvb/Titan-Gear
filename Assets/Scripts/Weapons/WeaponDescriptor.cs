using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class WeaponDescriptor
{
	public WeaponStyle Style;
	public float Cooldown;
	public float InitialBulletVelocity;
	public GameObject ProjectilePrefab;
	public float Duration;
}

public enum WeaponStyle
{
	Projectile,
	ProjectileStraight,
	Pulse,
	Hitscan,
	Laser
}
