using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dPaintSphereColor))]
	public class P3dPaintSphereColor_Editor : P3dEditor<P3dPaintSphereColor>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.PaintSphere == null));
				DrawDefault("paintSphere", "The color of the paint.");
			EndError();
			DrawDefault("color", "The color of the paint.");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to change the P3dPaintSphere.Color property from unity events (e.g. buttons), because Unity doesn't allow you to directly set colors.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintSphereColor")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint Sphere Color")]
	public class P3dPaintSphereColor : MonoBehaviour
	{
		/// <summary>The color of the paint.</summary>
		public P3dPaintSphere PaintSphere { set { paintSphere = value; } get { return paintSphere; } } [SerializeField] private P3dPaintSphere paintSphere;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>This will apply the color.</summary>
		public void Apply()
		{
			if (paintSphere != null)
			{
				paintSphere.Color = color;
			}
		}
	}
}