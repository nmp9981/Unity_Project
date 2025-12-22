using System.Collections.Generic;

public static class IslandBuilder
{
    public static List<Island> Build(List<CustomRigidBody> rigidBodies, List<ContactManifold> manifolds)
    {
        var islands = new List<Island>();//중복 방지

        //방문 체크
        var visited = new HashSet<CustomRigidBody>();//물체 단위, 물체가 노드

        foreach(var rigidBody in rigidBodies)
        {
            //정적 물체이거나 이미 방문
            if (rigidBody.isStatic || visited.Contains(rigidBody)) continue;

            //현재 Island
            var island = new Island();

            //DFS
            BuildIslandDFS(
                rigidBody,
                island,
                manifolds,
                visited
            );

            islands.Add(island);
        }
        return islands;
    }

    /// <summary>
    /// 물체 그룹을 묶는 함수(물체와 연결된 그래프 그리기)
    /// </summary>
    /// <param name="start"></param>
    /// <param name="island"></param>
    /// <param name="manifolds"></param>
    /// <param name="visited"></param>
    public static void BuildIslandDFS(
                CustomRigidBody start,
                Island island,
                List<ContactManifold> manifolds,
                HashSet<CustomRigidBody> visited
            )
    {
        var stack = new Stack<CustomRigidBody>();
        stack.Push(start);
        visited.Add(start);

        while (stack.Count > 0)
        {
            var body = stack.Pop();
            island.bodies.Add(body);

            foreach(var manifold in manifolds)
            {
                //이미 방문 함
                if(!ManifoldContains(manifold, body)) continue;

                //중복 방지를 위해 manifold는 island에 추가
                if (island.manifolds.Contains(manifold))
                {
                    island.bodies.Add(body);
                }

                //붙어있는 다른 물체
                var other = GetOtherBody(manifold, body);

                //static body, 정적 물체는 island의 멤버가 아님
                if (other==null || other.isStatic)
                {
                    continue;
                }

                //미방문
                if (!visited.Contains(other))
                {
                    //방문
                    visited.Add(other);
                    stack.Push(other);
                }
            }
        }
    }

    /// <summary>
    /// 이미 방문 했는가?
    /// </summary>
    /// <param name="m"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    static bool ManifoldContains(ContactManifold m, CustomRigidBody body)
    {
        return m.rigidA == body || m.rigidB == body;
    }
    /// <summary>
    /// 다른 물체 반환
    /// </summary>
    /// <param name="m"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    static CustomRigidBody GetOtherBody(ContactManifold m, CustomRigidBody body)
    {
        if (m.rigidA == body) return m.rigidB;
        if (m.rigidB == body) return m.rigidA;
        return null;
    }
}
