using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This is the base class for all paint commands. These commands (e.g. paint decal) are added to the command list for each P3dPaintableTexture, and are executed at the end of the frame to optimize state changes.</summary>
	public abstract class P3dCommand
	{
		public bool      Preview;
		public P3dModel  Model;
		public Matrix4x4 Matrix;

		public abstract Material Material
		{
			get;
		}

		public abstract bool RequireSwap
		{
			get;
		}

		public abstract bool RequireMesh
		{
			get;
		}

		public abstract void Apply();
		public abstract void Pool();
	}
}