using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class IEnumerableExtensions
{
	public static T MaxBy<T>(this IEnumerable<T> obj, Func<T, float> maxFunc) where T : class
	{
		T maxObj = null;
		float maxVal = float.MinValue;

		foreach (var item in obj)
		{
			var value = maxFunc(item);

			if (maxObj == null || value > maxVal)
			{
				maxVal = value;
				maxObj = item;
			}
		}

		return maxObj;
	}

	public static T MinBy<T>(this IEnumerable<T> obj, Func<T, float> minFunc) where T : class
	{
		T maxObj = null;
		float maxVal = float.MaxValue;

		foreach (var item in obj)
		{
			var value = minFunc(item);

			if (maxObj == null || value < maxVal)
			{
				maxVal = value;
				maxObj = item;
			}
		}

		return maxObj;
	}
}
