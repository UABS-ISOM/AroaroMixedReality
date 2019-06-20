using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintDecal))]
	public class P3dPaintDecal_Editor : P3dEditor<P3dPaintDecal>
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
			if (Any(t => t.BlendMode == P3dBlendMode.Replace))
			{
				BeginError(Any(t => t.Shape == null));
					DrawDefault("shape", "The shape of the decal when using Replace blending.");
				EndError();
			}
			DrawDefault("colorStyle", "The style of the paint color.");
			if (Any(t => t.ColorStyle == P3dPaintDecal.ColorStyles.SingleColor))
			{
				DrawDefault("color", "The color of the paint.");
			}
			if (Any(t => t.ColorStyle == P3dPaintDecal.ColorStyles.RandomColor))
			{
				DrawDefault("gradient", "The colors of the paint.");
			}
			DrawDefault("opacity", "The opacity of the brush.");
			BeginIndent();
				DrawDefault("opacityPressure", "If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.", "Pressure");
			EndIndent();

			Separator();

			DrawDefault("randomAngle", "Randomly rotate the decal?");
			if (Any(t => t.RandomAngle == false))
			{
				DrawDefault("angle", "The angle of the decal in degrees.");
			}
			DrawDefault("mirror", "Mirror decal?");
			BeginError(Any(t => t.Radius <= 0.0f));
				DrawDefault("radius", "The radius of the paint brush.");
			EndError();
			BeginIndent();
				DrawDefault("radiusPressure", "If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.", "Pressure");
			EndIndent();
			DrawDefault("overrideSize", "If this is non-zero, then the radius will be multiplied by this, allowing you to manually control the size and aspect ratio of the decal.");
			BeginError(Any(t => t.Depth <= 0.0f));
				DrawDefault("depth", "This allows you to control how near+far from the hit point the decal will appear in world space. If you're painting a flat surface head-on then you can use a low value here, but if you're painting something complex then you may need to set this higher.");
			EndError();
			BeginIndent();
				DrawDefault("depthPressure", "If you want the depth to increase with finger pressure, this allows you to set how much added depth is given at maximum pressure.", "Pressure");
			EndIndent();
			BeginError(Any(t => t.Hardness <= 0.0f));
				DrawDefault("hardness", "This allows you to control the sharpness of the near+far depth cut-off point.");
			EndError();
			BeginIndent();
				DrawDefault("hardnessPressure", "If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.", "Pressure");
			EndIndent();

			DrawDefault("normalFront", "This allows you to control how much the paint can wrap around the front of surfaces.\nFor example, if you want paint to wrap around curved surfaces then set this to a higher value.\nNOTE: If you set this to 0 then paint will not be applied to front facing surfaces.");
			DrawDefault("normalBack", "This works just like Normal Front, except for back facing surfaces.\nNOTE: If you set this to 0 then paint will not be applied to back facing surfaces.");
			DrawDefault("normalFade", "This allows you to control the smoothness of the depth cut-off point.");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to paint a decal at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintDecal")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint Decal")]
	public class P3dPaintDecal : MonoBehaviour, IHitHandler
	{
		public enum ColorStyles
		{
			SingleColor,
			RandomColor
		}

		/// <summary>The layers you want this paint to apply to.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>The groups you want this paint to apply to.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>If you only want to paint one specific texture, rather than all of them in the scene, then specify it here.</summary>
		public P3dPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private P3dPaintableTexture targetTexture;

		/// <summary>If you only want to paint one specific model/paintable, rather than all of them in the scene, then specify it here.</summary>
		public P3dModel TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private P3dModel targetModel;

		/// <summary>The style of blending.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode;

		/// <summary>The decal that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The shape of the decal when using Replace blending.</summary>
		public Texture Shape { set { shape = value; } get { return shape; } } [SerializeField] private Texture shape;

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

		/// <summary>Randomly rotate the decal?</summary>
		public bool RandomAngle { set { randomAngle = value; } get { return randomAngle; } } [SerializeField] private bool randomAngle;

		/// <summary>The angle of the decal in degrees.</summary>
		public float Angle { set { angle = value; } get { return angle; } } [Range(-180.0f, 180.0f)] [SerializeField] private float angle;

		/// <summary>Mirror decal?</summary>
		public bool Mirror { set { mirror = value; } get { return mirror; } } [SerializeField] private bool mirror;

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>If this is non-zero, then the radius will be multiplied by this, allowing you to manually control the size and aspect ratio of the decal.</summary>
		public Vector2 OverrideSize { set { overrideSize = value; } get { return overrideSize; } } [SerializeField] private Vector2 overrideSize;

		/// <summary>If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.</summary>
		public float RadiusPressure { set { radiusPressure = value; } get { return radiusPressure; } } [SerializeField] private float radiusPressure;

		/// <summary>This allows you to control how near+far from the hit point the decal will appear in world space.
		/// If you're painting a flat surface head-on then you can use a low value here, but if you're painting something complex then you may need to set this higher.</summary>
		public float Depth { set { depth = value; } get { return depth; } } [SerializeField] private float depth = 0.1f;

		/// <summary>If you want the depth to increase with finger pressure, this allows you to set how much added depth is given at maximum pressure.</summary>
		public float DepthPressure { set { depthPressure = value; } get { return depthPressure; } } [SerializeField] private float depthPressure;

		/// <summary>This allows you to control the sharpness of the near+far depth cut-off point.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 3.0f;

		/// <summary>If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.</summary>
		public float HardnessPressure { set { hardnessPressure = value; } get { return hardnessPressure; } } [SerializeField] private float hardnessPressure;

		/// <summary>This allows you to control how much the paint can wrap around the front of surfaces.
		/// For example, if you want paint to wrap around curved surfaces then set this to a higher value.
		/// NOTE: If you set this to 0 then paint will not be applied to front facing surfaces.</summary>
		public float NormalFront { set { normalFront = value; } get { return normalFront; } } [Range(0.0f, 1.0f)] [SerializeField] private float normalFront = 0.2f;

		/// <summary>This works just like <b>Normal Front</b>, except for back facing surfaces.
		/// NOTE: If you set this to 0 then paint will not be applied to back facing surfaces.</summary>
		public float NormalBack { set { normalBack = value; } get { return normalBack; } } [Range(0.0f, 1.0f)] [SerializeField] private float normalBack;

		/// <summary>This allows you to control the smoothness of the normal cut-off point.</summary>
		public float NormalFade { set { normalFade = value; } get { return normalFade; } } [Range(0.001f, 0.25f)] [SerializeField] private float normalFade = 0.01f;

		[System.NonSerialized]
		private Vector2 lastMousePosition;

		[ContextMenu("Toggle Mirror")]
		public void ToggleMirror()
		{
			mirror = !mirror;
		}

		public void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure)
		{
			var rotation = P3dHelper.NormalToCameraRotation(normal);

			HandleHit(position, rotation, preview, pressure);
		}

		public void HandleHit(Vector3 position, Quaternion rotation, bool preview, float pressure)
		{
			var finalAngle    = angle;
			var finalColor    = color;
			var finalOpacity  = opacity + (1.0f - opacity) * opacityPressure * pressure;
			var finalRadius   = radius + radiusPressure * pressure;
			var finalDepth    = depth + depthPressure * pressure;
			var finalHardness = hardness + hardnessPressure * pressure;

			if (randomAngle == true)
			{
				finalAngle = Random.Range(-180.0f, 180.0f);
			}

			if (colorStyle == ColorStyles.RandomColor && gradient != null)
			{
				finalColor = gradient.Evaluate(Random.value);
			}

			if (overrideSize != Vector2.zero)
			{
				P3dPainter.Decal.SetMatrix(position, rotation, finalAngle, overrideSize * finalRadius, texture, finalDepth, mirror);
			}
			else
			{
				P3dPainter.Decal.SetMatrix(position, rotation, finalAngle, finalRadius, texture, finalDepth, mirror);
			}

			P3dPainter.Decal.SetMaterial(blendMode, texture, finalHardness, normalBack, normalFront, normalFade, finalColor, finalOpacity, shape);

			// Paint something specific?
			if (targetModel != null || targetTexture != null)
			{
				P3dPainter.Decal.SubmitAll(preview, targetModel, targetTexture, groups);
			}
			// Paint everything?
			else
			{
				P3dPainter.Decal.SubmitAll(preview, layers, groups);
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = Matrix4x4.Translate(transform.position);

			Gizmos.DrawWireCube(Vector3.zero, new Vector3(radius, radius, depth));
		}
#endif
	}
}