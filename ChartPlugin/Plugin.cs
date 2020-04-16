using System.Collections;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using IPA;
using IPA.Config.Stores;
using SongChartVisualizer.Core;
using SongChartVisualizer.Models;
using SongChartVisualizer.UI.ViewControllers;
using SongChartVisualizer.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Config = IPA.Config.Config;
using Logger = IPA.Logging.Logger;

namespace SongChartVisualizer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        #region Properties

        public static Logger Log { get; private set; }

        #endregion

        #region BSIPA Events

        [Init]
        public Plugin(Logger logger, Config conf)
        {
            Log = logger;
            PluginConfig.Instance = conf.Generated<PluginConfig>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            BSEvents.menuSceneLoadedFresh += OnMenuSceneLoadedFresh;
            BSEvents.gameSceneLoaded += OnGameSceneActive;
        }

        #endregion

        #region Events

        private static void OnMenuSceneLoadedFresh()
        {
            BSMLSettings.instance.AddSettingsMenu("<size=75%>Song Chart Visualizer</size>", "SongChartVisualizer.UI.Views.settings.bsml", SettingsController.instance);
        }

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