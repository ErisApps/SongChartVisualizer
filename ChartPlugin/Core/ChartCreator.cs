using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BS_Utils.Gameplay;
using DigitalRuby.Tween;
using SongChartVisualizer.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SongChartVisualizer.Core
{
    public class ChartCreator : MonoBehaviour
    {
        public class NpsInfo
        {
            public float nps;
            public float fromTime;
            public float toTime;

            public NpsInfo(float nps, float fromTime, float toTime)
            {
                this.nps = nps;
                this.fromTime = fromTime;
                this.toTime = toTime;
            }
        }

        private LevelData _mainGameSceneSetupData;
        private BeatmapData _beatmapData;
        private AudioTimeSyncController _audioTimeSyncController;
        private List<NpsInfo> _npsSections;
        private WindowGraph _windowGraph;

        private AssetBundle _assetBundle;
        private bool _isInitialized;
        private bool _isFinished;
        private NpsInfo _currentSection;
        private int _currentSectionIdx;
        private GameObject _selfCursor;
        private int _hardestSectionIdx;
        private Canvas _canvas;
        private TextMeshProUGUI _text;

        private void Start()
        {
            _audioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
            _mainGameSceneSetupData = BS_Utils.Plugin.LevelData;
            _beatmapData = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.beatmapData;

            if (_beatmapData != null)
            {
                //Logger.log.Debug("There are " + _beatmapData.notesCount              + " notes");
                //Logger.log.Debug("There are " + _beatmapData.beatmapLinesData.Length + " lines");
                var songDuration = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.level?.beatmapLevelData?.audioClip?.length ?? -1f;
                if (songDuration >= 0)
                {
                    _npsSections = GetNpsSections();
                    for (var i = 0; i < _npsSections.Count; i++)
                    {
                        var npsInfos = _npsSections[i];
                        Logger.log.Debug($"Nps at section {i + 1}: {npsInfos.nps} (from [{npsInfos.fromTime}] to [{npsInfos.toTime}])");
                    }
                    Logger.log.Debug("Loading assetbundle..");
                    var assembly = Assembly.GetExecutingAssembly();
                    using (var stream = assembly.GetManifestResourceStream("SongChartVisualizer.UI.linegraph"))
                        _assetBundle = AssetBundle.LoadFromStream(stream);
                    if (!_assetBundle)
                        Logger.log.Warn("Failed to load AssetBundle! The chart may not work properly..");
                    else
                    {
                        var prefab = _assetBundle.LoadAsset<GameObject>("LineGraph");
                        var sprite = _assetBundle.LoadAsset<Sprite>("Circle");
                        var go = Instantiate(prefab, transform);
                        //go.transform.Find("Background").GetComponent<Image>().material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
                        //go.transform.Find("Background").GetComponent<Image>().enabled = true;
                        go.transform.Translate(0.04f, 0, 0);
                        _windowGraph = go.AddComponent<WindowGraph>();
                        _windowGraph.circleSprite = sprite;
                        _windowGraph.transform.localScale /= 10;
                        var npsValues = _npsSections.Select(info => info.nps).ToList();
                        _windowGraph.ShowGraph(npsValues, false);
                        _currentSectionIdx = 0;
                        _currentSection = _npsSections[_currentSectionIdx];
                        var highestValue = _npsSections.Max(info => info.nps);
                        _hardestSectionIdx = _npsSections.FindIndex(info => Math.Abs(info.nps - highestValue) < 0.001f);
                        PrepareWarningText();
                        CreateSelfCursor(Color.green);
                        _canvas.enabled = Plugin.config.Value.PeakWarning && _currentSectionIdx + 1 == _hardestSectionIdx;
                        if (_canvas.enabled)
                            FadeInText(_text, _currentSection.toTime * 0.2f);
                        _isInitialized = true;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _assetBundle.Unload(true);
        }

        private void PrepareWarningText()
        {
            _canvas = new GameObject("DiffWarningCanvas").AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.transform.position = new Vector3(0, 2.25f, 3.5f);
            _canvas.transform.localScale /= 100;
            var rectTransform = _canvas.transform as RectTransform;
            if (rectTransform != null)
                rectTransform.sizeDelta = new Vector2(140, 50);
            _text = Utils.CreateText((RectTransform)_canvas.transform,
                                     "",
                                     new Vector2());
            _text.alignment = TextAlignmentOptions.Center;
            _text.fontSize = 16f;
            _text.alpha = 0f;
            _canvas.enabled = false;
        }

        private static void FadeInText(TMP_Text text, float t)
        {
            text.gameObject.Tween("FadeInText" + text.gameObject.GetInstanceID(), 0, 1, t,
                                  TweenScaleFunctions.Linear, tween => { text.alpha = tween.CurrentValue; });
        }

        private void CreateSelfCursor(Color cursorColor)
        {
            _selfCursor = new GameObject("SelfCursor");
            _selfCursor.transform.SetParent(_windowGraph.GraphContainer, false);
            var image = _selfCursor.AddComponent<Image>();
            image.sprite = _windowGraph.circleSprite;
            image.useSpriteMesh = true;
            image.color = cursorColor;
            var rectTransform = _selfCursor.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(11, 11);
            var dotPos = _windowGraph.DotObjects[_currentSectionIdx].GetComponent<RectTransform>().position;
            dotPos.z -= 0.001f;
            _selfCursor.transform.position = dotPos;
        }

        private void Update()
        {
            if (_audioTimeSyncController && _isInitialized && !_isFinished)
            {
                if (_currentSection.toTime < _audioTimeSyncController.songTime)
                {
                    _currentSectionIdx += 1;
                    var oldState = _canvas.enabled;
                    _canvas.enabled = Plugin.config.Value.PeakWarning && _currentSectionIdx + 1 == _hardestSectionIdx;
                    if (_currentSectionIdx + 1 >= _npsSections.Count)
                    {
                        _isFinished = true;
                        return;
                    }
                    _currentSection = _npsSections[_currentSectionIdx];
                    if (Plugin.config.Value.PeakWarning && !oldState && _canvas && _canvas.enabled)
                        FadeInText(_text, (_currentSection.toTime - _audioTimeSyncController.songTime) * 0.2f);
                }
                var dotPos = Vector3.Lerp(_windowGraph.DotObjects[_currentSectionIdx].GetComponent<RectTransform>().position,
                                          _windowGraph.DotObjects[_currentSectionIdx + 1].GetComponent<RectTransform>().position,
                                          (_audioTimeSyncController.songTime - _currentSection.fromTime) / (_currentSection.toTime - _currentSection.fromTime));
                dotPos.z -= 0.001f;
                _selfCursor.transform.position = dotPos;
                if (Plugin.config.Value.PeakWarning && _canvas && _canvas.enabled)
                    _text.text = $"You're about to reach the peak difficulty in <color=#ffa500ff>{_currentSection.toTime - _audioTimeSyncController.songTime:F1}</color> seconds!";
            }
        }

        private List<NpsInfo> GetNpsSections()
        {
            var npsSections = new List<NpsInfo>();
            var songDuration = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.level?.beatmapLevelData?.audioClip?.length ?? -1f;
            if (!(songDuration >= 0)) return npsSections;
            var notes = new List<BeatmapObjectData>();

            notes = _beatmapData.beatmapLinesData.Aggregate(notes, (current, beatmapLineData) => current.Concat(beatmapLineData.beatmapObjectsData)
                                                                                                        .Where(data => data.beatmapObjectType == BeatmapObjectType.Note && ((NoteData)data).noteType != NoteType.Bomb).ToList());
            notes.Sort((s1, s2) => s1.time.CompareTo(s2.time));

            if (notes.Count > 0)
            {
                var tempNoteCount = 0;
                var startingTime = notes[0].time;
                npsSections.Add(new NpsInfo(0, 0, startingTime));
                for (var i = 0; i < notes.Count; ++i)
                {
                    tempNoteCount += 1;
                    if (i > 0 && (i % 25 == 0 || i + 1 == notes.Count))
                    {
                        var nps = tempNoteCount / (notes[i].time - startingTime);
                        if (!float.IsInfinity(nps))
                            npsSections.Add(new NpsInfo(nps, startingTime, notes[i].time));
                        tempNoteCount = 0;
                        startingTime = notes[i].time;
                    }
                }
                npsSections.Add(new NpsInfo(0, startingTime, songDuration));
            }

            return npsSections;
        }
    }
}