using SongChartVisualizer.UI.ViewControllers;
using Zenject;

namespace SongChartVisualizer.Installers
{
	internal class SvcGameInstaller : Installer<SvcGameInstaller>
	{
		private readonly PluginConfig _pluginConfig;
		private readonly GameplayCoreSceneSetupData _gameCoreSceneSetupData;
		private readonly AudioTimeSyncController _audioTimeSyncController;

		public SvcGameInstaller(PluginConfig pluginConfig, GameplayCoreSceneSetupData gameplayCoreSceneSetupData, AudioTimeSyncController audioTimeSyncController)
		{
			_pluginConfig = pluginConfig;
			_gameCoreSceneSetupData = gameplayCoreSceneSetupData;
			_audioTimeSyncController = audioTimeSyncController;
		}

		public override void InstallBindings()
		{
			if (!_pluginConfig.EnablePlugin || _gameCoreSceneSetupData.playerSpecificSettings.noTextsAndHuds || _gameCoreSceneSetupData.difficultyBeatmap?.beatmapData == null || _audioTimeSyncController.songLength < 0)
			{
				return;
			}

			Container.BindInterfacesAndSelfTo<ChartViewController>().FromNewComponentAsViewController().AsSingle();
		}
	}
}