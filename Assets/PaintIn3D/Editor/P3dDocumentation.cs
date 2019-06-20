using UnityEditor;
using UnityEditor.Callbacks;

namespace PaintIn3D
{
	/// <summary>Unity hijacks html file opening and passes it to the default text editor. For documentation files we want to use an actual browser for this, so hijack it back!</summary>
	public static class P3dDocumentation
	{
		[OnOpenAsset(1)]
		public static bool step1(int instanceID, int line)
		{
			var path = AssetDatabase.GetAssetPath(instanceID);

			if (path.Contains("PaintIn3D") == true && path.EndsWith("DOCUMENTATION.html") == true)
			{
				System.Diagnostics.Process.Start(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), path));

				return true;
			}

			return false;
		}
	}
}