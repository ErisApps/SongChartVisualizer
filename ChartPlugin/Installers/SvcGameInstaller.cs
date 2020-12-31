using SiraUtil;
using SongChartVisualizer.UI.ViewControllers;
using Zenject;

namespace SongChartVisualizer.Installers
{
	internal class SvcGameInstaller : Installer<SvcGameInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<ChartViewController>().FromNewComponentAsViewController().AsSingle();
		}
	}
}