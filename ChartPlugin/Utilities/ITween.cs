using System;

// ReSharper disable once CheckNamespace
namespace DigitalRuby.Tween
{
	/// <summary>
	/// Interface for a tween object.
	/// </summary>
	public interface ITween
	{
		/// <summary>
		/// The key that identifies this tween - can be null
		/// </summary>
		object? Key { get; }

		/// <summary>
		/// Gets the current state of the tween.
		/// </summary>
		TweenState State { get; }

		/// <summary>
		/// Start the tween.
		/// </summary>
		void Start();

		/// <summary>
		/// Pauses the tween.
		/// </summary>
		void Pause();

		/// <summary>
		/// Resumes the paused tween.
		/// </summary>
		void Resume();

		/// <summary>
		/// Stops the tween.
		/// </summary>
		/// <param name="stopBehavior">The behavior to use to handle the stop.</param>
		void Stop(TweenStopBehavior stopBehavior);

		/// <summary>
		/// Updates the tween.
		/// </summary>
		/// <param name="elapsedTime">The elapsed time to add to the tween.</param>
		/// <returns>True if done, false if not</returns>
		bool Update(float elapsedTime);
	}

	/// <summary>
	/// Interface for a tween object that handles a specific type.
	/// </summary>
	/// <typeparam name="T">The type to tween.</typeparam>
	public interface ITween<T> : ITween where T : struct
	{
		/// <summary>
		/// Gets the current value of the tween.
		/// </summary>
		T CurrentValue { get; }

		/// <summary>
		/// Gets the current progress of the tween.
		/// </summary>
		float CurrentProgress { get; }

		/// <summary>
		/// Initialize a tween.
		/// </summary>
		/// <param name="start">The start value.</param>
		/// <param name="end">The end value.</param>
		/// <param name="duration">The duration of the tween.</param>
		/// <param name="scaleFunc">A function used to scale progress over time.</param>
		/// <param name="progress">Progress callback</param>
		/// <param name="completion">Called when the tween completes</param>
		Tween<T> Setup(T start, T end, float duration, Func<float, float> scaleFunc, Action<ITween<T>> progress, Action<ITween<T>>? completion = null);
	}
}