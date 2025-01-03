using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public class Point
    {
        public Vector2 position;
        public bool isStart;
        public int segmentIndex;

        public Point(Vector2 pos, bool start, int index)
        {
            position = pos;
            isStart = start;
            segmentIndex = index;
        }
    }

    private List<Vector2[]> segments;
    private List<Point> points;
    private SortedSet<int> activeSegments;
    private List<Vector2> intersections;

    public List<Vector2> FindIntersections(List<Vector2[]> lineSegments)
    {
        segments = lineSegments;
        points = new List<Point>();
        activeSegments = new SortedSet<int>();
        intersections = new List<Vector2>();

        // 모든 선분의 시작점과 끝점을 points 리스트에 추가
        for (int i = 0; i < segments.Count; i++)
        {
            Vector2 start = segments[i][0];
            Vector2 end = segments[i][1];

            if (start.x > end.x)
            {
                // 시작점이 항상 왼쪽에 오도록 정렬
                var temp = start;
                start = end;
                end = temp;
            }

            points.Add(new Point(start, true, i));
            points.Add(new Point(end, false, i));
        }

        // x좌표를 기준으로 정렬
        points.Sort((a, b) =>
            a.position.x != b.position.x ?
            a.position.x.CompareTo(b.position.x) :
            a.position.y.CompareTo(b.position.y));

        // sweep line 알고리즘 실행
        foreach (var point in points)
        {
            if (point.isStart)
            {
                // 현재 활성화된 모든 선분과 교차점 검사
                foreach (var activeIndex in activeSegments)
                {
                    CheckIntersection(point.segmentIndex, activeIndex);
                }
                activeSegments.Add(point.segmentIndex);
            }
            else
            {
                activeSegments.Remove(point.segmentIndex);
            }
        }

        return intersections;
    }

    private void CheckIntersection(int seg1, int seg2)
    {
        Vector2 p1 = segments[seg1][0];
        Vector2 p2 = segments[seg1][1];
        Vector2 p3 = segments[seg2][0];
        Vector2 p4 = segments[seg2][1];

        Vector2? intersection = LineIntersection(p1, p2, p3, p4);
        if (intersection.HasValue)
        {
            intersections.Add(intersection.Value);
        }
    }

    private Vector2? LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

        if (Mathf.Approximately(denominator, 0f))
            return null;

        float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
        float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

        if (ua < 0 || ua > 1 || ub < 0 || ub > 1)
            return null;

        return new Vector2(
            p1.x + ua * (p2.x - p1.x),
            p1.y + ua * (p2.y - p1.y)
        );
    }

    private void Start()
    {
        var segments = new List<Vector2[]>
    {
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 2) },
        new Vector2[] { new Vector2(0, 2), new Vector2(2, 0) }
    };

        List<Vector2> intersections = FindIntersections(segments);
        
        //겹치는 파이프 존재
        if(intersections.Count > 0)
        {
            //파이프 옮김
        }
        else
        {
            //파이프 그대로 설치
        }
    }
}
