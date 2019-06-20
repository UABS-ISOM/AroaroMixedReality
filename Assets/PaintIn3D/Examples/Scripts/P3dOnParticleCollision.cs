using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dOnParticleCollision))]
	public class P3dOnParticleCollision_Editor : P3dEditor<P3dOnParticleCollision>
	{
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		protected override void OnInspector()
		{
			DrawDefault("useHitNormal", "Use the normal of the surface the particle hit, or the particle velocity?");

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
	/// <summary>This can be added to any ParticleSystem to listen for and send off hit events, so you can paint things at the hit point.</summary>
	[RequireComponent(typeof(ParticleSystem))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dOnParticleCollision")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "On Particle Collision")]
	public class P3dOnParticleCollision : MonoBehaviour
	{
		/// <summary>Use the normal of the surface the particle hit, or the particle velocity?</summary>
		public bool UseHitNormal { set { useHitNormal = value; } get { return useHitNormal; } } [SerializeField] private bool useHitNormal;

		[System.NonSerialized]
		private ParticleSystem cachedParticleSystem;

		[System.NonSerialized]
		private static List<ParticleCollisionEvent> particleCollisionEvents = new List<ParticleCollisionEvent>();

		[System.NonSerialized]
		private List<IHitHandler> hitHandlers = new List<IHitHandler>();

		protected virtual void OnEnable()
		{
			cachedParticleSystem = GetComponent<ParticleSystem>();
		}

		protected virtual void OnParticleCollision(GameObject hitGameObject)
		{
			if (hitHandlers.Count == 0)
			{
				GetComponentsInChildren(hitHandlers);
			}

			// Get the collision events array
			var count = cachedParticleSystem.GetSafeCollisionEventSize();

			// Expand collisionEvents list to fit all particles
			for (var i = particleCollisionEvents.Count; i < count; i++)
			{
				particleCollisionEvents.Add(new ParticleCollisionEvent());
			}

			count = cachedParticleSystem.GetCollisionEvents(hitGameObject, particleCollisionEvents);

			// Paint the surface next to the collision intersection point
			for (var i = 0; i < count; i++)
			{
				var collisionEvent = particleCollisionEvents[i];
				var position       = collisionEvent.intersection;
				var normal         = useHitNormal == true ? collisionEvent.normal : -collisionEvent.velocity;

				for (var j = 0; j < hitHandlers.Count; j++)
				{
					hitHandlers[j].HandleHit(position, normal, false, 1.0f);
				}
			}
		}
	}
}