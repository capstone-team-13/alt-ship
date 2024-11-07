#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class AddShaderScript
{
    [MenuItem("Assets/Create/URP Shader/Unlit", false, 81)]
    public static void CreateNewUnlit()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/New Unlit Shader.shader",
            null,
            "Packages/com.unity.render-pipelines.universal/Shaders/Unlit.shader");
    }

    [MenuItem("Assets/Create/URP Shader/SimpleLit", false, 82)]
    public static void CreateNewSimpleLit()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/New SimpleLit Shader.shader",
            null,
            "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLit.shader");
    }

    [MenuItem("Assets/Create/URP Shader/Lit", false, 83)]
    public static void CreatNewLit()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/New Lit Shader.shader",
            null,
            "Packages/com.unity.render-pipelines.universal/Shaders/Lit.shader");
    }

    [MenuItem("Assets/Create/URP Shader/ComplexLit", false, 84)]
    public static void CreateNewComplexLit()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/New ComplexLit Shader.shader",
            null,
            "Packages/com.unity.render-pipelines.universal/Shaders/ComplexLit.shader");
    }

    [MenuItem("Assets/Create/URP Shader/BakedLit", false, 85)]
    public static void CreateNewBakedLit()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/New BakedLit Shader.shader",
            null,
            "Packages/com.unity.render-pipelines.universal/Shaders/BakedLit.shader");
    }

    [MenuItem("Assets/Create/URP Shader/CameraMotionVectors", false, 86)]
    public static void CreateNewCameraMotionVectors()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/New CameraMotionVectors Shader.shader",
            null,
            "Packages/com.unity.render-pipelines.universal/Shaders/CameraMotionVectors.shader");
    }

    [MenuItem("Assets/Create/URP Shader/ObjectMotionVectors", false, 87)]
    public static void CreateNewObjectMotionVectors()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/New ObjectMotionVectors Shader.shader",
            null,
            "Packages/com.unity.render-pipelines.universal/Shaders/ObjectMotionVectors.shader");
    }

    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }

        return path;
    }
}

class MyDoCreateScriptAsset : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }

    internal static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
        string fullPath = Path.GetFullPath(pathName);
        StreamReader streamReader = new(resourceFile);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);

        string[] nn = pathName.Split('/');
        if (nn.Length >= 3)
        {
            string[] mm = nn[2].Split('_');
            if (mm.Length == 2)
            {
                text = Regex.Replace(text, "#PATH#", mm[1]);
            }
        }

        text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
        text = Regex.Replace(text, "#TIME#", System.DateTime.Now.ToString("yyyy-MM-dd"));

        if (pathName.Contains("Assets/Game/Activity/"))
        {
            var arr = pathName.Split('/');
            if (arr.Length >= 4)
            {
                text = Regex.Replace(text, "#KEY#", arr[3]);
            }
        }

        bool encoderShouldEmitUTF8Identifier = false;
        bool throwOnInvalidBytes = false;
        UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
    }
}
#endif