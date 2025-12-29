using System.Collections.Generic;

/// <summary>
/// 충돌 정보 : 결과 보고서 개념, 추후 삭제
/// </summary>
public class ContactInfo
{
    //두 충돌체
    public CustomRigidBody rigidA;
    public CustomRigidBody rigidB;

    public bool isCollide;//충돌 여부
    public Vec3 normal;//어느 방향으로 밀어낼지
    public float penetration;//겹침량, 위치 제약
    public Vec3 tangent;//탄젠트 벡터
    public Vec3 contactPoint;// 회전

    public float normalImpulse;   // 누적 Jn, Solver 기억 값
    public float tangentImpulse;  // 누적 Jt, Solver 기억 값

    public float invMassSum;//두 물체의 질량 역수의 합
    public float frictionValue;//마찰 계수

    public List<ContactPoint> contacts = new List<ContactPoint>(4);
}

/// <summary>
/// 같은 두 물체 사이의 contact 묶음(최대 4개)
/// </summary>
public class ContactManifold
{
    //두 물체
    public CustomRigidBody rigidA;
    public CustomRigidBody rigidB;

    //노말 벡터, 마찰 벡터와 두 물체사이의 마찰 계수
    public Vec3 normal;
    public Vec3 tangent;
    public float frictionValue;

    //새로운 접촉점 여부
    public bool hasNewContact;
    //접촉점 모음
    public List<ContactPoint> points = new List<ContactPoint>(4);
}

/// <summary>
/// Impulse를 저장하는 최소 단위 (한 접촉점)
/// </summary>
public class ContactPoint
{
    public Vec3 localPointA;//a 로컬 좌표
    public Vec3 localPointB;//b 로컬 좌표

    public float normalImpulse;//누적 jn
    public float tangentImpulse;//누적 jt

     //이전 프레임
 public float prevNormalImpulse;
 public float prevTangentImpulse;
    
    //이번 프레임 - 이전 프레임
    public float deltaNormalImpulse;
    public float deltaTangentImpulse;

    public float positionalImpulse;//Split impulse 전용

    //각 정보
    public Vec3 contactNormal;//접촉 노멀
    //중심->접촉점 벡터
    public Vec3 rotationA;
    public Vec3 rotationB;

    //선속도
    public Vec3 linearVelocityA;
    public Vec3 linearVelocityB;

    //각속도
    public Vec3 angularVelocityA;
    public Vec3 angularVelocityB;

    //관성모먼트
    public float IMomentA;
    public float IMomentB;

    public float restitution;
}

/// <summary>
/// 접촉점 모음(그래프)
/// </summary>
public class Island
{
    public List<CustomRigidBody> bodies = new();//접촉한 물체가 속한 그룹 리스트
    public List<ContactManifold> manifolds = new();//접촉한 물체가 속한 manifold 리스트

    public bool isSleeping = false;//정지 여부
    public int sleepCounter = 0;//타이머
}
