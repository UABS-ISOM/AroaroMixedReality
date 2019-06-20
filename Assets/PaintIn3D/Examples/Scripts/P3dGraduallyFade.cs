using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dGraduallyFade))]
	public class P3dGraduallyFade_Editor : P3dEditor<P3dGraduallyFade>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.PaintableTexture == null));
				DrawDefault("paintableTexture", "This is the paintable texture whose pixels we will count.");
			EndError();
			DrawDefault("blendMode", "The style of blending.");
			DrawDefault("texture", "The texture that will be faded toward.");
			DrawDefault("color", "The color that will be faded toward.");

			Separator();

			BeginError(Any(t => t.Threshold <= 0.0f));
				DrawDefault("threshold", "The radius of the paint brush.");
			EndError();
			BeginError(Any(t => t.Speed <= 0.0f));
				DrawDefault("speed", "The speed of the fading, where 1 = 1 second.");
			EndError();
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to fade the pixels of the specified P3dPaintableTexture.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dGraduallyFade")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Gradually Fade")]
	public class P3dGraduallyFade : MonoBehaviour
	{
		/// <summary>This is the paintable texture whose pixels we will fade.</summary>
		public P3dPaintableTexture PaintableTexture { set { paintableTexture = value; } get { return paintableTexture; } } [SerializeField] private P3dPaintableTexture paintableTexture;

		/// <summary>The style of blending.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode;

		/// <summary>The texture that will be faded toward.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color that will be faded toward.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The amount of change that .</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [Range(0.0f, 1.0f)] [SerializeField] private float threshold = 0.1f;

		/// <summary>The speed of the fading, where 1 = 1 second.</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 1.0f;

		[SerializeField]
		private float counter;

		protected virtual void Update()
		{
			if (paintableTexture != null && paintableTexture.Activated == true)
			{
				if (speed > 0.0f)
				{
					counter += speed * Time.deltaTime;
				}

				if (counter >= threshold)
				{
					var step = Mathf.FloorToInt(counter * 255.0f);

					if (step > 0)
					{
						var change  = step / 255.0f;
						var current = paintableTexture.Current;

						counter -= change;

						if (P3dPainter.Fill.Blit(blendMode, ref current, texture, color, Mathf.Min(change, 1.0f)) == true)
						{
							paintableTexture.Current = current;
						}

						paintableTexture.NotifyOnModified(false);
					}
				}
			}
		}
	}
}