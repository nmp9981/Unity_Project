using UnityEngine;

public class Temp
{
    [Range(0, 255)] public int rule = 90;   // 90=시에르핀스키, 30=카오스, 110=복잡
    public int width = 401;                  // 가로 셀 수 (홀수면 가운데가 딱 떨어짐)
    public int height = 200;                 // 세로 = 시간 스텝 수

    public Color aliveColor = Color.white;
    public Color deadColor = Color.black;

    void Start()
    {
        // 1) 규칙 번호 → 8비트 규칙표
        //    ruleTable[i] = 입력 패턴 i(0~7)에 대한 출력(0/1)
        int[] ruleTable = new int[8];
        for (int i = 0; i < 8; i++)
            ruleTable[i] = (rule >> i) & 1;   // rule의 i번째 비트

        // 2) 첫 줄: 가운데 한 칸만 1
        int[] current = new int[width];
        current[width / 2] = 1;

        // 3) 텍스처 준비
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;    // 셀이 또렷하게 (블러 X)

        // 4) 한 줄씩 계산하며 텍스처에 그리기
        for (int y = 0; y < height; y++)
        {
            // 현재 줄을 텍스처에 그림 (위에서 아래로 쌓이게 y 뒤집기)
            for (int x = 0; x < width; x++)
                tex.SetPixel(x, height - 1 - y, current[x] == 1 ? aliveColor : deadColor);

            // 다음 줄 계산
            int[] next = new int[width];
            for (int x = 0; x < width; x++)
            {
                // 토러스 경계: 양 끝이 이어짐
                int left = current[(x - 1 + width) % width];
                int self = current[x];
                int right = current[(x + 1) % width];

                // (왼,자신,오) → 0~7 인덱스. 왼쪽이 최상위 비트.
                int pattern = (left << 2) | (self << 1) | right;
                next[x] = ruleTable[pattern];
            }
            current = next;
        }

        tex.Apply();
    }
}
