using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class KongregateExport: ScriptableObject {

  // Have we loaded the prefs yet
  private static bool prefsLoaded = false;
  // The Preferences
  public static string kongMobileDir = "";

  private static string KONG_DIR_PREF_KEY = "KongMobileDirPreferenceKey";
  private static BuildTarget iosBuildTarget = BuildTarget.iOS;

  delegate void BuildCmd();

  [PreferenceItem ("Kongregate")]
  static void PreferencesGUI () {
    // Load the preferences
    if (!prefsLoaded) {
      kongMobileDir = EditorPrefs.GetString (KONG_DIR_PREF_KEY, kongMobileDir);
      prefsLoaded = true;
    }

    // Preferences GUI
    kongMobileDir = EditorGUILayout.TextField ("Mobile Repository Location", kongMobileDir);

    // Save the preferences
    if (GUI.changed) {
      EditorPrefs.SetString (KONG_DIR_PREF_KEY, kongMobileDir);
    }
  }

  static string GetMobileRepoPath() {
    string path = CommandLineReader.GetCustomArgument("mobile.repo");
    if (path == "") {
      Debug.Log("no CLI arg for path");
      path = EditorPrefs.GetString (KONG_DIR_PREF_KEY, kongMobileDir);
    }
    Debug.Log("using path "+path);
    return path;
  }

  [MenuItem("Tools/Kongregate/Build iOS", false, 2)]
  static void BuildiOS ()
  {
    EditorUserBuildSettings.SwitchActiveBuildTarget(iosBuildTarget);
    SetVersion();
    PlayerSettings.applicationIdentifier = "com.kongregate.mobile.games.angryBots";
    string error = BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/iOS",iosBuildTarget,BuildOptions.Development);
    EditorApplication.ExecuteMenuItem("Tools/prime[31]/Manually run Xcode post processor...");
    if (!String.IsNullOrEmpty(error)) {
      Debug.Log("error building iOS: " + error);
      throw new Exception("error: " + error);
    }
  }
  [MenuItem("Tools/Kongregate/Build Android", false, 3)]
  static void BuildAndroid ()
  {
    //http://docs.unity3d.com/Documentation/ScriptReference/PlayerSettings.Android.html
    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
    SetVersion();
    PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
    PlayerSettings.applicationIdentifier = "com.kongregate.android.games.angrybots";

    // do the build
    string error = BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Android/build.apk",BuildTarget.Android,BuildOptions.Development);
    if (!String.IsNullOrEmpty(error)) {
      Debug.Log("error building Android: " + error);
      throw new Exception("error: " + error);
    }
  }

  [MenuItem("Tools/Kongregate/Build Active", false, 4)]
  static void BuildActiveTarget () {
    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
      BuildAndroid();
    } else {
      BuildiOS(); //if current is iPhone or not set
    }
  }

  static void BuildInActiveTarget () {
    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
      BuildiOS();
    } else {
      BuildAndroid(); //if current is iPhone or not set
    }
  }

  static void BuildAllTargets() {
    Dictionary<BuildTarget,BuildCmd> buildCmds = new Dictionary<BuildTarget,BuildCmd>()
    {
      { BuildTarget.WebGL, () => { BuildWebGL(); } },
      { BuildTarget.Android, () => { BuildAndroid(); } },
      { iosBuildTarget, () => { BuildiOS(); } }
    };

    if(buildCmds.ContainsKey(EditorUserBuildSettings.activeBuildTarget)) {
      Debug.Log("Building: " + EditorUserBuildSettings.activeBuildTarget);
      buildCmds[EditorUserBuildSettings.activeBuildTarget]();
      buildCmds.Remove(EditorUserBuildSettings.activeBuildTarget);
    }

    foreach(KeyValuePair<BuildTarget,BuildCmd> entry in buildCmds) {
      Debug.Log("Building: " + entry.Key);
      entry.Value();
    }
  }

  static void BuildWeb() {
    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebPlayer);
    string error = BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Web",BuildTarget.WebPlayer,BuildOptions.Development);
    if (!String.IsNullOrEmpty(error)) {
      Debug.Log("error building web: " + error);
      throw new Exception("error");
    }
  }

  static void BuildWebGL() {
    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebGL);
    string error = BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/WebGL",BuildTarget.WebGL,BuildOptions.Development);
    if (!String.IsNullOrEmpty(error)) {
      Debug.Log("error building webGL: " + error);
      throw new Exception("error");
    }
  }

  static void SetVersion() {
    string version = CommandLineReader.GetCustomArgument("mobile.sdk.version");
    if (version != "") {
      PlayerSettings.bundleVersion = version;
    }
    string tag = CommandLineReader.GetCustomArgument("mobile.sdk.tag");
    if (tag != "") {
      PlayerSettings.productName = "AngryBots-" + tag;
    } else {
      PlayerSettings.productName = "AngryBots";
    }
  }

  // [MenuItem ("Tools/Kongregate/Build Working Unity Package", false, 1)]
  static void Export() {
    string kongDir = GetMobileRepoPath();
    if (kongDir == "") {
      failMiserably();
    } else {
      ClearLog();
      // this is all that goes into the .unitypackage
      string[] files = {
        "Assets/Plugins/Android/KongregateSDK.aar",
        "Assets/Plugins/Android/adjust.jar",
        "Assets/Plugins/Android/deltadna-sdk.aar",
        "Assets/Plugins/iOS/KongUnityUtils.h",
        "Assets/Plugins/iOS/KongUnityUtils.mm",
        "Assets/Plugins/iOS/KongregateAppController.h",
        "Assets/Plugins/iOS/KongregateAppController.mm",
        "Assets/Plugins/iOS/libStoreKit.a",
        "Assets/Plugins/KongregateAPI.cs",
        "Assets/Plugins/Kongregate",
        "Assets/Editor/KongregatePostProcessor.dll",
        "Assets/Editor/XCodeSettings.projmods",
        "Assets/Editor/KongregateSDK.bundle",
        "Assets/Editor/KongregateUnityWrapper.framework",
        "Assets/Editor/KongregateSDK.framework",
        "Assets/Editor/DeltaDNA.framework",
        "Assets/Editor/AdjustSDK.framework"
      };

      string fileName = kongDir+"/out/production/Kongregate.unitypackage";
      if (!Directory.Exists(kongDir+"/out/production")) {
        Directory.CreateDirectory(kongDir+"/out/production");
      }
      AssetDatabase.ExportPackage(files, fileName, ExportPackageOptions.Recurse);
      Debug.Log("Kong: Exported package to "+fileName);
      // EditorApplication.Beep();
      EditorUtility.DisplayDialog("Kongregate Package Exported", "File is located at "+fileName, "Ok");
    }
  }

  [MenuItem ("Tools/Kongregate/Build Release Unity Package", false, 1)]
  static void Release() {
    Debug.Log("Building release...");
    BuildKongregateAPIDLL();
    UpdateSDKFilesWithPaths();
    Export();
  }

  [MenuItem ("Tools/Kongregate/Update working SDK files", false, 0)]
  static void UpdateSDK() {
    UpdateSDKFilesWithPaths();
    EditorApplication.Beep();
  }

  private static void UpdateSDKFilesWithPaths() {
    string kongDir = GetMobileRepoPath();
    if (kongDir == "") {
      failMiserably();
    } else {
      ClearLog();
      string kongregateAARPath = "/KongdroidSDK/build/outputs/aar/KongdroidSDK-release.aar";
      string adjustJARPath = "/lib/adjust.jar";
      string deltaAARPath = "/lib/deltadna-sdk-4.9.0.aar";
      string appDir = Application.dataPath;
      string unityResDir = appDir + "/Plugins/Kongregate/Resources/Kongregate";

      string[] dirs = { unityResDir };
      foreach(string dir in dirs) {
        if (!Directory.Exists(dir)) {
          Directory.CreateDirectory(dir);
        }
      }

      // Copy main Kongregate archive and support files
      FileUtil.ReplaceFile(kongDir + kongregateAARPath, appDir + "/Plugins/Android/KongregateSDK.aar");
      FileUtil.ReplaceFile(kongDir + adjustJARPath, appDir + "/Plugins/Android/adjust.jar");
      FileUtil.ReplaceFile(kongDir + deltaAARPath, appDir + "/Plugins/Android/deltadna-sdk.aar");

      // Copy resources that may be used directly by Unity
      string[] unityResources = {
        "kongregate_button.png",
        "notification_1.png",
        "notification_2.png",
        "notification_3.png",
        "notification_4.png",
        "notification_5.png",
        "notification_6.png",
        "notification_7.png",
        "notification_8.png",
        "notification_9.png",
      };
      foreach(string resource in unityResources) {
        FileUtil.ReplaceFile(kongDir+"/KongdroidSDK/res/drawable-xhdpi/" + resource, unityResDir + "/" + resource);
      }

      Debug.Log("Kong: Replaced JAR and Android Resources");

      ReplaceFileRecursiveDirectory(kongDir+"/UnitySDKWrapper/Assets", appDir);
      FileUtil.ReplaceFile(kongDir + "/UnitySDKWrapper/Adapters/SteamworksAdapter.cs", appDir + "/Scripts/Misc/SteamworksAdapter.cs");
      AssetDatabase.Refresh();
    }
  }

  private static void ReplaceFileRecursiveDirectory(string fromDir, string toDir) {
    DirectoryInfo dir = new DirectoryInfo(fromDir);
    if (!Directory.Exists(toDir)) {
      Directory.CreateDirectory(toDir);
    }
    FileInfo[] info = dir.GetFiles("*.*");
    foreach (FileInfo f in info) {
      string file = "/"+f.Name;
      FileUtil.ReplaceFile (fromDir+file, toDir+file);
      Debug.Log("Kong: Replaced "+toDir+file);
    }
    DirectoryInfo[] dirs = dir.GetDirectories();
    foreach (DirectoryInfo d in dirs) {
      ReplaceFileRecursiveDirectory(fromDir+"/"+d.Name, toDir+"/"+d.Name);
    }
  }

  private static void BuildKongregateAPIDLL() {
    string mobileRepo = GetMobileRepoPath();
    string angryBots = Application.dataPath + "/../";
    Debug.Log("Building Kongregate API DLL...");

    System.Diagnostics.Process proc = new System.Diagnostics.Process();
    proc.StartInfo.WorkingDirectory = mobileRepo + "/UnitySDKWrapper";
    proc.StartInfo.FileName = "/bin/sh";
    proc.StartInfo.Arguments = "-c \"" + mobileRepo + "/UnitySDKWrapper/build.sh\"";
    proc.StartInfo.UseShellExecute = false;
    proc.StartInfo.RedirectStandardError = true;
    proc.StartInfo.RedirectStandardOutput = true;
    proc.Start();
  	proc.WaitForExit();

    string stdOut = proc.StandardOutput.ReadToEnd();
    string stdErr = proc.StandardError.ReadToEnd();
    Debug.Log(stdOut);

    if(stdErr.Length > 0) {
      Debug.LogError("Building of Kongregate API DLL failed:\n" + stdErr);
      throw new SystemException("Building of Kongregate API DLL failed");
    } else {
      Debug.Log("Kongregate API DLL built successfully!");
    }

    proc.Close();
  }

  private static void failMiserably() {
    EditorUtility.DisplayDialog("Missing Kong Mobile Repo Location", "You first need to set your Mobile repo location in the Editor preferences", "Ok");
  }

  private static void ClearLog() {
    Assembly assembly = Assembly.GetAssembly(typeof(SceneView));

    System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
    MethodInfo method = type.GetMethod("Clear");
    method.Invoke(new object(), null);
  }

  static string[] GetScenePaths()
  {
    return new string[] { "Assets/KongregateGameObjectScene.unity" };
  }

}
