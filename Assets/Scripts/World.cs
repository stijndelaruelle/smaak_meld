using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField]
    private string m_RootFolder;
    private DirectoryInfo m_RootDirectoryInfo;

    [SerializeField]
    private string m_MeshFolder;

    [SerializeField]
    private string m_TextureFolder;

    public void Serialize()
    {
        m_RootDirectoryInfo = new DirectoryInfo(m_RootFolder);


        //SAVE ALL THE MESHES
        SerializeMeshFilters();

        //SAVE ALL THE MATERIALS & TEXTURES
        SerializeMeshRenderers();
    }

    private void SerializeMeshFilters()
    {
        DirectoryInfo directoryInfo = ExtentionMethods.FindOrCreateDirectory(m_RootDirectoryInfo, m_MeshFolder);
        string pathName = directoryInfo.FullName + Path.DirectorySeparatorChar;

        List<string> extraUsedNames = new List<string>();

        //Loop trough all the mesh filters
        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            string fileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                                                  meshFilter.gameObject.name + ".obj",
                                                                  extraUsedNames);

            string filePath = pathName + fileName;
            meshFilter.SaveToOBJ(filePath);

            int index = filePath.IndexOf("Assets");
            string assetRelativePath = filePath.Substring(index);

            AssetDatabase.ImportAsset(assetRelativePath);
            Mesh newMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetRelativePath);

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
            //Only tile renderers need saving (LAME find better way of filtering)
            if (meshRenderer.gameObject.name.Contains("Tile") == false)
                continue;

            Material[] materials = meshRenderer.materials;

            //Save all the materials
            for (int i = 0; i < materials.Length; ++i)
            {
                Texture2D texture = (Texture2D)materials[i].mainTexture;

                //Save the main texture
                if (texture != null)
                {
                    string textureFileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                             meshRenderer.gameObject.name + " material " + i + " mainTexture.png",
                                             extraUsedNames);

                    byte[] byteArr = texture.EncodeToPNG();

                    string textureFilePath = pathName + textureFileName;
                    File.WriteAllBytes(textureFilePath, byteArr);

                    string assetRelativeTexturePath = textureFilePath.Substring(textureFilePath.IndexOf("Assets"));

                    AssetDatabase.ImportAsset(assetRelativeTexturePath);
                    Texture2D newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetRelativeTexturePath);

                    materials[i].mainTexture = newTexture;
                }

                string fileName = ExtentionMethods.FindUniqueFileName(directoryInfo,
                                  meshRenderer.gameObject.name + " material " + i + ".mat",
                                  extraUsedNames);

                string filePath = pathName + fileName;
                string assetRelativePath = filePath.Substring(filePath.IndexOf("Assets"));

                AssetDatabase.CreateAsset(materials[i], assetRelativePath);
                AssetDatabase.SaveAssets();

                AssetDatabase.ImportAsset(assetRelativePath);
                Material newMaterial = AssetDatabase.LoadAssetAtPath<Material>(assetRelativePath);

                materials[i] = newMaterial;
            }
        }
    }

    private void SerializeMaterial()
    {

    }
}
