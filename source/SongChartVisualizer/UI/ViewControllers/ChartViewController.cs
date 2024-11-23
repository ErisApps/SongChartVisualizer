using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using SiraUtil.Logging;
using SongChartVisualizer.Core;
using SongChartVisualizer.Models;
using SongChartVisualizer.Services;
using TMPro;
using Tweening;
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
		private ScvAssetLoader _assetLoader = null!;

		private AudioTimeSyncController _audioTimeSyncController = null!;
		private IReadonlyBeatmapData _beatmapData = null!;
		private BeatmapKey _beatmapKey;
		private TimeTweeningManager _timeTweeningManager = null!;

		private FloatingScreen _floatingScreen = null!;
		private WindowGraph _windowGraph = null!;

		private AssetBundle? _assetBundle;

		private List<NpsInfo>? _npsSections;
		private int _currentSectionIdx;
		private NpsInfo? _currentSection;
		private GameObject _selfCursor = null!;

		private GameObject? _peakWarningGo;
		private int _hardestSectionIdx;
		private TextMeshProUGUI? _text;
		private float _timeTillPeak;

		private bool _shouldNotRunTick;

		[Inject]
		internal void Construct(SiraLog siraLog, PluginConfig config, ScvAssetLoader assetLoader, AudioTimeSyncController audioTimeSyncController,
			IReadonlyBeatmapData beatmap, BeatmapKey beatmapKey, TimeTweeningManager timeTweeningManager)
		{
			_timeTweeningManager = timeTweeningManager;
			_assetLoader = assetLoader;
			_siraLog = siraLog;
			_config = config;
			_audioTimeSyncController = audioTimeSyncController;
			_beatmapData = beatmap;
			_beatmapKey = beatmapKey;

			name = $"{nameof(SongChartVisualizer)} View";
		}

		public void Initialize()
		{
			var is360Level = _beatmapKey.beatmapCharacteristic.requires360Movement;
			var pos = is360Level ? _config.Chart360LevelPosition : _config.ChartStandardLevelPosition;
			var rot = is360Level
				? Quaternion.Euler(_config.Chart360LevelRotation)
				: Quaternion.Euler(_config.ChartStandardLevelRotation);
			_floatingScreen = FloatingScreen.CreateFloatingScreen(_config.ChartSize, false, pos, rot, curvatureRadius: 0f, hasBackground: _config.HasBackground);
			_floatingScreen.SetRootViewController(this, AnimationType.None);
			_floatingScreen.name = nameof(SongChartVisualizer);

			if (_config.HasBackground)
			{
				var imageView = _floatingScreen.GetComponentInChildren<ImageView>();
				imageView.material = _assetLoader.UINoGlowMaterial;
				imageView.color = _config.CombinedBackgroundColor;

				transform.SetParent(imageView.transform);
			}

			if (_audioTimeSyncController.songLength < 0)
			{
				_shouldNotRunTick = true;
				return;
			}

			// _siraLog.Debug($"There are {_beatmapData.beatmapObjectsData.Count(x => x.beatmapObjectType == BeatmapObjectType.Note)} notes");
			// _siraLog.Debug($"There are {_beatmapData.beatmapLinesData.Count} lines");

			_npsSections = GetNpsSections(_beatmapData);
#if DEBUG
			for (var i = 0; i < _npsSections.Count; i++)
			{
				var npsInfos = _npsSections[i];
				_siraLog.Debug($"Nps at section {i + 1}: {npsInfos.Nps} (from [{npsInfos.FromTime}] to [{npsInfos.ToTime}])");
			}
#endif

			_siraLog.Debug("Loading assetbundle..");
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream("SongChartVisualizer.UI.linegraph"))
			{
				_assetBundle = AssetBundle.LoadFromStream(stream);
			}

			if (!_assetBundle)
			{
				_siraLog.Warn("Failed to load AssetBundle! The chart will not work properly..");
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
				_windowGraph.ShowGraph(npsValues, false, linkColor: _config.LineColor);

				_currentSectionIdx = 0;
				_currentSection = _npsSections[_currentSectionIdx];

				CreateSelfCursor(_config.PointerColor);

				if (_config.PeakWarning)
				{
					var highestValue = _npsSections.Max(info => info.Nps);
					_hardestSectionIdx = _npsSections.FindIndex(info => Math.Abs(info.Nps - highestValue) < 0.001f);
					PrepareWarningText();

					FadeInTextIfNeeded();
				}
			}
		}

		public void Tick()
		{
			if (_shouldNotRunTick)
			{
				return;
			}

			if (_audioTimeSyncController.songTime > _currentSection!.ToTime)
			{
				_currentSectionIdx++;

				if (_currentSectionIdx + 1 >= _npsSections!.Count)
				{
					_shouldNotRunTick = true;
					return;
				}

				_currentSection = _npsSections[_currentSectionIdx];

				if (_config.PeakWarning)
				{
					FadeInTextIfNeeded();
				}
			}

			var dotPos = Vector3.Lerp(_windowGraph.DotObjects[_currentSectionIdx].GetComponent<RectTransform>().position,
				_windowGraph.DotObjects[_currentSectionIdx + 1].GetComponent<RectTransform>().position,
				(_audioTimeSyncController.songTime - _currentSection.FromTime) / (_currentSection.ToTime - _currentSection.FromTime));
			dotPos.z -= 0.001f;
			_selfCursor.transform.position = dotPos;

			if (_config.PeakWarning && _peakWarningGo!.activeSelf)
			{
				var timeTillPeakLocal = _currentSection.ToTime - _audioTimeSyncController.songTime;
				if (_timeTillPeak - timeTillPeakLocal < 0.05f)
				{
					return;
				}

				_timeTillPeak = timeTillPeakLocal;

				_text!.text = $"You're about to reach the peak difficulty in <color=#ffa500ff>{_timeTillPeak:F1}</color> seconds!";
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

			var dotPos = _windowGraph.DotObjects[_currentSectionIdx].GetComponent<RectTransform>().position;
			dotPos.z -= 0.001f;

			_selfCursor.transform.position = dotPos;
		}

		// ReSharper disable once CognitiveComplexity
		private List<NpsInfo> GetNpsSections(IReadonlyBeatmapData beatmapData)
		{
			var npsSections = new List<NpsInfo>();

			var songDuration = _audioTimeSyncController.songLength;
			if (songDuration < 0)
			{
				return npsSections;
			}

			var notes = beatmapData.GetBeatmapDataItems<NoteData>(0)
				.Where(noteData => noteData.gameplayType != NoteData.GameplayType.Bomb)
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

				float nps;
				if (tempNoteCount >= 25)
				{
					nps = tempNoteCount / (notes[i].time - startingTime);
				}
				else // end of a map or a map with notes.Count < 25
				{
					// if total notes count < 25 - do the usual way
					// if there are more than 25 notes - try to normalize nps with data from tempNoteCount and (25 - tempNoteCount) notes from a section before
					nps = notes.Count < 25
						? tempNoteCount / (notes[i].time - notes[0].time)
						: 25 / (notes[i].time - notes[i - 25].time);
				}

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
			_peakWarningGo = new GameObject("DiffWarningCanvas");
			var canvas = _peakWarningGo.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.WorldSpace;

			_peakWarningGo.AddComponent<CurvedCanvasSettings>().SetRadius(0f);

			var ct = canvas.transform;
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

			_peakWarningGo.SetActive(false);

			_timeTillPeak = _audioTimeSyncController.songLength;
		}

		private void FadeInTextIfNeeded()
		{
			var oldState = _peakWarningGo!.activeSelf;
			var newState = _currentSectionIdx + 1 == _hardestSectionIdx;
			if (oldState == newState)
			{
				return;
			}

			_peakWarningGo.SetActive(newState);
			if (!oldState && newState)
			{
				FadeInText(_text!, (_currentSection!.ToTime - _audioTimeSyncController.songTime) * 0.2f);
			}
		}

		private void FadeInText(TMP_Text text, float t)
		{
			_timeTweeningManager.AddTween(new FloatTween(0, 1, value => text.alpha = value, t, EaseType.Linear), text);
		}
	}
}