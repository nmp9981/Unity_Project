using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] Transform _gunBody;//총 부분, 회전하는 부분
    [SerializeField] float _range = 0f;//공격 범위
    [SerializeField] LayerMask _layerMask = 0;//특정 적만 공격하게
    [SerializeField] float spinSpeed = 0;//적 발견시 얼마나 빠르게 회전할지
    [SerializeField] float _fireRate = 0f;//연사속도
    float currentFireRate;//실제 연사속도

 
    Transform _target = null;//최종 공격 타겟
    
    void SearchEnemy()
    {
        Collider[] inCols = Physics.OverlapSphere(transform.position, _range, _layerMask);//범위내 해당 layermask가 존재하는 모든 콜라이더
        Transform shortestTarget = null;//가장 가까운 오브젝트

        if (inCols.Length > 0)//존재
        {
            float shortestDistance = Mathf.Infinity;
            foreach(Collider col in inCols)
            {
                float distance = Vector3.SqrMagnitude(transform.position - col.transform.position);
                if(shortestDistance > distance)//최소 거리 갱신
                {
                    shortestDistance = distance;
                    shortestTarget = col.transform;
                }
            }
        }
        _target = shortestTarget;
    }
    void Start()
    {
        currentFireRate = _fireRate;
        InvokeRepeating("SearchEnemy", 0f, 0.5f);//0.5초마다 SearchEnemy() 실행
    }

    // Update is called once per frame
    void Update()
    {
        if(_target == null)
        {
            _gunBody.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
        }
        else
        {
            Quaternion _lookRotation = Quaternion.LookRotation(_target.position);//적의 좌표를 바라보게, 특정 좌표를 바라보게 하는 회전값리턴
            Vector3 _euler = Quaternion.RotateTowards(_gunBody.rotation, _lookRotation, spinSpeed * Time.deltaTime).eulerAngles;//a->b 까지 c스피드로 회전

            _gunBody.rotation = Quaternion.Euler(0, _euler.y, 0);//y축 회전만 반영(포신은 y축만 회전)

            //터렛이 조준할 최종 방향
            Quaternion fireRotation = Quaternion.Euler(0, _lookRotation.y, 0);
            if (Quaternion.Angle(_gunBody.rotation, fireRotation) < 5f)//각도차
            {
                currentFireRate -= Time.deltaTime;
                if (currentFireRate < 0f)
                {
                    currentFireRate = _fireRate;
                    Debug.Log("발사");
                }
            }
        }
    }
}
