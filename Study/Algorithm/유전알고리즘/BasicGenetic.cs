using UnityEngine;

public class BasicGenetic : MonoBehaviour
{
    //정답 비밀 번호
    int[] password = {4,8,8,5,8,5 };

    //1) generate : 임의의 정수 6자리를 갖는 염색체 10개를 생성하는 함수
    int[] generate(int N)
    {
        int[] parent = { 0,0,0,0,0,0};
        for(int i = 0; i < N; i++)
        {
            parent[i] = Random.Range(0, 10);
        }
        return parent;
    }


    //2) fitness : 염색체의 6자리 숫자와 password 가 일치하는지 여부를 점수로 나타냅니다. 전체 일치는 6점
    int fitness(int[] chromosome)
    {
        int score = 0;
        for(int idx = 0; idx < password.Length;idx++){
            if (password[idx] == chromosome[idx])
            {
                score += 1;
            }
        }
        return score;
    }


    //3) mutate : 돌연변이를 생성
    int[] mutate(int[] chromosome)
    {
        int index = Random.Range(0, chromosome.Length);

        int newgene = Random.Range(0, 10);
        int alternate = Random.Range(0, 10);
       
        if (chromosome[index] != newgene)
            chromosome[index] = newgene;
        else
            chromosome[index] = alternate;

        return chromosome;
    }

    private void Start()
    {
        //아무거나 하나 생성하고 점수를 매김
        int[] parent = generate(6);
        int parent_fitness = fitness(parent);
        int i = 0;


        while (true)
        {
            int[] child = mutate(parent);
            int child_fitness = fitness(child);
            i = i + 1;

            if (child_fitness <= parent_fitness)
            {
                continue;
            }
            Debug.Log("child chrosome : "+child+ "| child_fitness :"+ child_fitness+"| iter:"+ i);

            if(child_fitness >= password.Length)
            {
                break;
            }
            parent = child;
            parent_fitness = child_fitness;
        }
    }
}
