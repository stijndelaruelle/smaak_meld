using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Text;

public static class ExtentionMethods
{
    public static void Shuffle<T>(this IList<T> list)
    {
        //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        System.Random rand = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector2 Copy(this Vector2 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector3 Copy(this Vector3 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    public static Quaternion Copy(this Quaternion quaternion)
    {
        return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    public static float GetOverlapPercentage(this Rect rect, Rect otherRect)
    {
        float xOverlap = Mathf.Max(0, Mathf.Min(rect.xMax, otherRect.xMax) - Mathf.Max(rect.xMin, otherRect.xMin));
        float yOverlap = Mathf.Max(0, Mathf.Min(rect.yMax, otherRect.yMax) - Mathf.Max(rect.yMin, otherRect.yMin));

        float ourSize = rect.size.x * rect.size.y;
        float overlapSize = xOverlap * yOverlap;

        float ratio = overlapSize / ourSize;
        return ratio;
    }

    public static Rect GetViewportRect(this SpriteRenderer spriteRenderer)
    {
        Bounds bounds = spriteRenderer.bounds;
        Vector3 viewPortMin = Camera.main.WorldToViewportPoint(bounds.min);
        Vector3 viewPortMax = Camera.main.WorldToViewportPoint(bounds.max);

        return new Rect(viewPortMin.x, viewPortMin.y, viewPortMax.x - viewPortMin.x, viewPortMax.y - viewPortMin.y);
    }

    public static Toggle GetActive(this ToggleGroup toggleGroup)
    {
        return toggleGroup.ActiveToggles().FirstOrDefault();
    }


    public static void SaveToOBJ(this MeshFilter meshFilter, string path)
    {
        //http://wiki.unity3d.com/index.php?title=ObjExporter

        Mesh mesh = meshFilter.mesh;
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("#\n");
        stringBuilder.Append("# " + meshFilter.name + " vertices\n");
        stringBuilder.Append("#\n\n");

        //Vertices
        foreach (Vector3 v in mesh.vertices)
        {
            stringBuilder.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        stringBuilder.Append("# " + mesh.vertices.Length + " vertices\n\n");

        //Normals
        foreach (Vector3 v in mesh.normals)
        {
            stringBuilder.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        stringBuilder.Append("# " + mesh.normals.Length + " normals\n\n");

        //UV Coorindates
        foreach (Vector3 v in mesh.uv)
        {
            stringBuilder.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        stringBuilder.Append("# " + mesh.uv.Length + " texture coords\n\n");

        //Faces
        for (int subMeshID = 0; subMeshID < mesh.subMeshCount; subMeshID++)
        {
            string name = meshFilter.name + " " + subMeshID;
            name = name.Replace(" ", "");
            stringBuilder.Append("o ").Append(name).Append("\n");

            //stringBuilder.Append("\n");
            //stringBuilder.Append("usemtl ").Append(mats[material].name).Append("\n");
            //stringBuilder.Append("usemap ").Append(mats[material].name).Append("\n");

            int[] triangles = mesh.GetTriangles(subMeshID);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                stringBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }

            stringBuilder.Append("# " + triangles.Length + " faces\n\n");
        }

        File.WriteAllText(path, stringBuilder.ToString());
    }

    
    public static DirectoryInfo FindOrCreateDirectory(DirectoryInfo rootDirectory, string name)
    {
        if (rootDirectory == null)
            return null;

        //Check if that folder already exists
        DirectoryInfo[] subDirectories = rootDirectory.GetDirectories();
        DirectoryInfo ourDirectory = null;

        foreach (DirectoryInfo directory in subDirectories)
        {
            if (directory.Name == name)
            {
                ourDirectory = directory;
                break;
            }
        }

        //If not, create it.
        if (ourDirectory == null)
        {
            ourDirectory = rootDirectory.CreateSubdirectory(name);
        }

        return ourDirectory;
    }

    public static string FindUniqueDirectoryName(DirectoryInfo rootDirectory, string originalDirectoryName)
    {
        if (rootDirectory == null)
            return "";

        string uniqueName = originalDirectoryName;
        DirectoryInfo[] directories = rootDirectory.GetDirectories();

        int count = 0;
        for (int i = 0; i < directories.Length; ++i)
        {
            if (directories[i].Name.ToString().StartsWith(originalDirectoryName))
            {
                string testFilename = originalDirectoryName;
                if (count > 0) { testFilename += " (" + (count + 1) + ")"; }

                if (directories[i].Name.ToString() != testFilename)
                {
                    uniqueName = testFilename;
                    break;
                }

                ++count;

                uniqueName = originalDirectoryName + " (" + (count + 1) + ")";
            }
        }

        return uniqueName;
    }

    public static string FindUniqueFileName(DirectoryInfo rootDirectory, string originalFileName, List<string> extraUsedNames)
    {
        int dotIndex = originalFileName.LastIndexOf(".");
        string extention = originalFileName.Substring(dotIndex);
        originalFileName = originalFileName.Remove(dotIndex);

        string uniqueFileName = "";
        bool fileExists = true;
        int count = 0;
        while (fileExists == true || count >= 100) //Safety net
        {
            uniqueFileName = originalFileName;
            if (count > 0) uniqueFileName += " (" + (count + 1) + ")";
            uniqueFileName += extention;

            fileExists = extraUsedNames.Contains(uniqueFileName);

            if (fileExists == false)
                fileExists = File.Exists(rootDirectory.FullName + Path.DirectorySeparatorChar + uniqueFileName);

            ++count;
        }

        return uniqueFileName;
    }
}