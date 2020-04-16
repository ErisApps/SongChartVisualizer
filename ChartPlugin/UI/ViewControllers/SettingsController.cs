using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using SongChartVisualizer.Models;
using UnityEngine;

namespace SongChartVisualizer.UI.ViewControllers
{
    public class SettingsController : PersistentSingleton<SettingsController>
    {
        [UIParams]
        private BSMLParserParams parserParams;

        private Float3 _stdPos = new Float3();
        private Float3 _stdRot = new Float3();
        private Float3 _noStdPos = new Float3();
        private Float3 _noStdRot = new Float3();

        [UIValue("enabled-bool")]
        public bool enabledValue
        {
            get => PluginConfig.Instance.EnablePlugin;
            set => PluginConfig.Instance.EnablePlugin = value;
        }

        [UIValue("peak-warning-bool")]
        public bool peakWarningValue
        {
            get => PluginConfig.Instance.PeakWarning;
            set => PluginConfig.Instance.PeakWarning = value;
        }

        [UIValue("std-panel-x-pos-float")]
        public float stdPanelXPosValue
        {
            get => PluginConfig.Instance.ChartStandardLevelPosition.x;
            set => _stdPos = new Float3(value, _stdPos.y, _stdPos.z);
        }

        [UIValue("std-panel-y-pos-float")]
        public float stdPanelYPosValue
        {
            get => PluginConfig.Instance.ChartStandardLevelPosition.y;
            set => _stdPos = new Float3(_stdPos.x, value, _stdPos.z);
        }

        [UIValue("std-panel-z-pos-float")]
        public float stdPanelZPosValue
        {
            get => PluginConfig.Instance.ChartStandardLevelPosition.z;
            set => _stdPos = new Float3(_stdPos.x, _stdPos.y, value);
        }

        [UIValue("std-panel-x-rot-float")]
        public float stdPanelXRotValue
        {
            get => PluginConfig.Instance.ChartStandardLevelRotation.x;
            set => _stdRot = new Float3(value, _stdRot.y, _stdRot.z);
        }

        [UIValue("std-panel-y-rot-float")]
        public float stdPanelYRotValue
        {
            get => PluginConfig.Instance.ChartStandardLevelRotation.y;
            set => _stdRot = new Float3(_stdRot.x, value, _stdRot.z);
        }

        [UIValue("std-panel-z-rot-float")]
        public float stdPanelZRotValue
        {
            get => PluginConfig.Instance.ChartStandardLevelRotation.z;
            set => _stdRot = new Float3(_stdRot.x, _stdRot.y, value);
        }

        [UIValue("no-std-panel-x-pos-float")]
        public float noStdPanelXPosValue
        {
            get => PluginConfig.Instance.Chart360LevelPosition.x;
            set => _noStdPos = new Float3(value, _noStdPos.y, _noStdPos.z);
        }

        [UIValue("no-std-panel-y-pos-float")]
        public float noStdPanelYPosValue
        {
            get => PluginConfig.Instance.Chart360LevelPosition.y;
            set => _noStdPos = new Float3(_noStdPos.x, value, _noStdPos.z);
        }

        [UIValue("no-std-panel-z-pos-float")]
        public float noStdPanelZPosValue
        {
            get => PluginConfig.Instance.Chart360LevelPosition.z;
            set => _noStdPos = new Float3(_noStdPos.x, _noStdPos.y, value);
        }

        [UIValue("no-std-panel-x-rot-float")]
        public float noStdPanelXRotValue
        {
            get => PluginConfig.Instance.Chart360LevelRotation.x;
            set => _noStdRot = new Float3(value, _noStdRot.y, _noStdRot.z);
        }

        [UIValue("no-std-panel-y-rot-float")]
        public float noStdPanelYRotValue
        {
            get => PluginConfig.Instance.Chart360LevelRotation.y;
            set => _noStdRot = new Float3(_noStdRot.x, value, _noStdRot.z);
        }

        [UIValue("no-std-panel-z-rot-float")]
        public float noStdPanelZRotValue
        {
            get => PluginConfig.Instance.Chart360LevelRotation.z;
            set => _noStdRot = new Float3(_noStdRot.x, _noStdRot.y, value);
        }

        [UIObject("std-pos-x-field")] public GameObject stdPosXField;
        [UIObject("std-pos-y-field")] public GameObject stdPosYField;
        [UIObject("std-pos-z-field")] public GameObject stdPosZField;
        [UIObject("std-rot-x-field")] public GameObject stdRotXField;
        [UIObject("std-rot-y-field")] public GameObject stdRotYField;
        [UIObject("std-rot-z-field")] public GameObject stdRotZField;
        [UIObject("no-std-pos-x-field")] public GameObject noStdPosXField;
        [UIObject("no-std-pos-y-field")] public GameObject noStdPosYField;
        [UIObject("no-std-pos-z-field")] public GameObject noStdPosZField;
        [UIObject("no-std-rot-x-field")] public GameObject noStdRotXField;
        [UIObject("no-std-rot-y-field")] public GameObject noStdRotYField;
        [UIObject("no-std-rot-z-field")] public GameObject noStdRotZField;

        private static void ResizeValuePicker(GameObject go)
        {
            if (go == null) return;
            var rectPicker = go.transform.Find("ValuePicker")?.GetComponent<RectTransform>();
            if (rectPicker)
                rectPicker.sizeDelta = new Vector2(25, rectPicker.sizeDelta.y);
        }

        [UIAction("#post-parse")]
        internal void Setup()
        {
            var list = new List<GameObject> {
                stdPosXField, stdPosYField, stdPosZField, stdRotXField, stdRotYField, stdRotZField,
                noStdPosXField, noStdPosYField, noStdPosZField, noStdRotXField, noStdRotYField, noStdRotZField
            };
            foreach (var go in list)
                ResizeValuePicker(go);
        }

        [UIAction("#apply")]
        public void OnApply()
        {
            PluginConfig.Instance.EnablePlugin = enabledValue;
            PluginConfig.Instance.PeakWarning = peakWarningValue;
            PluginConfig.Instance.ChartStandardLevelPosition = _stdPos;
            PluginConfig.Instance.ChartStandardLevelRotation = _stdRot;
            PluginConfig.Instance.Chart360LevelPosition = _noStdPos;
            PluginConfig.Instance.Chart360LevelRotation = _noStdRot;
        }
    }
}
