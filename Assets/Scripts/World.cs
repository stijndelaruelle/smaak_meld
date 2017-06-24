using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class World : MonoBehaviour
{
    private DirectoryInfo m_RootDirectoryInfo;

    [SerializeField]
    private Waypoint m_WaypointPrefab;

    private const string m_MeshFolder = "Meshes";
    private const string m_TextureFolder = "Textures";

    private float m_Progress = 0.0f;
    private const float m_ProgressPerBlock = 0.25f;

    private Coroutine m_SerializeCoroutine;

    //Serialization
    public void Serialize()
    {
        #if UNITY_EDITOR
            if (m_SerializeCoroutine != null)
            {
                Debug.LogWarning("Still saving... please don't press the button repeatedly.");
                return;
            }

            m_SerializeCoroutine = StartCoroutine(SerializeRoutine());
        #endif
    }

    private void SetProgressText(string text)
    {
        if (EditorUtility.DisplayCancelableProgressBar("Serializing map", text, m_Progress))
        {
            return;
        }
    }

#if UNITY_EDITOR

    private IEnumerator SerializeRoutine()
    {
        m_Progress = 0.0f;

        //Make the user select a folder
        string rootFolderPath = EditorUtility.SaveFolderPanel("Select a folder to save this map to", Application.dataPath, "");
        if (rootFolderPath.Length <= 0)
            yield return null;

        m_RootDirectoryInfo = new DirectoryInfo(rootFolderPath);

        //Save & reassign all the meshes
        SerializeMeshFilters();
        m_Progress = 0.25f;

        //Save and reassign all the materials & textures
        SerializeMeshRenderers();
        m_Progress = 0.5f;

        //Generate waypoints
        GenerateWaypoints();
        m_Progress = 0.75f;

        //Save the prefab
        CreatePrefab();
        m_Progress = 1.0f;

        m_SerializeCoroutine = null;

        EditorUtility.ClearProgressBar();

        yield return null;
    }

    private void SerializeMeshFilters()
    {
        SetProgressText("Started serializing mesh filters...");

        DirectoryInfo directoryInfo = ExtentionMethods.FindOrCreateDirectory(m_RootDirectoryInfo, m_MeshFolder);
        string pathName = directoryInfo.FullName + Path.DirectorySeparatorChar;

        List<string> extraUsedNames = new List<string>();

        //Loop trough all the mesh filters
        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
        float progressPerMeshFilter = m_ProgressPerBlock / meshFilters.Length;

        for (int i = 0; i < meshFilters.Length; ++i)
        {
            m_Progress += progressPerMeshFilter;
            SetProgressText("Serializing mesh filter " + (i + 1) + "/" + meshFilters.Length);

            MeshFilter meshFilter = meshFilters[i];

            //Save mesh to OBJ
            string fileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                                                  meshFilter.gameObject.name + ".obj",
                                                                  extraUsedNames);

            string filePath = pathName + fileName;
            meshFilter.SaveToOBJ(filePath);

            int index = filePath.IndexOf("Assets");
            string assetRelativePath = filePath.Substring(index);

            //Load the model as an asset in Unity for further use.
            UnityEditor.AssetDatabase.ImportAsset(assetRelativePath);

            //Assign newly saved mesh to the mesh filter
            Mesh newMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(assetRelativePath);
            meshFilter.mesh = newMesh;

            extraUsedNames.Add(fileName);
        }

        SetProgressText("Finished serializing mesh filters!");
    }

    private void SerializeMeshRenderers()
    {
        SetProgressText("Started serializing mesh renderers...");

        DirectoryInfo directoryInfo = ExtentionMethods.FindOrCreateDirectory(m_RootDirectoryInfo, m_TextureFolder);
        string pathName = directoryInfo.FullName + Path.DirectorySeparatorChar;

        List<string> extraUsedNames = new List<string>();

        //Loop trough all the mesh filters
        MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
        float progressPerMeshRenderer = m_ProgressPerBlock / meshRenderers.Length;

        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            m_Progress += progressPerMeshRenderer;
            SetProgressText("Serializing mesh renderer " + (i + 1) + "/" + meshRenderers.Length);

            MeshRenderer meshRenderer = meshRenderers[i];

            //Only tile renderers need saving (LAME: find better way of filtering)
            if (meshRenderer.gameObject.name.Contains("Tile") == false)
                continue;

            Material[] materials = meshRenderer.materials;

            //Save all the materials
            for (int j = 0; j < materials.Length; ++j)
            {
                Texture2D texture = (Texture2D)materials[j].mainTexture;

                if (texture != null)
                {
                    //Save the main texture to PNG
                    string textureFileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                             meshRenderer.gameObject.name + " material " + j + " mainTexture.png",
                                             extraUsedNames);

                    byte[] byteArr = texture.EncodeToPNG();

                    string textureFilePath = pathName + textureFileName;
                    File.WriteAllBytes(textureFilePath, byteArr);

                    string assetRelativeTexturePath = textureFilePath.Substring(textureFilePath.IndexOf("Assets"));

                    //Load the texture as an asset in Unity for further use.
                    UnityEditor.AssetDatabase.ImportAsset(assetRelativeTexturePath);

                    //Assign newly saved texture to the mesh renderer
                    Texture2D newTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetRelativeTexturePath);
                    materials[j].mainTexture = newTexture;
                }

                //Create an asset for the material itself
                string fileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                  meshRenderer.gameObject.name + " material " + j + ".mat",
                                  extraUsedNames);

                string filePath = pathName + fileName;
                string assetRelativePath = filePath.Substring(filePath.IndexOf("Assets"));

                UnityEditor.AssetDatabase.CreateAsset(materials[j], assetRelativePath);
                UnityEditor.AssetDatabase.SaveAssets();

                //Assign newly saved material to the mesh renderer
                UnityEditor.AssetDatabase.ImportAsset(assetRelativePath);
                Material newMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetRelativePath);

                materials[j] = newMaterial;
            }
        }

        SetProgressText("Finished serializing mesh renderers!");
    }

    public void GenerateWaypoints()
    {
        SetProgressText("Started generating waypoints...");

        //Loop trough all the mesh roads
        Road[] roads = transform.GetComponentsInChildren<Road>();
        float progressPerRoad = m_ProgressPerBlock / roads.Length;

        //Generate and link road waypoints
        for (int i = 0; i < roads.Length; ++i)
        {
            m_Progress += progressPerRoad;
            SetProgressText("Generating waypoints for road " + (i + 1) + "/" + roads.Length);

            Road road = roads[i];
            road.GenerateWaypoints(m_WaypointPrefab);
            road.LinkWaypoints();
        }

        //Link the roads together
        SetProgressText("Started linking roads...");
        for (int i = 0; i < roads.Length; ++i)
        {
            roads[i].LinkRoads();
        }

        SetProgressText("Finished generating waypoints!");
    }

    private void CreatePrefab()
    {
        SetProgressText("Started creating prefab...");

        //Change the name of this object
        this.gameObject.name = m_RootDirectoryInfo.Name;

        //Save it as a prefab
        string pathName = m_RootDirectoryInfo.FullName + Path.DirectorySeparatorChar;

        string filePath = pathName + m_RootDirectoryInfo.Name + ".prefab";
        string assetRelativePath = filePath.Substring(filePath.IndexOf("Assets"));
        assetRelativePath = assetRelativePath.Replace("\\", "/"); //Why? http://answers.unity3d.com/questions/1136969/prefabutilitycreateprefab-results-in-not-a-valid-a.html

        PrefabUtility.CreatePrefab(assetRelativePath, this.gameObject);

        SetProgressText("Finished creating prefab!");
    }

#endif
}
