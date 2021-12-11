using SongChartVisualizer.UI.ViewControllers;
using Zenject;

namespace SongChartVisualizer.Installers
{
	internal class SvcGameInstaller : Installer<SvcGameInstaller>
	{
		private readonly PluginConfig _pluginConfig;
		private readonly GameplayCoreSceneSetupData _gameCoreSceneSetupData;

		public SvcGameInstaller(PluginConfig pluginConfig, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
		{
			_pluginConfig = pluginConfig;
			_gameCoreSceneSetupData = gameplayCoreSceneSetupData;
		}

		public override void InstallBindings()
		{
			if (!_pluginConfig.EnablePlugin || _gameCoreSceneSetupData.playerSpecificSettings.noTextsAndHuds)
			{
				return;
			}

			Container.BindInterfacesAndSelfTo<ChartViewController>().FromNewComponentAsViewController().AsSingle();
		}
	}
}