using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dInputManager))]
	public class P3dInputManager_Editor : P3dEditor<P3dInputManager>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component automatically converts mouse and touch inputs into a simple format.", MessageType.Info);
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component converts mouse and touch inputs into a single interface.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dInputManager")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Input Manager")]
	public class P3dInputManager : P3dLinkedBehaviour<P3dInputManager>
	{
		public class Finger
		{
			public int     Index;
			public bool    Marked;
			public float   Pressure;
			public bool    Down;
			public bool    Set;
			public bool    Up;
			public bool    StartedOverGui;
			public Vector2 StartScreenPosition;
			public Vector2 LastScreenPosition;
			public Vector2 ScreenPosition;
		}

		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		private static List<Finger> fingers = new List<Finger>();

		private static List<Finger> tempFingers = new List<Finger>();

		public static float ScaleFactor
		{
			get
			{
				var dpi = Screen.dpi;

				if (dpi <= 0)
				{
					dpi = 200.0f;
				}

				return 200.0f / dpi;
			}
		}

		public static bool AnyMouseButtonSet
		{
			get
			{
				for (var i = 0; i < 4; i++)
				{
					if (Input.GetMouseButton(i) == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public static List<Finger> GetFingers(bool ignoreIfStartedOverGui)
		{
			if (InstanceCount == 0)
			{
				new GameObject(typeof(P3dInputManager).Name).AddComponent<P3dInputManager>();
			}

			tempFingers.Clear();

			for (var i = 0; i < fingers.Count; i++)
			{
				var finger = fingers[i];

				if (finger.Set == false)
				{
					continue;
				}

				if (ignoreIfStartedOverGui == true && finger.StartedOverGui == true)
				{
					continue;
				}

				tempFingers.Add(finger);
			}

			return tempFingers;
		}

		public static Vector2 GetScaledDelta(List<Finger> fingers)
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.ScreenPosition - finger.LastScreenPosition;
					count += 1;
				}
			}

			if (count > 0)
			{
				total *= ScaleFactor;
				total /= count;

			}

			return total;
		}

		public static Vector2 GetLastScreenCenter(List<Finger> fingers)
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.LastScreenPosition;
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static Vector2 GetScreenCenter(List<Finger> fingers)
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.ScreenPosition;
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static float GetScreenDistance(List<Finger> fingers, Vector2 center)
		{
			var total = 0.0f;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += Vector2.Distance(center, finger.ScreenPosition);
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static float GetLastScreenDistance(List<Finger> fingers, Vector2 center)
		{
			var total = 0.0f;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += Vector2.Distance(center, finger.LastScreenPosition);
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static float GetPinchScale(List<Finger> fingers, float wheelSensitivity = 0.0f)
		{
			var center       = GetScreenCenter(fingers);
			var lastCenter   = GetLastScreenCenter(fingers);
			var distance     = GetScreenDistance(fingers, center);
			var lastDistance = GetLastScreenDistance(fingers, lastCenter);

			if (lastDistance > 0.0f)
			{
				return distance / lastDistance;
			}

			if (wheelSensitivity != 0.0f)
			{
				var scroll = Input.mouseScrollDelta.y;

				if (scroll > 0.0f)
				{
					return 1.0f - wheelSensitivity;
				}

				if (scroll < 0.0f)
				{
					return 1.0f + wheelSensitivity;
				}
			}

			return 1.0f;
		}

		public static bool PointOverGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition).Count > 0;
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition, 1 << 5);
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();

			var currentEventSystem = EventSystem.current;

			if (currentEventSystem != null)
			{
				// Create point event data for this event system?
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				// Raycast event system at the specified point
				tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

				// Loop through all results and remove any that don't match the layer mask
				if (tempRaycastResults.Count > 0)
				{
					for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
					{
						var raycastResult = tempRaycastResults[i];
						var raycastLayer  = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(i);
						}
					}
				}
			}

			return tempRaycastResults;
		}

		protected override void OnEnable()
		{
			if (InstanceCount > 0)
			{
				Debug.LogWarning("Your scene already contains an instance of " + typeof(P3dInputManager).Name + "!", FirstInstance);
			}

			base.OnEnable();
		}

		protected virtual void Update()
		{
			if (this == FirstInstance)
			{
				Mark();
				Poll();
				Sweep();
			}
		}

		private void Mark()
		{
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Up == true)
				{
					finger.Up  = false;
					finger.Set = false;
				}

				finger.Marked = true;
			}
		}

		private void Sweep()
		{
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Marked == true && finger.Set == true)
				{
					finger.Up = true;
				}
			}
		}

		private void Poll()
		{
			// Update real fingers
			if (Input.touchCount > 0)
			{
				for (var i = 0; i < Input.touchCount; i++)
				{
					var touch = Input.GetTouch(i);

					if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
					{
						AddFinger(touch.fingerId, touch.position, touch.pressure);
					}
				}
			}
			// If there are no real touches, simulate some from the mouse?
			else
			{
				var screen        = new Rect(0, 0, Screen.width, Screen.height);
				var mousePosition = (Vector2)Input.mousePosition;

				for (var i = 0; i < 4; i++)
				{
					// Is the mouse within the screen?
					if (Input.GetMouseButton(i) == true && screen.Contains(mousePosition) == true)
					{
						AddFinger(-1 - i, mousePosition, 1.0f);
					}
				}
			}
		}

		private void AddFinger(int index, Vector2 screenPosition, float pressure)
		{
			var finger = GetFinger(index);

			if (finger.Set == true)
			{
				finger.Index              = index;
				finger.Marked             = false;
				finger.Pressure           = pressure;
				finger.Down               = false;
				finger.Set                = true;
				finger.Up                 = false;
				//finger.StartedOverGui      = PointOverGui(screenPosition);
				//finger.StartScreenPosition = screenPosition;
				finger.LastScreenPosition = finger.ScreenPosition;
				finger.ScreenPosition     = screenPosition;
			}
			else
			{
				finger.Index               = index;
				finger.Marked              = false;
				finger.Pressure            = pressure;
				finger.Down                = true;
				finger.Set                 = true;
				finger.Up                  = false;
				finger.StartedOverGui      = PointOverGui(screenPosition);
				finger.StartScreenPosition = screenPosition;
				finger.LastScreenPosition  = screenPosition;
				finger.ScreenPosition      = screenPosition;
			}
		}

		private Finger GetFinger(int index)
		{
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Index == index)
				{
					return finger;
				}
			}

			var newFinger = new Finger();

			newFinger.Index = index;

			fingers.Add(newFinger);

			return newFinger;
		}
	}
}