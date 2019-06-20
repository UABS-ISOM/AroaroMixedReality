using UnityEngine;

namespace PaintIn3D
{
	/// <summary>Components like P3dDragRaycast find a point in the scene where you want to apply paint, and apply it by searching for all components that implement this interface, and then calling the HandleHit method with the hit information.
	/// This hit information can be used for other things too, like the P3dSpawner component implements this to spawn a prefab at the hit location.</summary>
	public interface IHitHandler
	{
		void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure);
	}
}