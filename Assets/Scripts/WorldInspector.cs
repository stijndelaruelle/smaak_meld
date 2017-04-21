using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        World world = (World)target;
        if (GUILayout.Button("Serialize"))
        {
            world.Serialize();
        }
    }
}