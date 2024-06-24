using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    static PortalManager _portalInstance;
   
    public static PortalManager PortalInstance { get { Init(); return _portalInstance; } }
    static void Init()
    {
        if (_portalInstance == null)
        {
            GameObject gm = GameObject.Find("PortalManager");
            if (gm == null)
            {
                gm = new GameObject { name = "PortalManager" };

                gm.AddComponent<PortalManager>();
            }
            DontDestroyOnLoad(gm);
            _portalInstance = gm.GetComponent<PortalManager>();
        }
    }
   
    void Awake()
    {
        Init();
    }
    private void Update()
    {
        TimeFlow();
    }
    void TimeFlow()
    {
        _curTime += Time.deltaTime;
    }
    public List<Portal> portalist;
    public bool _isUsePortal;
    float _curTime;
    float _coolTime = 2f;
    public bool IsUsePortal { get { return _isUsePortal; } set { _isUsePortal = value; } }
    public float Curtime { get { return _curTime; } set { _curTime = value; } }
    public float Cooltime { get { return _coolTime; }}
}
