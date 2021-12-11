using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SongChartVisualizer
{
	internal class PluginConfig
	{
		public virtual bool EnablePlugin { get; set; } = true;
		public virtual bool PeakWarning { get; set; } = true;

		[Ignore]
		public virtual Vector3 ChartSize { get; } = new Vector2(105, 65);

		[UseConverter]
		public virtual Vector3 ChartStandardLevelPosition { get; set; } = new Vector3(0, -0.4f, 2.25f);

		[UseConverter]
		public virtual Vector3 ChartStandardLevelRotation { get; set; } = new Vector3(35, 0, 0);

		[UseConverter]
		public virtual Vector3 Chart360LevelPosition { get; set; } = new Vector3(0, 3.5f, 3);

		[UseConverter]
		public virtual Vector3 Chart360LevelRotation { get; set; } = new Vector3(-30, 0, 0);

		public virtual bool HasBackground { get; set; } = false;
		public virtual float BackgroundOpacity { get; set; } = .05f;

		[UseConverter(typeof(HexColorConverter))]
		public virtual Color BackgroundColor { get; set; } = Color.blue;

		[Ignore]
		public Color CombinedBackgroundColor => new Color(BackgroundColor.r, BackgroundColor.b, BackgroundColor.b, BackgroundOpacity);

		[UseConverter(typeof(HexColorConverter))]
		public Color LineColor { get; set; } = Color.white;

		[UseConverter(typeof(HexColorConverter))]
		public Color PointerColor { get; set; } = Color.green;

		public virtual IDisposable ChangeTransaction() => null!;
	}
}