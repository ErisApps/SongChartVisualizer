using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using SiraUtil.Zenject;
using SongChartVisualizer.Installers;

namespace SongChartVisualizer
{
	[Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
	public class Plugin
	{
		[Init]
		public Plugin(Logger logger, Config config, Zenjector zenject)
		{
			zenject.UseLogger(logger);
			zenject.UseMetadataBinder<Plugin>();

			zenject.Install<ScvAppInstaller>(Location.App,config.Generated<PluginConfig>());
			zenject.Install<SvcMenuInstaller>(Location.Menu);
			zenject.Install<SvcGameInstaller>(Location.StandardPlayer | Location.MultiPlayer);
		}
	}
}