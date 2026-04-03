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
                "breakWhenBurnOut",
                "spawnWhenBreak"
            );

            var breakProp = serializedObject.FindProperty("breakWhenBurnOut");
            var spawnProp = serializedObject.FindProperty("spawnWhenBreak");

            EditorGUILayout.PropertyField(breakProp);

            using var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(breakProp.boolValue));
            if (group.visible)
            {
                EditorGUILayout.PropertyField(spawnProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}