﻿using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dTapThrow))]
	public class P3dTapThrow_Editor : P3dEditor<P3dTapThrow>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				DrawDefault("prefab", "The prefab that will be thrown.");
			EndError();
			DrawDefault("speed", "Rotate the decal to the hit normal?");
			DrawDefault("storeStates", "Should painting triggered from this component be eligible for being undone?");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component will spawn and throw Rigidbody prefabs from the camera when you tap the mouse or a finger.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dTapThrow")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Tap Throw")]
	public class P3dTapThrow : MonoBehaviour
	{
		/// <summary>The prefab that will be thrown.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		/// <summary>The speed that the object will be thrown at.</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 10.0f;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] protected bool storeStates;

		protected virtual void Update()
		{
			if (prefab != null && Input.GetMouseButtonDown(0) == true && P3dInputManager.PointOverGui(Input.mousePosition) == false)
			{
				var camera = P3dHelper.GetCamera();

				if (camera != null)
				{
					if (storeStates == true)
					{
						P3dStateManager.StoreAllStates();
					}

					// Find the ray for this screen position
					var ray      = camera.ScreenPointToRay(Input.mousePosition);
					var rotation = Quaternion.LookRotation(ray.direction);

					// Loop through all prefabs and spawn them
					var clone = Instantiate(prefab, ray.origin, rotation);

					// Throw with velocity?
					var cloneRigidbody = clone.GetComponent<Rigidbody>();

					if (cloneRigidbody != null)
					{
						cloneRigidbody.velocity = clone.transform.forward * Speed;
					}
				}
			}
		}
	}
}