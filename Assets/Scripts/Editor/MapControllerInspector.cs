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

        GUILayout.Space(20);

        if (GUILayout.Button("Generate map (Runtime only)"))
        {
            if (Application.isPlaying)
            {
                mapController.GenerateMap();
            }
        }
    }
}