using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dSprayCan))]
	public class P3dSprayCan_Editor : P3dEditor<P3dSprayCan>
	{
		protected override void OnInspector()
		{
			DrawDefault("particles", "The particle system that will be enabled/disabled based on mouse/touch.");
			DrawDefault("aimSensitivity", "The amount the spray can rotates relative to the mouse/finger position on screen.");
			DrawDefault("storeStates", "Should painting triggered from this component be eligible for being undone?");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component fires hit events when you click/tap, and also optionally when the mouse or finger drags across the screen at fixed pixel intervals.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dSprayCan")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Spray Can")]
	public class P3dSprayCan : MonoBehaviour
	{
		/// <summary>The particle system that will be enabled/disabled based on mouse/touch.</summary>
		public ParticleSystem Particles { set { particles = value; } get { return particles; } } [SerializeField] private ParticleSystem particles;

		/// <summary>The amount the spray can rotates relative to the mouse/finger position on screen.</summary>
		public float AimSensitivity { set { aimSensitivity = value; } get { return aimSensitivity; } } [SerializeField] private float aimSensitivity = 30.0f;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] protected bool storeStates;

		protected virtual void LateUpdate()
		{
			if (particles != null)
			{
				var mousePosition = (Vector2)Input.mousePosition;

				if (Input.GetMouseButton(0) == true && P3dInputManager.PointOverGui(mousePosition) == false)
				{
					if (storeStates == true && particles.isPlaying == false)
					{
						P3dStateManager.StoreAllStates();
					}

					particles.Play();
				}
				else
				{
					particles.Stop();
				}
			}

			if (aimSensitivity != 0.0f)
			{
				var sensitivity = AimSensitivity / Screen.width;
				var aimX        = (Input.mousePosition.y - Screen.width  * 0.5f) * sensitivity;
				var aimY        = (Input.mousePosition.x - Screen.height * 0.5f) * sensitivity;

				transform.localRotation = Quaternion.Euler(-aimX, aimY, 0.0f);
			}
		}
	}
}