using UnityEngine;

public static class VectorExtensions
{
	public static Vector2 AsVector2(this Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	public static Vector3 ToFlatVector3(this Vector2 v)
	{
		return new Vector3(v.x, 0, v.y);
	}
}