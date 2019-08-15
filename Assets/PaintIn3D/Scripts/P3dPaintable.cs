﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintable))]
	public class P3dPaintable_Editor : P3dEditor<P3dPaintable>
	{
		protected override void OnInspector()
		{
			DrawDefault("activation", "This allows you to control when this component actually activates and becomes ready for painting. You probably don't need to change this.");

			DrawDefault("otherRenderers", "If this material is used in multiple renderers, you can specify them here. This usually happens with different LOD levels.");

			Separator();

			if (Any(t => t.GetComponent<P3dPaintableTexture>() == null))
			{
				EditorGUILayout.HelpBox("Your paintable doesn't have any paintable textures!", MessageType.Warning);
			}

			if (Button("Add Material Cloner") == true)
			{
				Each(t => t.gameObject.AddComponent<P3dMaterialCloner>());
			}

			if (Button("Add Paintable Texture") == true)
			{
				Each(t => t.gameObject.AddComponent<P3dPaintableTexture>());
			}

			if (Button("Analyze Mesh") == true)
			{
				P3dMeshAnalysis.OpenWith(Target.gameObject);
			}
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component marks the current GameObject as being paintable.
	/// NOTE: This GameObject must has a MeshFilter + MeshRenderer, or a SkinnedMeshRenderer.
	/// To actually paint your object, you must also add the SgtPaintableTexture component.</summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintable")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paintable")]
	public class P3dPaintable : P3dModel
	{
		public enum ActivationType
		{
			Awake,
			OnEnable,
			Start,
			OnFirstUse
		}

		public override P3dPaintable Paintable { set {  } get { paintable = this; return paintable; } }

		/// <summary>This allows you to control when this component actually activates and becomes ready for painting. You probably don't need to change this.</summary>
		public ActivationType Activation { set { activation = value; } get { return activation; } } [SerializeField] private ActivationType activation = ActivationType.Awake;

		/// <summary>If this material is used in multiple renderers, you can specify them here. This usually happens with different LOD levels.</summary>
		public List<Renderer> OtherRenderers { set { otherRenderers = value; } get { return otherRenderers; } } [SerializeField] private List<Renderer> otherRenderers;

		[SerializeField]
		private bool activated;

		[System.NonSerialized]
		private List<P3dPaintableTexture> paintableTextures = new List<P3dPaintableTexture>();

		[System.NonSerialized]
		private static List<P3dMaterialCloner> tempMaterialCloners = new List<P3dMaterialCloner>();

		/// <summary>This lets you know if this paintable has been activated.
		/// Being activated means each associated P3dMaterialCloner and P3dPaintableTexture has been Activated.
		/// NOTE: If you manually add P3dMaterialCloner or P3dPaintableTexture components after activation, then you must manually Activate().</summary>
		public bool Activated
		{
			get
			{
				return activated;
			}
		}

		/// <summary>This gives you a list of all P3dPaintableTexture components that have been activated.</summary>
		public List<P3dPaintableTexture> PaintableTextures
		{
			get
			{
				return paintableTextures;
			}
		}

		/// <summary>This allows you to manually activate all attached P3dMaterialCloner and P3dPaintableTexture components.</summary>
		[ContextMenu("Activate")]
		public void Activate()
		{
			// Activate material cloners
			GetComponents(tempMaterialCloners);

			for (var i = tempMaterialCloners.Count - 1; i >= 0; i--)
			{
				tempMaterialCloners[i].Activate();
			}

			// Activate textures
			GetComponents(paintableTextures);

			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				paintableTextures[i].Activate();
			}

			activated = true;
		}

		/// <summary>This allows you to clear the pixels of all activated P3dPaintableTexture components associated with this P3dPaintable with the specified color.</summary>
		public void ClearAll(Color color)
		{
			ClearAll(default(Texture), color);
		}

		/// <summary>This allows you to clear the pixels of all activated P3dPaintableTexture components associated with this P3dPaintable with the specified color and texture.</summary>
		public void ClearAll(Texture texture, Color color)
		{
			if (activated == true)
			{
				for (var i = paintableTextures.Count - 1; i >= 0; i--)
				{
					paintableTextures[i].Clear(texture, color);
				}
			}
		}

		/// <summary>This allows you to manually register a P3dPaintableTexture.</summary>
		public void Register(P3dPaintableTexture paintableTexture)
		{
			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				if (paintableTextures[i] == paintableTexture)
				{
					return;
				}
			}

			paintableTextures.Add(paintableTexture);
		}

		/// <summary>This allows you to manually unregister a P3dPaintableTexture.</summary>
		public void Unregister(P3dPaintableTexture paintableTexture)
		{
			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				if (paintableTextures[i] == paintableTexture)
				{
					paintableTextures.RemoveAt(i);
				}
			}
		}

		protected virtual void Awake()
		{
			if (activation == ActivationType.Awake && activated == false)
			{
				Activate();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (activation == ActivationType.OnEnable && activated == false)
			{
				Activate();
			}

			if (P3dPaintableManager.InstanceCount == 0)
			{
				var paintableManager = new GameObject(typeof(P3dPaintableManager).Name);

				paintableManager.hideFlags = HideFlags.DontSave;
				
				paintableManager.AddComponent<P3dPaintableManager>();
			}
		}

		protected virtual void Start()
		{
			if (activation == ActivationType.Start && activated == false)
			{
				Activate();
			}
		}
	}
}