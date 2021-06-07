﻿// ReSharper disable once CheckNamespace

namespace DigitalRuby.Tween
{
	/// <summary>
	/// State of an ITween object
	/// </summary>
	internal enum TweenState
	{
		/// <summary>
		/// The tween is running.
		/// </summary>
		Running,

		/// <summary>
		/// The tween is paused.
		/// </summary>
		Paused,

		/// <summary>
		/// The tween is stopped.
		/// </summary>
		Stopped
	}
}