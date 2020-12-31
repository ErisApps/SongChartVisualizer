using IPA.Logging;
using SiraUtil;
using Zenject;

namespace SongChartVisualizer.Installers
{
	internal class ScvAppInstaller : Installer<Logger, PluginConfig, string, ScvAppInstaller>
	{
		private readonly Logger _logger;
		private readonly PluginConfig _config;
		private readonly string _name;

		public ScvAppInstaller(Logger logger, PluginConfig config, string name)
		{
			_logger = logger;
			_config = config;
			_name = name;
		}
		public override void InstallBindings()
		{
			Container.BindInstance(_name).WithId("svcModName");

			Container.BindLoggerAsSiraLogger(_logger);

			Container.BindInstance(_config).AsSingle();
		}
	}
}