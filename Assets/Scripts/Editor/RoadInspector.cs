using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Road))]
public class RoadInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Road road = (Road)target;

        GUILayout.Space(10);

        //Follow button
        Color origColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.52f, 0.83f, 0.90f);

        if (GUILayout.Button("Link Roads (debug)", GUILayout.Height(35.0f)))
        {
            road.LinkRoads();
        }
    }
}