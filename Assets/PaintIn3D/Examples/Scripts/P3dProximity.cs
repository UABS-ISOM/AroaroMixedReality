using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dProximity))]
	public class P3dProximity_Editor : P3dEditor<P3dProximity>
	{
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		protected override void OnInspector()
		{
			BeginError(Any(t => t.Delay <= 0.0f));
				DrawDefault("delay", "The time in seconds between each raycast.");
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
	/// <summary>This component raycasts between two points, and fires hit events when the ray hits something.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dProximity")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Proximity")]
	public class P3dProximity : MonoBehaviour
	{
		/// <summary>The time in seconds between each raycast.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 0.05f;

		[System.NonSerialized]
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		[System.NonSerialized]
		private float current;

		protected virtual void FixedUpdate()
		{
			if (delay > 0.0f)
			{
				current += Time.fixedDeltaTime;

				if (current >= delay)
				{
					current %= delay;

					var position = transform.position;
					var normal   = transform.forward;

					GetComponentsInChildren(hitHandlers);

					for (var i = 0; i < hitHandlers.Count; i++)
					{
						hitHandlers[i].HandleHit(position, normal, false, 1.0f);
					}
				}
			}
		}
	}
}