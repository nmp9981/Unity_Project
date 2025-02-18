using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleComputeShaderArray : MonoBehaviour
{
    //컴퓨트 쉐이더 참조
    public ComputeShader computeShader;
    //실행하는 커널의 인덱스
    int kernerIndex_kernelFunctionA;
    int kernerIndex_kernelFunctionB;
    //실행결과를 저장하는 버퍼
    ComputeBuffer intComputeBuffer;

    // Start is called before the first frame update
    void Start()
    {
        //실행할 커널의 인덱스 획득
        this.kernerIndex_kernelFunctionA = this.computeShader.FindKernel("KernerFunction_A");//0
        this.kernerIndex_kernelFunctionB = this.computeShader.FindKernel("KernerFunction_B");//1

        //CPU에 저장할 버퍼 공간 준비
        // 저장 공간의 크기, 저장 데이터의 단위당 크기를 지정하여 초기화합니다.여기에서는 int타입의 크기 10개 만큼의 영역
        // int[10]
        this.intComputeBuffer = new ComputeBuffer(10, sizeof(int));

        //kernelFunctionA가 실행될때 intBuffer를 사용, intComputeBuffer라는 CPU버퍼에 저장
        this.computeShader.SetBuffer(this.kernerIndex_kernelFunctionA, "intBuffer", this.intComputeBuffer);

        //값 전달
        //computeShader.Set~을 통해 변수 설정
        this.computeShader.SetInt("intValue", 3);

        //컴퓨트 쉐이더 실행
        //실행할 커널의 인덱스, 그룹(3차원)
        this.computeShader.Dispatch(this.kernerIndex_kernelFunctionA, 1, 1, 1);

        int[] result= new int[10];
        //실행결과는  ComputeBuffer.GetData()를 이용해 가져온다.
        this.intComputeBuffer.GetData(result);
        for(int i = 0; i < 10; i++)
        {
            Debug.Log(result[i]);
        }

        this.computeShader.SetBuffer
           (this.kernerIndex_kernelFunctionB, "intBuffer", this.intComputeBuffer);
        this.computeShader.Dispatch(this.kernerIndex_kernelFunctionB, 1, 1, 1);

        this.intComputeBuffer.GetData(result);

        Debug.Log("RESULT : KernelFunction_B");

        for (int i = 0; i < 5; i++)
        {
            Debug.Log(result[i]);
        }
        //버퍼 해제
        this.intComputeBuffer.Release();
    }
}
