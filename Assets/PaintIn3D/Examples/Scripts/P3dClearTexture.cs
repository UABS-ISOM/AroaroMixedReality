using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dClearTexture))]
	public class P3dClearTexture_Editor : P3dEditor<P3dClearTexture>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.PaintableTexture == null));
				DrawDefault("paintableTexture", "This is the paintable texture whose pixels we will count.");
			EndError();
			DrawDefault("texture", "The texture that will be faded toward.");
			DrawDefault("color", "The color that will be faded toward.");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to clear a paintable texture with the specified texture and color.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dClearTexture")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Clear Texture")]
	public class P3dClearTexture : MonoBehaviour
	{
		/// <summary>This is the paintable texture whose pixels we will fade.</summary>
		public P3dPaintableTexture PaintableTexture { set { paintableTexture = value; } get { return paintableTexture; } } [SerializeField] private P3dPaintableTexture paintableTexture;

		/// <summary>The texture that will be faded toward.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color that will be faded toward.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		[ContextMenu("Clear")]
		public void Clear()
		{
			if (paintableTexture != null && paintableTexture.Activated == true)
			{
				paintableTexture.Clear(texture, color);
			}
		}
	}
}