using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dUndoAll))]
	public class P3dUndoAll_Editor : P3dEditor<P3dUndoAll>
	{
		protected override void OnInspector()
		{
			Each(t => t.Unregister());
				DrawDefault("target", "If you want a UI Button to be automatically set up with undo functionality, then set it here.");
			Each(t => t.Register());
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to manually trigger the Undo All action, or for it to automatically be set up with a UI Button.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dUndoAll")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Undo All")]
	public class P3dUndoAll : MonoBehaviour
	{
		/// <summary>If you want a UI Button to be automatically set up with undo functionality, then set it here.</summary>
		public Button Target { set { Unregister(); target = value; Register(); } get { return target; } } [SerializeField] private Button target;

		/// <summary>If you want to manually triggger UndoAll, then call this function.</summary>
		[ContextMenu("Undo All")]
		public void UndoAll()
		{
			P3dStateManager.UndoAll();
		}

		public void Register()
		{
			if (target != null)
			{
				if (target.onClick == null)
				{
					target.onClick = new Button.ButtonClickedEvent();
				}

				target.onClick.AddListener(UndoAll);
			}
		}

		public void Unregister()
		{
			if (target != null)
			{
				target.onClick.RemoveListener(UndoAll);
			}
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			target = GetComponent<Button>();
		}
#endif

		protected virtual void OnEnable()
		{
			Register();
		}

		protected virtual void OnDisable()
		{
			Unregister();
		}

		protected virtual void Update()
		{
			if (target != null)
			{
				target.interactable = P3dStateManager.CanUndo;
			}
		}
	}
}