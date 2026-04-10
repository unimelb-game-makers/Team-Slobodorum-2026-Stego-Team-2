using System;
using UnityEditor;

namespace TeamSlobodorum.Flammable.Editor
{
    [CustomEditor(typeof(Flammable))]
    [CanEditMultipleObjects]
    public class FlammableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject,
                "useBoundsFromMeshFilter",
                "bounds",
                "useVoxel",
                "voxelSize",
                
                "useBurnMark",
                "burnMarkColor",
                "emberColor",
                
                "breakWhenBurnOut",
                "spawnWhenBreak"
            );

            var useBoundsFromMeshFilterProp = serializedObject.FindProperty("useBoundsFromMeshFilter");
            EditorGUILayout.PropertyField(useBoundsFromMeshFilterProp);
            using (var group =
                   new EditorGUILayout.FadeGroupScope(1 - Convert.ToSingle(useBoundsFromMeshFilterProp.boolValue)))
            {
                EditorGUI.indentLevel++;
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"));
                }
                EditorGUI.indentLevel--;
            }

            var useVoxelProp = serializedObject.FindProperty("useVoxel");
            EditorGUILayout.PropertyField(useVoxelProp);
            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(useVoxelProp.boolValue)))
            {
                EditorGUI.indentLevel++;
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("voxelSize"));
                }
                EditorGUI.indentLevel--;
            }
            
            var useBurnMarkProps = serializedObject.FindProperty("useBurnMark");
            EditorGUILayout.PropertyField(useBurnMarkProps);
            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(useBurnMarkProps.boolValue)))
            {
                EditorGUI.indentLevel++;
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("burnMarkColor"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("emberColor"));
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}