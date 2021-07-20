using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DigitalRuby.Tween
{
	/// <summary>
	/// Tween manager - do not add directly as a script, instead call the static methods in your other scripts.
	/// </summary>
	internal class TweenFactory : MonoBehaviour
	{
		private static GameObject? _root;
		private static readonly List<ITween> Tweens = new List<ITween>();
		private static GameObject? _toDestroy;

		private static void EnsureCreated()
		{
			if (_root == null && Application.isPlaying)
			{
				_root = GameObject.Find("DigitalRubyTween");
				if (_root == null || _root.GetComponent<TweenFactory>() == null)
				{
					if (_root != null)
					{
						_toDestroy = _root;
					}

					_root = new GameObject {name = "DigitalRubyTween", hideFlags = HideFlags.HideAndDontSave};
					_root.AddComponent<TweenFactory>().hideFlags = HideFlags.HideAndDontSave;
				}

				if (Application.isPlaying)
				{
					DontDestroyOnLoad(_root);
				}
			}
		}

		private void Start()
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManagerSceneLoaded;
			if (_toDestroy != null)
			{
				Destroy(_toDestroy);
				_toDestroy = null;
			}
		}

		private void SceneManagerSceneLoaded(UnityEngine.SceneManagement.Scene s, UnityEngine.SceneManagement.LoadSceneMode m)
		{
			if (ClearTweensOnLevelLoad)
			{
				Tweens.Clear();
			}
		}

		private void Update()
		{
			for (var i = Tweens.Count - 1; i >= 0; i--)
			{
				ITween t = Tweens[i];
				if (t.Update(Time.deltaTime) && i < Tweens.Count && Tweens[i] == t)
				{
					Tweens.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Start and add a float tween
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>FloatTween</returns>
		public static FloatTween Tween(object key, float start, float end, float duration, Func<float, float>? scaleFunc, Action<ITween<float>>? progress,
			Action<ITween<float>>? completion = null)
		{
			FloatTween t = new FloatTween {Key = key};
			t.Setup(start, end, duration, scaleFunc, progress, completion);
			t.Start();
			AddTween(t);

			return t;
		}

		/// <summary>
		/// Start and add a Vector2 tween
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>Vector2Tween</returns>
		public static Vector2Tween Tween(object key, Vector2 start, Vector2 end, float duration, Func<float, float>? scaleFunc, Action<ITween<Vector2>>? progress,
			Action<ITween<Vector2>>? completion = null)
		{
			Vector2Tween t = new Vector2Tween {Key = key};
			t.Setup(start, end, duration, scaleFunc, progress, completion);
			t.Start();
			AddTween(t);

			return t;
		}

		/// <summary>
		/// Start and add a Vector3 tween
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>Vector3Tween</returns>
		public static Vector3Tween Tween(object key, Vector3 start, Vector3 end, float duration, Func<float, float>? scaleFunc, Action<ITween<Vector3>>? progress,
			Action<ITween<Vector3>>? completion = null)
		{
			Vector3Tween t = new Vector3Tween {Key = key};
			t.Setup(start, end, duration, scaleFunc, progress, completion);
			t.Start();
			AddTween(t);

			return t;
		}

		/// <summary>
		/// Start and add a Vector4 tween
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>Vector4Tween</returns>
		public static Vector4Tween Tween(object key, Vector4 start, Vector4 end, float duration, Func<float, float>? scaleFunc, Action<ITween<Vector4>>? progress,
			Action<ITween<Vector4>>? completion = null)
		{
			Vector4Tween t = new Vector4Tween {Key = key};
			t.Setup(start, end, duration, scaleFunc, progress, completion);
			t.Start();
			AddTween(t);

			return t;
		}

		/// <summary>
		/// Start and add a Color tween
		/// </summary>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>ColorTween</returns>
		public static ColorTween Tween(object key, Color start, Color end, float duration, Func<float, float>? scaleFunc, Action<ITween<Color>>? progress,
			Action<ITween<Color>>? completion = null)
		{
			ColorTween t = new ColorTween {Key = key};
			t.Setup(start, end, duration, scaleFunc, progress, completion);
			t.Start();
			AddTween(t);

			return t;
		}

		/// <summary>
		/// Start and add a Quaternion tween
		/// </summary>
		/// <param name="key">The key for the generated tween</param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="duration">Duration in seconds</param>
		/// <param name="scaleFunc">Scale function</param>
		/// <param name="progress">Progress handler</param>
		/// <param name="completion">Completion handler</param>
		/// <returns>QuaternionTween</returns>
		public static QuaternionTween Tween(object key, Quaternion start, Quaternion end, float duration, Func<float, float>? scaleFunc, Action<ITween<Quaternion>>? progress,
			Action<ITween<Quaternion>>? completion = null)
		{
			QuaternionTween t = new QuaternionTween {Key = key};
			t.Setup(start, end, duration, scaleFunc, progress, completion);
			t.Start();
			AddTween(t);

			return t;
		}

		/// <summary>
		/// Add a tween
		/// </summary>
		/// <param name="tween">Tween to add</param>
		public static void AddTween(ITween tween)
		{
			EnsureCreated();
			if (tween.Key != null)
			{
				RemoveTweenKey(tween.Key, AddKeyStopBehavior);
			}

			Tweens.Add(tween);
		}

		/// <summary>
		/// Remove a tween
		/// </summary>
		/// <param name="tween">Tween to remove</param>
		/// <param name="stopBehavior">Stop behavior</param>
		/// <returns>True if removed, false if not</returns>
		public static bool RemoveTween(ITween tween, TweenStopBehavior stopBehavior)
		{
			tween.Stop(stopBehavior);
			return Tweens.Remove(tween);
		}

		/// <summary>
		/// Remove a tween by key
		/// </summary>
		/// <param name="key">Key to remove</param>
		/// <param name="stopBehavior">Stop behavior</param>
		/// <returns>True if removed, false if not</returns>
		public static bool RemoveTweenKey(object? key, TweenStopBehavior stopBehavior)
		{
			if (key == null)
			{
				return false;
			}

			var foundOne = false;
			for (var i = Tweens.Count - 1; i >= 0; i--)
			{
				ITween t = Tweens[i];
				if (key.Equals(t.Key))
				{
					t.Stop(stopBehavior);
					Tweens.RemoveAt(i);
					foundOne = true;
				}
			}

			return foundOne;
		}

		/// <summary>
		/// Clear all tweens
		/// </summary>
		public static void Clear()
		{
			Tweens.Clear();
		}

		/// <summary>
		/// Stop behavior if you add a tween with a key and tweens already exist with the key
		/// </summary>
		public static TweenStopBehavior AddKeyStopBehavior = TweenStopBehavior.DoNotModify;

		/// <summary>
		/// Whether to clear tweens on level load, default is false
		/// </summary>
		public static bool ClearTweensOnLevelLoad;
	}
}