using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dBetweenRaycast))]
	public class P3dBetweenRaycast_Editor : P3dEditor<P3dBetweenRaycast>
	{
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		protected override void OnInspector()
		{
			BeginError(Any(t => t.PointA == null));
				DrawDefault("pointA", "The start point of the raycast.");
			EndError();
			BeginError(Any(t => t.PointB == null));
				DrawDefault("pointB", "The end point of the raycast.");
			EndError();
			BeginError(Any(t => t.Layers == 0));
				DrawDefault("layers", "The layers you want the raycast to hit.");
			EndError();
			DrawDefault("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			DrawDefault("useHitNormal", "Rotate the to the hit normal?");
			BeginError(Any(t => t.Delay < 0.0f));
				DrawDefault("delay", "The time in seconds between each raycast.");
			EndError();
			DrawDefault("preview", "Should the applied paint be applied as a preview?");

			Separator();

			Target.GetComponentsInChildren(hitHandlers);

			for (var i = 0; i < hitHandlers.Count; i++)
			{
				EditorGUILayout.HelpBox("This component is sending hit events to " + hitHandlers[i], MessageType.Info);
			}

			if (hitHandlers.Count == 0)
			{
				EditorGUILayout.HelpBox("This component is sending hit events to nothing.", MessageType.Warning);
			}
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component raycasts between two points, and fires hit events when the ray hits something.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dBetweenRaycast")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Between Raycast")]
	public class P3dBetweenRaycast : MonoBehaviour
	{
		/// <summary>The start point of the raycast.</summary>
		public Transform PointA { set { pointA = value; } get { return pointA; } } [SerializeField] private Transform pointA;

		/// <summary>The end point of the raycast.</summary>
		public Transform PointB { set { pointB = value; } get { return pointB; } } [SerializeField] private Transform pointB;

		/// <summary>The layers you want the raycast to hit.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = Physics.DefaultRaycastLayers;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>Rotate the to the hit normal?</summary>
		public bool UseHitNormal { set { useHitNormal = value; } get { return useHitNormal; } } [SerializeField] private bool useHitNormal;

		/// <summary>The time in seconds between each raycast.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 0.05f;

		/// <summary>Should the applied paint be applied as a preview?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		[System.NonSerialized]
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		[System.NonSerialized]
		private float current;

		protected virtual void LateUpdate()
		{
			if (pointA != null && pointB != null)
			{
				current += Time.deltaTime;

				if (current >= delay)
				{
					var vector = pointB.position - pointA.position;
					var ray    = new Ray(pointA.position, vector);
					var hit    = default(RaycastHit);

					if (Physics.Raycast(ray, out hit, vector.magnitude, layers) == true)
					{
						var point  = hit.point;
						var normal = -ray.direction;

						if (offset != 0.0f)
						{
							point += hit.normal * offset;
						}

						if (useHitNormal == true)
						{
							normal = hit.normal;
						}

						GetComponentsInChildren(hitHandlers);

						for (var i = 0; i < hitHandlers.Count; i++)
						{
							hitHandlers[i].HandleHit(point, normal, preview, 1.0f);
						}
					}
				}
			}
		}
	}
}