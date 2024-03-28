using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MGIZMO.Editor
{
    public class JenkinsBuildConfig : MonoBehaviour
    {
        private static readonly string licensePath = "C:\\unity-build-result\\Aspose.3DProductFamily.lic";
        private static readonly string buildDirectoryPath =
            "C:\\NAVER WORKS Drive\\. Public_Root\\프로젝트@21000000089073\\DSEC_MetaMarine\\99. Build";
        private static readonly string naverCloudDirectoryPath =
            "C:\\NAVER WORKS Drive\\. Public_Root\\프로젝트@21000000089073\\DSEC_MetaMarine\\99. Build";


        [MenuItem("Build/clean build start")]
        public static void CleanBuildStart()
        {
            AutoBuildConfig("clean");
        }
        [MenuItem("Build/genernal build start")]
        public static void GenernalBuildStart()
        {
            var buildResultDirectoryPath = AutoBuildConfig("genernal");
            PressZip.ZipFiles(buildResultDirectoryPath, $"{buildResultDirectoryPath}.zip");
        }

        public static string AutoBuildConfig(string state)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("00");
            var day = DateTime.Now.Day.ToString("00");
            var nowDirectoryPath = $"{buildDirectoryPath}/{year}{month}{day}/{state}";
            var projectName = $"Meta-Marine_{year}{month}{day}";

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Metacle UI/Scenes/MainScene.unity" },
                locationPathName = $"{nowDirectoryPath}/{projectName}.exe",
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
            string dataFolderPath = $"{nowDirectoryPath}/{projectName}_Data";
            if (Directory.Exists(dataFolderPath) && File.Exists(licensePath))
            {
                System.IO.File.Copy(licensePath, $"{dataFolderPath}/Aspose.3DProductFamily.lic", true);
            }

            return nowDirectoryPath;
        }
    }
}
