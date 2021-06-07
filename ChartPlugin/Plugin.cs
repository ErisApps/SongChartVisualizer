using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using IPA.Logging;
using SiraUtil.Zenject;
using SongChartVisualizer.Installers;

namespace SongChartVisualizer
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		[Init]
		public Plugin(Logger logger, Config config, PluginMetadata metadata, Zenjector zenject)
		{
			zenject.OnApp<ScvAppInstaller>().WithParameters(logger, config.Generated<PluginConfig>(), metadata.Name);
			zenject.OnMenu<SvcMenuInstaller>();
			zenject.OnGame<SvcGameInstaller>(false).ShortCircuitForTutorial();
		}

		[OnEnable, OnDisable]
		public void OnStateChanged()
		{
			// Zenject is poggers
		}
	}
}