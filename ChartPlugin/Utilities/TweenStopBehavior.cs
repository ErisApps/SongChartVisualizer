// ReSharper disable once CheckNamespace

namespace DigitalRuby.Tween
{
	/// <summary>
	/// The behavior to use when manually stopping a tween.
	/// </summary>
	public enum TweenStopBehavior
	{
		/// <summary>
		/// Does not change the current value.
		/// </summary>
		DoNotModify,

		/// <summary>
		/// Causes the tween to progress to the end value immediately.
		/// </summary>
		Complete
	}
}