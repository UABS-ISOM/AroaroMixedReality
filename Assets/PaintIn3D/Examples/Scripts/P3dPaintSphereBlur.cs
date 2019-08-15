using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintSphereBlur))]
	public class P3dPaintSphereBlur_Editor : P3dEditor<P3dPaintSphereBlur>
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

			BeginError(Any(t => t.Radius <= 0.0f));
				DrawDefault("radius", "The radius of the paint brush.");
			EndError();
			BeginIndent();
				DrawDefault("radiusPressure", "If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.", "Pressure");
			EndIndent();
			DrawDefault("kernelSize", "The blur kernel size.");
			BeginIndent();
				DrawDefault("kernelSizePressure", "If you want the kernel size to increase with finger pressure, this allows you to set how much added kernel size is given at maximum pressure.", "Pressure");
			EndIndent();

			Separator();

			DrawDefault("opacity", "The opacity of the brush.");
			BeginIndent();
				DrawDefault("opacityPressure", "If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.", "Pressure");
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
	/// <summary>This allows you to blur a sphere at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintSphereBlur")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint Sphere Blur")]
	public class P3dPaintSphereBlur : MonoBehaviour, IHitHandler
	{
		/// <summary>The layers you want this paint to apply to.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>The layers you want this paint to apply to.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>If you only want to paint one specific texture, rather than all of them in the scene, then specify it here.</summary>
		public P3dPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private P3dPaintableTexture targetTexture;

		/// <summary>If you only want to paint one specific model/paintable, rather than all of them in the scene, then specify it here.</summary>
		public P3dModel TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private P3dModel targetModel;

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.</summary>
		public float RadiusPressure { set { radiusPressure = value; } get { return radiusPressure; } } [SerializeField] private float radiusPressure;

		/// <summary>The blur kernel size.</summary>
		public float KernelSize { set { kernelSize = value; } get { return kernelSize; } } [SerializeField] private float kernelSize = 0.001f;

		/// <summary>If you want the kernel size to increase with finger pressure, this allows you to set how much added kernel size is given at maximum pressure.</summary>
		public float KernelSizePressure { set { kernelSizePressure = value; } get { return kernelSizePressure; } } [SerializeField] private float kernelSizePressure;

		/// <summary>The hardness of the paint brush.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 1.0f;

		/// <summary>If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.</summary>
		public float HardnessPressure { set { hardnessPressure = value; } get { return hardnessPressure; } } [SerializeField] private float hardnessPressure;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.</summary>
		public float OpacityPressure { set { opacityPressure = value; } get { return opacityPressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacityPressure;

		/// <summary>This allows you to paint a decal at the specified point.</summary>
		public void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure)
		{
			var finalRadius     = radius + radiusPressure * pressure;
			var finalHardness   = hardness + hardnessPressure * pressure;
			var finalOpacity    = opacity + (1.0f - opacity) * opacityPressure * pressure;
			var finalKernelSize = kernelSize + kernelSizePressure * pressure;

			P3dPainter.SphereBlur.SetMatrix(position, finalRadius);
			P3dPainter.SphereBlur.SetMaterial(finalHardness, finalOpacity, finalKernelSize);

			// Paint something specific?
			if (targetModel != null || targetTexture != null)
			{
				P3dPainter.SphereBlur.SubmitAll(preview, targetModel, targetTexture, groups);
			}
			// Paint everything?
			else
			{
				P3dPainter.SphereBlur.SubmitAll(preview, layers, groups);
			}
		}
	}
}