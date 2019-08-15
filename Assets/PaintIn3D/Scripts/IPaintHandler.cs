using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This interface can be used to perform actions on a texture after it has been painted (e.g. counting pixels).</summary>
	public interface IPaintHandler
	{
		/// <summary>After the specified P3dPaintableTexture has been painted, this method will be called.</summary>
		void HandlePaint(P3dPaintableTexture paintableTexture, bool preview);
	}
}