using UnityEngine;

public static class TimeExtensions
{
	public static float deltaTimeHours { get { return Time.deltaTime / 3600f; } }
}