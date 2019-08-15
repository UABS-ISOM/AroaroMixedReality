using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dPixelCounterText))]
	public class P3dPixelCounterText_Editor : P3dEditor<P3dPixelCounterText>
	{
		protected override void OnInspector()
		{
			DrawDefault("custom");
			if (Any(t => t.Custom == true))
			{
				BeginIndent();
					BeginError(Any(t => t.PixelCounters == null || t.PixelCounters.Count == 0 || t.PixelCounters.Exists(p => p == null)));
						DrawDefault("pixelCounters");
					EndError();
				EndIndent();
			}
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to output the totals of all the specified pixel counters to a UI Text component.</summary>
	[RequireComponent(typeof(Text))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPixelCounterText")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Pixel Counter Text")]
	public class P3dPixelCounterText : MonoBehaviour
	{
		/// <summary>If you disable this then all P3dPixelCounters active and enabled in the scene will be used.</summary>
		public bool Custom { set { custom = value; } get { return custom; } } [SerializeField] private bool custom;

		/// <summary>The custom list of pixel counters you want to use.</summary>
		public List<P3dPixelCounter> PixelCounters { set { pixelCounters = value; } get { return pixelCounters; } } [SerializeField] private List<P3dPixelCounter> pixelCounters;

		[System.NonSerialized]
		private Text cachedText;

		protected virtual void OnEnable()
		{
			cachedText = GetComponent<Text>();
		}

		protected virtual void Update()
		{
			var totalR = 0;
			var totalG = 0;
			var totalB = 0;
			var totalA = 0;

			// Use all?
			if (custom == false)
			{
				var pixelCounter = P3dPixelCounter.FirstInstance;

				for (var i = 0; i < P3dPixelCounter.InstanceCount; i++)
				{
					totalR += pixelCounter.TotalR;
					totalG += pixelCounter.TotalG;
					totalB += pixelCounter.TotalB;
					totalA += pixelCounter.TotalA;

					pixelCounter = pixelCounter.NextInstance;
				}
			}
			// Use specific instances?
			else if (pixelCounters != null)
			{
				for (var i = pixelCounters.Count - 1; i >= 0; i--)
				{
					var pixelCounter = pixelCounters[i];

					if (pixelCounter != null)
					{
						totalR += pixelCounter.TotalR;
						totalG += pixelCounter.TotalG;
						totalB += pixelCounter.TotalB;
						totalA += pixelCounter.TotalA;
					}
				}
			}

			cachedText.text =
				"Total R = " + totalR + "\n" +
				"Total G = " + totalG + "\n" +
				"Total B = " + totalB + "\n" +
				"Total A = " + totalA + "\n";
		}
	}
}