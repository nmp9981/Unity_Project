using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    Vector3[] sixDirection =
    {
        new Vector3(0,180,0),//뒤
        new Vector3(0,0,0),//앞
        new Vector3(0,90,0),//왼쪽
        new Vector3(0,-90,0),//오른쪽
        new Vector3(90,0,90),//위
        new Vector3(-90,0,-90)//아래
    };
    [SerializeField] Camera basicCamera;
    [SerializeField] GameObject mainobj;
    [SerializeField] GameObject childButton;
    [SerializeField] GameObject child2Button;
    [SerializeField] GameObject cube;
    void Start()
    {
        FileSetting();
    }
    void CameraSetting()
    {
        childButton.SetActive(true);
        child2Button.SetActive(true);

        transform.position = basicCamera.transform.position;
        Camera.main.backgroundColor = Color.white;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        GameObject.Find("기본 카메라").SetActive(false);
        GameObject.Find("기본 카메라 marker").SetActive(false);
        GameObject.Find("거리 라이트").SetActive(false);
        GameObject.Find("거리 라이트 (1)").SetActive(false);
        GameObject.Find("기본 카메라 up").SetActive(false);

        var sceneView = SceneView.lastActiveSceneView;
        sceneView.pivot = transform.position;
        sceneView.orthographic = true;
    }
    void FileSetting()
    {
        string filePath = "C:\\Users\\tybna\\SShot";
        DirectoryInfo di = new DirectoryInfo(filePath);

        if (!di.Exists) di.Create();
    }
    //카메라 위치 세팅
    void CameraPosSetting(int dir, GameObject obj)
    {
        if(obj.GetComponent<MeshRenderer>()==null) obj.AddComponent<MeshRenderer>();
       
        Bounds bounds = obj.GetComponent<MeshRenderer>().bounds;
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        switch (dir)
        {
            case 0:
                this.transform.position = new Vector3(335,100,280);
                
                break;
            case 1:
                this.transform.position = new Vector3(335, 100, 135); ;
                
                break;
            case 2:
                this.transform.position = new Vector3(270, 105, 210);
                
                break;
            case 3:
                this.transform.position = new Vector3(400, 105, 210);
                
                break;
            case 4:
                this.transform.position = new Vector3(330, 155, 210);
                
                break;
            case 5:
                this.transform.position = new Vector3(335, 40, 210);
               
                break;
        }
    }
    public void ObjectSearch()
    {
        Camera.main.fieldOfView = 60;
        CameraSetting();
        childButton.SetActive(false);
        child2Button.SetActive(false);
        
        foreach (Transform child in mainobj.transform)
        {
            child.gameObject.SetActive(false);
        }
        StartCoroutine(ScreenShoterChild());
    }

    public void ChildObjectSearch()
    {
        Camera.main.fieldOfView = 50;
        CameraSetting();
        childButton.SetActive(false);
        child2Button.SetActive(false);

        foreach (Transform child in mainobj.transform)
        {
            child.gameObject.SetActive(true);
            foreach(Transform child2 in child.transform)
            {
                child2.gameObject.SetActive(false);
            }
        }
        StartCoroutine(ScreenShoterSecondChild());
    }
    IEnumerator ScreenShoterChild()
    {
        for (int k = 0; k < 5; k++)
        {
            mainobj.transform.GetChild(k).gameObject.SetActive(true);

            var sceneView = SceneView.lastActiveSceneView;
            float ScreenGet = 6;
            int i = 0;
            while (i < ScreenGet)
            {
                //카메라 세팅
                sceneView.rotation = Quaternion.Euler(sixDirection[i]);
                this.transform.rotation = sceneView.rotation;
                CameraPosSetting(i, mainobj.transform.GetChild(k).gameObject);

                i++;
               
                //파일 위치 저장
                string filePath = "C:\\Users\\tybna\\SShot\\" + mainobj.transform.GetChild(k).gameObject.name;              
                DirectoryInfo di = new DirectoryInfo(filePath);

                if (!di.Exists) di.Create();
                string storePath = filePath + "\\"+i;
                ScreenCapture.CaptureScreenshot(storePath + ".png");//사진 찍기
                yield return null;
            }
            mainobj.transform.GetChild(k).gameObject.SetActive(false);
        }
        Debug.Log("완료");
        childButton.SetActive(true);
        child2Button.SetActive(true);
    }

    IEnumerator ScreenShoterSecondChild()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject childObj = mainobj.transform.GetChild(i).gameObject;
            for (int j = 0; j < childObj.transform.childCount; j++)
            {
                childObj.transform.GetChild(j).gameObject.SetActive(true);

                //Vector3 localPos = childObj.transform.GetChild(j).gameObject.transform.localPosition;//원래 로컬 좌표
                //childObj.transform.GetChild(j).gameObject.transform.TransformPoint(localPos);

                var sceneView = SceneView.lastActiveSceneView;
                float ScreenGet = 6;
                int k = 0;
                while (k < ScreenGet)
                {
                    sceneView.rotation = Quaternion.Euler(sixDirection[k]);
                    this.transform.rotation = sceneView.rotation;
                    CameraPosSetting(k, childObj.transform.GetChild(j).gameObject);

                    childObj.transform.GetChild(j).gameObject.transform.position = cube.transform.position;
                    k++;
                   
                    //파일 위치 저장
                    string filePath = "C:\\Users\\tybna\\SShot\\" + childObj.name+"\\"+ childObj.transform.GetChild(j).gameObject.name;
                    if (filePath.Contains("\"")) filePath = filePath.Replace("\"", "@");
                    Debug.Log(filePath);
                    DirectoryInfo di = new DirectoryInfo(filePath);

                    if (!di.Exists) di.Create();
                    string storePath = filePath + "\\" + k;
                    ScreenCapture.CaptureScreenshot(storePath + ".png"); //여기서 사진 찍기
                    yield return null;
                }
                childObj.transform.GetChild(j).gameObject.SetActive(false);
            }
        }
        Debug.Log("완료");
        childButton.SetActive(true);
        child2Button.SetActive(true);
    }
}
