using UnityEngine;
using System.Text;

public class TransformMutationTracer : MonoBehaviour
{
    private Vector3 _posAfterUpdate;
    private Vector3 _posAfterLateUpdate;

    void Update()
    {
        _posAfterUpdate = transform.position;
    }

    void LateUpdate()
    {
        _posAfterLateUpdate = transform.position;
    }

    void FixedUpdate()
    {
        // only useful when a physics step lands between rendered frames
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 260, 600, 120), GUI.skin.box);
        GUILayout.Label($"After Update:     {_posAfterUpdate}");
        GUILayout.Label($"After LateUpdate: {_posAfterLateUpdate}");
        GUILayout.Label($"Current:          {transform.position}");
        GUILayout.EndArea();
    }
}