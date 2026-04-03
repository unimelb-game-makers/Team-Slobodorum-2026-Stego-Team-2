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
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"));
                }
            }

            var useVoxelProp = serializedObject.FindProperty("useVoxel");
            EditorGUILayout.PropertyField(useVoxelProp);
            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(useVoxelProp.boolValue)))
            {
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("voxelSize"));
                }
            }
            
            var useBurnMarkProps = serializedObject.FindProperty("useBurnMark");
            EditorGUILayout.PropertyField(useBurnMarkProps);
            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(useBurnMarkProps.boolValue)))
            {
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("burnMarkColor"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("emberColor"));
                }
            }

            var breakProp = serializedObject.FindProperty("breakWhenBurnOut");
            EditorGUILayout.PropertyField(breakProp);
            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(breakProp.boolValue)))
            {
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnWhenBreak"));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}