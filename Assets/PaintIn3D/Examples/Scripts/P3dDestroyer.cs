using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dDestroyer))]
	public class P3dDestroyer_Editor : P3dEditor<P3dDestroyer>
	{
		protected override void OnInspector()
		{
			DrawDefault("seconds", "The remaining seconds until this GameObject gets destroyed (-1 if never).");
			DrawDefault("secondsPerHit", "The amount of seconds that get decremented each time this gets hit by something.");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component auatomatically destroys this GameObject after some time.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dDestroyer")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Destroyer")]
	public class P3dDestroyer : MonoBehaviour, IHitHandler
	{
		/// <summary>The remaining seconds until this GameObject gets destroyed (-1 if never).</summary>
		public float Seconds { set { seconds = value; } get { return seconds; } } [SerializeField] private float seconds;

		/// <summary>The amount of seconds that get decremented each time this gets hit by something.</summary>
		public float SecondsPerHit { set { secondsPerHit = value; } get { return secondsPerHit; } } [SerializeField] private float secondsPerHit = 10.0f;

		public void DestroyNow()
		{
			Destroy(gameObject);
		}

		protected virtual void Update()
		{
			if (seconds >= 0.0f)
			{
				seconds -= Time.deltaTime;

				if (seconds <= 0.0f)
				{
					DestroyNow();
				}
			}
		}

		public void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure)
		{
			seconds -= secondsPerHit;

			if (seconds <= 0.0f)
			{
				DestroyNow();
			}
		}

		public void HandleHit(Vector3 position, Quaternion rotation, bool preview, float pressure)
		{
			seconds -= secondsPerHit;

			if (seconds <= 0.0f)
			{
				DestroyNow();
			}
		}
	}
}