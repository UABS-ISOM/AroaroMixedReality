using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	public static partial class P3dPainter
	{
		public class Sphere : P3dCommand
		{
			[System.NonSerialized]
			private static Material[] cachedMaterials;

			[System.NonSerialized]
			private static bool[] cachedSwaps;

			[System.NonSerialized]
			private static Material cachedMaterial;

			[System.NonSerialized]
			private static bool cachedSwap;

			[System.NonSerialized]
			private static Matrix4x4 cachedMatrix = Matrix4x4.identity;

			[System.NonSerialized]
			private static Vector3 cachedPosition;

			[System.NonSerialized]
			private static float cachedSqrRadius;

			[System.NonSerialized]
			private static Color cachedColor;

			[System.NonSerialized]
			private static float cachedOpacity;

			[System.NonSerialized]
			private static float cachedHardness;

			[System.NonSerialized]
			private Material material;

			[System.NonSerialized]
			private bool swap;

			[System.NonSerialized]
			private Matrix4x4 matrix;

			[System.NonSerialized]
			private float hardness;

			[System.NonSerialized]
			private Color color;

			[System.NonSerialized]
			private float opacity;

			public override Material Material
			{
				get
				{
					return material;
				}
			}

			public override bool RequireSwap
			{
				get
				{
					return swap;
				}
			}

			public override bool RequireMesh
			{
				get
				{
					return true;
				}
			}

			static Sphere()
			{
				cachedMaterials = BuildMaterialsBlendModes("Hidden/Paint in 3D/Sphere");
				cachedSwaps     = BuildSwapBlendModes();
			}

			public static void SetMatrix(Vector3 position, float radius)
			{
				cachedMatrix.m00 = cachedMatrix.m11 = cachedMatrix.m22 = radius;
				cachedMatrix.m03 = position.x;
				cachedMatrix.m13 = position.y;
				cachedMatrix.m23 = position.z;

				cachedPosition  = position;
				cachedSqrRadius = radius * radius;
			}

			public static void SetMatrix(Vector3 position, Vector3 radius, Quaternion rotation)
			{
				//cachedMatrix.m00 = radius.x;
				//cachedMatrix.m11 = radius.y;
				//cachedMatrix.m22 = radius.z;
				//cachedMatrix.m03 = position.x;
				//cachedMatrix.m13 = position.y;
				//cachedMatrix.m23 = position.z;

				cachedMatrix = Matrix4x4.TRS(position, rotation, radius);

				cachedPosition  = position;
				cachedSqrRadius = Mathf.Max(radius.x, Mathf.Max(radius.y, radius.z));
			}

			public static void SetMaterial(P3dBlendMode blendMode, float hardness, Color color, float opacity)
			{
				cachedMaterial = cachedMaterials[(int)blendMode];
				cachedSwap     = cachedSwaps[(int)blendMode];
				cachedHardness = hardness;
				cachedColor    = color;
				cachedOpacity  = opacity;
			}

			public static void SubmitAll(bool preview, P3dModel model, P3dPaintableTexture paintableTexture, int groupMask = -1)
			{
				if (model != null)
				{
					if (paintableTexture != null)
					{
						Submit(model, paintableTexture, preview);
					}
					else
					{
						SubmitAll(preview, model, groupMask);
					}
				}
				else
				{
					if (paintableTexture != null)
					{
						Submit(paintableTexture.CachedPaintable, paintableTexture, preview);
					}
				}
			}

			public static void SubmitAll(bool preview = false, int layerMask = -1, int groupMask = -1)
			{
				var models = P3dModel.FindOverlap(cachedPosition, cachedSqrRadius, layerMask);

				for (var i = models.Count - 1; i >= 0; i--)
				{
					SubmitAll(preview, models[i], groupMask);
				}
			}

			public static void SubmitAll(bool preview, P3dModel model, int groupMask = -1)
			{
				var paintableTextures = P3dPaintableTexture.Filter(model, groupMask);

				for (var i = paintableTextures.Count - 1; i >= 0; i--)
				{
					Submit(model, paintableTextures[i], preview);
				}
			}

			public override void Apply()
			{
				material.SetMatrix(P3dShader._Matrix, matrix.inverse);
				material.SetFloat(P3dShader._Hardness, hardness);
				material.SetColor(P3dShader._Color, color);
				material.SetFloat(P3dShader._Opacity, opacity);
			}

			public override void Pool()
			{
				pool.Add(this); poolCount++;
			}

			public static void CopyTo(Sphere command)
			{
				command.material = cachedMaterial;
				command.swap     = cachedSwap;
				command.matrix   = cachedMatrix;
				command.hardness = cachedHardness;
				command.color    = cachedColor;
				command.opacity  = cachedOpacity;
			}

			public static void Submit(P3dModel model, P3dPaintableTexture paintableTexture, bool preview)
			{
				var command = paintableTexture.AddCommand(model, pool, ref poolCount, preview);

				CopyTo(command);
			}

			private static int poolCount;

			private static List<Sphere> pool = new List<Sphere>();
		}
	}
}