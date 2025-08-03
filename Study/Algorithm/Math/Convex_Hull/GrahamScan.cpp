//좌표 구조체
struct Point{
    int x,y;
};

Point pivot;//기준점

// CCW 함수: 세 점의 방향을 판별
// -1: 반시계 방향 (좌회전)
// 1: 시계 방향 (우회전)
// 0: 일직선
int ccw(Point p1, Point p2, Point p3) {
    int val = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
    if (val == 0) return 0;
    return (val > 0) ? -1 : 1;
}

// 점들의 정렬 기준: 반시계 방향 각도
bool comparePoints(Point p1, Point p2) {
    int orientation = ccw(pivot, p1, p2);
    if (orientation == 0) {//일직선
        // 각도가 같으면 거리가 가까운 점이 먼저 오도록 정렬
        int p1Dist = (p1.x - pivot.x) * (p1.x - pivot.x) + (p1.y - pivot.y) * (p1.y - pivot.y);
        int p2Dist = (p2.x - pivot.x) * (p2.x - pivot.x) + (p2.y - pivot.y) * (p2.y - pivot.y);
        return p1Dist < p2Dist;
    }
    return orientation < 0;//반시계 방향이 우선
}

// 그라함 스캔 알고리즘
vector<Point> GrahamScan(vector<Point> points) {
    int n_size = points.size();
    if (n_size < 3) return points;//컨벡스헐 구성 불가

    // 1단계: 가장 낮은 y좌표를 가진 점을 찾기 (pivot 설정)
    //y가 같으면 x좌표가 더 작은것으로 설정
    int minIndex = 0;
    for (int idx=1;idx<n;idx++) {
        if (points[idx].y < points[minIndex].y ||
            (points[idx].y == points[minIndex].y && points[idx].x < points[minIndex].x)) {
            minIndex = idx;
        }
    }
    swap(points[0], points[minIndex]);
    pivot = points[0];


    // 2단계: 기준점을 중심으로 점들을 정렬
    sort(points.begin() + 1, points.end(), comparePoints);


    // 3단계: 정렬 후 일직선상에 있는 중복 점들 제거 (가장 먼 점만 남김)
    int finalNum = 1;//최종 점들 번호(0은 시작점 이므로 1번 부터 시작)
    for (int idx = 1; idx < n_size; idx++) {
        //일직선이 아닐때까지 계속
        while (idx < n_size - 1 && ccw(pivot, points[idx], points[idx + 1]) == 0) {
            idx++;
        }
        points[finalNum] = points[idx];//일직선상의 맨 끝점
        finalNum++;//다음 번호
    }
    points.resize(finalNum);//정렬
    n_size = finalNum;//최종 사이즈(최종 점 개수)
    //최종 점의 개수가 3개 미만이면 컨벡스 헐을 만들 수 없으므로 빈 벡터 반환
    if (n_size < 3) return {};


    // 4단계: 스택을 사용하여 볼록 껍질 구성
    stack<Point> hullStack;
    hullStack.push(points[0]);
    hullStack.push(points[1]);
   
    for (int idx = 2; idx < n_size; idx++) {
        while (hullStack.size() >= 2) {//맨위 2개를 뺀다.
            Point p2 = hullStack.top();//맨위
            hullStack.pop();
            Point p1 = hullStack.top();//2번째 위
            
            // p1, p2, points[idx]의 방향을 확인합니다.
            if (ccw(p1, p2, points[idx]) < 0) { // 반시계 방향일 경우
                hullStack.push(p2); // p2를 다시 스택에 넣고
                break; // while 루프를 종료합니다.
            }
        }
        hullStack.push(points[idx]);
    }
    
    // 스택의 결과를 벡터로 변환
    vector<Point> convexHull;
    while (!hullStack.empty()) {
        convexHull.push_back(hullStack.top());
        hullStack.pop();
    }
    reverse(convexHull.begin(), convexHull.end());

    return convexHull;
}
