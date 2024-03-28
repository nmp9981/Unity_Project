using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting.IonicZip;
using System.IO.Compression;
using System;

public class PressZip : MonoBehaviour
{
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
    //압축 풀기
    public static bool UnzipFile(string zipPath, string unzipPath)
    {
        try
        {
            if (Directory.Exists(unzipPath))
            {
                Directory.Delete(unzipPath);
            }

            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, unzipPath);

            Debug.Log("해제 성공");
            return true;
        }
        catch
        {
            Debug.Log("해제 실패");
            return false;
        }
    }
}
