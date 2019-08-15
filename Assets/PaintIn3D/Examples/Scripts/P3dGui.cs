using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dGui))]
	public class P3D_Gui_Editor : P3dEditor<P3dGui>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component is used in all example scenes to drive the GUI buttons.", MessageType.Info);
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component is used in all example scenes to drive the GUI buttons.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dGui")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "GUI")]
	public class P3dGui : MonoBehaviour
	{
		/// <summary>You can manually call this method to reload the current scene.</summary>
		public void ClickReload()
		{
			LoadLevel(GetCurrentLevel());
		}

		/// <summary>You can manually call this method to load the previous scene.</summary>
		public void ClickPrev()
		{
			var index = GetCurrentLevel() - 1;

				if (index < 0)
				{
					index = GetLevelCount() - 1;
				}

				LoadLevel(index);
		}

		/// <summary>You can manually call this method to load the next scene.</summary>
		public void ClickNext()
		{
			var index = GetCurrentLevel() + 1;

			if (index >= GetLevelCount())
			{
				index = 0;
			}

			LoadLevel(index);
		}

		private static int GetCurrentLevel()
		{
			return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
		}

		private static int GetLevelCount()
		{
			return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		}

		private static void LoadLevel(int index)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(index);
		}
	}
}