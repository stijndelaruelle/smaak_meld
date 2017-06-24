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

        GUILayout.Space(10);

        Color origColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.64f, 0.90f, 0.52f);

        if (GUILayout.Button("Serialize", GUILayout.Height(35.0f)))
        {
            world.Serialize();
        }
        GUI.backgroundColor = origColor;

        //GUILayout.Space(5);

        //if (world.Progress > 0.0f)
        //{
        //    Rect rect = EditorGUILayout.BeginVertical();
        //        EditorGUI.ProgressBar(rect, world.Progress, world.ProgressText);
        //        GUILayout.Space(16);
        //    EditorGUILayout.EndVertical();
        //}
    }

    //public override bool RequiresConstantRepaint()
    //{
    //    return true;
    //}
}