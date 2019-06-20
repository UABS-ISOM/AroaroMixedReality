using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dScatterSpawner))]
	public class P3dScatterSpawner_Editor : P3dEditor<P3dScatterSpawner>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				DrawDefault("prefab", "The prefab that will be spawned.");
			EndError();
			DrawDefault("offset", "The offset from the hit point based on the normal in world space.");
			DrawDefault("radius", "The maximum radius from the offset point the prefab can be spawned at in world space.");
			DrawDefault("count", "The amount of prefabs that wil be spawned.");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to spawn a prefab at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dScatterSpawner")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Scatter Spawner")]
	public class P3dScatterSpawner : MonoBehaviour, IHitHandler
	{
		/// <summary>The prefab that will be spawned.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		/// <summary>The offset from the hit point based on the normal in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>The maximum radius from the offset point the prefab can be spawned at in world space.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.5f;

		/// <summary>The amount of prefabs that wil be spawned.</summary>
		public int Count { set { count = value; } get { return count; } } [SerializeField] private int count = 10;

		public void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure)
		{
			if (prefab != null)
			{
				var center   = position + normal * offset;
				var rotation = Quaternion.LookRotation(normal);

				for (var i = 0; i < count; i++)
				{
					var splat = rotation * Random.insideUnitCircle * radius;

					Instantiate(prefab, center + splat, rotation, default(Transform));
				}
			}
		}
	}
}