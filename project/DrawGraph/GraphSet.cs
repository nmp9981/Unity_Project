using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphSet : MonoBehaviour
{
    public InputField functionFormal;
    [SerializeField] string function;//함수
    [SerializeField] List<string> elemental;//다항식 항
    
    public GameObject objectFulling;
    public float y;//함수 결과값
    
    //항 쪼개기
    public void GraphSetting()
    {
        function = functionFormal.GetComponent<InputField>().text;
        Debug.Log("y="+function);
        int startIndex = 0;
        int endIndex = 0;
        for(int idx = 0; idx < function.Length; idx++)
        {
            if (function[idx] == '+') {
                endIndex = idx - 1;//포함
                elemental.Add(function.Substring(startIndex,endIndex-startIndex+1));
                startIndex = idx + 1;
            }else if (function[idx] == '-')
            {
                if (idx == 0) continue;//최고차항 계수가 음수

                endIndex = idx - 1;//포함
                elemental.Add(function.Substring(startIndex, endIndex - startIndex + 1));
                startIndex = idx;//-도 포함
            }
        }
        elemental.Add(function.Substring(startIndex));
    }
    //값으로 변환(여기서 다양한 함수들이 나온다.)
    float ChangeValue(string monomial, float x)
    {
        float result = 1.0f;//곱셈의 항등원
        int numDigit = 0;//숫자부분 길이
        //Debug.Log(monomial);
        switch (monomial[0])
        {
            case '-':
                monomial = monomial.Substring(1);//맨앞 - 제거

                for(int idx = 0; idx < monomial.Length; idx++)
                {
                    if (monomial[idx] == 'x') result *= x;
                    else numDigit++;
                }
                if(numDigit>0) result *= float.Parse(monomial.Substring(0,numDigit));

                result *= (-1);
                break;
            default:
                for (int idx = 0; idx < monomial.Length; idx++)
                {
                    if (monomial[idx] == 'x') result *= x;
                    else numDigit++;
                }
                if (numDigit > 0) result *= float.Parse(monomial.Substring(0, numDigit));

                break;
        }
        return result;
    }
    public float F(float x)
    {
        float y = 0.0f;
        for(int idx = 0; idx <elemental.Count; idx++)
        {
            y += ChangeValue(elemental[idx],x);
        }
        return y;
    }
    //그래프 그리기
    public void DrawGraph()
    {
        objectFulling.GetComponent<ObjectFulling>().ActiveObject();
    }
}
