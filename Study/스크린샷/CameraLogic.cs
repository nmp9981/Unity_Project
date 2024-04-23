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
    [SerializeField] GameObject pipeObj;
    [SerializeField] GameObject childButton;
    [SerializeField] GameObject child2Button;
    [SerializeField] GameObject buttonSet;
    [SerializeField] GameObject cube;

    Color pipeColor = new Color(1, 102/255f, 51/255f, 1);
    string[] direction = new string[6]{ "back", "front", "left", "right", "up", "down" };
    HashSet<GameObject> pipeObjectList = new HashSet<GameObject>();
    void Start()
    {
        FileSetting();
        SearchPipe(pipeObj);
       
        foreach(GameObject go in pipeObjectList) go.SetActive(true);
    }
    void CameraSetting()
    {
        childButton.SetActive(false);
        child2Button.SetActive(false);
        buttonSet.SetActive(false);

        gameObject.transform.position = basicCamera.transform.position;
        Camera.main.backgroundColor = Color.white;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        
        GameObject.Find("기본 카메라").SetActive(false);
        GameObject.Find("기본 카메라 marker").SetActive(false);
        GameObject.Find("거리 라이트").SetActive(false);
        GameObject.Find("거리 라이트 (1)").SetActive(false);
        GameObject.Find("기본 카메라 up").SetActive(false);

        var sceneView = SceneView.lastActiveSceneView;
        sceneView.pivot = gameObject.transform.position;
        sceneView.orthographic = true;
    }
    void FileSetting()
    {
        string filePath = "C:\\Users\\tybna\\ChildShotOthQHD2";
        DirectoryInfo di = new DirectoryInfo(filePath);

        if (!di.Exists) di.Create();
    }
    //파이프 검색
    void SearchPipe(GameObject gm)
    {
        foreach (Transform child in gm.transform)
        {
            if (child.GetComponent<MeshRenderer>() != null)
            {
                child.GetComponent<MeshRenderer>().material.color = pipeColor;
                pipeObjectList.Add(gm);
            }
            else SearchPipe(child.gameObject);
        }
    }
    //전체 촬영
    public void AllObjectPipeOn()
    {
        Camera.main.GetComponent<Camera>().orthographic = true;
        Camera.main.fieldOfView = 26.7f;
        Camera.main.orthographicSize = 26.7f;
        CameraSetting();
        StartCoroutine(ScreenShoterAll(true));
    }
    public void AllObjectPipeOff()
    {
        foreach (GameObject go in pipeObjectList) go.SetActive(false);

        Camera.main.GetComponent<Camera>().orthographic = true;
        Camera.main.fieldOfView = 26.7f;
        Camera.main.orthographicSize = 26.7f;
        CameraSetting();
        StartCoroutine(ScreenShoterAll(false));
    }
    //부분 촬영
    void PartPictureCameraSetting(int dir,Bounds bound)
    {
        Debug.Log(bound.center + "  " + bound.size);
        float distance = 1;
        switch(dir){
            case 0://뒤
                distance = bound.size.z*20;
                Camera.main.orthographicSize = distance;
                this.gameObject.transform.position = bound.center + distance * Vector3.forward;
                break;
            case 1://앞
                distance = bound.size.z * 20;
                Camera.main.orthographicSize = distance;
                this.gameObject.transform.position = bound.center + distance * Vector3.back;
                break;
            case 2://왼
                distance = bound.size.x * 20;
                Camera.main.orthographicSize = distance;
                this.gameObject.transform.position = bound.center + distance * Vector3.left;
                break;
            case 3://오
                distance = bound.size.x * 20;
                Camera.main.orthographicSize = distance;
                this.gameObject.transform.position = bound.center + distance * Vector3.right;
                break;
            case 4://위
                distance = bound.size.y * 20;
                Camera.main.orthographicSize = distance;
                this.gameObject.transform.position = bound.center + distance * Vector3.up;
                break;
            case 5://아래
                distance = bound.size.y * 20;
                Camera.main.orthographicSize = distance;
                this.gameObject.transform.position = bound.center + distance * Vector3.down;
                break;
        }
    }
    IEnumerator PipeOnPicture()
    {
        foreach (GameObject go in pipeObjectList)
        {
            go.SetActive(true);
            Bounds bound = go.GetComponentInChildren<MeshRenderer>().bounds;
            
            float ScreenGet = 6;
            int i = 0;
            while (i < ScreenGet)
            {
                //카메라 세팅
                var sceneView = SceneView.lastActiveSceneView;
                sceneView.rotation = Quaternion.Euler(sixDirection[i]);
                this.transform.rotation = sceneView.rotation;

                PartPictureCameraSetting(i, bound);
                i++;

                //파일 위치 저장
                string filePath = $"C:\\Users\\tybna\\SShotPipeOn\\{go.name}";
                DirectoryInfo di = new DirectoryInfo(filePath);

                if (!di.Exists) di.Create();
                string storePath = filePath + "\\" + go.name + "_" + direction[i - 1];
                ScreenCapture.CaptureScreenshot(storePath + ".png");//사진 찍기
                
                yield return new WaitForSeconds(0.1f);
                
            }
            go.SetActive(false);
        }
    }
    public void PartPipeOn()
    {
        CameraSetting();
        StartCoroutine(PipeOnPicture());
    }
    public void PartPipeOff()
    {
        foreach (GameObject go in pipeObjectList) go.SetActive(false);
        CameraSetting();
        StartCoroutine(PipeOnPicture());
    }
    #region 전체 오브젝트(전체, 부분)
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
                if (child2.transform.childCount >= 2)
                {
                    child2.gameObject.SetActive(true);
                    foreach (Transform child3 in child2.transform)
                    {
                        child3.gameObject.SetActive(false);
                    }
                }
                else
                {
                    child2.gameObject.SetActive(false);
                }
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
        for (int i = 2; i < 3; i++)
        {
            GameObject childObj = mainobj.transform.GetChild(i).gameObject;
            for (int j = 0; j < childObj.transform.childCount; j++)
            {
                GameObject secondChildObj = childObj.transform.GetChild(j).gameObject;

                if (secondChildObj.transform.childCount <= 1)
                {
                    childObj.transform.GetChild(j).gameObject.SetActive(true);

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
                        if (objName.Contains("\"")) objName = objName.Replace("\"", "@");
                        string filePath = "C:\\Users\\tybna\\ChildShotOthQHD2\\" + childObj.name + "\\" + objName;

                        DirectoryInfo di = new DirectoryInfo(filePath);
                        if (!di.Exists) di.Create();
                        string storePath = filePath + "\\" + objName + "_" + direction[k - 1];

                        ScreenCapture.CaptureScreenshot(storePath + ".png"); //여기서 사진 찍기
                        yield return null;
                    }
                    childObj.transform.GetChild(j).gameObject.SetActive(false);
                }
                else
                {
                    for(int m=0;m< secondChildObj.transform.childCount; m++)
                    {
                        secondChildObj.transform.GetChild(m).gameObject.SetActive(true);

                        var sceneView = SceneView.lastActiveSceneView;
                        float ScreenGet = 6;
                        int k = 0;
                        while (k < ScreenGet)
                        {
                            Camera.main.transform.position = secondChildObj.transform.GetChild(m).gameObject.transform.position;
                            secondChildObj.transform.GetChild(m).gameObject.transform.position = cube.transform.position;

                            sceneView.rotation = Quaternion.Euler(sixDirection[k]);
                            this.transform.rotation = sceneView.rotation;
                            CameraPosAndBoundSetting(k, secondChildObj.transform.GetChild(m).gameObject);

                            k++;

                            //파일 위치 저장
                            string objName = secondChildObj.transform.GetChild(m).gameObject.name;
                            string secondObjName = secondChildObj.name;
                            if (secondObjName.Contains("\"")) secondObjName = secondObjName.Replace("\"", "@");
                            if (objName.Contains("\"")) objName = objName.Replace("\"", "@");
                            string filePath = "C:\\Users\\tybna\\ChildShotOthQHD2\\" + childObj.name + "\\"+secondObjName +"\\"+ objName;

                            DirectoryInfo di = new DirectoryInfo(filePath);
                            if (!di.Exists) di.Create();
                            string storePath = filePath + "\\" + secondObjName+"_"+ objName + "_" + direction[k - 1];

                            ScreenCapture.CaptureScreenshot(storePath + ".png"); //여기서 사진 찍기
                            yield return null;
                        }
                        secondChildObj.transform.GetChild(m).gameObject.SetActive(false);
                    }
                }
                
            }
        }
        Debug.Log("완료");
        childButton.SetActive(true);
        child2Button.SetActive(true);
    }
    IEnumerator ScreenShoterAll(bool onoff)
    {
        var sceneView = SceneView.lastActiveSceneView;
        float ScreenGet = 6;
        int i = 0;
        while (i < ScreenGet)
        {
            //카메라 세팅
            sceneView.rotation = Quaternion.Euler(sixDirection[i]);
            this.transform.rotation = sceneView.rotation;
            CameraPosSetting(i, mainobj);
            i++;

            //파일 위치 저장
            string filePath;
            if (onoff) filePath = $"C:\\Users\\tybna\\SShotAllPipeOn\\{mainobj.gameObject.name}";
            else filePath = $"C:\\Users\\tybna\\SShotAllPipeOff\\{mainobj.gameObject.name}";
            DirectoryInfo di = new DirectoryInfo(filePath);

            if (!di.Exists) di.Create();
            string storePath = filePath + "\\" + mainobj.name + "_" + direction[i - 1];
            ScreenCapture.CaptureScreenshot(storePath + ".png");//사진 찍기
            yield return null;
        }
        Debug.Log("완료");
        childButton.SetActive(true);
        child2Button.SetActive(true);
    }
    #endregion
}
