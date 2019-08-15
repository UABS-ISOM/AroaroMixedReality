using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	public partial class P3dWindow : P3dEditorWindow
	{
		private void DrawTab2()
		{
			var brush         = P3dWindowData.Instance.Brush;
			var presetBrushes = P3dWindowData.Instance.PresetBrushes;

			if (CanPaint() == true)
			{
				EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup(CanUndo() == false);
						if (GUILayout.Button("Undo") == true)
						{
							Undo();
						}
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(CanWrite() == false);
						P3dHelper.BeginColor(NeedsWrite() == true ? Color.green : GUI.color);
							if (GUILayout.Button(new GUIContent("Write", "If you're editing imported textures (e.g. .png), then they must be written to apply changes.")) == true)
							{
								Write();
							}
						P3dHelper.EndColor();
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(CanRedo() == false);
						if (GUILayout.Button("Redo") == true)
						{
							Redo();
						}
					EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.HelpBox("Before you can paint, you need to lock at least one texture.", MessageType.Info);
			}

			EditorGUILayout.Separator();

			brush.OnGUI();

			if (GUILayout.Button("Save As Preset") == true)
			{
				var exists = presetBrushes.Exists(p => p.Name == brush.Name);

				if (exists == false || EditorUtility.DisplayDialog("Overwrite brush?", "You already have a preset brush with this name. Overwrite it?", "Yes", "Cancel") == true)
				{
					var newPreset = presetBrushes.Find(p => p.Name == brush.Name);

					if (newPreset == null)
					{
						newPreset = new P3dWindowBrush();

						presetBrushes.Add(newPreset);
					}

					brush.CopyTo(newPreset);
				}
			}

			EditorGUILayout.Separator();

			brush.Tip0.OnGUI();

			EditorGUILayout.Separator();

			brush.Tip1.OnGUI();

			EditorGUILayout.Separator();

			brush.Tip2.OnGUI();

			EditorGUILayout.Separator();

			brush.Tip3.OnGUI();

			if (presetBrushes.Count > 0)
			{
				EditorGUILayout.Separator();

				EditorGUILayout.LabelField("Preset Brushes", EditorStyles.boldLabel);

				for (var i = 0; i < presetBrushes.Count; i++)
				{
					var presetBrush = presetBrushes[i];

					EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup(i <= 0);
						if (GUILayout.Button("▲", GUILayout.Width(20)) == true)
						{
							presetBrushes[i] = presetBrushes[i - 1];
							presetBrushes[i - 1] = presetBrush;
						}
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(i >= presetBrushes.Count - 1);
						if (GUILayout.Button("▼", GUILayout.Width(20)) == true)
						{
							presetBrushes[i] = presetBrushes[i + 1];
							presetBrushes[i + 1] = presetBrush;
						}
					EditorGUI.EndDisabledGroup();
					if (GUILayout.Button(presetBrush.Name) == true)
					{
						presetBrush.CopyTo(brush);
					}
					P3dHelper.BeginColor(Color.red);
						if (GUILayout.Button("X", GUILayout.Width(20)) == true)
						{
							if (P3dWindowData.Instance.ConfirmDeletePreset == false || EditorUtility.DisplayDialog("Delete preset brush?", "This cannot be undone.\n(You can disable this warning in the Extras tab)", "Delete", "Cancel") == true)
							{
								presetBrushes.RemoveAt(i);
							}
						}
					P3dHelper.EndColor();
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private bool CanPaint()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanPaint() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool CanUndo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanUndo() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool CanWrite()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanWrite() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool NeedsWrite()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].NeedsWrite() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool CanRedo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanRedo() == true)
				{
					return true;
				}
			}

			return false;
		}

		private void Undo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				paintables[i].Undo();
			}
		}

		private void Write()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				paintables[i].Write();
			}
		}

		private void Redo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				paintables[i].Redo();
			}
		}
	}
}