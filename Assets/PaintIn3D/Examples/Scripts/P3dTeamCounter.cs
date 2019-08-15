using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dTeamCounter))]
	public class P3dTeamCounter_Editor : P3dEditor<P3dTeamCounter>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.PaintableTexture == null));
				DrawDefault("paintableTexture", "This is the paintable texture whose pixels will will count.");
			EndError();
			BeginError(Any(t => t.DownsampleSteps < 0));
				DrawDefault("downsampleSteps", "Counting all the pixels of a texture can be slow, so you can pick how many times the texture is downsampled before it gets counted. One downsample = half width & height or 1/4 of the pixels. NOTE: The pixel totals will be multiplied to account for this downsampling.");
			EndError();
			DrawDefault("threshold", "The RGBA values must be within this range of a team for it to be counted.");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component will search the specified paintable texture for pixel colors matching an active and enabled P3dTeam color.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dTeamCounter")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Team Counter")]
	public class P3dTeamCounter : MonoBehaviour, IPaintHandler
	{
		class TempTeam
		{
			public byte R;
			public byte G;
			public byte B;
			public byte A;
			public int  Count;
		}

		/// <summary>This is the paintable texture whose pixels will will count.</summary>
		public P3dPaintableTexture PaintableTexture { set { paintableTexture = value; } get { return paintableTexture; } } [SerializeField] private P3dPaintableTexture paintableTexture;

		/// <summary>Counting all the pixels of a texture can be slow, so you can pick how many times the texture is downsampled before it gets counted. One downsample = half width & height or 1/4 of the pixels. NOTE: The pixel totals will be multiplied to account for this downsampling.</summary>
		public int DownsampleSteps { set { downsampleSteps = value; } get { return downsampleSteps; } } [SerializeField] private int downsampleSteps = 3;

		/// <summary>The RGBA values must be within this range of a team for it to be counted.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [Range(0.0f, 1.0f)] [SerializeField] private float threshold = 0.5f;

		[System.NonSerialized]
		private static List<TempTeam> tempTeams = new List<TempTeam>();

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
			var threshold32 = (int)(threshold * 255.0f);
			var width       = renderTexture.width;
			var height      = renderTexture.height;
			var texture2D   = P3dHelper.GetReadableCopy(renderTexture);
			var pixels32    = texture2D.GetPixels32();

			P3dHelper.Destroy(texture2D);

			PrepareTemp();

			for (var y = 0; y < height; y++)
			{
				var offset = y * width;

				for (var x = 0; x < height; x++)
				{
					var pixel32      = pixels32[offset + x];
					var bestIndex    = -1;
					var bestDistance = threshold32;

					for (var i = 0; i < P3dTeam.InstanceCount; i++)
					{
						var tempTeam = tempTeams[i];
						var distance = 0;

						distance += System.Math.Abs(tempTeam.R - pixel32.r);
						distance += System.Math.Abs(tempTeam.G - pixel32.g);
						distance += System.Math.Abs(tempTeam.B - pixel32.b);
						distance += System.Math.Abs(tempTeam.A - pixel32.a);

						if (distance <= bestDistance)
						{
							bestIndex    = i;
							bestDistance = distance;
						}
					}

					if (bestIndex >= 0)
					{
						tempTeams[bestIndex].Count++;
					}
				}
			}

			// Multiply totals to account for downsampling
			Contribute(scale);
		}

		private void PrepareTemp()
		{
			var team = P3dTeam.FirstInstance;

			tempTeams.Clear();

			for (var i = tempTeams.Count; i < P3dTeam.InstanceCount; i++)
			{
				tempTeams.Add(new TempTeam());
			}

			for (var i = 0; i < P3dTeam.InstanceCount; i++)
			{
				var tempTeam  = tempTeams[i];
				var tempColor = (Color32)team.Color;

				tempTeam.R     = tempColor.r;
				tempTeam.G     = tempColor.g;
				tempTeam.B     = tempColor.b;
				tempTeam.A     = tempColor.a;
				tempTeam.Count = 0;

				team = team.NextInstance;
			}
		}

		private void Contribute(int scale)
		{
			var team = P3dTeam.FirstInstance;

			for (var i = 0; i < P3dTeam.InstanceCount; i++)
			{
				team.Contribute(this, tempTeams[i].Count * scale);

				team = team.NextInstance;
			}
		}
	}
}