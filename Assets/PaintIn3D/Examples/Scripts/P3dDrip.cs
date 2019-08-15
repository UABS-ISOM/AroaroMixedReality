using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dDrip))]
	public class P3dDrip_Editor : P3dEditor<P3dDrip>
	{
		protected override void OnInspector()
		{
			DrawDefault("speed", "The speed this GameObject moves down in world units per second.");
			DrawDefault("dampening", "The speed at which the Speed value reaches 0 (0 = no dampening).");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component makes the current gameObject follow the specified camera..</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dDrip")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Drip")]
	public class P3dDrip : MonoBehaviour
	{
		/// <summary>The speed this GameObject moves down in world units per second.</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 1.0f;

		/// <summary>The speed at which the Speed value reaches 0 (0 = no dampening).</summary>
		public float Dampening { set { dampening = value; } get { return dampening; } } [SerializeField] private float dampening = 1.0f;

		protected virtual void LateUpdate()
		{
			transform.position += Vector3.down * speed * Time.deltaTime;

			var factor = P3dHelper.DampenFactor(dampening, Time.deltaTime);

			speed = Mathf.Lerp(speed, 0.0f, factor);
		}
	}
}