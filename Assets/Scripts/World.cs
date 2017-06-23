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

    [SerializeField]
    private List<Waypoint> m_Waypoints;

    [SerializeField]
    private string m_PrefabName;
    private string m_MeshFolder = "Meshes";
    private string m_TextureFolder = "Textures";

    public void GenerateWaypoints()
    {
        m_Waypoints = new List<Waypoint>();
        //Loop trough all the mesh roads
        Road[] roads = transform.GetComponentsInChildren<Road>();
        foreach (Road road in roads)
        {
            road.GenerateWaypoints(m_Waypoints, m_WaypointPrefab);
        }
    }

    //Serialization
    public void Serialize()
    {
        #if UNITY_EDITOR

            //Make the user select a folder
            string rootFolderPath = EditorUtility.SaveFolderPanel("Select a folder to save this map to", Application.dataPath, "");
            if (rootFolderPath.Length <= 0)
                return;

            m_RootDirectoryInfo = new DirectoryInfo(rootFolderPath);

            //Save & reassign all the meshes
            SerializeMeshFilters();

            //Save and reassign all the materials & textures
            SerializeMeshRenderers();

            //Save the prefab
            CreatePrefab();
        #endif
    }

#if UNITY_EDITOR

    private void SerializeMeshFilters()
    {
        DirectoryInfo directoryInfo = ExtentionMethods.FindOrCreateDirectory(m_RootDirectoryInfo, m_MeshFolder);
        string pathName = directoryInfo.FullName + Path.DirectorySeparatorChar;

        List<string> extraUsedNames = new List<string>();

        //Loop trough all the mesh filters
        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
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
    }

    private void SerializeMeshRenderers()
    {
        DirectoryInfo directoryInfo = ExtentionMethods.FindOrCreateDirectory(m_RootDirectoryInfo, m_TextureFolder);
        string pathName = directoryInfo.FullName + Path.DirectorySeparatorChar;

        List<string> extraUsedNames = new List<string>();

        //Loop trough all the mesh filters
        MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            //Only tile renderers need saving (LAME: find better way of filtering)
            if (meshRenderer.gameObject.name.Contains("Tile") == false)
                continue;

            Material[] materials = meshRenderer.materials;

            //Save all the materials
            for (int i = 0; i < materials.Length; ++i)
            {
                Texture2D texture = (Texture2D)materials[i].mainTexture;

                if (texture != null)
                {
                    //Save the main texture to PNG
                    string textureFileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                             meshRenderer.gameObject.name + " material " + i + " mainTexture.png",
                                             extraUsedNames);

                    byte[] byteArr = texture.EncodeToPNG();

                    string textureFilePath = pathName + textureFileName;
                    File.WriteAllBytes(textureFilePath, byteArr);

                    string assetRelativeTexturePath = textureFilePath.Substring(textureFilePath.IndexOf("Assets"));

                    //Load the texture as an asset in Unity for further use.
                    UnityEditor.AssetDatabase.ImportAsset(assetRelativeTexturePath);

                    //Assign newly saved texture to the mesh renderer
                    Texture2D newTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetRelativeTexturePath);
                    materials[i].mainTexture = newTexture;
                }

                //Create an asset for the material itself
                string fileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                  meshRenderer.gameObject.name + " material " + i + ".mat",
                                  extraUsedNames);

                string filePath = pathName + fileName;
                string assetRelativePath = filePath.Substring(filePath.IndexOf("Assets"));

                UnityEditor.AssetDatabase.CreateAsset(materials[i], assetRelativePath);
                UnityEditor.AssetDatabase.SaveAssets();

                //Assign newly saved material to the mesh renderer
                UnityEditor.AssetDatabase.ImportAsset(assetRelativePath);
                Material newMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetRelativePath);

                materials[i] = newMaterial;
            }
        }
    }

    private void CreatePrefab()
    {
        //Change the name of this object
        this.gameObject.name = m_PrefabName;

        //Save it as a prefab
        string pathName = m_RootDirectoryInfo.FullName + Path.DirectorySeparatorChar;

        string filePath = pathName + m_PrefabName + ".prefab";
        string assetRelativePath = filePath.Substring(filePath.IndexOf("Assets"));
        assetRelativePath = assetRelativePath.Replace("\\", "/"); //Why? http://answers.unity3d.com/questions/1136969/prefabutilitycreateprefab-results-in-not-a-valid-a.html

        PrefabUtility.CreatePrefab(assetRelativePath, this.gameObject);
    }

#endif
}
