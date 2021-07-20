/*
The MIT License (MIT)
Copyright (c) 2016 Digital Ruby, LLC
http://www.digitalruby.com
Created by Jeff Johnson

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DigitalRuby.Tween
{
	/// <summary>
	/// An implementation of a tween object.
	/// </summary>
	/// <typeparam name="T">The type to tween.</typeparam>
	internal class Tween<T> : ITween<T> where T : struct
	{
		private readonly Func<ITween<T>, T, T, float, T> _lerpFunc;

		private Func<float, float>? _scaleFunc;
		private Action<ITween<T>>? _progressCallback;
		private Action<ITween<T>>? _completionCallback;

		private ITween? _continueWith;

		/// <summary>
		/// The key that identifies this tween - can be null
		/// </summary>
		public object? Key { get; set; }

		/// <summary>
		/// Gets the current time of the tween.
		/// </summary>
		public float CurrentTime { get; private set; }

		/// <summary>
		/// Gets the duration of the tween.
		/// </summary>
		public float Duration { get; private set; }

		/// <summary>
		/// Delay before starting the tween
		/// </summary>
		public float Delay { get; set; }

		/// <summary>
		/// Gets the current state of the tween.
		/// </summary>
		public TweenState State { get; private set; }

		/// <summary>
		/// Gets the starting value of the tween.
		/// </summary>
		public T StartValue { get; private set; }

		/// <summary>
		/// Gets the ending value of the tween.
		/// </summary>
		public T EndValue { get; private set; }

		/// <summary>
		/// Gets the current value of the tween.
		/// </summary>
		public T CurrentValue { get; private set; }

		/// <summary>
		/// The game object - null if none
		/// </summary>
		public GameObject? GameObject { get; set; }

		/// <summary>
		/// The renderer - null if none
		/// </summary>
		public Renderer? Renderer { get; set; }

		/// <summary>
		/// Whether to force update even if renderer is null or not visible or deactivated, default is false
		/// </summary>
		public bool ForceUpdate { get; set; }

		/// <summary>
		/// Gets the current progress of the tween (0 - 1).
		/// </summary>
		public float CurrentProgress { get; private set; }

		/// <summary>
		/// Initializes a new Tween with a given lerp function.
		/// </summary>
		/// <remarks>
		/// C# generics are good but not good enough. We need a delegate to know how to
		/// interpolate between the start and end values for the given type.
		/// </remarks>
		/// <param name="lerpFunc">The interpolation function for the tween type.</param>
		public Tween(Func<ITween<T>, T, T, float, T> lerpFunc)
		{
			_lerpFunc = lerpFunc;
			State = TweenState.Stopped;
		}

		/// <summary>
		/// Initialize a tween.
		/// </summary>
		/// <param name="start">The start value.</param>
		/// <param name="end">The end value.</param>
		/// <param name="duration">The duration of the tween.</param>
		/// <param name="scaleFunc">A function used to scale progress over time.</param>
		/// <param name="progress">Progress callback</param>
		/// <param name="completion">Called when the tween completes</param>
		public Tween<T> Setup(T start, T end, float duration, Func<float, float>? scaleFunc, Action<ITween<T>>? progress = null, Action<ITween<T>>? completion = null)
		{
			scaleFunc ??= TweenScaleFunctions.Linear;
			CurrentTime = 0;
			Duration = duration;
			_scaleFunc = scaleFunc;
			_progressCallback = progress;
			_completionCallback = completion;
			StartValue = start;
			EndValue = end;

			return this;
		}

		/// <summary>
		/// Starts a tween. Setup must be called first.
		/// </summary>
		public void Start()
		{
			if (State != TweenState.Running)
			{
				if (Duration <= 0.0f && Delay <= 0.0f)
				{
					// complete immediately
					CurrentValue = EndValue;
					_progressCallback?.Invoke(this);

					_completionCallback?.Invoke(this);

					return;
				}

				State = TweenState.Running;
				UpdateValue();
			}
		}

		/// <summary>
		/// Pauses the tween.
		/// </summary>
		public void Pause()
		{
			if (State == TweenState.Running)
			{
				State = TweenState.Paused;
			}
		}

		/// <summary>
		/// Resumes the paused tween.
		/// </summary>
		public void Resume()
		{
			if (State == TweenState.Paused)
			{
				State = TweenState.Running;
			}
		}

		/// <summary>
		/// Stops the tween.
		/// </summary>
		/// <param name="stopBehavior">The behavior to use to handle the stop.</param>
		public void Stop(TweenStopBehavior stopBehavior)
		{
			if (State != TweenState.Stopped)
			{
				State = TweenState.Stopped;
				if (stopBehavior == TweenStopBehavior.Complete)
				{
					CurrentTime = Duration;
					UpdateValue();
					if (_completionCallback != null)
					{
						_completionCallback.Invoke(this);
						_completionCallback = null;
					}

					if (_continueWith != null)
					{
						_continueWith.Start();
						TweenFactory.AddTween(_continueWith);
						_continueWith = null;
					}
				}
			}
		}

		/// <summary>
		/// Updates the tween.
		/// </summary>
		/// <param name="elapsedTime">The elapsed time to add to the tween.</param>
		/// <returns>True if done, false if not</returns>
		public bool Update(float elapsedTime)
		{
			if (State == TweenState.Running)
			{
				if (Delay > 0.0f)
				{
					CurrentTime += elapsedTime;
					if (CurrentTime <= Delay)
					{
						// delay is not over yet
						return false;
					}

					// set to left-over time beyond delay
					CurrentTime = (CurrentTime - Delay);
					Delay = 0.0f;
				}
				else
				{
					CurrentTime += elapsedTime;
				}

				if (CurrentTime >= Duration)
				{
					Stop(TweenStopBehavior.Complete);
					return true;
				}

				UpdateValue();
				return false;
			}

			return (State == TweenState.Stopped);
		}

		/// <summary>
		/// Set another tween to execute when this tween finishes. Inherits the Key and if using Unity, GameObject, Renderer and ForceUpdate properties.
		/// </summary>
		/// <typeparam name="TNewTween">Type of new tween</typeparam>
		/// <param name="tween">New tween</param>
		/// <returns>New tween</returns>
		public Tween<TNewTween> ContinueWith<TNewTween>(Tween<TNewTween> tween) where TNewTween : struct
		{
			tween.Key = Key;

			tween.GameObject = GameObject;
			tween.Renderer = Renderer;
			tween.ForceUpdate = ForceUpdate;

			_continueWith = tween;
			return tween;
		}

		/// <summary>
		/// Helper that uses the current time, duration, and delegates to update the current value.
		/// </summary>
		private void UpdateValue()
		{
			if (Renderer == null || Renderer.isVisible || ForceUpdate)
			{
				CurrentProgress = _scaleFunc!(CurrentTime / Duration);
				CurrentValue = _lerpFunc(this, StartValue, EndValue, CurrentProgress);
				_progressCallback?.Invoke(this);
			}
		}
	}

	/// <summary>
	/// Object used to tween float values.
	/// </summary>
	internal class FloatTween : Tween<float>
	{
		private static float LerpFloat(ITween<float> t, float start, float end, float progress)
		{
			return start + (end - start) * progress;
		}

		private static readonly Func<ITween<float>, float, float, float, float> LerpFunc = LerpFloat;

		/// <summary>
		/// Initializes a new FloatTween instance.
		/// </summary>
		public FloatTween() : base(LerpFunc) { }
	}

	/// <summary>
	/// Object used to tween Vector2 values.
	/// </summary>
	internal class Vector2Tween : Tween<Vector2>
	{
		private static Vector2 LerpVector2(ITween<Vector2> t, Vector2 start, Vector2 end, float progress)
		{
			return Vector2.Lerp(start, end, progress);
		}

		private static readonly Func<ITween<Vector2>, Vector2, Vector2, float, Vector2> LerpFunc = LerpVector2;

		/// <summary>
		/// Initializes a new Vector2Tween instance.
		/// </summary>
		public Vector2Tween() : base(LerpFunc) { }
	}

	/// <summary>
	/// Object used to tween Vector3 values.
	/// </summary>
	internal class Vector3Tween : Tween<Vector3>
	{
		private static Vector3 LerpVector3(ITween<Vector3> t, Vector3 start, Vector3 end, float progress)
		{
			return Vector3.Lerp(start, end, progress);
		}

		private static readonly Func<ITween<Vector3>, Vector3, Vector3, float, Vector3> LerpFunc = LerpVector3;

		/// <summary>
		/// Initializes a new Vector3Tween instance.
		/// </summary>
		public Vector3Tween() : base(LerpFunc) { }
	}

	/// <summary>
	/// Object used to tween Vector4 values.
	/// </summary>
	internal class Vector4Tween : Tween<Vector4>
	{
		private static Vector4 LerpVector4(ITween<Vector4> t, Vector4 start, Vector4 end, float progress)
		{
			return Vector4.Lerp(start, end, progress);
		}

		private static readonly Func<ITween<Vector4>, Vector4, Vector4, float, Vector4> LerpFunc = LerpVector4;

		/// <summary>
		/// Initializes a new Vector4Tween instance.
		/// </summary>
		public Vector4Tween() : base(LerpFunc) { }
	}

	/// <summary>
	/// Object used to tween Color values.
	/// </summary>
	internal class ColorTween : Tween<Color>
	{
		private static Color LerpColor(ITween<Color> t, Color start, Color end, float progress)
		{
			return Color.Lerp(start, end, progress);
		}

		private static readonly Func<ITween<Color>, Color, Color, float, Color> LerpFunc = LerpColor;

		/// <summary>
		/// Initializes a new ColorTween instance.
		/// </summary>
		public ColorTween() : base(LerpFunc) { }
	}

	/// <summary>
	/// Object used to tween Quaternion values.
	/// </summary>
	internal class QuaternionTween : Tween<Quaternion>
	{
		private static Quaternion LerpQuaternion(ITween<Quaternion> t, Quaternion start, Quaternion end, float progress)
		{
			return Quaternion.Lerp(start, end, progress);
		}

		private static readonly Func<ITween<Quaternion>, Quaternion, Quaternion, float, Quaternion> LerpFunc = LerpQuaternion;

		/// <summary>
		/// Initializes a new QuaternionTween instance.
		/// </summary>
		public QuaternionTween() : base(LerpFunc) { }
	}
}