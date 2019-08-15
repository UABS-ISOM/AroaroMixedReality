using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintSphereTriplanar))]
	public class P3dPaintSphereTriplanar_Editor : P3dEditor<P3dPaintSphereTriplanar>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Layers == 0));
				DrawDefault("layers", "The layers you want this paint to apply to.");
			EndError();
			BeginError(Any(t => t.Groups == 0));
				DrawDefault("groups", "The groups you want this paint to apply to.");
			EndError();
			DrawDefault("targetTexture", "If you only want to paint one specific texture, rather than all of them in the scene, then specify it here.");
			DrawDefault("targetModel", "If you only want to paint one specific model/paintable, rather than all of them in the scene, then specify it here.");

			Separator();

			DrawDefault("blendMode", "The style of blending.");
			BeginError(Any(t => t.Texture == null));
				DrawDefault("texture", "The decal that will be painted.");
			EndError();
			BeginError(Any(t => t.Strength <= 0.0f));
				DrawDefault("strength", "The texture strength.");
			EndError();
			BeginError(Any(t => t.Tiling == 0.0f));
				DrawDefault("tiling", "The texture tiling scale.");
			EndError();
			DrawDefault("colorStyle", "The style of the paint color.");
			if (Any(t => t.ColorStyle == P3dPaintSphereTriplanar.ColorStyles.SingleColor))
			{
				DrawDefault("color", "The color of the paint.");
			}
			if (Any(t => t.ColorStyle == P3dPaintSphereTriplanar.ColorStyles.RandomColor))
			{
				DrawDefault("gradient", "The colors of the paint.");
			}
			DrawDefault("opacity", "The opacity of the brush.");
			BeginIndent();
				DrawDefault("opacityPressure", "If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.", "Pressure");
			EndIndent();

			Separator();

			BeginError(Any(t => t.Radius <= 0.0f));
				DrawDefault("radius", "The radius of the paint brush.");
			EndError();
			BeginIndent();
				DrawDefault("radiusPressure", "If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.", "Pressure");
			EndIndent();
			BeginError(Any(t => t.Hardness <= 0.0f));
				DrawDefault("hardness", "The hardness of the paint brush.");
			EndError();
			BeginIndent();
				DrawDefault("hardnessPressure", "If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.", "Pressure");
			EndIndent();
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to paint a sphere with triplanar texturing at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintSphereTriplanar")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint Sphere Triplanar")]
	public class P3dPaintSphereTriplanar : MonoBehaviour, IHitHandler
	{
		public enum ColorStyles
		{
			SingleColor,
			RandomColor
		}

		/// <summary>The layers you want this paint to apply to.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>The layers you want this paint to apply to.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>If you only want to paint one specific texture, rather than all of them in the scene, then specify it here.</summary>
		public P3dPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private P3dPaintableTexture targetTexture;

		/// <summary>If you only want to paint one specific model/paintable, rather than all of them in the scene, then specify it here.</summary>
		public P3dModel TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private P3dModel targetModel;

		/// <summary>The style of blending.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode;

		/// <summary>The decal that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The opacity of the brush.</summary>
		public float Strength { set { strength = value; } get { return strength; } } [SerializeField] private float strength = 1.0f;

		/// <summary>The opacity of the brush.</summary>
		public float Tiling { set { tiling = value; } get { return tiling; } } [SerializeField] private float tiling = 1.0f;

		/// <summary>The style of color generation.</summary>
		public ColorStyles ColorStyle { set { colorStyle = value; } get { return colorStyle; } } [SerializeField] private ColorStyles colorStyle;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The colors of the paint.</summary>
		public Gradient Gradient { set { gradient = value; } get { return gradient; } } [SerializeField] private Gradient gradient;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.</summary>
		public float OpacityPressure { set { opacityPressure = value; } get { return opacityPressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacityPressure;

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.</summary>
		public float RadiusPressure { set { radiusPressure = value; } get { return radiusPressure; } } [SerializeField] private float radiusPressure;

		/// <summary>The hardness of the paint brush.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 1.0f;

		/// <summary>If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.</summary>
		public float HardnessPressure { set { hardnessPressure = value; } get { return hardnessPressure; } } [SerializeField] private float hardnessPressure;

		/// <summary>This allows you to paint a decal at the specified point.</summary>
		public void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure)
		{
			var finalColor    = color;
			var finalOpacity  = opacity + (1.0f - opacity) * opacityPressure * pressure;
			var finalRadius   = radius + radiusPressure * pressure;
			var finalHardness = hardness + hardnessPressure * pressure;

			if (colorStyle == ColorStyles.RandomColor && gradient != null)
			{
				finalColor = gradient.Evaluate(Random.value);
			}

			P3dPainter.SphereTriplanar.SetMatrix(position, finalRadius);
			P3dPainter.SphereTriplanar.SetMaterial(blendMode, finalHardness, texture, strength, tiling, finalColor, finalOpacity);
			
			// Paint something specific?
			if (targetModel != null || targetTexture != null)
			{
				P3dPainter.SphereTriplanar.SubmitAll(preview, targetModel, targetTexture, groups);
			}
			// Paint everything?
			else
			{
				P3dPainter.SphereTriplanar.SubmitAll(preview, layers, groups);
			}
		}
	}
}