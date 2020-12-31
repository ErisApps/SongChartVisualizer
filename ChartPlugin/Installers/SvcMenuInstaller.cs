using SongChartVisualizer.UI;
using SongChartVisualizer.UI.ViewControllers;
using Zenject;

namespace SongChartVisualizer.Installers
{
	internal class SvcMenuInstaller : Installer<SvcMenuInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<SettingsController>().AsSingle();
			Container.BindInterfacesAndSelfTo<SettingsControllerManager>().AsSingle();
		}
	}
}