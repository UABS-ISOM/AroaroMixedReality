using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dSpawner))]
	public class P3dSpawner_Editor : P3dEditor<P3dSpawner>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				DrawDefault("prefab", "The prefab that will be spawned.");
			EndError();
			DrawDefault("offset", "The offset from the hit point based on the normal in world space.");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to spawn a prefab at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dSpawner")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Spawner")]
	public class P3dSpawner : MonoBehaviour, IHitHandler
	{
		/// <summary>The prefab that will be spawned.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		/// <summary>The offset from the hit point based on the normal in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>Call this if you want to manually spawn the specified prefab.</summary>
		public void Spawn()
		{
			Spawn(transform.position, transform.rotation);
		}

		public void Spawn(Vector3 position, Vector3 normal)
		{
			Spawn(position, Quaternion.LookRotation(normal));
		}

		public void Spawn(Vector3 position, Quaternion rotation)
		{
			if (prefab != null)
			{
				Instantiate(prefab, position, transform.rotation, default(Transform));
			}
		}

		public void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure)
		{
			Spawn(position + normal * offset, Quaternion.LookRotation(normal));
		}
	}
}