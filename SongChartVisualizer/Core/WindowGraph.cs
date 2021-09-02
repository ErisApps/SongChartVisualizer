/*
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace SongChartVisualizer.Core
{
	internal class WindowGraph : MonoBehaviour
	{
		private static readonly Color DefaultLinkColor = new Color(1, 1, 1, .5f);

		private RectTransform _labelTemplateX = null!;
		private RectTransform _labelTemplateY = null!;
		private RectTransform _dashTemplateX = null!;
		private RectTransform _dashTemplateY = null!;

		public Sprite? circleSprite;

		public RectTransform GraphContainer { get; private set; } = null!;
		public List<GameObject> DotObjects { get; }
		public List<GameObject> LinkObjects { get; }
		public List<GameObject> LabelXObjects { get; }
		public List<GameObject> LabelYObjects { get; }
		public List<GameObject> DashXObjects { get; }
		public List<GameObject> DashYObjects { get; }

		private WindowGraph()
		{
			DotObjects = new List<GameObject>();
			LinkObjects = new List<GameObject>();
			LabelXObjects = new List<GameObject>();
			LabelYObjects = new List<GameObject>();
			DashXObjects = new List<GameObject>();
			DashYObjects = new List<GameObject>();
		}

		private void Awake()
		{
			GraphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

			_labelTemplateX = GraphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
			_labelTemplateY = GraphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
			_dashTemplateX = GraphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
			_dashTemplateY = GraphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
		}

		// ReSharper disable once CognitiveComplexity
		public void ShowGraph(List<float> valueList, bool makeDotsVisible = true, bool makeLinksVisible = true, bool makeOriginZero = false, int maxVisibleValueAmount = -1,
			Func<float, string>? getAxisLabelX = null, Func<float, string>? getAxisLabelY = null, Color? linkColor = null)
		{
			getAxisLabelX ??= i => i.ToString(CultureInfo.InvariantCulture);

			getAxisLabelY ??= f => Mathf.RoundToInt(f).ToString();

			if (maxVisibleValueAmount <= 0)
			{
				maxVisibleValueAmount = valueList.Count;
			}

			ClearOldData();

			var graphSizeDelta = GraphContainer.sizeDelta;
			var graphWidth = graphSizeDelta.x;
			var graphHeight = graphSizeDelta.y;

			var yMaximum = valueList[0];
			var yMinimum = valueList[0];

			for (var i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
			{
				var value = valueList[i];
				if (value > yMaximum)
				{
					yMaximum = value;
				}

				if (value < yMinimum)
				{
					yMinimum = value;
				}
			}

			var yDifference = yMaximum - yMinimum;
			if (yDifference <= 0)
			{
				yDifference = 5f;
			}

			yMaximum += (yDifference * 0.2f);
			yMinimum -= (yDifference * 0.2f);

			if (makeOriginZero)
			{
				yMinimum = 0f; // Start the graph at zero
			}

			var xSize = graphWidth / (maxVisibleValueAmount + 1);
			var xIndex = 0;

			linkColor = linkColor == null ? DefaultLinkColor : new Color(linkColor.Value.r, linkColor.Value.g, linkColor.Value.b, .5f);

			GameObject? lastCircleGameObject = null;
			for (var i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
			{
				var xPosition = xSize + xIndex * xSize;
				var yPosition = (valueList[i] - yMinimum) / (yMaximum - yMinimum) * graphHeight;
				var circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), makeDotsVisible);
				DotObjects.Add(circleGameObject);
				if (lastCircleGameObject != null)
				{
					var dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
						circleGameObject.GetComponent<RectTransform>().anchoredPosition,
						makeLinksVisible,
						linkColor.Value);
					LinkObjects.Add(dotConnectionGameObject);
				}

				lastCircleGameObject = circleGameObject;

				var labelX = Instantiate(_labelTemplateX, GraphContainer, false);
				var labelXGo = labelX.gameObject;
				labelXGo.SetActive(true);
				labelX.anchoredPosition = new Vector2(xPosition, -7f);
				labelX.GetComponent<Text>().text = getAxisLabelX(i);
				LabelXObjects.Add(labelXGo);

				var dashX = Instantiate(_dashTemplateX, GraphContainer, false);
				var dashXGo = dashX.gameObject;
				dashXGo.SetActive(true);
				dashX.anchoredPosition = new Vector2(yPosition, -3);
				DashXObjects.Add(dashXGo);

				xIndex++;
			}

			const int SEPARATOR_COUNT = 10;
			for (var i = 0; i <= SEPARATOR_COUNT; i++)
			{
				var labelY = Instantiate(_labelTemplateY, GraphContainer, false);
				var labelYGo = labelY.gameObject;
				labelYGo.SetActive(true);
				var normalizedValue = i * 1f / SEPARATOR_COUNT;
				labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
				labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
				LabelYObjects.Add(labelYGo);

				var dashY = Instantiate(_dashTemplateY, GraphContainer, false);
				var dashYGo = dashY.gameObject;
				dashYGo.SetActive(true);
				dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
				DashYObjects.Add(dashYGo);
			}
		}

		private GameObject CreateCircle(Vector2 anchoredPosition, bool makeDotsVisible)
		{
			var go = new GameObject("Circle", typeof(ImageView));
			go.transform.SetParent(GraphContainer, false);
			var image = go.GetComponent<ImageView>();
			image.sprite = circleSprite;
			image.useSpriteMesh = true;
			image.enabled = makeDotsVisible;

			var rectTransform = go.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = anchoredPosition;
			rectTransform.sizeDelta = new Vector2(8, 8);
			rectTransform.anchorMin = new Vector2(0, 0);
			rectTransform.anchorMax = new Vector2(0, 0);

			return go;
		}

		private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, bool makeLinkVisible, Color linkColor)
		{
			var go = new GameObject("DotConnection", typeof(ImageView));
			go.transform.SetParent(GraphContainer, false);

			var image = go.GetComponent<ImageView>();
			image.color = linkColor;
			image.enabled = makeLinkVisible;

			var rectTransform = go.GetComponent<RectTransform>();
			var dir = (dotPositionB - dotPositionA).normalized;
			var distance = Vector2.Distance(dotPositionA, dotPositionB);
			rectTransform.anchorMin = new Vector2(0, 0);
			rectTransform.anchorMax = new Vector2(0, 0);
			rectTransform.sizeDelta = new Vector2(distance, 2f);
			rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
			rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

			return go;
		}

		private void ClearOldData()
		{
			static void ClearGameObjectList(ICollection<GameObject>? list)
			{
				if (list == null)
				{
					return;
				}

				foreach (var go in list)
				{
					Destroy(go);
				}

				list.Clear();
			}

			ClearGameObjectList(DotObjects);
			ClearGameObjectList(LinkObjects);
			ClearGameObjectList(LabelXObjects);
			ClearGameObjectList(LabelYObjects);
			ClearGameObjectList(DashXObjects);
			ClearGameObjectList(DashYObjects);
		}
	}
}