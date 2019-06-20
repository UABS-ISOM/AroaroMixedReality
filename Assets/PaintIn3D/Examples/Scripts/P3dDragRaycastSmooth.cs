using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dDragRaycastSmooth))]
	public class P3dDragRaycastSmooth_Editor : P3dEditor<P3dDragRaycastSmooth>
	{
		private static List<IHitHandler> hitHandlers = new List<IHitHandler>();

		protected override void OnInspector()
		{
			BeginError(Any(t => t.Layers == 0));
				DrawDefault("layers", "The layers you want the raycast to check.");
			EndError();
			BeginError(Any(t => t.DragStep <= 0.0f));
				DrawDefault("dragStep", "This allows you to set how many pixels are between each step (0 = no drag).");
			EndError();
			BeginError(Any(t => t.MinimumStep < 1));
				DrawDefault("minimumStep", "If you want the paint to continuously apply while moving the mouse, this allows you to set how many pixels are between each step (0 = no drag).");
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
	/// <summary>This component does the same thing as P3dDragRaycast, but it uses hermite interpolation to smooth the points.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dDragRaycastSmooth")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Drag Raycast Smooth")]
	public class P3dDragRaycastSmooth : P3dDragRaycast
	{
		// This stores extra information for each finger unique to this component
		class Link
		{
			public P3dInputManager.Finger Finger;
			public bool                   StartDrawn;
			public Vector2                LastPointA;
			public Vector2                LastPointB;
			public Vector2                LastPointC;
			public Vector2                LastPointD;
		}

		/// <summary>The higher this value, the smoother small painting movements will become.</summary>
		public int MinimumStep { set { minimumStep = value; } get { return minimumStep; } } [SerializeField] private int minimumStep = 3;

		[System.NonSerialized]
		private List<Link> links = new List<Link>();

		protected override void Paint(P3dInputManager.Finger finger)
		{
			var link = GetLink(finger);

			if (finger.Down == true)
			{
				link.StartDrawn = false;

				link.LastPointA = link.LastPointB = link.LastPointC = link.LastPointD = finger.ScreenPosition;

				PaintAt(finger.ScreenPosition, false, finger.Pressure);
			}

			if (CalculateSteps(finger.ScreenPosition - link.LastPointD) >= minimumStep || finger.Up == true)
			{
				link.LastPointA = link.LastPointB;
				link.LastPointB = link.LastPointC;
				link.LastPointC = link.LastPointD;
				link.LastPointD = finger.ScreenPosition;

				if (link.StartDrawn == false)
				{
					link.StartDrawn = true;

					Draw(link.LastPointA, link.LastPointA, link.LastPointB, link.LastPointC, finger.Pressure);
				}

				Draw(link.LastPointA, link.LastPointB, link.LastPointC, link.LastPointD, finger.Pressure);
			}

			if (finger.Up == true)
			{
				Draw(link.LastPointB, link.LastPointC, link.LastPointD, link.LastPointD, finger.Pressure);
			}
		}

		private void Draw(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float pressure)
		{
			var steps = CalculateSteps(b - c);
			var step  = P3dHelper.Reciprocal(steps);

			for (var i = 1; i <= steps; i++)
			{
				var screenPosition = Hermite(a, b, c, d, i * step);

				PaintAt(screenPosition, false, pressure);
			}
		}

		private static Vector2 Hermite(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
		{
			var mu2 = t * t;
			var mu3 = mu2 * t;
			var x   = HermiteInterpolate(a.x, b.x, c.x, d.x, t, mu2, mu3);
			var y   = HermiteInterpolate(a.y, b.y, c.y, d.y, t, mu2, mu3);

			return new Vector2(x, y);
		}

		private static float HermiteInterpolate(float y0,float y1, float y2,float y3, float mu, float mu2, float mu3)
		{
			var m0 = (y1 - y0) * 0.5f + (y2 - y1) * 0.5f;
			var m1 = (y2 - y1) * 0.5f + (y3 - y2) * 0.5f;
			var a0 =  2.0f * mu3 - 3.0f * mu2 + 1.0f;
			var a1 =         mu3 - 2.0f * mu2 + mu;
			var a2 =         mu3 -        mu2;
			var a3 = -2.0f * mu3 + 3.0f * mu2;

			return(a0*y1+a1*m0+a2*m1+a3*y2);
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