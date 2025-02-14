using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    [SerializeField] Color color = Color.black;
    [SerializeField, Range(1, 8)] protected int depth = 5;//세대
    [SerializeField, Range(1f, 5f)] protected float length = 2f;//나뭇가지 길이
    [SerializeField, Range(0.5f, 0.9f)] protected float attenuation = 0.635f;//길이 공비
    [SerializeField, Range(0f, 90f)] protected float angle = 20f;//회전 각도

    Material lineMat;

    /// <summary>
    /// 쉐이더, 머테리얼 등록
    /// </summary>
    void OnEnable()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        if (shader == null)
        {
            Debug.LogWarning("Shader Hidden/Internal-Colored not found");
        }
        lineMat = new Material(shader);
    }

    /// <summary>
    /// 그리기
    /// </summary>
    /// <param name="depth">세대</param>
    /// <param name="length">가지 길이</param>
    void DrawLSystem(int depth, float length = 2f)
    {
        //선 색상 설정
        lineMat.SetColor("_Color", color);
        //GL을 그리기 위해 아래 코드를 실행해야함
        lineMat.SetPass(0);

        //프랙탈 그리기
        DrawFractal(transform.localToWorldMatrix, depth, length);
    }
    /// <summary>
    /// 프랙탈 그리기
    /// </summary>
    /// <param name="current"></param>
    /// <param name="depth"></param>
    /// <param name="length"></param>
    void DrawFractal(Matrix4x4 current, int depth, float length)
    {
        if (depth <= 0) return;

        //찐 그리기
        GL.MultMatrix(current);//모델 행렬을 지정 행렬로 설정
        GL.Begin(GL.LINES);//렌더링 모드 시작, GL.LINES : 선을 그림
        GL.Vertex(Vector3.zero);
        GL.Vertex(new Vector3(0f, length, 0f));//길이 만큼 렌더링
        GL.End();//렌더링 작업 종료

        //왼쪽, z축 기준 angle만큼 회전
        GL.PushMatrix();//현재 트랜스폼을 스택에 저장
        var ml = current * Matrix4x4.TRS(new Vector3(0f, length, 0f), Quaternion.AngleAxis(-angle, Vector3.forward), Vector3.one);
        DrawFractal(ml, depth - 1, length * attenuation);
        GL.PopMatrix();//스택에 저장한 트랜폼을 불러오는 함수

        //오른쪽
        GL.PushMatrix();
        var mr = current * Matrix4x4.TRS(new Vector3(0f, length, 0f), Quaternion.AngleAxis(angle, Vector3.forward), Vector3.one);
        DrawFractal(mr, depth - 1, length * attenuation);
        GL.PopMatrix();
    }

    /// <summary>
    /// 씬 렌더링 관련 이벤트 함수
    /// 모든 씬 렌더링 종료 후에 호출됩니다. 카메라 장면 렌더링 후 호출
    /// 스크립트가 있는 모든 객체에서 호출
    /// GL 클래스 또는 Graphics.DrawMeshNow을 사용하여 이시점에 사용자 지정 지오메트리를 그릴 수 있습니다.
    /// </summary>
    void OnRenderObject()
    {
        DrawLSystem(depth, length);
    }
}
