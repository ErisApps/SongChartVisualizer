using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DigitalRuby.Tween
{
	/// <summary>
	/// Extensions for tween for game objects - unity only
	/// </summary>
	internal static class GameObjectTweenExtensions
	{
		/// <summary>
		/// Start and add a float tween
		/// </summary>
		/// <param name="obj">Game object</param>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>FloatTween</returns>
		public static FloatTween Tween(this GameObject obj, object key, float start, float end, float duration, Func<float, float> scaleFunc, Action<ITween<float>> progress,
			Action<ITween<float>>? completion = null)
		{
			FloatTween t = TweenFactory.Tween(key, start, end, duration, scaleFunc, progress, completion);
			t.GameObject = obj;
			t.Renderer = obj.GetComponent<Renderer>();
			return t;
		}

		/// <summary>
		/// Start and add a Vector2 tween
		/// </summary>
		/// <param name="obj">Game object</param>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>Vector2Tween</returns>
		public static Vector2Tween Tween(this GameObject obj, object key, Vector2 start, Vector2 end, float duration, Func<float, float> scaleFunc, Action<ITween<Vector2>> progress,
			Action<ITween<Vector2>>? completion = null)
		{
			Vector2Tween t = TweenFactory.Tween(key, start, end, duration, scaleFunc, progress, completion);
			t.GameObject = obj;
			t.Renderer = obj.GetComponent<Renderer>();
			return t;
		}

		/// <summary>
		/// Start and add a Vector3 tween
		/// </summary>
		/// <param name="obj">Game object</param>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>Vector3Tween</returns>
		public static Vector3Tween Tween(this GameObject obj, object key, Vector3 start, Vector3 end, float duration, Func<float, float> scaleFunc, Action<ITween<Vector3>> progress,
			Action<ITween<Vector3>>? completion = null)
		{
			Vector3Tween t = TweenFactory.Tween(key, start, end, duration, scaleFunc, progress, completion);
			t.GameObject = obj;
			t.Renderer = obj.GetComponent<Renderer>();
			return t;
		}

		/// <summary>
		/// Start and add a Vector4 tween
		/// </summary>
		/// <param name="obj">Game object</param>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>Vector4Tween</returns>
		public static Vector4Tween Tween(this GameObject obj, object key, Vector4 start, Vector4 end, float duration, Func<float, float> scaleFunc, Action<ITween<Vector4>> progress,
			Action<ITween<Vector4>>? completion = null)
		{
			Vector4Tween t = TweenFactory.Tween(key, start, end, duration, scaleFunc, progress, completion);
			t.GameObject = obj;
			t.Renderer = obj.GetComponent<Renderer>();
			return t;
		}

		/// <summary>
		/// Start and add a Color tween
		/// </summary>
		/// <param name="obj">Game object</param>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>ColorTween</returns>
		public static ColorTween Tween(this GameObject obj, object key, Color start, Color end, float duration, Func<float, float>? scaleFunc, Action<ITween<Color>>? progress,
			Action<ITween<Color>>? completion = null)
		{
			ColorTween t = TweenFactory.Tween(key, start, end, duration, scaleFunc, progress, completion);
			t.GameObject = obj;
			t.Renderer = obj.GetComponent<Renderer>();
			return t;
		}

		/// <summary>
		/// Start and add a Quaternion tween
		/// </summary>
		/// <param name="obj">Game object</param>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>QuaternionTween</returns>
		public static QuaternionTween Tween(this GameObject obj, object key, Quaternion start, Quaternion end, float duration, Func<float, float> scaleFunc, Action<ITween<Quaternion>> progress,
			Action<ITween<Quaternion>>? completion = null)
		{
			QuaternionTween t = TweenFactory.Tween(key, start, end, duration, scaleFunc, progress, completion);
			t.GameObject = obj;
			t.Renderer = obj.GetComponent<Renderer>();
			return t;
		}
	}
}