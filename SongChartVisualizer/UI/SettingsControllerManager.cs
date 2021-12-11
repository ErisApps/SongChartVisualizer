using System;
using BeatSaberMarkupLanguage.Settings;
using IPA.Loader;
using SiraUtil.Zenject;
using SongChartVisualizer.UI.ViewControllers;
using Zenject;

namespace SongChartVisualizer.UI
{
	internal class SettingsControllerManager : IInitializable, IDisposable
	{
		private readonly string _name;
		private SettingsController? _settingsHost;

		public SettingsControllerManager(SettingsController settingsHost, UBinder<Plugin, PluginMetadata> pluginMetadata)
		{
			_settingsHost = settingsHost;
			_name = pluginMetadata.Value.Name;
		}

		public void Initialize()
		{
			BSMLSettings.instance.AddSettingsMenu($"<size=75%>{_name}</size>", "SongChartVisualizer.UI.Views.settings.bsml", _settingsHost);
		}

		public void Dispose()
		{
			if (_settingsHost == null)
			{
				return;
			}

			BSMLSettings.instance.RemoveSettingsMenu(_settingsHost);
			_settingsHost = null!;
		}
	}
}