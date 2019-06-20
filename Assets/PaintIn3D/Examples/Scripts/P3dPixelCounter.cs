using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dPixelCounter))]
	public class P3dPixelCounter_Editor : P3dEditor<P3dPixelCounter>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.PaintableTexture == null));
				DrawDefault("paintableTexture", "This is the paintable texture whose pixels we will count.");
			EndError();
			BeginError(Any(t => t.DownsampleSteps < 0));
				DrawDefault("downsampleSteps", "Counting all the pixels of a texture can be slow, so you can pick how many times the texture is downsampled before it gets counted. One downsample = half width & height or 1/4 of the pixels. NOTE: The pixel totals will be multiplied to account for this downsampling.");
			EndError();
			DrawDefault("threshold", "The RGBA value must be higher than this for it to be counted.");

			Separator();

			BeginDisabled();
				DrawDefault("totalR");
				DrawDefault("totalG");
				DrawDefault("totalB");
				DrawDefault("totalA");
			EndDisabled();
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component will total up all RGBA channels in the specified texture every time it detects a texture update.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPixelCounter")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Pixel Counter")]
	public class P3dPixelCounter : P3dLinkedBehaviour<P3dPixelCounter>, IPaintHandler
	{
		/// <summary>This is the paintable texture whose pixels we will count.</summary>
		public P3dPaintableTexture PaintableTexture { set { paintableTexture = value; } get { return paintableTexture; } } [SerializeField] private P3dPaintableTexture paintableTexture;

		/// <summary>Counting all the pixels of a texture can be slow, so you can pick how many times the texture is downsampled before it gets counted. One downsample = half width & height or 1/4 of the pixels.
		/// NOTE: The pixel totals will be multiplied to account for this downsampling.</summary>
		public int DownsampleSteps { set { downsampleSteps = value; } get { return downsampleSteps; } } [SerializeField] private int downsampleSteps = 3;

		/// <summary>The RGBA value must be higher than this for it to be counted.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [Range(0.0f, 1.0f)] [SerializeField] private float threshold = 0.5f;

		[SerializeField]
		private int totalR;

		[SerializeField]
		private int totalG;

		[SerializeField]
		private int totalB;

		[SerializeField]
		private int totalA;

		public int TotalR
		{
			get
			{
				return totalR;
			}
		}

		public int TotalG
		{
			get
			{
				return totalG;
			}
		}

		public int TotalB
		{
			get
			{
				return totalB;
			}
		}

		public int TotalA
		{
			get
			{
				return totalA;
			}
		}

		public void HandlePaint(P3dPaintableTexture newPaintableTexture, bool preview)
		{
			if (paintableTexture == null)
			{
				paintableTexture = newPaintableTexture;
			}

			if (paintableTexture == newPaintableTexture && preview == false)
			{
				Calculate();
			}
		}

		private void Calculate()
		{
			if (paintableTexture != null && paintableTexture.Activated == true)
			{
				var renderTexture = paintableTexture.Current;
				var temporary     = default(RenderTexture);

				if (P3dHelper.Downsample(renderTexture, downsampleSteps, ref temporary) == true)
				{
					Calculate(temporary, 1 << downsampleSteps);

					P3dHelper.ReleaseRenderTexture(temporary);
				}
				else
				{
					Calculate(renderTexture, 1);
				}
			}
		}

		private void Calculate(RenderTexture renderTexture, int scale)
		{
			var threshold32 = (byte)(threshold * 255.0f);
			var width       = renderTexture.width;
			var height      = renderTexture.height;
			var texture2D   = P3dHelper.GetReadableCopy(renderTexture);
			var pixels32    = texture2D.GetPixels32();

			P3dHelper.Destroy(texture2D);

			// Reset totals
			totalR = 0;
			totalG = 0;
			totalB = 0;
			totalA = 0;

			// Calculate totals
			for (var y = 0; y < height; y++)
			{
				var offset = y * width;

				for (var x = 0; x < height; x++)
				{
					var pixel32 = pixels32[offset + x];

					if (pixel32.r >= threshold32) totalR++;
					if (pixel32.g >= threshold32) totalG++;
					if (pixel32.b >= threshold32) totalB++;
					if (pixel32.a >= threshold32) totalA++;
				}
			}

			// Scale totals to account for downsampling
			totalR *= scale;
			totalG *= scale;
			totalB *= scale;
			totalA *= scale;
		}
	}
}