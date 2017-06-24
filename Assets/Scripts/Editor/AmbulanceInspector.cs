using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Ambulance))]
public class RoadInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Ambulance ambulance = (Ambulance)target;

        GUILayout.Space(10);

        Color origColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.64f, 0.90f, 0.52f);

        if (GUILayout.Button("Calculate path", GUILayout.Height(35.0f)))
        {
            ambulance.CalculatePath();
        }
        GUI.backgroundColor = origColor;
    }
}