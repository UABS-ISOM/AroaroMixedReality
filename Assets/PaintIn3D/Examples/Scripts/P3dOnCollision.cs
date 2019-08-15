using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dOnCollision))]
	public class P3dOnCollision_Editor : P3dEditor<P3dOnCollision>
	{
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		protected override void OnInspector()
		{
			DrawDefault("threshold", "The relative speed required for a paint to occur.");
			DrawDefault("onlyUseFirstContact", "If there are multiple contact points, skip them?");
			BeginError(Any(t => t.Delay < 0.0f));
				DrawDefault("delay", "The time in seconds between each collision if you want to limit it.");
			EndError();

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
	/// <summary>This can be added to any Rigidbody to listen for and send off hit events, so you can paint things at the hit point.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dOnCollision")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "On Collision")]
	public class P3dOnCollision : MonoBehaviour
	{
		/// <summary>The relative speed required for a paint to occur.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [SerializeField] private float threshold = 1.0f;

		/// <summary>If there are multiple contact points, skip them?</summary>
		public bool OnlyUseFirstContact { set { onlyUseFirstContact = value; } get { return onlyUseFirstContact; } } [SerializeField] private bool onlyUseFirstContact = true;

		/// <summary>The time in seconds between each collision if you want to limit it.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay;

		[System.NonSerialized]
		private List<IHitHandler> hitHandlers = new List<IHitHandler>();

		[SerializeField]
		private float cooldown;

		protected virtual void OnCollisionEnter(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}

		private void CheckCollision(Collision collision)
		{
			if (cooldown > 0.0f)
			{
				return;
			}

			if (hitHandlers.Count == 0)
			{
				GetComponentsInChildren(hitHandlers);
			}

			// Only handle the collision if the impact was strong enough
			if (collision.relativeVelocity.magnitude > threshold)
			{
				var contacts = collision.contacts;

				cooldown = delay;

				for (var i = contacts.Length - 1; i >= 0; i--)
				{
					var contact  = contacts[i];
					var position = contact.point;
					var normal   = contact.normal;

					for (var j = 0; j < hitHandlers.Count; j++)
					{
						hitHandlers[j].HandleHit(position, normal, false, 1.0f);
					}

					if (onlyUseFirstContact == true)
					{
						break;
					}
				}
			}
		}
	}
}