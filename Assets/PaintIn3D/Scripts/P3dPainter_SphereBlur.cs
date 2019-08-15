using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	public static partial class P3dPainter
	{
		public class SphereBlur : P3dCommand
		{
			[System.NonSerialized]
			private static Material cachedMaterial;

			[System.NonSerialized]
			private static Matrix4x4 cachedMatrix = Matrix4x4.identity;

			[System.NonSerialized]
			private static Vector3 cachedPosition;

			[System.NonSerialized]
			private static float cachedSqrRadius;

			[System.NonSerialized]
			private static float cachedHardness;

			[System.NonSerialized]
			private static float cachedOpacity;

			[System.NonSerialized]
			private static float cachedKernelSize;

			[System.NonSerialized]
			private Matrix4x4 matrix;

			[System.NonSerialized]
			private float hardness;

			[System.NonSerialized]
			private float opacity;

			[System.NonSerialized]
			private float kernelSize;

			public override Material Material
			{
				get
				{
					return cachedMaterial;
				}
			}

			public override bool RequireSwap
			{
				get
				{
					return true;
				}
			}

			public override bool RequireMesh
			{
				get
				{
					return true;
				}
			}

			static SphereBlur()
			{
				cachedMaterial = BuildMaterial("Hidden/Paint in 3D/Sphere Blur");
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

			public static void SetMaterial(float hardness, float opacity, float kernelSize)
			{
				cachedHardness   = hardness;
				cachedOpacity    = opacity;
				cachedKernelSize = kernelSize;
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
				cachedMaterial.SetMatrix(P3dShader._Matrix, matrix.inverse);
				cachedMaterial.SetFloat(P3dShader._Hardness, hardness);
				cachedMaterial.SetFloat(P3dShader._Opacity, opacity);
				cachedMaterial.SetFloat(P3dShader._KernelSize, kernelSize);
			}

			public override void Pool()
			{
				pool.Add(this); poolCount++;
			}

			public static void CopyTo(SphereBlur command)
			{
				command.matrix     = cachedMatrix;
				command.hardness   = cachedHardness;
				command.opacity    = cachedOpacity;
				command.kernelSize = cachedKernelSize;
			}

			public static void Submit(P3dModel model, P3dPaintableTexture paintableTexture, bool preview)
			{
				var command = paintableTexture.AddCommand(model, pool, ref poolCount, preview);

				CopyTo(command);
			}

			private static int poolCount;

			private static List<SphereBlur> pool = new List<SphereBlur>();
		}
	}
}