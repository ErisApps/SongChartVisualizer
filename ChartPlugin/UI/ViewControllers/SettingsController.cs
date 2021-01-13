using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine;

namespace SongChartVisualizer.UI.ViewControllers
{
	internal class SettingsController
	{
		private readonly PluginConfig _configuration;

		internal SettingsController(PluginConfig configuration)
		{
			_configuration = configuration;
		}

		private Vector3 _stdPos;
		private Vector3 _stdRot;
		private Vector3 _noStdPos;
		private Vector3 _noStdRot;

		[UIValue("enabled-bool")]
		internal bool EnabledValue
		{
			get => _configuration.EnablePlugin;
			set => _configuration.EnablePlugin = value;
		}

		[UIValue("peak-warning-bool")]
		internal bool PeakWarningValue
		{
			get => _configuration.PeakWarning;
			set => _configuration.PeakWarning = value;
		}

		[UIValue("std-panel-x-pos-float")]
		internal float StdPanelXPosValue
		{
			get => _configuration.ChartStandardLevelPosition.x;
			set => _stdPos = new Vector3(value, _stdPos.y, _stdPos.z);
		}

		[UIValue("std-panel-y-pos-float")]
		internal float StdPanelYPosValue
		{
			get => _configuration.ChartStandardLevelPosition.y;
			set => _stdPos = new Vector3(_stdPos.x, value, _stdPos.z);
		}

		[UIValue("std-panel-z-pos-float")]
		internal float StdPanelZPosValue
		{
			get => _configuration.ChartStandardLevelPosition.z;
			set => _stdPos = new Vector3(_stdPos.x, _stdPos.y, value);
		}

		[UIValue("std-panel-x-rot-float")]
		internal float StdPanelXRotValue
		{
			get => _configuration.ChartStandardLevelRotation.x;
			set => _stdRot = new Vector3(value, _stdRot.y, _stdRot.z);
		}

		[UIValue("std-panel-y-rot-float")]
		internal float StdPanelYRotValue
		{
			get => _configuration.ChartStandardLevelRotation.y;
			set => _stdRot = new Vector3(_stdRot.x, value, _stdRot.z);
		}

		[UIValue("std-panel-z-rot-float")]
		internal float StdPanelZRotValue
		{
			get => _configuration.ChartStandardLevelRotation.z;
			set => _stdRot = new Vector3(_stdRot.x, _stdRot.y, value);
		}

		[UIValue("no-std-panel-x-pos-float")]
		internal float NoStdPanelXPosValue
		{
			get => _configuration.Chart360LevelPosition.x;
			set => _noStdPos = new Vector3(value, _noStdPos.y, _noStdPos.z);
		}

		[UIValue("no-std-panel-y-pos-float")]
		internal float NoStdPanelYPosValue
		{
			get => _configuration.Chart360LevelPosition.y;
			set => _noStdPos = new Vector3(_noStdPos.x, value, _noStdPos.z);
		}

		[UIValue("no-std-panel-z-pos-float")]
		internal float NoStdPanelZPosValue
		{
			get => _configuration.Chart360LevelPosition.z;
			set => _noStdPos = new Vector3(_noStdPos.x, _noStdPos.y, value);
		}

		[UIValue("no-std-panel-x-rot-float")]
		internal float NoStdPanelXRotValue
		{
			get => _configuration.Chart360LevelRotation.x;
			set => _noStdRot = new Vector3(value, _noStdRot.y, _noStdRot.z);
		}

		[UIValue("no-std-panel-y-rot-float")]
		internal float NoStdPanelYRotValue
		{
			get => _configuration.Chart360LevelRotation.y;
			set => _noStdRot = new Vector3(_noStdRot.x, value, _noStdRot.z);
		}

		[UIValue("no-std-panel-z-rot-float")]
		internal float NoStdPanelZRotValue
		{
			get => _configuration.Chart360LevelRotation.z;
			set => _noStdRot = new Vector3(_noStdRot.x, _noStdRot.y, value);
		}

		[UIObject("std-pos-x-field")]
		internal GameObject StdPosXField = null!;

		[UIObject("std-pos-y-field")]
		internal GameObject StdPosYField = null!;

		[UIObject("std-pos-z-field")]
		internal GameObject StdPosZField = null!;

		[UIObject("std-rot-x-field")]
		internal GameObject StdRotXField = null!;

		[UIObject("std-rot-y-field")]
		internal GameObject StdRotYField = null!;

		[UIObject("std-rot-z-field")]
		internal GameObject StdRotZField = null!;

		[UIObject("no-std-pos-x-field")]
		internal GameObject NoStdPosXField = null!;

		[UIObject("no-std-pos-y-field")]
		internal GameObject NoStdPosYField = null!;

		[UIObject("no-std-pos-z-field")]
		internal GameObject NoStdPosZField = null!;

		[UIObject("no-std-rot-x-field")]
		internal GameObject NoStdRotXField = null!;

		[UIObject("no-std-rot-y-field")]
		internal GameObject NoStdRotYField = null!;

		[UIObject("no-std-rot-z-field")]
		internal GameObject NoStdRotZField = null!;

		[UIValue("show-background-bool")]
		internal bool ShowBackground
		{
			get => _configuration.HasBackground;
			set => _configuration.HasBackground = value;
		}

		[UIValue("background-opacity-float")]
		internal float BackgroundOpacity
		{
			get => _configuration.BackgroundOpacity;
			set => _configuration.BackgroundOpacity = (float) Math.Round(value, 2);
		}

		[UIAction("background-opacity-formatter")]
		internal string BackgroundOpacityFormat(float opacity)
		{
			return $"{(int) (opacity * 100)}%";
		}

		[UIValue("background-color")]
		internal Color BackgroundColor
		{
			get => _configuration.BackgroundColor;
			set => _configuration.BackgroundColor = value;
		}

		[UIValue("line-color")]
		internal Color LineColor
		{
			get => _configuration.LineColor;
			set => _configuration.LineColor = value;
		}

		[UIValue("pointer-color")]
		internal Color PointerColor
		{
			get => _configuration.PointerColor;
			set => _configuration.PointerColor = value;
		}

		[UIAction("#post-parse")]
		internal void Setup()
		{
			var list = new List<GameObject>
			{
				StdPosXField,
				StdPosYField,
				StdPosZField,
				StdRotXField,
				StdRotYField,
				StdRotZField,
				NoStdPosXField,
				NoStdPosYField,
				NoStdPosZField,
				NoStdRotXField,
				NoStdRotYField,
				NoStdRotZField
			};

			foreach (var go in list)
			{
				ResizeValuePicker(go);
			}
		}

		[UIAction("#apply")]
		public void OnApply()
		{
			using var changeHandle = _configuration.ChangeTransaction();
			_configuration.ChartStandardLevelPosition = _stdPos;
			_configuration.ChartStandardLevelRotation = _stdRot;
			_configuration.Chart360LevelPosition = _noStdPos;
			_configuration.Chart360LevelRotation = _noStdRot;
		}

		private static void ResizeValuePicker(GameObject go)
		{
			if (go == null)
			{
				return;
			}

			var rectPicker = go.transform.Find("ValuePicker")?.GetComponent<RectTransform>();
			if (rectPicker != null)
			{
				rectPicker.sizeDelta = new Vector2(25, rectPicker.sizeDelta.y);
			}
		}
	}
}