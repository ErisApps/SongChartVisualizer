using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using SiraUtil.Converters;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SongChartVisualizer
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public bool EnablePlugin { get; set; } = true;
        public bool PeakWarning { get; set; } = true;

        [Ignore]
        public Vector3 ChartSize { get; } = new Vector2(105, 65);

        [UseConverter(typeof(Vector3Converter))]
        public Vector3 ChartStandardLevelPosition { get; set; } = new Vector3(0, -0.4f, 2.25f);

        [UseConverter(typeof(Vector3Converter))]
        public Vector3 ChartStandardLevelRotation { get; set; } = new Vector3(35, 0, 0);

        [UseConverter(typeof(Vector3Converter))]
        public Vector3 Chart360LevelPosition { get; set; } = new Vector3(0, 3.5f, 3);

        [UseConverter(typeof(Vector3Converter))]
        public Vector3 Chart360LevelRotation { get; set; } = new Vector3(-30, 0, 0);
    }
}
