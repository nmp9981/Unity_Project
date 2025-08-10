//회전하는 캘리퍼스 (최종 수정됨)
double RotatingCalipers(vector<Point> convex_hull) {
    int nSize = convex_hull.size();

    if (nSize < 2) return 0.0;
    if (nSize == 2) return sqrt(dist_sq(convex_hull[0], convex_hull[1]));

    ll max_dist_sq = 0;
    int k = 1;

    for (int i = 0; i < nSize; ++i) {
        // k가 다음 변으로 이동할 때까지 k를 갱신
        // 외적 값이 같거나 작을 때도 k를 갱신해야 모든 대척점을 찾을 수 있음
        while (cross_product(convex_hull[i], convex_hull[(i + 1) % nSize], convex_hull[k]) <= cross_product(convex_hull[i], convex_hull[(i + 1) % nSize], convex_hull[(k + 1) % nSize])) {
            k = (k + 1) % nSize;
        }

        // i와 k 사이의 거리 갱신
        max_dist_sq = max(max_dist_sq, dist_sq(convex_hull[i], convex_hull[k]));
        max_dist_sq = max(max_dist_sq, dist_sq(convex_hull[(i + 1) % nSize], convex_hull[k]));
    }
    
    return sqrt(max_dist_sq);
}
