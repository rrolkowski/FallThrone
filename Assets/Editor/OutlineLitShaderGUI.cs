using UnityEditor;
using UnityEngine;

public class OutlineLitShaderGUI : ShaderGUI
{
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		// Poka¿ wszystkie w³aœciwoœci automatycznie
		EditorGUI.BeginChangeCheck();
		base.OnGUI(materialEditor, properties);
		EditorGUI.EndChangeCheck();

		// Poka¿ outline rêcznie
		MaterialProperty outlineColor = FindProperty("_OutlineColor", properties);
		MaterialProperty outlineWidth = FindProperty("_OutlineWidth", properties);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Outline Settings", EditorStyles.boldLabel);
		materialEditor.ColorProperty(outlineColor, "Outline Color");
		materialEditor.FloatProperty(outlineWidth, "Outline Width");
	}
}
