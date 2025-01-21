using UnityEngine;

public class SupporterUnit : MonoBehaviour
{
    [SerializeField]
    float unitSpeed;

    [SerializeField]
    float rayInspectDist;//ray 인식 거리

    GameObject attackTarget;
    const float maxInspectDist = 15;

    void Awake()
    {
        
    }
    private void OnEnable()
    {
        
    }
    
    void Update()
    {
        MoveSupportUnit();
        InspectEnemy();
    }
    /// <summary>
    /// 소환수 이동
    /// 전투중이 아닐때만 이동
    /// </summary>
    void MoveSupportUnit()
    {
        if (!GameManager.Instance.IsFighting)
        {
            transform.position += Vector3.right * Time.deltaTime * unitSpeed;
        }
    }
    /// <summary>
    /// 적 인식
    /// </summary>
    void InspectEnemy()
    {
        //RayCast로 물체 인식
        RaycastHit2D rayHitObj = Physics2D.Raycast(transform.position, transform.right, rayInspectDist);
        Debug.DrawRay(transform.position, transform.right * rayInspectDist, Color.red, 10f);
        if (rayHitObj)
        {
            //적일 경우
            if (rayHitObj.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("몬스터 인식");
                GameManager.Instance.IsFighting = true;
            }
        }
    }
}
