using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimTexGenerator))]
public class TexGeneratorTool : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AnimTexGenerator script = (AnimTexGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
    }
}
