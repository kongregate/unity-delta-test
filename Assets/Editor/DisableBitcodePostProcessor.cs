using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public static class DisableBitCodePostProcessor
{
  [PostProcessBuild(999)]
  public static void OnPostProcessBuild( BuildTarget buildTarget, string path)
  {
    #if UNITY_IPHONE

    if(buildTarget == BuildTarget.iOS)
    {
      Debug.Log("DisableBitCodePostProcessor - start");
      string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
      UnityEditor.iOS.Xcode.PBXProject pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
      pbxProject.ReadFromFile(projectPath);

      string target = pbxProject.TargetGuidByName("Unity-iPhone");            
      pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

      pbxProject.WriteToFile (projectPath);
      Debug.Log("DisableBitCodePostProcessor - done");
    }

    #endif

  }
}