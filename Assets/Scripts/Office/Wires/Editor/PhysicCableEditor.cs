using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicCable))]
public class PhysicCableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PhysicCable myScript = (PhysicCable)target;
        if (GUILayout.Button("Reset Points"))
        {
            myScript.ResetPoints();
        }

        if (GUILayout.Button("Add Point"))
        {
            myScript.AddPoint();
        }

        if (GUILayout.Button("Remove Point"))
        {
            myScript.RemovePoint();
        }
    }
}
