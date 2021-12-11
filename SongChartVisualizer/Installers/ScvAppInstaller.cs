using SongChartVisualizer.Services;
using Zenject;

namespace SongChartVisualizer.Installers
{
	internal class ScvAppInstaller : Installer<PluginConfig, ScvAppInstaller>
	{
		private readonly PluginConfig _config;

		public ScvAppInstaller(PluginConfig config)
		{
			_config = config;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(_config).AsSingle();
			Container.BindInterfacesAndSelfTo<ScvAssetLoader>().AsSingle().Lazy();
		}
	}
}