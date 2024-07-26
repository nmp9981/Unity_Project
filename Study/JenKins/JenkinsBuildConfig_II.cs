using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;

namespace Jenkins
{
    public class JenkinsBuildConfig : MonoBehaviour
    {
        [MenuItem("Build/build start")]
        public static void AutoBuildConfig()
        {
            var toDate = DateTime.Now.ToString("yyyy_MM_dd");
            var buildDirectoryPath = $"C:/unity build/{toDate}";
            var naverCloudDirectoryPath = "C:/NAVER WORKS Drive/. Public_Root/프로젝트@21000000089073/삼성/Build";
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Main.unity" },
                locationPathName = $"{buildDirectoryPath}/{toDate}_Metacle_MARProject_Dev_{PlayerSettings.bundleVersion}.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            System.IO.Directory.CreateDirectory(buildDirectoryPath);

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"Build Succeeded : {summary.totalSize} bytes");
                    string nowDirectoryPath = $"{buildDirectoryPath}";
                    if (Directory.Exists(nowDirectoryPath))
                    {
                        string naverCloudDirectoryPathname = $"{naverCloudDirectoryPath}/{toDate}_Metacle_MARProject_Dev_{PlayerSettings.bundleVersion}.exe";
                        if (!Directory.Exists(naverCloudDirectoryPathname))
                        {
                            Directory.CreateDirectory(naverCloudDirectoryPathname);
                        }
                        ZipFiles(nowDirectoryPath, $"{naverCloudDirectoryPathname}.zip");
                    }
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Failed:
                    Debug.Log("Build failed");
                    //EditorApplication.Exit(1);
                    break;
                case BuildResult.Unknown:
                case BuildResult.Cancelled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        //압축 하기
        public static bool ZipFiles(string directoryPath, string outputZipPath)
        {
            try
            {
                if (File.Exists(outputZipPath))//폴더 존재 여부
                {
                    File.Delete(outputZipPath);
                }
                System.IO.Compression.ZipFile.CreateFromDirectory(directoryPath, outputZipPath);
                Debug.Log("압축 성공");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("압축 실패");
                return false;
            }
        }
    }
}
