using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dMaterialCloner))]
	public class P3dMaterialCloner_Editor : P3dEditor<P3dMaterialCloner>
	{
		protected override void OnInspector()
		{
			if (All(t => t.Activated == true))
			{
				EditorGUILayout.HelpBox("This component has already activated.", MessageType.Info);
			}

			if (Any(t => t.Activated == false))
			{
				BeginError(Any(t => t.Index < 0 || t.Index >= t.GetComponent<Renderer>().sharedMaterials.Length));
					DrawDefault("index", "The material index that will be cloned. This matches the Materials list in your MeshRenderer/SkinnedMeshRenderer, where 0 is the first material.");
				EndError();
				BeginDisabled();
					EditorGUILayout.ObjectField("Material", P3dHelper.GetMaterial(Target.gameObject, Target.Index), typeof(Material), false);
				EndDisabled();
			}
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to duplicate a material before you paint on it. This is useful if the material is shared between multiple GameObjects (e.g. prefabs).</summary>
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(P3dPaintable))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dMaterialCloner")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Material Cloner")]
	public class P3dMaterialCloner : MonoBehaviour
	{
		/// <summary>The material index that will be cloned. This matches the Materials list in your MeshRenderer/SkinnedMeshRenderer, where 0 is the first material.</summary>
		public int Index { set { index = value; } get { return index; } } [SerializeField] private int index;

		/// <summary>This lets you know if this component has already been activated and has executed.</summary>
		public bool Activated
		{
			get
			{
				return activated;
			}
		}

		[SerializeField]
		private bool activated;

		/// <summary>If you want to deactivate this component so you can clone a different material then you can call this first.</summary>
		[ContextMenu("Reset Activation")]
		public void ResetActivation()
		{
			activated = false;
		}

		/// <summary>This allows you to manually activate this component, cloning the specified material.
		/// NOTE: This will automatically be called from P3dPaintable to clone the material.</summary>
		[ContextMenu("Activate")]
		public void Activate()
		{
			if (activated == false && index >= 0)
			{
				var paintable = GetComponent<P3dPaintable>();
				var renderer  = GetComponent<Renderer>();
				var materials = renderer.sharedMaterials;

				activated = true;

				if (index < materials.Length)
				{
					var oldMaterial = materials[index];

					if (oldMaterial != null)
					{
						var newMaterial = Instantiate(oldMaterial);

						Replace(renderer, materials, oldMaterial, newMaterial);

						if (paintable.OtherRenderers != null)
						{
							for (var i = paintable.OtherRenderers.Count - 1; i >= 0; i--)
							{
								var otherRenderer = paintable.OtherRenderers[i];

								if (otherRenderer != null)
								{
									Replace(otherRenderer, otherRenderer.sharedMaterials, oldMaterial, newMaterial);
								}
							}
						}
					}
				}
			}
		}

		private void Replace(Renderer renderer, Material[] materials, Material oldMaterial, Material newMaterial)
		{
			var replaced = false;

			for (var i = materials.Length - 1; i >= 0; i--)
			{
				var material = materials[i];

				if (material == oldMaterial)
				{
					materials[i] = newMaterial;

					replaced = true;
				}
			}

			if (replaced == true)
			{
				renderer.sharedMaterials = materials;
			}
		}
	}
}