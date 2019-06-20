using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dDragRaycast))]
	public class P3dDragRaycast_Editor : P3dEditor<P3dDragRaycast>
	{
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		protected override void OnInspector()
		{
			BeginError(Any(t => t.Layers == 0));
				DrawDefault("layers", "The layers you want the raycast to check.");
			EndError();
			BeginError(Any(t => t.DragStep < 0.0f));
				DrawDefault("dragStep", "If you want the paint to continuously apply while moving the mouse, this allows you to set how many pixels are between each step (0 = no drag).");
			EndError();
			DrawDefault("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			DrawDefault("useHitNormal", "Rotate the to the hit normal?");
			DrawDefault("showPreview", "Show a painting preview under the mouse?");
			DrawDefault("storeStates", "Should painting triggered from this component be eligible for being undone?");

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
	/// <summary>This component fires hit events when you click/tap, and also optionally when the mouse or finger drags across the screen at fixed pixel intervals.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dDragRaycast")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Drag Raycast")]
	public class P3dDragRaycast : MonoBehaviour
	{
		// This stores extra information for each finger unique to this component
		class Link
		{
			public P3dInputManager.Finger Finger;
			public Vector2                LastPoint;
		}

		/// <summary>The layers you want the raycast to check.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] protected LayerMask layers = Physics.DefaultRaycastLayers;

		/// <summary>If you want the paint to continuously apply while moving the mouse, this allows you to set how many pixels are between each step (0 = no drag).</summary>
		public float DragStep { set { dragStep = value; } get { return dragStep; } } [SerializeField] protected float dragStep = 5.0f;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] protected float offset;

		/// <summary>Rotate the to the hit normal?</summary>
		public bool UseHitNormal { set { useHitNormal = value; } get { return useHitNormal; } } [SerializeField] protected bool useHitNormal;

		/// <summary>Show a painting preview under the mouse?</summary>
		public bool ShowPreview { set { showPreview = value; } get { return showPreview; } } [SerializeField] protected bool showPreview;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] protected bool storeStates;

		[System.NonSerialized]
		protected static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		[System.NonSerialized]
		private List<Link> links = new List<Link>();

		protected virtual void LateUpdate()
		{
			// Use mouse hover preview?
			if (showPreview == true)
			{
				if (Input.touchCount == 0 && P3dInputManager.AnyMouseButtonSet == false && P3dInputManager.PointOverGui(Input.mousePosition) == false)
				{
					PaintAt(Input.mousePosition, true, 1.0f);
				}
			}

			var fingers = P3dInputManager.GetFingers(true);

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Index >= -1)
				{
					Paint(finger);
				}
			}
		}

		protected virtual void Paint(P3dInputManager.Finger finger)
		{
			var link = GetLink(finger);

			if (dragStep > 0.0f)
			{
				if (finger.Down == true)
				{
					link.LastPoint = finger.ScreenPosition;

					if (storeStates == true)
					{
						P3dStateManager.StoreAllStates();
					}

					PaintAt(finger.ScreenPosition, false, finger.Pressure);
				}

				var steps = CalculateSteps(finger.ScreenPosition - link.LastPoint);

				if (steps > 0)
				{
					var step           = P3dHelper.Reciprocal(steps);
					var screenPosition = default(Vector2);

					for (var i = 1; i <= steps; i++)
					{
						screenPosition = Vector2.Lerp(link.LastPoint, finger.ScreenPosition, i * step);

						PaintAt(screenPosition, false, finger.Pressure);
					}

					link.LastPoint = screenPosition;
				}
			}
			else
			{
				if (showPreview == true)
				{
					if (finger.Up == true)
					{
						if (storeStates == true)
						{
							P3dStateManager.StoreAllStates();
						}

						PaintAt(finger.ScreenPosition, false, finger.Pressure);
					}
					else
					{
						PaintAt(finger.ScreenPosition, true, finger.Pressure);
					}
				}
				else if (finger.Down == true)
				{
					if (storeStates == true)
					{
						P3dStateManager.StoreAllStates();
					}

					PaintAt(finger.ScreenPosition, false, finger.Pressure);
				}
			}
		}

		protected void PaintAt(Vector2 screenPosition, bool preview, float pressure)
		{
			var camera = P3dHelper.GetCamera();

			if (camera != null)
			{
				var hit = default(RaycastHit);
				var ray = camera.ScreenPointToRay(screenPosition);

				if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layers) == true)
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
						hitHandlers[i].HandleHit(point, normal, preview, pressure);
					}
				}
			}
		}

		protected int CalculateSteps(Vector2 delta)
		{
			if (dragStep > 0.0f)
			{
				var scaledDelta = delta.magnitude;
				//var scaledDelta = delta.magnitude * P3dInputManager.ScaleFactor;

				return Mathf.FloorToInt(scaledDelta / dragStep);
			}

			return 0;
		}

		private Link GetLink(P3dInputManager.Finger finger)
		{
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link = links[i];

				if (link.Finger == finger)
				{
					return link;
				}
			}

			var newLink = new Link();

			newLink.Finger = finger;

			links.Add(newLink);

			return newLink;
		}
	}
}