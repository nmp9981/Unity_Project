#include <iostream>
#include <vector>
#include <stack>
#include <algorithm>

using namespace std;
typedef long long ll;

//좌표 구조체
struct Point {
    ll x, y;
};

int n;//입력 점 개수
vector<Point> pos;//점들 배열

// CCW 함수: 세 점의 방향을 판별
// -1: 반시계 방향 (좌회전)
// 1: 시계 방향 (우회전)
// 0: 일직선
ll ccw(Point p1, Point p2, Point p3) {
    ll val = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
    if (val == 0) return 0;
    return (val > 0) ? -1 : 1;
}

// 두 점 사이의 거리 제곱
long long dist_sq(Point a, Point b) {
    return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
}

// 주어진 점 집합에서 볼록 껍질을 계산하는 함수
vector<Point> JarvisMarch(vector<Point> points) {
    int n_size = points.size();
    if (n < 3) return {}; // 볼록 껍질을 만들 수 없음

    // 1단계: y좌표가 가장 작은 점을 찾아서 시작점으로 설정
    int startIdx = 0;//y좌표가 가장 작은 점의 인덱스
    for (int i = 1; i < n_size; i++) {
        //y좌표가 같은 경우 x좌표가 더 작은 점으로
        if (points[i].y < points[startIdx].y || (points[i].y == points[startIdx].y && points[i].x < points[startIdx].x)) {
            startIdx = i;
        }
    }

    //볼록 껍질 제작
    vector<Point> hull;//볼록 껍질 점들
    int curPoint = startIdx;

    do {
        // 2단계: 현재 점을 볼록 껍질에 추가
        hull.push_back(points[curPoint]);

        // 3단계: 다음 점을 찾기
        int nextPoint = (curPoint + 1) % n;
        // 모든 점들을 순회하며 가장 반시계 방향에 있는 점을 찾음
        for (int i = 0; i < n_size; ++i) {
            int direction = ccw(points[curPoint], points[nextPoint], points[i]);

            // `points[i]`가 `nextPoint`보다 반시계 방향일 경우 갱신
            if (direction == -1) {
                nextPoint = i;
            }
            // 세 점이 일직선일 경우, 더 먼 점을 다음 점으로 선택
            else if (direction == 0) {
                if (dist_sq(points[curPoint], points[i]) > dist_sq(points[curPoint], points[nextPoint])) {
                    nextPoint = i;
                }
            }
        }
        curPoint = nextPoint;
    } while (curPoint != startIdx); // 다음 점이 시작점과 같아질 때까지 반복

    return hull;
}

//입력
void Input() {
    ios::sync_with_stdio(0);
    cin.tie(0);

    cin >> n;
    for (int i = 0; i < n; i++) {
        ll x, y;
        cin >> x >> y;
        pos.push_back({ x,y });
    }
}

//출력
void Output(vector<Point> convexHull) {
    int convexSize = convexHull.size();
    
    cout <<"볼록 다각형 크기 : " << convexSize << "\n";
    cout << "좌표 목록\n";
    for (int i = 0; i < convexSize; i++) {
        cout << convexHull[i].x << " " << convexHull[i].y << "\n";
    }
}

int main()
{
    Input();//입력
    vector<Point> convexHull = JarvisMarch(pos);//컨벡스 헐
    
