using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FactorizationIntoPrimes : MonoBehaviour
{
    ulong[] judgePrimeList = new ulong[15];

    List<ulong> factors = new List<ulong>();
    private void Awake()
    {
        Under50Prime();
    }

    /// <summary>
    /// 기능 :50 이하 소수 등록
    /// </summary>
    void Under50Prime()
    {
        judgePrimeList[0] = 2;
        judgePrimeList[1] = 3;
        judgePrimeList[2] = 5;
        judgePrimeList[3] = 7;
        judgePrimeList[4] = 11;
        judgePrimeList[5] = 13;
        judgePrimeList[6] = 17;
        judgePrimeList[7] = 19;
        judgePrimeList[8] = 23;
        judgePrimeList[9] = 29;
        judgePrimeList[10] = 31;
        judgePrimeList[11] = 37;
        judgePrimeList[12] = 41;
        judgePrimeList[13] = 43;
        judgePrimeList[14] = 47;
    }

    /// <summary>
    ///기능 : 오버플로우 없이 (p * q) % mod를 구하기 위한 함수
    /// </summary>
    /// <param name="p"></param>
    /// <param name="q"></param>
    /// <param name="mod"></param>
    /// <returns></returns>
    ulong mult(ulong p, ulong q, ulong mod)
    {
        p %= mod;//일단 나머지 연산을 한다.
        q %= mod;

        ulong r = 0;
        ulong w = p;

        while (q>0)
        {//q의 비트를 보면서, 덧셈을 통해 곱을 만든다.
            if (q % 2==1)
            {
                r = (r + w) % mod;//0번째 비트가 1인 경우 결과에 더해주고
            }
            w = (2 * w) % mod;//다음으로 넣을 수도 있을 값을 계산해준다.
            q >>= 1;//다음 비트를 볼 것
        }

        return (ulong)r;
    }

    /// <summary>
    /// 기능 ; 두 수의 최대공약수 구하기
    /// </summary>
    ulong GCD(ulong x, ulong y)
    {
        if (x < y)
        {
            ulong temp = x;
            x = y;
            y = temp;
        }
        while (y != 0)
        {
            ulong r = x % y;
            x = y;
            y = r;
        }
        return x;
    }

    /// <summary>
    /// 기능 : a^m % p를 구하는 함수. 동작 방식은 위의 mult()와 동일하다.
    /// </summary>
    ulong pow_mod(ulong a, ulong m, ulong p)
    {
        ulong ret = 1;
        a %= p;

        while (m>0)
        {
            if (m % 2==1) ret = mult(ret, a, p);
            a = mult(a, a, p);
            m >>= 1;
        }

        return ret;
    }

    /// <summary>
    /// 기능 : 밀러-라빈 소수 판정
    /// 조건 : 2이상의 수
    /// </summary>
    /// <param name="n">판정할 수</param>
    public bool MillerLavinJudgePrime(ulong N, ulong a)
    {
        ulong k = N - 1;//k = 2^h * m으로 나타낼 수 있다. 이때 h >= 1, m은 홀수다.

        while (true)
        {//a^{2^h * m}에서부터 거꾸로 진행한다.
            ulong d = pow_mod(a, k, N);//a^k가 통과하나?
            if (k % 2==1) return (d == 1 || d == N - 1);//만약 k가 홀수라면, a^m을 확인하고 있다는 말이다. 이 경우 a^m === 1 (mod n)인지를 확인한다.
            if (d == N - 1) return true;//k가 짝수인 경우로, a^k === -1 (mod n)인 경우다.
            k >>= 1;//2로 나눠보자
        }
    }
    /// <summary>
    /// 기능 : 소수 판별
    /// </summary>
    public bool IsPrime(ulong N)
    {
        for (int i = 0; i < judgePrimeList.Length; i++)
        {
            if (N == judgePrimeList[i]) return true;//소수만 들어있는 배열 a의 원소와 같은 경우, 소수다.
            if (N % judgePrimeList[i] == 0) return false;//소수의 배수이므로 합성수다.
            if (!MillerLavinJudgePrime(N, judgePrimeList[i])) return false;//밀러-라빈 검사를 통과하지 못한 경우 합성수다.
        }
        return true;
    }
    /// <summary>
    /// 기능 : 폴라-로 알고리즘을 이용한 소인수 분해
    /// </summary>
    /// <param name="N"></param>
    ulong PollarRhoFunction(ulong N)
    {
        //소수
        if (IsPrime(N))
        {
            return N;
        }
        //짝수
        if (N % 2 == 0)
        {
            return 2;
        }

        ulong x = (ulong)Random.Range(2, N);
        ulong y = x;
        ulong c = (ulong)Random.Range(1, N);
        ulong d = 1;

        while (d == 1)
        {
            x = (((x * x) % N) + c + N) % N;
            y = (((y * y) % N) + c + N) % N;
            y = (((y * y) % N) + c + N) % N;
            d = GCD((ulong)Mathf.Abs(x - y), N);

            if (d == N)
            {
                return PollarRhoFunction(N);
            }
        }

        if (IsPrime(d))
        {
            return d;
        }
        return PollarRhoFunction(d);
    }
    /// <summary>
    /// 기능 : 소인수분해 결과
    /// 본 클래스의 메인 코드 -> 여기서 소인수 분해 결과가 나온다.
    /// </summary>
    public List<ulong> FactorizationPrimes(ulong N)
    {
        factors.Clear();
        while (N > 1)
        {
            ulong divisor = PollarRhoFunction(N);//소인수 구하기
            factors.Add(divisor);
            N = N / divisor;
        }
        factors.Sort();//정렬
        return factors;
    }
}
