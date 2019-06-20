using UnityEngine;
using UnityEngine.Rendering;

namespace PaintIn3D
{
	/// <summary>This class caches shader information, speeding them up.</summary>
	public static class P3dShader
	{
		public static readonly int BLEND_MODE_COUNT = 8;

		public static int _Buffer;
		public static int _Channel;
		public static int _Color;
		public static int _Direction;
		public static int _DstA;
		public static int _DstRGB;
		public static int _Hardness;
		public static int _KernelSize;
		public static int _Matrix;
		public static int _NormalBack;
		public static int _NormalFront;
		public static int _Op;
		public static int _Opacity;
		public static int _Shape;
		public static int _SrcA;
		public static int _SrcRGB;
		public static int _Strength;
		public static int _Texture;
		public static int _Tiling;

		static P3dShader()
		{
			_Buffer = Shader.PropertyToID("_Buffer");
			_Channel = Shader.PropertyToID("_Channel");
			_Color = Shader.PropertyToID("_Color");
			_Direction = Shader.PropertyToID("_Direction");
			_DstA = Shader.PropertyToID("_DstA");
			_DstRGB = Shader.PropertyToID("_DstRGB");
			_Hardness = Shader.PropertyToID("_Hardness");
			_KernelSize = Shader.PropertyToID("_KernelSize");
			_Matrix = Shader.PropertyToID("_Matrix");
			_NormalBack = Shader.PropertyToID("_NormalBack");
			_NormalFront = Shader.PropertyToID("_NormalFront");
			_Op = Shader.PropertyToID("_Op");
			_Opacity = Shader.PropertyToID("_Opacity");
			_Shape = Shader.PropertyToID("_Shape");
			_SrcA = Shader.PropertyToID("_SrcA");
			_SrcRGB = Shader.PropertyToID("_SrcRGB");
			_Strength = Shader.PropertyToID("_Strength");
			_Texture = Shader.PropertyToID("_Texture");
			_Tiling = Shader.PropertyToID("_Tiling");
		}

		public static Shader Load(string shaderName)
		{
			var shader = Shader.Find(shaderName);

			if (shader == null)
			{
				throw new System.Exception("Failed to find shader called: " + shaderName);
			}

			return shader;
		}

		public static Material Build(Shader shader)
		{
			var material = new Material(shader);
#if UNITY_EDITOR
			material.hideFlags = HideFlags.DontSave;
#endif
			return material;
		}

		public static Material Build(Shader shader, int uv, P3dBlendMode blendMode)
		{
			var material = Build(shader);

			switch (blendMode)
			{
				case P3dBlendMode.AlphaBlend:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.SrcAlpha);
					material.SetInt(_DstRGB, (int)BlendMode.OneMinusSrcAlpha);
					material.SetInt(_SrcA, (int)BlendMode.OneMinusDstAlpha);
					material.SetInt(_DstA, (int)BlendMode.One);
					material.SetInt(_Op, (int)BlendOp.Add);
				}
				break;

				case P3dBlendMode.AlphaBlendRGB:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.SrcAlpha);
					material.SetInt(_DstRGB, (int)BlendMode.OneMinusSrcAlpha);
					material.SetInt(_SrcA, (int)BlendMode.Zero);
					material.SetInt(_DstA, (int)BlendMode.One);
					material.SetInt(_Op, (int)BlendOp.Add);
				}
				break;

				case P3dBlendMode.Additive:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.One);
					material.SetInt(_DstRGB, (int)BlendMode.One);
					material.SetInt(_SrcA, (int)BlendMode.One);
					material.SetInt(_DstA, (int)BlendMode.One);
					material.SetInt(_Op, (int)BlendOp.Add);
					material.EnableKeyword("P3D_A"); // Additive
				}
				break;

				case P3dBlendMode.Subtractive:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.One);
					material.SetInt(_DstRGB, (int)BlendMode.One);
					material.SetInt(_SrcA, (int)BlendMode.One);
					material.SetInt(_DstA, (int)BlendMode.One);
					material.SetInt(_Op, (int)BlendOp.ReverseSubtract);
					material.EnableKeyword("P3D_A"); // Additive
				}
				break;

				case P3dBlendMode.SoftAdditive:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.OneMinusDstColor);
					material.SetInt(_DstRGB, (int)BlendMode.One);
					material.SetInt(_SrcA, (int)BlendMode.OneMinusDstAlpha);
					material.SetInt(_DstA, (int)BlendMode.One);
					material.SetInt(_Op, (int)BlendOp.Add);
					material.EnableKeyword("P3D_A"); // Additive
				}
				break;

				case P3dBlendMode.AlphaBlendAdvanced:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.One);
					material.SetInt(_DstRGB, (int)BlendMode.Zero);
					material.SetInt(_SrcA, (int)BlendMode.One);
					material.SetInt(_DstA, (int)BlendMode.Zero);
					material.SetInt(_Op, (int)BlendOp.Add);
					material.EnableKeyword("P3D_B"); // Swapped alpha
				}
				break;

				case P3dBlendMode.Replace:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.One);
					material.SetInt(_DstRGB, (int)BlendMode.Zero);
					material.SetInt(_SrcA, (int)BlendMode.One);
					material.SetInt(_DstA, (int)BlendMode.Zero);
					material.SetInt(_Op, (int)BlendOp.Add);
					material.EnableKeyword("P3D_C"); // Shape
				}
				break;

				case P3dBlendMode.Multiply:
				{
					material.SetInt(_SrcRGB, (int)BlendMode.DstColor);
					material.SetInt(_DstRGB, (int)BlendMode.Zero);
					material.SetInt(_SrcA, (int)BlendMode.One);
					material.SetInt(_DstA, (int)BlendMode.Zero);
					material.SetInt(_Op, (int)BlendOp.Add);
					material.EnableKeyword("P3D_D"); // Multiply
				}
				break;
			}

			return material;
		}
	}
}