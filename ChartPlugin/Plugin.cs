using System.Collections;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using IPA;
using IPA.Config;
using IPA.Utilities;
using SongChartVisualizer.Core;
using SongChartVisualizer.Models;
using SongChartVisualizer.UI.ViewControllers;
using SongChartVisualizer.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace SongChartVisualizer
{
    public class Plugin : IBeatSaberPlugin
    {
        internal static Ref<PluginConfig> config;
        internal static IConfigProvider   configProvider;

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            BSEvents.menuSceneLoadedFresh += OnMenuSceneLoadedFresh;
            BSEvents.gameSceneLoaded += OnGameSceneActive;
            configProvider = cfgProvider;

            config = cfgProvider.MakeLink<PluginConfig>((p, v) => {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }

        private static void OnMenuSceneLoadedFresh()
        {
            BSMLSettings.instance.AddSettingsMenu("<size=75%>Song Chart Visualizer</size>", "SongChartVisualizer.UI.Views.settings.bsml", SettingsController.instance);
        }

        private static IEnumerator ShowFloating()
        {
            yield return new WaitForEndOfFrame();
            var is360Level = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.beatmapData?.spawnRotationEventsCount > 0;
            var pos = is360Level ? Float3.ToVector3(config.Value.Chart360LevelPosition) : Float3.ToVector3(config.Value.ChartStandardLevelPosition);
            var rot = is360Level
                ? Quaternion.Euler(Float3.ToVector3(config.Value.Chart360LevelRotation))
                : Quaternion.Euler(Float3.ToVector3(config.Value.ChartStandardLevelRotation));
            var floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(105, 65), false, pos, rot);
            floatingScreen.SetRootViewController(BeatSaberUI.CreateViewController<ChartViewController>(), true);
            floatingScreen.GetComponent<Image>().enabled = false;
            floatingScreen.gameObject.AddComponent<ChartCreator>();
        }

        private static void OnGameSceneActive()
        {
            if (config.Value.EnablePlugin)
                new UnityTask(ShowFloating());
        }

        public void OnApplicationStart() { }

        public void OnApplicationQuit() { }

        public void OnFixedUpdate() { }

        public void OnUpdate() { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) { }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }

        public void OnSceneUnloaded(Scene scene) { }
    }
}