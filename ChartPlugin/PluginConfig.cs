using SongChartVisualizer.Models;

namespace SongChartVisualizer
{
    internal class PluginConfig
    {
        public bool RegenerateConfig = true;
        public bool EnablePlugin = true;
        public bool PeakWarning = true;
        public Float3 ChartStandardLevelPosition = new Float3(0, -0.4f, 2.25f);
        public Float3 ChartStandardLevelRotation = new Float3(35, 0, 0);
        public Float3 Chart360LevelPosition = new Float3(0, 3.5f, 3);
        public Float3 Chart360LevelRotation = new Float3(-30, 0, 0);
    }
}
