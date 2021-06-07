using System;
using System.Linq;
using UnityEngine;

namespace SongChartVisualizer.Services
{
	internal class ScvAssetLoader : IDisposable
	{
		private Material? _uiNoGlowMaterial;
		public Material UINoGlowMaterial => _uiNoGlowMaterial ??= Resources.FindObjectsOfTypeAll<Material>().First(x => x.name == "UINoGlow");

		public void Dispose()
		{
			_uiNoGlowMaterial = null;
		}
	}
}