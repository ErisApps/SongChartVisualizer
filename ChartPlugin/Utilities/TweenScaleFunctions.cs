using System.Diagnostics.CodeAnalysis;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DigitalRuby.Tween
{
	/// <summary>
	/// Tween scale functions
	/// Implementations based on http://theinstructionlimit.com/flash-style-tweeneasing-functions-in-c, which are based on http://www.robertpenner.com/easing/
	/// </summary>
	public static class TweenScaleFunctions
	{
		private const float HALF_PI = Mathf.PI * 0.5f;

		/// <summary>
		/// A linear progress scale function.
		/// </summary>
		public static float Linear(float progress)
		{
			return progress;
		}

		/// <summary>
		/// A quadratic (x^2) progress scale function that eases in.
		/// </summary>
		public static float QuadraticEaseIn(float progress)
		{
			return EaseInPower(progress, 2);
		}

		/// <summary>
		/// A quadratic (x^2) progress scale function that eases out.
		/// </summary>
		public static float QuadraticEaseOut(float progress)
		{
			return EaseOutPower(progress, 2);
		}

		/// <summary>
		/// A quadratic (x^2) progress scale function that eases in and out.
		/// </summary>
		public static float QuadraticEaseInOut(float progress)
		{
			return EaseInOutPower(progress, 2);
		}

		/// <summary>
		/// A cubic (x^3) progress scale function that eases in.
		/// </summary>
		public static float CubicEaseIn(float progress)
		{
			return EaseInPower(progress, 3);
		}

		/// <summary>
		/// A cubic (x^3) progress scale function that eases out.
		/// </summary>
		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		private static float CubicEaseOut(float progress)
		{
			return EaseOutPower(progress, 3);
		}

		/// <summary>
		/// A cubic (x^3) progress scale function that eases in and out.
		/// </summary>
		public static float CubicEaseInOut(float progress)
		{
			return EaseInOutPower(progress, 3);
		}

		/// <summary>
		/// A quartic (x^4) progress scale function that eases in.
		/// </summary>
		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		private static float QuarticEaseIn(float progress)
		{
			return EaseInPower(progress, 4);
		}

		/// <summary>
		/// A quartic (x^4) progress scale function that eases out.
		/// </summary>
		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		public static float QuarticEaseOut(float progress)
		{
			return EaseOutPower(progress, 4);
		}

		/// <summary>
		/// A quartic (x^4) progress scale function that eases in and out.
		/// </summary>
		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		public static float QuarticEaseInOut(float progress)
		{
			return EaseInOutPower(progress, 4);
		}

		/// <summary>
		/// A quintic (x^5) progress scale function that eases in.
		/// </summary>
		public static float QuinticEaseIn(float progress)
		{
			return EaseInPower(progress, 5);
		}

		/// <summary>
		/// A quintic (x^5) progress scale function that eases out.
		/// </summary>
		public static float QuinticEaseOut(float progress)
		{
			return EaseOutPower(progress, 5);
		}

		/// <summary>
		/// A quintic (x^5) progress scale function that eases in and out.
		/// </summary>
		public static float QuinticEaseInOut(float progress)
		{
			return EaseInOutPower(progress, 5);
		}

		/// <summary>
		/// A sine progress scale function that eases in.
		/// </summary>
		public static float SineEaseIn(float progress)
		{
			return Mathf.Sin(progress * HALF_PI - HALF_PI) + 1;
		}

		/// <summary>
		/// A sine progress scale function that eases out.
		/// </summary>
		public static float SineEaseOut(float progress)
		{
			return Mathf.Sin(progress * HALF_PI);
		}

		/// <summary>
		/// A sine progress scale function that eases in and out.
		/// </summary>
		public static float SineEaseInOut(float progress)
		{
			return (Mathf.Sin(progress * Mathf.PI - HALF_PI) + 1) / 2;
		}

		private static float EaseInPower(float progress, int power)
		{
			return Mathf.Pow(progress, power);
		}

		private static float EaseOutPower(float progress, int power)
		{
			var sign = power % 2 == 0 ? -1 : 1;
			return (sign * (Mathf.Pow(progress - 1, power) + sign));
		}

		private static float EaseInOutPower(float progress, int power)
		{
			progress *= 2.0f;
			if (progress < 1)
			{
				return Mathf.Pow(progress, power) / 2.0f;
			}

			var sign = power % 2 == 0 ? -1 : 1;
			return (sign / 2.0f * (Mathf.Pow(progress - 2, power) + sign * 2));
		}
	}
}