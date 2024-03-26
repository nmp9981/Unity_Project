using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;
using Cysharp.Threading.Tasks;

public class ATTReader : MonoBehaviour
{
    string path = "C:\\Users\\tybna\\OneDrive\\바탕 화면\\ResourcesAsset\\Outf0320\\Outfit\\Outfit\\";
    string LargeObjName = "DM-DF-AD0_16CTF-OUTF.att";//7442kb
    string MiddleObjName = "DM-ME-A40_16CUE-SEAT.att";//785kb
    string SmallObjName = "DM-AE-H40_94KTE-TRAY.att";//24kb

    string objPath;
    bool flag = false;
    Stopwatch watch;

    void Awake()
    {
        SettingFile();
        //StreamReaderResult();
        CreateAttObject();
        //StreamAllReaderResult();
        //BinaryReaderResult();
        EndFile();
    }
    void SettingFile()
    {
        objPath = path + LargeObjName;
        watch = new Stopwatch();
        watch.Start();
    }
    void EndFile()
    {
        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds + " ms");
    }
    void StreamReaderResult()
    {
        StreamReader streamReader = new StreamReader(objPath);


        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            if (!flag)
            {
                if (line.Substring(0, 3) == "END")
                {
                    flag = true;
                    continue;
                }
                continue;
            }
            //UnityEngine.Debug.Log(line);

        }
    }
    void StreamAllReaderResult()
    {
        StreamReader streamReader = new StreamReader(objPath);
        string line = streamReader.ReadToEnd();
        UnityEngine.Debug.Log(line);
        UnityEngine.Debug.Log(line[35]);
    }
    void BinaryReaderResult()
    {
        byte[] bytes = File.ReadAllBytes(objPath);
        foreach (byte s in bytes)
        {
            UnityEngine.Debug.Log(s);
        }
    }
    void AllFileStream()
    {
        StreamWriter sw = new StreamWriter(objPath, false);
        StreamReader sr = new StreamReader(objPath);

        int bufferSize = 4096; // Basic Stream Size
        int remainBuffer = 0;
        char[] buffer = new char[bufferSize];

        while (sr.EndOfStream == false)
        {
            remainBuffer = sr.Read(buffer, 0, bufferSize);

            if (sr.EndOfStream) break;
            sw.Write(buffer);
        }

        // write reamin contents
        for (int i = 0; i < remainBuffer; i++)
        {
            sw.Write(buffer[i]);
        }
    }
    public async UniTaskVoid CreateAttObject()
    {
        ATTReader attReader = new ATTReader();
        
        try
        {
            List<AttObject> parsingResult = new List<AttObject>();
            AttObject attObject = null;
            StreamReader streamReader = new StreamReader(objPath);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                line = line.Trim();//공백 제거
                if (line.Contains("NEW"))
                {
                    attObject = new AttObject(objectName: line.Substring(3));
                    parsingResult.Add(attObject);
                }else if (!line.Contains("END"))
                {
                    int sIdx = line.IndexOf(":=");
                    string key = line.Substring(0, sIdx).Trim();
                    string value = line.Substring(sIdx + 2).Trim();

                    if (attObject != null)
                    {
                        attObject.getAttributeList().Add(new Attribute(key,value));
                    }
                }
                //UnityEngine.Debug.Log(line);
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("생성 불가");
        }
    }
}
