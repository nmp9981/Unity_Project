// 2차원 좌표를 나타내는 구조체
struct Point {
    int x, y;
};

//CCW : 점C가 벡터AB에 대해 어느 방향에 위치하는지
int CCW(Point A, Point B, Point C){
    int crossValue = (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x);
    
    //결과
    if(crossValue > 0) return 1;//반시계
    else if(crossValue == 0) return 0;//일직선
    else return -1;//시계
}
