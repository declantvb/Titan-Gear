using UnityEngine;

public static class GameObjectExtensions
{
	public static void SetLayerRecursively(this GameObject thisObj, int layer)
	{
		thisObj.layer = layer;

		foreach (var child in thisObj.transform)
		{
			SetLayerRecursively(((Transform)child).gameObject, layer);
		}
	}
}
