using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintableManager))]
	public class P3dPaintableManager_Editor : P3dEditor<P3dPaintableManager>
	{
		[InitializeOnLoad]
		public class ExecutionOrder
		{
			static ExecutionOrder()
			{
				ForceExecutionOrder(100);
			}
		}

		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.", MessageType.Info);
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.</summary>
	[DisallowMultipleComponent]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintableManager")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paintable Manager")]
	public class P3dPaintableManager : P3dLinkedBehaviour<P3dPaintableManager>
	{
		protected virtual void LateUpdate()
		{
			if (this == FirstInstance && P3dModel.InstanceCount > 0)
			{
				ClearAll();
				UpdateAll();
			}
			else
			{
				P3dHelper.Destroy(gameObject);
			}
		}

		private void ClearAll()
		{
			var model = P3dModel.FirstInstance;

			for (var i = 0; i < P3dModel.InstanceCount; i++)
			{
				model.Prepared = false;

				model = model.NextInstance;
			}
		}

		private void UpdateAll()
		{
			var paintableTexture = P3dPaintableTexture.FirstInstance;

			for (var i = 0; i < P3dPaintableTexture.InstanceCount; i++)
			{
				paintableTexture.ExecuteCommands();

				paintableTexture = paintableTexture.NextInstance;
			}
		}
	}
}