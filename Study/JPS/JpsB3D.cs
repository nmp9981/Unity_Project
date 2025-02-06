using System.Collections.Generic;
using System;

namespace Metacle.Core.Common.Algorithm
{
    public class JpsB3D
    {
        private int _mapX;
        private int _mapY;
        private int _mapZ;

        private readonly HashSet<(int, int, int)> _obstacles = new();

        private readonly (int, int, int)[] _directions =
        {
            (-1, 0, 0), (1, 0, 0),
            (0, -1, 0), (0, 1, 0),
            (0, 0, -1), (0, 0, 1)
        };

        public JpsB3D(int mapX, int mapY, int mapZ)
        {
            _mapX = mapX;
            _mapY = mapY;
            _mapZ = mapZ;
        }

        public bool IsWalkable(int x, int y, int z) => !_obstacles.Contains((x, y, z));
        public bool IsInBounds(int x, int y, int z) => x >= 0 && y >= 0 && z >= 0 && x < _mapX && y < _mapY && z < _mapZ;

        private (int, int, int)? Jump(int x, int y, int z, int dx, int dy, int dz, (int, int, int) goal)
        {
            if (!IsInBounds(x, y, z) || !IsWalkable(x, y, z))
            {
                return null;
            }

            if ((x, y, z) == goal)
            {
                return (x, y, z);
            }

            foreach (var (fx, fy, fz) in _directions)
            {
                if ((fx, fy, fz) == (dx, dy, dz))
                {
                    continue;
                }

                int nx = x + fx, ny = y + fy, nz = z + fz;
                int px = x - fx, py = y - fy, pz = z - fz;

                if (IsInBounds(nx, ny, nz) && IsWalkable(nx, ny, nz) && (!IsInBounds(px, py, pz) || !IsWalkable(px, py, pz)))
                {
                    return (x, y, z);
                }
            }

            return Jump(x + dx, y + dy, z + dz, dx, dy, dz, goal);
        }

        public Dictionary<(int, int, int), List<(int, int, int)>> FindPaths(List<(int, int, int)> starts, (int, int, int) goal)
        {
            var paths = new Dictionary<(int, int, int), List<(int, int, int)>>();

            foreach (var start in starts)
            {
                var fullPath = new List<(int, int, int)>();

                if (start.Item3 != 0)
                {
                    var toGroundPath = FindPath(start, (start.Item1, start.Item2, 0));
                    if (toGroundPath.Count == 0)
                    {
                        continue; // 내려올 수 없는 경우 건너뜀
                    }

                    fullPath.AddRange(toGroundPath);
                }

                var mainPath = FindPath((start.Item1, start.Item2, 0), goal);
                if (mainPath.Count == 0)
                {
                    continue; // 목표까지 갈 수 없는 경우 건너뜀
                }

                fullPath.AddRange(mainPath);

                paths[start] = fullPath;
            }

            return paths;
        }


        private List<(int, int, int)> FindPath((int, int, int) start, (int, int, int) goal)
        {
            var openSet = new PriorityQueue<(int, int, int), int>();
            var cameFrom = new Dictionary<(int, int, int), (int, int, int)>();
            var gScore = new Dictionary<(int, int, int), int>();
            var fScore = new Dictionary<(int, int, int), int>();

            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);
            openSet.Enqueue(start, fScore[start]);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();
                if (current == goal) return ReconstructPath(cameFrom, current);

                foreach (var (dx, dy, dz) in _directions)
                {
                    var jumpPoint = Jump(current.Item1 + dx, current.Item2 + dy, current.Item3 + dz, dx, dy, dz, goal);
                    if (jumpPoint == null) continue;

                    var neighbor = jumpPoint.Value;
                    int tentativeGScore = gScore[current] + 1;

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor, goal);
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            return new List<(int, int, int)>();
        }

        private List<(int, int, int)> ReconstructPath(Dictionary<(int, int, int), (int, int, int)> cameFrom, (int, int, int) current)
        {
            var path = new List<(int, int, int)> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }

        private int Heuristic((int, int, int) a, (int, int, int) b)
        {
            return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2) + Math.Abs(a.Item3 - b.Item3);
        }

        public void AddObstacle(int x, int y, int z) => _obstacles.Add((x, y, z));
        public void RemoveObstacle(int x, int y, int z) => _obstacles.Remove((x, y, z));

        public List<(int, int, int)> CompressPath(List<(int, int, int)> rawPath)
        {
            List<(int, int, int)> result = new();
            int previousX = 0;
            int previousY = 0;
            int previousZ = 0;

            for (int i = 0; i < rawPath.Count; i++)
            {
                int currentX = rawPath[i].Item1;
                int currentY = rawPath[i].Item2;
                int currentZ = rawPath[i].Item3;
                if (i == 0)
                {
                    result.Add(rawPath[i]);

                    previousX = currentX;
                    previousY = currentY;
                    previousZ = currentZ;
                    continue;
                }

                bool isOneDifferent = (previousX != currentX ? 1 : 0) + (previousY != currentY ? 1 : 0) + (previousZ != currentZ ? 1 : 0) == 1;

                if (isOneDifferent)
                {
                    continue;
                }

                previousX = currentX;
                previousY = currentY;
                previousZ = currentZ;
                result.Add(rawPath[i]);
            }

            return result;
        }
    }
}
