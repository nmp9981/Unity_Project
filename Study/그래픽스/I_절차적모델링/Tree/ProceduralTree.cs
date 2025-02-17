using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProceduralModeling
{
    public class ProceduralTree : MonoBehaviour
    {
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;


        //트리 데이터
        public TreeData Data { get { return data; } }

        [SerializeField] TreeData data;
        //세대 수
        [SerializeField, Range(2, 7)] protected int generations = 5;

        //길이
        [SerializeField, Range(0.5f, 5f)] protected float length = 1f;

        //반지름
        [SerializeField, Range(0.1f, 2f)] protected float radius = 0.15f;

        const float PI2 = Mathf.PI * 2f;

        [SerializeField]
        GameObject pointObj;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {   
            MakeTree();
        }

        /// <summary>
        /// 나무 제작
        /// </summary>
        void MakeTree()
        {
            //근원 (뿌리)
            var root = new TreeBranch(
                generations,
                length,
                radius,
                data
            );

            Mesh mesh = new Mesh();
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();

            //나무의 총 길이를 계산
            //가지의 길이를 총 길이로 나누어 uv 좌표의 높이(u,v)가
            //뿌리부터 가지 끝까지 [0.0 ~ 1.0] 범위로 변화 할 수 있도록 설정한다 
            float maxLength = TraverseMaxLength(root);
            int num = 0;
            //재귀적으로 모든 가지를 접근하여 각각의 가지에 대응하는 메쉬를 생성한다 
            Traverse(root, (branch) =>
            {
                //현재까지의 정점 개수
                //각 가지마다 삼각형 번호 부여시 필요
                var offset = vertices.Count;

                var vOffset = branch.Offset / maxLength;//순서
                var vLength = branch.Length / maxLength;//길이 비율
                //가지 하나의 정점데이터를 생성
                //각 가지는 Tubular의 집합
                for (int i = 0, n = branch.Segments.Count; i < n; i++)//몇번 실린더
                {
                    var t = 1f * i / (n - 1);
                    var v = vOffset + vLength * t;

                    //법선벡터, 종법선벡터 구하기
                    var segment = branch.Segments[i];
                    var N = segment.Frame.Normal;
                    var B = segment.Frame.Binormal;
                    for (int j = 0; j <= data.radialSegments; j++)//N각형
                    {
                        // 0.0 ~ 2PI
                        var u = 1f * j / data.radialSegments;
                        float rad = u * PI2;
                        float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
                        //방향벡터 구하기
                        var normal = (cos * N + sin * B).normalized;
                        //정점 위치 = 현위치+r*N
                        vertices.Add(segment.Position + segment.Radius * normal);

                        //노말, 탄젠트, uv 좌표 추가
                        normals.Add(normal);
                        var tangent = segment.Frame.Tangent;
                        tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
                        uvs.Add(new Vector2(u, v));
                    }
                }
                //가지 모델의 삼각형들을 만든다
                for (int j = 1; j <= data.heightSegments; j++)//몇번 실린더
                {
                    for (int i = 1; i <= data.radialSegments; i++)//n각형
                    {
                        int a = (data.radialSegments + 1) * (j - 1) + (i - 1);
                        int b = (data.radialSegments + 1) * j + (i - 1);
                        int c = (data.radialSegments + 1) * j + i;
                        int d = (data.radialSegments + 1) * (j - 1) + i;

                        a += offset;
                        b += offset;
                        c += offset;
                        d += offset;

                        triangles.Add(a); triangles.Add(d); triangles.Add(b);
                        triangles.Add(b); triangles.Add(d); triangles.Add(c);
                    }
                }
            });

            //메시 인스턴스 설정
            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.tangents = tangents.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = triangles.ToArray();
            //만들어진 메시의 경계영역 계산(culling에 필요)
            mesh.RecalculateBounds();
            //삼각면과 정점으로부터 메쉬 노멀 재계산
            mesh.RecalculateNormals();
            //메시필터로 만든 메시 그리기
            meshFilter.mesh = mesh;
            
        }
        
        /// <summary>
        /// 나뭇가지 총 길이
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        static float TraverseMaxLength(TreeBranch branch)
        {
            float max = 0f;
            branch.Children.ForEach(c => {
                max = Mathf.Max(max, TraverseMaxLength(c));
            });
            return branch.Length + max;//현재 길이 + 나뭇가지
        }

        //경로따라 이동
        static void Traverse(TreeBranch from, Action<TreeBranch> action)
        {
            if (from.Children.Count > 0)
            {
                from.Children.ForEach(child => {
                    Traverse(child, action);
                });
            }
            action(from);
        }
    }
    public class TreeSegment
    {
        // TreeSegment가 향하는 방향 벡터 tangent,
        //이와 직교하는 normal, binormal 벡터를 가진 FrenetFrame
        FrenetFrame frame;
        // TreeSegment의 위치
        Vector3 position;
        // TreeSegment의 폭(반경)
        float radius;

        public FrenetFrame Frame { get { return frame; } }
        public Vector3 Position { get { return position; } }
        public float Radius { get { return radius; } }

        //생성자
        public TreeSegment(FrenetFrame frame, Vector3 position, float radius)
        {
            this.frame = frame;
            this.position = position;
            this.radius = radius;
        }
    }

    [System.Serializable]
    public class TreeData
    {
        public int randomSeed = 0;
        [Range(0.5f, 0.99f)] public float lengthAttenuation = 0.9f, radiusAttenuation = 0.6f;
        [Range(1, 3)] public int branchesMin = 1, branchesMax = 3;
        [Range(-45f, 0f)] public float growthAngleMin = -15f;
        [Range(0f, 45f)] public float growthAngleMax = 15f;
        [Range(1f, 10f)] public float growthAngleScale = 4f;
        [Range(4, 20)] public int heightSegments = 10, radialSegments = 8;
        [Range(0.0f, 0.35f)] public float bendDegree = 0.1f;

        // UnityEngine.Randomではなく、このRandクラスから生成された乱数を用いることで、
        // 同じTreeDataのパラメータであれば同じ形の木が生成できるようにしており、
        // 欲しい分岐パターンの木を生成しやすくしている。
        Rand rnd;

        public void Setup()
        {
            rnd = new Rand(randomSeed);
        }

        public int Range(int a, int b)
        {
            return UnityEngine.Random.Range(a, b);
        }

        public float Range(float a, float b)
        {
            return UnityEngine.Random.Range(a, b);
        }

        public int GetRandomBranches()
        {
            return UnityEngine.Random.Range(branchesMin, branchesMax + 1);
        }

        public float GetRandomGrowthAngle()
        {
            return UnityEngine.Random.Range(growthAngleMin, growthAngleMax);
        }

        public float GetRandomBendDegree()
        {
            return UnityEngine.Random.Range(-bendDegree, bendDegree);
        }
    }

    // 一つの枝を表現するクラス
    public class TreeBranch
    {
        public int Generation { get { return generation; } }
        public List<TreeSegment> Segments { get { return segments; } }
        public List<TreeBranch> Children { get { return children; } }

        public Vector3 From { get { return from; } }
        public Vector3 To { get { return to; } }
        public float Length { get { return length; } }
        public float Offset { get { return offset; } }

        // 自身の世代（0が枝先）
        int generation;

        // モデルを生成する際に必要な、枝を分割する節
        List<TreeSegment> segments;

        // 自身から分岐したTreeBranch
        List<TreeBranch> children;

        // 一本の枝の根元と先端の位置
        Vector3 from, to;

        // 根元の太さと先端の太さ
        float fromRadius, toRadius;

        // 枝の長さ
        float length;

        // 根元から自身の根元に至るまでの長さ
        float offset;

        // 根元のコンストラクタ
        public TreeBranch(int generations, float length, float radius, TreeData data) : this(generations, generations, Vector3.zero, Vector3.up, Vector3.right, Vector3.back, length, radius, 0f, data)
        {
        }

        protected TreeBranch(int generation, int generations, Vector3 from, Vector3 tangent, Vector3 normal, Vector3 binormal, float length, float radius, float offset, TreeData data)
        {
            this.generation = generation;

            this.fromRadius = radius;

            // 枝先である場合は先端の太さが0になる
            this.toRadius = (generation == 0) ? 0f : radius * data.radiusAttenuation;

            this.from = from;

            // 枝先ほど分岐する角度が大きくなる
            var scale = Mathf.Lerp(1f, data.growthAngleScale, 1f - 1f * generation / generations);

            // normal方向の回転
            var qn = Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), normal);

            // binormal方向の回転
            var qb = Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), binormal);

            // 枝先が向いているtangent方向にqn * qbの回転をかけつつ、枝先の位置を決める
            this.to = from + (qn * qb) * tangent * length;

            this.length = length;
            this.offset = offset;

            // モデル生成に必要な節を構築
            segments = BuildSegments(data, fromRadius, toRadius, normal, binormal);

            children = new List<TreeBranch>();
            if (generation > 0)
            {
                // 分岐する数を取得
                int count = data.GetRandomBranches();
                for (int i = 0; i < count; i++)
                {
                    float ratio; // [0.0 ~ 1.0]
                    if (count == 1)
                    {
                        // 分岐数が1である場合（0除算を回避）
                        ratio = 1f;
                    }
                    else
                    {
                        ratio = Mathf.Lerp(0.5f, 1f, (1f * i) / (count - 1));
                    }

                    // 分岐元の節を取得
                    var index = Mathf.FloorToInt(ratio * (segments.Count - 1));
                    var segment = segments[index];

                    // 分岐元の節が持つベクトルをTreeBranchに渡すことで滑らかな分岐を得る
                    Vector3 nt = segment.Frame.Tangent;
                    Vector3 nn = segment.Frame.Normal;
                    Vector3 nb = segment.Frame.Binormal;

                    var child = new TreeBranch(
                        this.generation - 1,
                        generations,
                        segment.Position,
                        nt,
                        nn,
                        nb,
                        length * Mathf.Lerp(1f, data.lengthAttenuation, ratio),
                        radius * Mathf.Lerp(1f, data.radiusAttenuation, ratio),
                        offset + length,
                        data
                    );

                    children.Add(child);
                }
            }
        }

        List<TreeSegment> BuildSegments(TreeData data, float fromRadius, float toRadius, Vector3 normal, Vector3 binormal)
        {
            var segments = new List<TreeSegment>();

            var curve = new CatmullRomCurve();
            curve.Points.Clear();

            var length = (to - from).magnitude;
            var bend = length * (normal * data.GetRandomBendDegree() + binormal * data.GetRandomBendDegree());
            curve.Points.Add(from);
            curve.Points.Add(Vector3.Lerp(from, to, 0.25f) + bend);
            curve.Points.Add(Vector3.Lerp(from, to, 0.75f) + bend);
            curve.Points.Add(to);

            var frames = curve.ComputeFrenetFrames(data.heightSegments, normal, binormal, false);
            for (int i = 0, n = frames.Count; i < n; i++)
            {
                var u = 1f * i / (n - 1);
                var radius = Mathf.Lerp(fromRadius, toRadius, u);

                var position = curve.GetPointAt(u);
                var segment = new TreeSegment(frames[i], position, radius);
                segments.Add(segment);
            }
            return segments;
        }

    }
    public class Rand
    {
        System.Random rnd;

        public float value
        {
            get
            {
                return (float)rnd.NextDouble();
            }
        }

        public Rand(int seed)
        {
            rnd = new System.Random(seed);
        }

        public int Range(int a, int b)
        {
            var v = value;
            return Mathf.FloorToInt(Mathf.Lerp(a, b, v));
        }

        public float Range(float a, float b)
        {
            var v = value;
            return Mathf.Lerp(a, b, v);
        }
    }
}
