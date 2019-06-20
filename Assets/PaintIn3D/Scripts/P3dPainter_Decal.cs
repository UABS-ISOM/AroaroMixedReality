using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This handles the decal painting commands.</summary>
	public static partial class P3dPainter
	{
		/// <summary>Test.</summary>
		public class Decal : P3dCommand
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
			private static Vector3 cachedDirection;

			[System.NonSerialized]
			private static Texture cachedTexture;

			[System.NonSerialized]
			private static Texture cachedShape;

			[System.NonSerialized]
			private static float cachedHardness;

			[System.NonSerialized]
			private static Vector2 cachedNormalFront;

			[System.NonSerialized]
			private static Vector2 cachedNormalBack;

			[System.NonSerialized]
			private static Color cachedColor;

			[System.NonSerialized]
			private static float cachedOpacity;

			[System.NonSerialized]
			private Material material;

			[System.NonSerialized]
			private bool swap;

			[System.NonSerialized]
			private Matrix4x4 matrix;

			[System.NonSerialized]
			private Vector3 direction;

			[System.NonSerialized]
			private Texture texture;

			[System.NonSerialized]
			private Texture shape;

			[System.NonSerialized]
			private float hardness;

			[System.NonSerialized]
			private Vector2 normalFront;

			[System.NonSerialized]
			private Vector2 normalBack;

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

			static Decal()
			{
				cachedMaterials = BuildMaterialsBlendModes("Hidden/Paint in 3D/Decal");
				cachedSwaps     = BuildSwapBlendModes();
			}

			public static void SetMatrix(Vector3 position, Vector3 normal, float angle, float size, Texture decal, float depth, bool mirror)
			{
				var rotation = P3dHelper.NormalToCameraRotation(normal);

				SetMatrix(position, rotation, angle, size, decal, depth, mirror);
			}

			public static void SetMatrix(Vector3 position, Quaternion rotation, float angle, Vector2 size, Texture decal, float depth, bool mirror)
			{
				if (mirror == true)
				{
					size.x = -size.x;
				}

				SetMatrix(position, rotation, angle, new Vector3(size.x, size.y, depth));
			}

			public static void SetMatrix(Vector3 position, Quaternion rotation, float angle, float size, Texture decal, float depth, bool mirror)
			{
				var width  = size;
				var height = size;

				if (decal != null)
				{
					if (decal.width > decal.height)
					{
						height *= decal.height / (float)decal.width;
					}
					else
					{
						width *= decal.width / (float)decal.height;
					}

					if (mirror == true)
					{
						width = -width;
					}
				}

				SetMatrix(position, rotation, angle, new Vector3(width, height, depth));
			}

			public static void SetMatrix(Vector3 position, Quaternion rotation, float angle, Vector3 size)
			{
				var up     = Vector3.up;
				var camera = P3dHelper.GetCamera();

				if (camera != null)
				{
					up = camera.transform.up;
				}

				//cachedMatrix    = Matrix4x4.Translate(position) * Matrix4x4.Rotate(rotation * Quaternion.Euler(0.0f, 0.0f, angle)) * Matrix4x4.Scale(size);
				cachedMatrix    = Matrix4x4.TRS(position, rotation * Quaternion.Euler(0.0f, 0.0f, angle), size);
				cachedPosition  = position;
				cachedSqrRadius = (size * 0.5f).sqrMagnitude;
				cachedDirection = rotation * Vector3.forward;
			}

			public static void SetMaterial(P3dBlendMode blendMode, Texture decal, float hardness, float normalBack, float normalFront, float normalFade, Color color, float opacity, Texture shape)
			{
				cachedMaterial = cachedMaterials[(int)blendMode];
				cachedSwap     = cachedSwaps[(int)blendMode];
				cachedColor    = color;
				cachedOpacity  = opacity;
				cachedHardness = hardness;
				cachedTexture  = decal;
				cachedShape    = shape;

				var pointA = normalFront - 1.0f - normalFade;
				var pointB = normalFront - 1.0f;
				var pointC = 1.0f - normalBack;
				var pointD = 1.0f - normalBack + normalFade;

				cachedNormalFront = new Vector2(pointA, P3dHelper.Reciprocal(pointB - pointA));
				cachedNormalBack  = new Vector2(pointC, P3dHelper.Reciprocal(pointD - pointC));
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

			public static void ApplyStatic(P3dChannel channel)
			{
				cachedMaterial.SetMatrix(P3dShader._Matrix, cachedMatrix.inverse);
				cachedMaterial.SetVector(P3dShader._Direction, cachedDirection);
				cachedMaterial.SetColor(P3dShader._Color, cachedColor);
				cachedMaterial.SetFloat(P3dShader._Opacity, cachedOpacity);
				cachedMaterial.SetFloat(P3dShader._Hardness, cachedHardness);
				cachedMaterial.SetTexture(P3dShader._Texture, cachedTexture);
				cachedMaterial.SetTexture(P3dShader._Shape, cachedShape);
				cachedMaterial.SetVector(P3dShader._NormalFront, cachedNormalFront);
				cachedMaterial.SetVector(P3dShader._NormalBack, cachedNormalBack);
			}

			public override void Apply()
			{
				material.SetMatrix(P3dShader._Matrix, matrix.inverse);
				material.SetVector(P3dShader._Direction, direction);
				material.SetColor(P3dShader._Color, color);
				material.SetFloat(P3dShader._Opacity, opacity);
				material.SetFloat(P3dShader._Hardness, hardness);
				material.SetTexture(P3dShader._Texture, texture);
				material.SetTexture(P3dShader._Shape, shape);
				material.SetVector(P3dShader._NormalFront, normalFront);
				material.SetVector(P3dShader._NormalBack, normalBack);
			}

			public override void Pool()
			{
				pool.Add(this); poolCount++;
			}

			public static void CopyTo(Decal command)
			{
				command.material    = cachedMaterial;
				command.swap        = cachedSwap;
				command.matrix      = cachedMatrix;
				command.direction   = cachedDirection;
				command.color       = cachedColor;
				command.opacity     = cachedOpacity;
				command.hardness    = cachedHardness;
				command.texture     = cachedTexture;
				command.shape       = cachedShape;
				command.normalFront = cachedNormalFront;
				command.normalBack  = cachedNormalBack;
			}

			public static void Submit(P3dModel model, P3dPaintableTexture paintableTexture, bool preview)
			{
				var command = paintableTexture.AddCommand(model, pool, ref poolCount, preview);

				CopyTo(command);
			}

			private static int poolCount;

			private static List<Decal> pool = new List<Decal>();
		}
	}
}