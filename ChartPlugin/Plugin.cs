using System.Reflection;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using SongChartVisualizer.Installers;
using Config = IPA.Config.Config;
using Logger = IPA.Logging.Logger;

namespace SongChartVisualizer
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
	    [Init]
        public Plugin(Logger logger, Config config, PluginMetadata metadata, Zenjector zenject)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();

            zenject.OnApp<ScvAppInstaller>().WithParameters(logger, config.Generated<PluginConfig>(), metadata.Name ?? Assembly.GetExecutingAssembly().GetName().Name);
            zenject.OnMenu<SvcMenuInstaller>();
        }

        [OnEnable, OnDisable]
        public void OnStateChanged()
        {
	        // Zenject is poggers
        }
    }
}