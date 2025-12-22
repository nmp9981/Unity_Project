using NUnit.Framework;
using System.Collections.Generic;
using TMPro;

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
}
