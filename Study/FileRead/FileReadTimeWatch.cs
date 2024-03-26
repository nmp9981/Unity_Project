using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Diagnostics;

public class ATTReader : MonoBehaviour
{
    string path = "C:\\Users\\tybna\\OneDrive\\바탕 화면\\ResourcesAsset\\Outf0320\\Outfit\\Outfit\\";
    string LargeObjName = "DM-DF-AD0_16CTF-OUTF.att";
    string objName = "DM-AE-H40_94KTE-TRAY.att";

    bool flag = false;
    
    void Awake()
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();

        string objPath = path + LargeObjName;
        StreamReader streamReader = new StreamReader(objPath);

        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            if (line.Substring(0, 3) == "END" && !flag)
            {
                flag = true;
                continue;
            }

            if (!flag) continue;
            UnityEngine.Debug.Log(line);

        }
        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds+" ms");
    }
}
