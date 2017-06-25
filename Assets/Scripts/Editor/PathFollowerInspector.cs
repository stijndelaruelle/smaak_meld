using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PathFollower))]
public class PathFollowerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathFollower pathFollower = (PathFollower)target;

        GUILayout.Space(10);

        //Follow button
        Color origColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.52f, 0.83f, 0.90f);

        if (GUILayout.Button("Follow path (Runtime only)", GUILayout.Height(35.0f)))
        {
            if (Application.isPlaying)
                pathFollower.StartFollowing();
            else
                Debug.LogWarning("Following a path only works at runtime!");
        }

        //Calculate button
        GUI.backgroundColor = new Color(0.64f, 0.90f, 0.52f);

        if (GUILayout.Button("Calculate path", GUILayout.Height(35.0f)))
        {
            pathFollower.CalculatePath();
        }
        GUI.backgroundColor = origColor;
    }
}