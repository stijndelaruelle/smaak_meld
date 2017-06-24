using UnityEngine;
using System.Collections;
using UnityEditor;
using Mapbox.MeshGeneration;

[CustomEditor(typeof(MapController))]
public class MapControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapController mapController = (MapController)target;

        GUILayout.Space(10);


        Color origColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.52f, 0.83f, 0.90f);

        if (GUILayout.Button("Generate map (Runtime only)", GUILayout.Height(35.0f)))
        {
            if (Application.isPlaying)
                mapController.GenerateMap();
            else
                Debug.LogWarning("Genering a map only works at runtime!");
        }

        GUI.backgroundColor = origColor;
    }
}