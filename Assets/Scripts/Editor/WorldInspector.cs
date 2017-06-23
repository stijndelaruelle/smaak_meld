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

        GUILayout.Space(20);
        GUILayout.Label("Waypoints");

        if (GUILayout.Button("Generate waypoints"))
            world.GenerateWaypoints();

        GUILayout.Space(20);

        if (GUILayout.Button("Serialize"))
            world.Serialize();
    }
}