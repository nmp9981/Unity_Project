using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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

    string[] direction = new string[6]{ "back", "front", "left", "right", "up", "down" };
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
        string filePath = "C:\\Users\\tybna\\ChildShotOthQHD2";
        DirectoryInfo di = new DirectoryInfo(filePath);

        if (!di.Exists) di.Create();
    }
    //카메라 위치 세팅
    void CameraPosSetting(int dir, GameObject obj)
    {
        Bounds bounds = new Bounds();

        Transform[] myChildren = obj.GetComponentsInChildren<Transform>();
        foreach (Transform child in myChildren)
        {
            if (child.GetComponent<MeshRenderer>() != null)
            {
                if (child.GetComponent<BoxCollider>() == null) child.AddComponent<BoxCollider>();

                child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                child.GetComponent<MeshRenderer>().receiveShadows = false;
                bounds.Encapsulate(child.GetComponent<BoxCollider>().bounds);
                
            }
        }
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        
        float maxSize = Mathf.Min(size.x, size.y, size.z);
        Debug.Log(center + "@@@@" + size);
        float zoom = 1f;
    
        switch (dir)
        {
            case 0:
                this.transform.position = new Vector3(340, 105, 510);
                //this.transform.position = center + zoom * maxSize * Vector3.forward;
                break;
            case 1:
                this.transform.position = new Vector3(340, 105, 0);
                //this.transform.position = center + zoom * maxSize * Vector3.back;
                break;
            case 2:
                this.transform.position = new Vector3(0, 105, 210);
                //this.transform.position = center + zoom * maxSize * Vector3.left;
                break;
            case 3:
                this.transform.position = new Vector3(510, 105, 210);
                //this.transform.position = center + zoom * maxSize * Vector3.right;
                break;
            case 4:
                this.transform.position = new Vector3(335, 510, 210);
                //this.transform.position = center + zoom * maxSize * Vector3.up;
                break;
            case 5:
                this.transform.position = new Vector3(335, 0, 210);
                //this.transform.position = center + zoom * maxSize * Vector3.down;
                break;
        }
    }
    //카메라 위치 세팅
    void CameraPosAndBoundSetting(int dir, GameObject obj)
    {
        Bounds bounds = new Bounds();

        if (obj.transform.childCount == 0)
        {
            if (obj.GetComponent<BoxCollider>() == null) obj.AddComponent<BoxCollider>();

            obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            obj.GetComponent<MeshRenderer>().receiveShadows = false;
            bounds = obj.GetComponent<BoxCollider>().bounds;
        }
        else
        {
            foreach (Transform child in obj.transform)
            {
                if (child.GetComponent<MeshRenderer>() == null)
                {
                    foreach (Transform childSecond in child.transform)
                    {
                        if (childSecond.GetComponent<MeshRenderer>() == null)
                        {
                            foreach (Transform childthird in childSecond.transform)
                            {
                                if (childthird.GetComponent<BoxCollider>() == null) childthird.AddComponent<BoxCollider>();

                                bounds = childthird.GetComponent<BoxCollider>().bounds;
                                childthird.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                                childthird.GetComponent<MeshRenderer>().receiveShadows = false;
                            }
                        }
                        else
                        {
                            if (childSecond.GetComponent<BoxCollider>() == null) childSecond.AddComponent<BoxCollider>();

                            bounds = childSecond.GetComponent<BoxCollider>().bounds;
                            childSecond.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                            childSecond.GetComponent<MeshRenderer>().receiveShadows = false;
                        }
                        /*
                        if (childSecond.GetComponent<BoxCollider>() == null) childSecond.AddComponent<BoxCollider>();

                        bounds = childSecond.GetComponent<BoxCollider>().bounds;
                        childSecond.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        childSecond.GetComponent<MeshRenderer>().receiveShadows = false;
                        */
                    }
                }
                else
                {
                    if (child.GetComponent<BoxCollider>() == null) child.AddComponent<BoxCollider>();

                    bounds = child.GetComponent<BoxCollider>().bounds;
                    child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    child.GetComponent<MeshRenderer>().receiveShadows = false;
                }
            }
        }

        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        float maxSize = Mathf.Max(size.x, size.y, size.z);
        float zoom = 2.5f;

        switch (dir)
        {
            case 0:
                this.transform.position = center + zoom * maxSize * Vector3.forward;
                this.transform.position = new Vector3(340, 105, 510);
                break;
            case 1:
                this.transform.position = center + zoom * maxSize * Vector3.back;
                this.transform.position = new Vector3(340, 105, 0);
                break;
            case 2:
                this.transform.position = center + zoom * maxSize * Vector3.left;
                this.transform.position = new Vector3(0, 105, 210);
                break;
            case 3:
                this.transform.position = center + zoom * maxSize * Vector3.right;
                this.transform.position = new Vector3(510, 105, 210);
                break;
            case 4:
                this.transform.position = center + zoom * maxSize * Vector3.up;
                this.transform.position = new Vector3(335, 510, 210);
                break;
            case 5:
                this.transform.position = center + zoom * maxSize * Vector3.down;
                this.transform.position = new Vector3(335, 0, 210);
                break;
        }
    }
    public void ObjectSearch()
    {
        Camera.main.GetComponent<Camera>().orthographic = true;
        Camera.main.fieldOfView = 26.7f;
        Camera.main.orthographicSize = 26.7f;
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
        Camera.main.GetComponent<Camera>().orthographic = true;
        Camera.main.fieldOfView = 26.7f;
        Camera.main.orthographicSize = 26.7f;

        CameraSetting();
        childButton.SetActive(false);
        child2Button.SetActive(false);

        foreach (Transform child in mainobj.transform)
        {
            if (child.name == "Unknown") break;
            child.gameObject.SetActive(true);
            foreach (Transform child2 in child.transform)
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
                if (k == 4)
                {
                    for (int x = 0; x < 4; x++) mainobj.transform.GetChild(x).gameObject.SetActive(true);
                    CameraPosSetting(i, mainobj);
                }
                else CameraPosSetting(i, mainobj.transform.GetChild(k).gameObject);
               
                i++;

                //파일 위치 저장
                string filePath = $"C:\\Users\\tybna\\SShot\\{mainobj.transform.GetChild(k).gameObject.name}";
                DirectoryInfo di = new DirectoryInfo(filePath);

                if (!di.Exists) di.Create();
                string storePath = filePath + "\\" + mainobj.transform.GetChild(k).gameObject.name+"_"+direction[i-1];
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
                    Camera.main.transform.position = childObj.transform.GetChild(j).gameObject.transform.position;
                    childObj.transform.GetChild(j).gameObject.transform.position = cube.transform.position;

                    sceneView.rotation = Quaternion.Euler(sixDirection[k]);
                    this.transform.rotation = sceneView.rotation;
                    CameraPosAndBoundSetting(k, childObj.transform.GetChild(j).gameObject);

                    k++;

                    //파일 위치 저장
                    string objName = childObj.transform.GetChild(j).gameObject.name;
                    if (objName.Contains("\""))objName = objName.Replace("\"", "@");
                    string filePath = "C:\\Users\\tybna\\ChildShotOthQHD2\\" + childObj.name + "\\" + objName;   

                    DirectoryInfo di = new DirectoryInfo(filePath);
                    if (!di.Exists) di.Create();
                    string storePath = filePath + "\\" + objName +"_"+ direction[k-1];
                   
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
