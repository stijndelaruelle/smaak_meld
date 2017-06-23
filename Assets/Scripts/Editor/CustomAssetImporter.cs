using UnityEngine;
using UnityEditor;

//http://www.sarpersoher.com/a-custom-asset-importer-for-unity/
internal sealed class CustomAssetImporter : AssetPostprocessor
{
    //Pre Processors
    private void OnPreprocessTexture()
    {
        //Yet to come for all thetile meshes.
    }

    private void OnPreprocessModel()
    {
        //If the model is an OBJ, we most likely generated it ourselves.
        if (!assetPath.EndsWith(".obj"))
            return;

        ModelImporter modelImporter = assetImporter as ModelImporter;

        //Get rid of the animation. It's an obj we generated ourselves.
        modelImporter.animationType = ModelImporterAnimationType.None;
        modelImporter.importAnimation = false;

        //Disable other settings we don't need
        modelImporter.importBlendShapes = false;
        modelImporter.importMaterials = false;

        modelImporter.globalScale = 1;
        modelImporter.meshCompression = ModelImporterMeshCompression.Off;
        modelImporter.optimizeMesh = true; //This lets Unity get rid of any unused mesh data (bones, vertex colors etc.) on build
        //modelImporter.generateSecondaryUV = true; //Generates lightmap uvs, comment out if you don't use lightmapping OR you provide your own lightmap/second uvs.
    }

    private void OnPreprocessAudio()
    {
    }

    //Post Processors
    private void OnPostprocessTexture(Texture2D import)
    {
    }

    private void OnPostprocessModel(GameObject import)
    {
        // As described in the OnPreProcessModel(), determine if this is a static mesh based on the file name
        // If so, tick it as static
        if (import.name.Contains("Stat"))
            import.isStatic = true;

        // Sometimes the artist who created the model forgets to "freeze" the position and rotation of the mesh
        // I find it dirty and telling an artists to fix and re-export the same mesh pisses them off especially if you are working on a game with a lot of assets
        // So OnPostprocessModel() to the rescue!
        // We simply zero out the position and rotation fields so when we put these models in our scene they don't come with their saved transform data
        import.transform.position = Vector3.zero;
        import.transform.rotation = Quaternion.identity;
    }

    private void OnPostprocessAudio(AudioClip import) { }
}