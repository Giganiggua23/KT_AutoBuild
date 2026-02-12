using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System.Linq;

public static class AutoBuilder
{
    static string buildsRoot => Path.Combine("Builds", Application.version.Replace('.', '_'));

    static string[] GetEnabledScenes() => 
        EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

    static bool EnsureScenesExist(out string[] scenes) => 
        (scenes = GetEnabledScenes())?.Length > 0;

    static void RunBuild(BuildPlayerOptions options)
    {
        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;
        
        if (summary.result == BuildResult.Succeeded)
            Debug.Log($" {summary.platform}: {summary.totalSize} bytes");
        else
            Debug.LogError($" {summary.platform}: {summary.result}");
    }

    static void Build(string platform, BuildTarget target, string extension = "")
    {
        if (!EnsureScenesExist(out string[] scenes)) return;
        
        string path = Path.Combine(buildsRoot, platform);
        Directory.CreateDirectory(path);
        
        if (!string.IsNullOrEmpty(extension))
            path = Path.Combine(path, $"{Application.productName}{extension}");

        RunBuild(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = target,
            options = BuildOptions.None
        });
    }

    [MenuItem("Build/Windows x64")] public static void BuildWindows() => Build("Windows", BuildTarget.StandaloneWindows64, ".exe");
    [MenuItem("Build/Android")] public static void BuildAndroid() => Build("Android", BuildTarget.Android, ".apk");
    [MenuItem("Build/WebGL")] public static void BuildWebGL() => Build("WebGL", BuildTarget.WebGL);
    [MenuItem("Build/All")] public static void BuildAll() { BuildWindows(); BuildAndroid(); BuildWebGL(); }
    
    // Äëÿ CLI
    public static void BuildWindows_CLI() => BuildWindows();
    public static void BuildAndroid_CLI() => BuildAndroid();
    public static void BuildWebGL_CLI() => BuildWebGL();
    public static void BuildAll_CLI() => BuildAll();
}