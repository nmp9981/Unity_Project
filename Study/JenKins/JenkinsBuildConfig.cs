using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class JenkinsBuildConfig : MonoBehaviour
    {
        [MenuItem("Build/build start")]
        public static void AutoBuildConfig()
        {
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/SampleScene.unity" },
                locationPathName = $"Builds/Meta-Plant_{PlayerSettings.bundleVersion}.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result)
            {
                case UnityEditor.Build.Reporting.BuildResult.Succeeded:
                    Debug.Log($"Build Succeeded : {summary.totalSize} bytes");
                    break;
                case UnityEditor.Build.Reporting.BuildResult.Failed:
                    Debug.Log("Build Failed");
                    break;
                case UnityEditor.Build.Reporting.BuildResult.Unknown:
                case UnityEditor.Build.Reporting.BuildResult.Cancelled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
