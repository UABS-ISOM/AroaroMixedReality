using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintSphere_Modify))]
	public class P3dPaintSphere_Modify_Editor : P3dEditor<P3dPaintSphere_Modify>
	{
		protected override void OnInspector()
		{
			DrawDefault("progress", "The current progress of the modifications, where 0 is no progress, and 1 is completed.");
			DrawDefault("speed", "The speed of the progress change.\n1.0 = 1 second\n2 = 0.5 seconds");
			DrawDefault("autoDestroy", "Automatically destroy the specified GameObject when progress reaches 1?");

			Separator();

			DrawDefault("radius", "Should the P3dPaintSphere.Radius value be modified?");

			if (Any(t => t.Radius == true))
			{
				DrawDefault("radiusMin", "The P3dPaintSphere.Radius value when progress is 0.");
				DrawDefault("radiusMax", "The P3dPaintSphere.Radius value when progress is 1.");
				DrawDefault("radiusCurve", "This allows you to adjust the ease of the radius value.");
			}
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to paint a sphere at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[RequireComponent(typeof(P3dPaintSphere))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintSphere_Modify")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint Sphere Modify")]
	public class P3dPaintSphere_Modify : MonoBehaviour
	{
		/// <summary>The current progress of the modifications, where 0 is no progress, and 1 is completed.</summary>
		public float Progress { set { progress = value; } get { return progress; } } [Range(0.0f, 1.0f)] [SerializeField] private float progress;

		/// <summary>The speed of the progress change.\n1.0 = 1 second\n2 = 0.5 seconds</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 1.0f;

		/// <summary>Automatically destroy the specified GameObject when progress reaches 1?</summary>
		public GameObject AutoDestroy { set { autoDestroy = value; } get { return autoDestroy; } } [SerializeField] private GameObject autoDestroy;

		/// <summary>Should the P3dPaintSphere.Radius value be modified?</summary>
		public bool Radius { set { radius = value; } get { return radius; } } [SerializeField] private bool radius;

		/// <summary>The P3dPaintSphere.Radius value when progress is 0.</summary>
		public float RadiusMin { set { radiusMin = value; } get { return radiusMin; } } [SerializeField] private float radiusMin = 0.25f;

		/// <summary>The P3dPaintSphere.Radius value when progress is 1.</summary>
		public float RadiusMax { set { radiusMax = value; } get { return radiusMax; } } [SerializeField] private float radiusMax = 1.0f;

		/// <summary>This allows you to adjust the ease of the radius value.</summary>
		public AnimationCurve RadiusCurve { set { radiusCurve = value; } get { return radiusCurve; } } [SerializeField] private AnimationCurve radiusCurve;

		[System.NonSerialized]
		private P3dPaintSphere cachedPaintSphere;

		protected virtual void OnEnable()
		{
			if (cachedPaintSphere == null)
			{
				cachedPaintSphere = GetComponent<P3dPaintSphere>();
			}
		}

		protected virtual void Update()
		{
			progress += speed * Time.deltaTime;

			if (radius == true)
			{
				cachedPaintSphere.Radius = Mathf.Lerp(radiusMin, radiusMax, radiusCurve.Evaluate(progress));
			}

			if (progress >= 1.0f && autoDestroy != null)
			{
				Destroy(autoDestroy);
			}
		}
	}
}