using Metacle.Core.DataStructrue;
using UnityEngine;

namespace CollisionMathFunction
{
    public static class CollisionFunction
    {
        /// <summary>
        /// AABB 물리적 간섭
        /// </summary>
        /// <param name="a">bound A</param>
        /// <param name="b">bound B</param>
        /// <returns></returns>
        static bool AABBToAABBInCollision(Bounds a, Bounds b)
        {
            WorldVector3 aMin = a.min;
            WorldVector3 aMax = a.max;
            WorldVector3 bMin = b.min;
            WorldVector3 bMax = b.max;
            
            bool isXCollide = false;
            bool isYCollide = false;
            bool isZCollide = false;

            if (a.max.x > b.min.x && a.min.x < b.max.x)
            {
                isXCollide = true;
            }
            if (a.max.y > b.min.y && a.min.y < b.max.y)
            {
                isYCollide = true;
            }
            if (a.max.z > b.min.z && a.min.z < b.max.z)
            {
                isZCollide = true;
            }

            //충돌
            if (isXCollide && isYCollide && isZCollide)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// AABB 논리적 간섭
        /// </summary>
        /// <param name="a">bound A</param>
        /// <param name="b">bound B</param>
        /// <param name="size">영역</param>
        /// <returns></returns>
        static bool AABBToAABBInInterference(Bounds a, Bounds b, float size)
        {
            a.size *= size;
            b.size *= size;

            WorldVector3 aMin = a.min;
            WorldVector3 aMax = a.max;
            WorldVector3 bMin = b.min;
            WorldVector3 bMax = b.max;

            bool isXCollide = false;
            bool isYCollide = false;
            bool isZCollide = false;

            if (a.max.x > b.min.x && a.min.x < b.max.x)
            {
                isXCollide = true;
            }
            if (a.max.y > b.min.y && a.min.y < b.max.y)
            {
                isYCollide = true;
            }
            if (a.max.z > b.min.z && a.min.z < b.max.z)
            {
                isZCollide = true;
            }

            //충돌
            if (isXCollide && isYCollide && isZCollide)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 구형 오브젝트 간섭 판정
        /// </summary>
        /// <param name="center1">구1 중심</param>
        /// <param name="radius1">구1 반지름</param>
        /// <param name="center2">구2 중심</param>
        /// <param name="radius2">구2 반지름</param>
        /// <param name="interferenceSize">간섭 크기</param>
        /// <returns></returns>
        static bool SphereToSphere(WorldVector3 center1,float radius1,WorldVector3 center2, float radius2, float interferenceSize)
        {
            float dist = (center1 - center2).Magnitude;
            if(dist<=(radius1 + radius2)*interferenceSize)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 구와 AABB충돌 판정
        /// </summary>
        /// <param name="radius">구의 반지름</param>
        /// <param name="sphereCenter">구의 중심</param>
        /// <param name="box">AABB 박스</param>
        /// <param name="interferenceSize">간섭 크기</param>
        /// <returns></returns>
        static bool SphereToAABB(float radius, WorldVector3 sphereCenter, Bounds box, float interferenceSize)
        {
            //중심간의 거리
            float dist = (sphereCenter - (WorldVector3)box.center).Magnitude;

            //간섭 설정
            box.size *= interferenceSize;
            radius *= interferenceSize;

            //방향 벡터
            WorldVector3 dirVector = sphereCenter - (WorldVector3)box.center;

            //직선의 방정식
            float t = 0;
            float crossX = dirVector.X * t + sphereCenter.X;
            float crossY = dirVector.Y * t + sphereCenter.Y;
            float crossZ = dirVector.Z * t + sphereCenter.Z;

            //XY평면 - 1
            float crossZ1 = box.center.z+box.size.z*0.5f;
            float t1 = (crossZ1 - sphereCenter.Z) / dirVector.Z;
            float crossX1 = dirVector.X * t1 + sphereCenter.X;
            float crossY1 = dirVector.Y * t1 + sphereCenter.Y;
            WorldVector3 cross1 = new WorldVector3(crossX1,crossY1,crossZ1);
            //이 교차점이 구의 방정식 내부에 존재하는가?
            if((cross1.X- sphereCenter.X) * (cross1.X- sphereCenter.X)
                + (cross1.Y - sphereCenter.Y) * (cross1.Y - sphereCenter.Y)
                + (cross1.Z - sphereCenter.Z) * (cross1.Z - sphereCenter.Z) <= radius * radius)
            {
                return true;
            }

            //XY평면 - 2
            float crossZ2 = box.center.z - box.size.z * 0.5f;
            float t2 = (crossZ2 - sphereCenter.Z) / dirVector.Z;
            float crossX2 = dirVector.X * t2 + sphereCenter.X;
            float crossY2 = dirVector.Y * t2 + sphereCenter.Y;
            WorldVector3 cross2 = new WorldVector3(crossX2, crossY2, crossZ2);
            //이 교차점이 구의 방정식 내부에 존재하는가?
            if ((cross2.X - sphereCenter.X) * (cross2.X - sphereCenter.X)
                + (cross2.Y - sphereCenter.Y) * (cross2.Y - sphereCenter.Y)
                + (cross2.Z - sphereCenter.Z) * (cross2.Z - sphereCenter.Z) <= radius * radius)
            {
                return true;
            }

            //YZ평면 - 1
            float crossX3 = box.center.x + box.size.x * 0.5f;
            float t3 = (crossX3 - sphereCenter.X) / dirVector.X;
            float crossZ3 = dirVector.Z * t3 + sphereCenter.Z;
            float crossY3 = dirVector.Y * t3 + sphereCenter.Y;
            WorldVector3 cross3 = new WorldVector3(crossX3, crossY3, crossZ3);
            //이 교차점이 구의 방정식 내부에 존재하는가?
            if ((cross3.X - sphereCenter.X) * (cross3.X - sphereCenter.X)
                + (cross3.Y - sphereCenter.Y) * (cross3.Y - sphereCenter.Y)
                + (cross3.Z - sphereCenter.Z) * (cross3.Z - sphereCenter.Z) <= radius * radius)
            {
                return true;
            }

            //YZ평면 - 2
            float crossX4 = box.center.x - box.size.x * 0.5f;
            float t4 = (crossX3 - sphereCenter.X) / dirVector.X;
            float crossZ4 = dirVector.Z * t4 + sphereCenter.Z;
            float crossY4 = dirVector.Y * t4 + sphereCenter.Y;
            WorldVector3 cross4 = new WorldVector3(crossX4, crossY4, crossZ4);
            //이 교차점이 구의 방정식 내부에 존재하는가?
            if ((cross4.X - sphereCenter.X) * (cross4.X - sphereCenter.X)
                + (cross4.Y - sphereCenter.Y) * (cross4.Y - sphereCenter.Y)
                + (cross4.Z - sphereCenter.Z) * (cross4.Z - sphereCenter.Z) <= radius * radius)
            {
                return true;
            }

            //XZ평면 - 1
            float crossY5 = box.center.y + box.size.y * 0.5f;
            float t5 = (crossY5 - sphereCenter.Y) / dirVector.Y;
            float crossZ5 = dirVector.Z * t5 + sphereCenter.Z;
            float crossX5 = dirVector.X * t5 + sphereCenter.X;
            WorldVector3 cross5 = new WorldVector3(crossX5, crossY5, crossZ5);
            //이 교차점이 구의 방정식 내부에 존재하는가?
            if ((cross5.X - sphereCenter.X) * (cross5.X - sphereCenter.X)
                + (cross5.Y - sphereCenter.Y) * (cross5.Y - sphereCenter.Y)
                + (cross5.Z - sphereCenter.Z) * (cross5.Z - sphereCenter.Z) <= radius * radius)
            {
                return true;
            }

            //XZ평면 - 2
            float crossY6 = box.center.y - box.size.y * 0.5f;
            float t6 = (crossY6 - sphereCenter.Y) / dirVector.Y;
            float crossZ6 = dirVector.Z * t6 + sphereCenter.Z;
            float crossX6 = dirVector.X * t6 + sphereCenter.X;
            WorldVector3 cross6 = new WorldVector3(crossX6, crossY6, crossZ6);
            //이 교차점이 구의 방정식 내부에 존재하는가?
            if ((cross6.X - sphereCenter.X) * (cross6.X - sphereCenter.X)
                + (cross6.Y - sphereCenter.Y) * (cross6.Y - sphereCenter.Y)
                + (cross6.Z - sphereCenter.Z) * (cross6.Z - sphereCenter.Z) <= radius * radius)
            {
                return true;
            }

            return false;
        }
    }
}
