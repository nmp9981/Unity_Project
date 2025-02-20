using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCompute_Texture : MonoBehaviour
{
    //컴퓨트 쉐이더 참조
    public ComputeShader computeShader;
    //실행하는 커널의 인덱스
    int kernerIndex_kernelFunctionC;

    RenderTexture renderTextureC;

    struct ThreadSize
    {
        public int x;
        public int y;
        public int z;

        public ThreadSize(uint x, uint y, uint z)
        {
            this.x = (int)x;
            this.y = (int)y;
            this.z = (int)z;
        }
    }

    ThreadSize kernelThreadSize_KernelFunctionC;

    void Start()
    {
        //실행할 커널의 인덱스 획득
        this.kernerIndex_kernelFunctionC = this.computeShader.FindKernel("KernelFunctionC");//0

        //512*512 텍스처 해상도 지정, 색은 RGBA
        this.renderTextureC = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        //텍스처 쓰기 활성화
        this.renderTextureC.enableRandomWrite = true;
        this.renderTextureC.Create();

        //커널의 인덱스를 인수로 이용해 스레드 크기 가져옴
        uint threadSizeX, threadSizeY, threadSizeZ;

        //커널 인덱스, 커널의 크기
        this.computeShader.GetKernelThreadGroupSizes(this.kernerIndex_kernelFunctionC
            , out threadSizeX,out threadSizeY, out threadSizeZ);

        //스레드 크기
        this.kernelThreadSize_KernelFunctionC
            = new ThreadSize(threadSizeX, threadSizeY, threadSizeZ);
    }

    void Update()
    {
        //커널 실행
        //실행할 커널의 인덱스, 그룹(3차원)
        this.computeShader.Dispatch(this.kernerIndex_kernelFunctionC,
            this.renderTextureC.width/this.kernelThreadSize_KernelFunctionC.x,
            this.renderTextureC.height / this.kernelThreadSize_KernelFunctionC.y,
            this.kernelThreadSize_KernelFunctionC.z);

        //텍스처 입히기
        this.gameObject.GetComponent<Renderer>().material.mainTexture = this.renderTextureC;
    }
}
