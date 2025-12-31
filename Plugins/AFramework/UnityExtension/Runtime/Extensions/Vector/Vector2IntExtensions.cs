// ==========================================================
// 文件名：Vector2IntExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Vector2Int 扩展方法
    /// <para>提供 Vector2Int 的数学运算和实用操作扩展</para>
    /// </summary>
    public static class Vector2IntExtensions
    {
        #region 分量操作

        /// <summary>设置 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int WithX(this Vector2Int v, int x) => new Vector2Int(x, v.y);

        /// <summary>设置 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int WithY(this Vector2Int v, int y) => new Vector2Int(v.x, y);

        /// <summary>增加 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int AddX(this Vector2Int v, int x) => new Vector2Int(v.x + x, v.y);

        /// <summary>增加 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int AddY(this Vector2Int v, int y) => new Vector2Int(v.x, v.y + y);

        /// <summary>取反 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int NegateX(this Vector2Int v) => new Vector2Int(-v.x, v.y);

        /// <summary>取反 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int NegateY(this Vector2Int v) => new Vector2Int(v.x, -v.y);

        /// <summary>交换 X 和 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Swap(this Vector2Int v) => new Vector2Int(v.y, v.x);

        #endregion

        #region 数学运算

        /// <summary>取绝对值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Abs(this Vector2Int v) => new Vector2Int(Mathf.Abs(v.x), Mathf.Abs(v.y));

        /// <summary>钳制向量分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Clamp(this Vector2Int v, int min, int max)
        {
            return new Vector2Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        /// <summary>钳制向量分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Clamp(this Vector2Int v, Vector2Int min, Vector2Int max)
        {
            return new Vector2Int(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
        }

        /// <summary>获取最大分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxComponent(this Vector2Int v) => Mathf.Max(v.x, v.y);

        /// <summary>获取最小分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinComponent(this Vector2Int v) => Mathf.Min(v.x, v.y);

        /// <summary>获取分量之和</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum(this Vector2Int v) => v.x + v.y;

        /// <summary>获取分量之积</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Product(this Vector2Int v) => v.x * v.y;

        /// <summary>分量相乘</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Multiply(this Vector2Int a, Vector2Int b) => a * b;

        #endregion

        #region 距离

        /// <summary>计算到另一个点的距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Vector2Int from, Vector2Int to) => Vector2Int.Distance(from, to);

        /// <summary>计算曼哈顿距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistanceTo(this Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
        }

        /// <summary>计算切比雪夫距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChebyshevDistanceTo(this Vector2Int from, Vector2Int to)
        {
            return Mathf.Max(Mathf.Abs(to.x - from.x), Mathf.Abs(to.y - from.y));
        }

        #endregion

        #region 检查

        /// <summary>检查是否为零向量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector2Int v) => v.x == 0 && v.y == 0;

        /// <summary>检查是否为正向量 (所有分量 > 0)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this Vector2Int v) => v.x > 0 && v.y > 0;

        /// <summary>检查是否为非负向量 (所有分量 >= 0)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNonNegative(this Vector2Int v) => v.x >= 0 && v.y >= 0;

        /// <summary>检查是否在范围内</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this Vector2Int v, Vector2Int min, Vector2Int max)
        {
            return v.x >= min.x && v.x <= max.x && v.y >= min.y && v.y <= max.y;
        }

        #endregion

        #region 转换

        /// <summary>转换为 Vector2</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector2Int v) => new Vector2(v.x, v.y);

        /// <summary>转换为 Vector3Int (Z = 0)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this Vector2Int v) => new Vector3Int(v.x, v.y, 0);

        /// <summary>转换为 Vector3Int (指定 Z)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this Vector2Int v, int z) => new Vector3Int(v.x, v.y, z);

        /// <summary>转换为 Vector3 (Z = 0)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2Int v) => new Vector3(v.x, v.y, 0f);

        /// <summary>转换为 Vector3 (XZ 平面)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3XZ(this Vector2Int v, float y = 0f) => new Vector3(v.x, y, v.y);

        #endregion

        #region 方向

        /// <summary>获取四方向邻居</summary>
        public static Vector2Int[] GetNeighbors4(this Vector2Int v)
        {
            return new[]
            {
                new Vector2Int(v.x + 1, v.y),
                new Vector2Int(v.x - 1, v.y),
                new Vector2Int(v.x, v.y + 1),
                new Vector2Int(v.x, v.y - 1)
            };
        }

        /// <summary>获取八方向邻居</summary>
        public static Vector2Int[] GetNeighbors8(this Vector2Int v)
        {
            return new[]
            {
                new Vector2Int(v.x + 1, v.y),
                new Vector2Int(v.x - 1, v.y),
                new Vector2Int(v.x, v.y + 1),
                new Vector2Int(v.x, v.y - 1),
                new Vector2Int(v.x + 1, v.y + 1),
                new Vector2Int(v.x + 1, v.y - 1),
                new Vector2Int(v.x - 1, v.y + 1),
                new Vector2Int(v.x - 1, v.y - 1)
            };
        }

        #endregion
    }
}
