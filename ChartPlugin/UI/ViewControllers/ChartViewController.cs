using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using DigitalRuby.Tween;
using HMUI;
using SiraUtil.Tools;
using SongChartVisualizer.Core;
using SongChartVisualizer.Models;
using TMPro;
using UnityEngine;
using Zenject;

namespace SongChartVisualizer.UI.ViewControllers
{
	[HotReload(RelativePathToLayout = @"../Views/ChartView.bsml")]
	[ViewDefinition("SongChartVisualizer.UI.Views.ChartView.bsml")]
	internal class ChartViewController : BSMLAutomaticViewController, IInitializable, ITickable, IDisposable
	{
		private SiraLog _siraLog = null!;
		private PluginConfig _config = null!;
		private string _modName = null!;

		private AudioTimeSyncController _audioTimeSyncController = null!;
		private GameplayCoreSceneSetupData _gameplayCoreSceneSetupData = null!;

		private FloatingScreen _floatingScreen = null!;
		private WindowGraph _windowGraph = null!;

		private AssetBundle? _assetBundle;

		private BeatmapData? _beatmapData;
		private List<NpsInfo>? _npsSections;
		private int _currentSectionIdx;
		private NpsInfo? _currentSection;
		private GameObject _selfCursor = null!;

		private GameObject? _peakWarningGO;
		private int _hardestSectionIdx;
		private Canvas? _canvas;
		private TextMeshProUGUI? _text;

		private bool _isInitialized;
		private bool _isFinished;

		[Inject]
		internal void Construct(SiraLog siraLog, PluginConfig config, [Inject(Id = "scvModName")] string modName, AudioTimeSyncController audioTimeSyncController,
			GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
		{
			_siraLog = siraLog;
			_config = config;
			_modName = modName;
			_audioTimeSyncController = audioTimeSyncController;
			_gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
		}

		public void Initialize()
		{
			var is360Level = _gameplayCoreSceneSetupData.difficultyBeatmap?.beatmapData?.spawnRotationEventsCount > 0;
			var pos = is360Level ? _config.Chart360LevelPosition : _config.ChartStandardLevelPosition;
			var rot = is360Level
				? Quaternion.Euler(_config.Chart360LevelRotation)
				: Quaternion.Euler(_config.ChartStandardLevelRotation);
			_floatingScreen = FloatingScreen.CreateFloatingScreen(_config.ChartSize, false, pos, rot, curvatureRadius: 0f, hasBackground: false);
			_floatingScreen.SetRootViewController(this, AnimationType.None);
			_floatingScreen.name = _modName;
			name = $"{_modName} View";

			_beatmapData = _gameplayCoreSceneSetupData.difficultyBeatmap?.beatmapData;
			if (_beatmapData == null)
			{
				return;
			}

			// _siraLog.Debug($"There are {_beatmapData.beatmapObjectsData.Count(x => x.beatmapObjectType == BeatmapObjectType.Note)} notes");
			// _siraLog.Debug($"There are {_beatmapData.beatmapLinesData.Count} lines");
			var songDuration = _audioTimeSyncController.songLength;
			if (songDuration < 0)
			{
				return;
			}

			_npsSections = GetNpsSections();
			for (var i = 0; i < _npsSections.Count; i++)
			{
				var npsInfos = _npsSections[i];
				_siraLog.Debug($"Nps at section {i + 1}: {npsInfos.Nps} (from [{npsInfos.FromTime}] to [{npsInfos.ToTime}])");
			}

			_siraLog.Debug("Loading assetbundle..");
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream("SongChartVisualizer.UI.linegraph"))
			{
				_assetBundle = AssetBundle.LoadFromStream(stream);
			}

			if (!_assetBundle)
			{
				_siraLog.Warning("Failed to load AssetBundle! The chart will not work properly..");
			}
			else
			{
				var prefab = _assetBundle.LoadAsset<GameObject>("LineGraph");
				var sprite = _assetBundle.LoadAsset<Sprite>("Circle");
				var go = Instantiate(prefab, transform);

				go.transform.Translate(0.04f, 0, 0);
				_windowGraph = go.AddComponent<WindowGraph>();
				_windowGraph.circleSprite = sprite;
				_windowGraph.transform.localScale /= 10;
				var npsValues = _npsSections.Select(info => info.Nps).ToList();
				_windowGraph.ShowGraph(npsValues, false);

				_currentSectionIdx = 0;
				_currentSection = _npsSections[_currentSectionIdx];

				CreateSelfCursor(Color.green);

				if (_config.PeakWarning)
				{
					var highestValue = _npsSections.Max(info => info.Nps);
					_hardestSectionIdx = _npsSections.FindIndex(info => Math.Abs(info.Nps - highestValue) < 0.001f);
					PrepareWarningText();

					FadeInTextIfNeeded();
				}

				_isInitialized = true;
			}
		}

		public void Tick()
		{
			if (!_isInitialized || _isFinished)
			{
				return;
			}

			if (_currentSection!.ToTime < _audioTimeSyncController.songTime)
			{
				_currentSectionIdx++;

				if (_currentSectionIdx + 1 >= _npsSections!.Count)
				{
					_isFinished = true;
					return;
				}

				_currentSection = _npsSections[_currentSectionIdx];

				if (_config.PeakWarning)
				{
					FadeInTextIfNeeded();
				}
			}

			var dotPos = Vector3.Lerp(_windowGraph.DotObjects![_currentSectionIdx].GetComponent<RectTransform>().position,
				_windowGraph.DotObjects[_currentSectionIdx + 1].GetComponent<RectTransform>().position,
				(_audioTimeSyncController.songTime - _currentSection.FromTime) / (_currentSection.ToTime - _currentSection.FromTime));
			dotPos.z -= 0.001f;
			_selfCursor.transform.position = dotPos;

			if (_config.PeakWarning && _canvas!.enabled)
			{
				_text!.text = $"You're about to reach the peak difficulty in <color=#ffa500ff>{_currentSection.ToTime - _audioTimeSyncController.songTime:F1}</color> seconds!";
			}
		}

		public void Dispose()
		{
			if (_assetBundle != null)
			{
				_assetBundle.Unload(true);
			}
		}

		/// <remark>
		/// Make sure to call this method after the npsSections have been added to the windowGraph.
		/// </remark>
		/// <param name="cursorColor">The color of the cursor.</param>
		private void CreateSelfCursor(Color cursorColor)
		{
			_selfCursor = new GameObject("SelfCursor");
			_selfCursor.transform.SetParent(_windowGraph.GraphContainer, false);

			var image = _selfCursor.AddComponent<ImageView>();
			image.sprite = _windowGraph.circleSprite;
			image.useSpriteMesh = true;
			image.color = cursorColor;

			var rt = _selfCursor.GetComponent<RectTransform>();
			rt.sizeDelta = new Vector2(11, 11);

			var dotPos = _windowGraph.DotObjects![_currentSectionIdx].GetComponent<RectTransform>().position;
			dotPos.z -= 0.001f;

			_selfCursor.transform.position = dotPos;
		}

		private List<NpsInfo> GetNpsSections()
		{
			var npsSections = new List<NpsInfo>();

			var songDuration = _audioTimeSyncController.songLength;
			if (songDuration < 0)
			{
				return npsSections;
			}

			var notes = _beatmapData!.beatmapLinesData
				.SelectMany(beatmapLineData => beatmapLineData.beatmapObjectsData
					.Where(data => data.beatmapObjectType == BeatmapObjectType.Note && ((NoteData) data).colorType != ColorType.None))
				.OrderBy(s => s.time)
				.ToList();

			if (!notes.Any())
			{
				return npsSections;
			}

			var tempNoteCount = 0;
			var startingTime = notes[0].time;
			npsSections.Add(new NpsInfo(0, 0, startingTime));
			for (var i = 0; i < notes.Count; ++i)
			{
				tempNoteCount += 1;
				if (i <= 0 || (i % 25 != 0 && i + 1 != notes.Count))
				{
					continue;
				}

				var nps = tempNoteCount / (notes[i].time - startingTime);
				if (!float.IsInfinity(nps))
				{
					npsSections.Add(new NpsInfo(nps, startingTime, notes[i].time));
				}

				tempNoteCount = 0;
				startingTime = notes[i].time;
			}

			npsSections.Add(new NpsInfo(0, startingTime, songDuration));

			return npsSections;
		}

		private void PrepareWarningText()
		{
			_peakWarningGO = new GameObject("DiffWarningCanvas");
			_canvas = _peakWarningGO.AddComponent<Canvas>();
			_canvas.renderMode = RenderMode.WorldSpace;

			_peakWarningGO.AddComponent<CurvedCanvasSettings>().SetRadius(0f);

			var ct = _canvas.transform;
			ct.position = new Vector3(0, 2.25f, 3.5f);
			ct.localScale /= 100;

			if (ct is RectTransform crt)
			{
				crt.sizeDelta = new Vector2(140, 50);

				_text = BeatSaberUI.CreateText<TextMeshProUGUI>(crt, string.Empty, Vector2.zero);
				_text.alignment = TextAlignmentOptions.Center;
				_text.fontSize = 16f;
				_text.alpha = 0f;
			}

			_peakWarningGO.SetActive(false);
		}

		private void FadeInTextIfNeeded()
		{
			var oldState = _peakWarningGO!.activeSelf;
			var newState = _config.PeakWarning && _currentSectionIdx + 1 == _hardestSectionIdx;
			if (oldState == newState)
			{
				return;
			}

			_peakWarningGO.SetActive(newState);
			if (!oldState && newState)
			{
				FadeInText(_text!, (_currentSection!.ToTime - _audioTimeSyncController.songTime) * 0.2f);
			}
		}

		private static void FadeInText(TMP_Text text, float t)
		{
			text.gameObject.Tween("FadeInText" + text.gameObject.GetInstanceID(), 0, 1, t, TweenScaleFunctions.Linear, tween => { text.alpha = tween.CurrentValue; });
		}
	}
}