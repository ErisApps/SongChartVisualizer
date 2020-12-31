using System.Collections;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using SongChartVisualizer.Core;
using SongChartVisualizer.Installers;
using SongChartVisualizer.Models;
using SongChartVisualizer.UI.ViewControllers;
using SongChartVisualizer.Utilities;
using UnityEngine;
using UnityEngine.UI;
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

        #region Events

        private static void OnGameSceneActive()
        {
            if (PluginConfig.Instance.EnablePlugin)
                new UnityTask(ShowFloating());
        }

        #endregion

        #region Methods

        private static IEnumerator ShowFloating()
        {
            yield return new WaitForEndOfFrame();
            var is360Level = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.beatmapData?.spawnRotationEventsCount > 0;
            var pos = is360Level ? Float3.ToVector3(PluginConfig.Instance.Chart360LevelPosition) : Float3.ToVector3(PluginConfig.Instance.ChartStandardLevelPosition);
            var rot = is360Level
                ? Quaternion.Euler(Float3.ToVector3(PluginConfig.Instance.Chart360LevelRotation))
                : Quaternion.Euler(Float3.ToVector3(PluginConfig.Instance.ChartStandardLevelRotation));
            var floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(105, 65), false, pos, rot);
            floatingScreen.SetRootViewController(BeatSaberUI.CreateViewController<ChartViewController>(), true);
            floatingScreen.GetComponent<Image>().enabled = false;
            floatingScreen.gameObject.AddComponent<ChartCreator>();
        }

        #endregion
    }
}