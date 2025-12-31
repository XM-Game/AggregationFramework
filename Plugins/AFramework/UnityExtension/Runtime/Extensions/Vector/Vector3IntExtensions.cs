// ==========================================================
// 文件名：Vector3IntExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Vector3Int 扩展方法
    /// <para>提供 Vector3Int 的数学运算和实用操作扩展</para>
    /// </summary>
    public static class Vector3IntExtensions
    {
        #region 分量操作

        /// <summary>设置 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int WithX(this Vector3Int v, int x) => new Vector3Int(x, v.y, v.z);

        /// <summary>设置 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int WithY(this Vector3Int v, int y) => new Vector3Int(v.x, y, v.z);

        /// <summary>设置 Z 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int WithZ(this Vector3Int v, int z) => new Vector3Int(v.x, v.y, z);

        /// <summary>增加 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int AddX(this Vector3Int v, int x) => new Vector3Int(v.x + x, v.y, v.z);

        /// <summary>增加 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int AddY(this Vector3Int v, int y) => new Vector3Int(v.x, v.y + y, v.z);

        /// <summary>增加 Z 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int AddZ(this Vector3Int v, int z) => new Vector3Int(v.x, v.y, v.z + z);

        /// <summary>取反 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int NegateX(this Vector3Int v) => new Vector3Int(-v.x, v.y, v.z);

        /// <summary>取反 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int NegateY(this Vector3Int v) => new Vector3Int(v.x, -v.y, v.z);

        /// <summary>取反 Z 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int NegateZ(this Vector3Int v) => new Vector3Int(v.x, v.y, -v.z);

        /// <summary>将 Y 分量设为 0</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Flat(this Vector3Int v) => new Vector3Int(v.x, 0, v.z);

        #endregion

        #region 数学运算

        /// <summary>取绝对值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Abs(this Vector3Int v) => new Vector3Int(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        /// <summary>钳制向量分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Clamp(this Vector3Int v, int min, int max)
        {
            return new Vector3Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }

        /// <summary>钳制向量分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Clamp(this Vector3Int v, Vector3Int min, Vector3Int max)
        {
            return new Vector3Int(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z));
        }

        /// <summary>获取最大分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxComponent(this Vector3Int v) => Mathf.Max(v.x, Mathf.Max(v.y, v.z));

        /// <summary>获取最小分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinComponent(this Vector3Int v) => Mathf.Min(v.x, Mathf.Min(v.y, v.z));

        /// <summary>获取分量之和</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum(this Vector3Int v) => v.x + v.y + v.z;

        /// <summary>获取分量之积</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Product(this Vector3Int v) => v.x * v.y * v.z;

        /// <summary>分量相乘</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Multiply(this Vector3Int a, Vector3Int b) => a * b;

        #endregion

        #region 距离

        /// <summary>计算到另一个点的距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Vector3Int from, Vector3Int to) => Vector3Int.Distance(from, to);

        /// <summary>计算曼哈顿距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistanceTo(this Vector3Int from, Vector3Int to)
        {
            return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y) + Mathf.Abs(to.z - from.z);
        }

        /// <summary>计算切比雪夫距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChebyshevDistanceTo(this Vector3Int from, Vector3Int to)
        {
            return Mathf.Max(Mathf.Abs(to.x - from.x), Mathf.Max(Mathf.Abs(to.y - from.y), Mathf.Abs(to.z - from.z)));
        }

        #endregion

        #region 检查

        /// <summary>检查是否为零向量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector3Int v) => v.x == 0 && v.y == 0 && v.z == 0;

        /// <summary>检查是否为正向量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this Vector3Int v) => v.x > 0 && v.y > 0 && v.z > 0;

        /// <summary>检查是否为非负向量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNonNegative(this Vector3Int v) => v.x >= 0 && v.y >= 0 && v.z >= 0;

        /// <summary>检查是否在范围内</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this Vector3Int v, Vector3Int min, Vector3Int max)
        {
            return v.x >= min.x && v.x <= max.x && v.y >= min.y && v.y <= max.y && v.z >= min.z && v.z <= max.z;
        }

        #endregion

        #region 转换

        /// <summary>转换为 Vector3</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector3Int v) => new Vector3(v.x, v.y, v.z);

        /// <summary>转换为 Vector2Int (XY)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2Int(this Vector3Int v) => new Vector2Int(v.x, v.y);

        /// <summary>转换为 Vector2Int (XZ)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2IntXZ(this Vector3Int v) => new Vector2Int(v.x, v.z);

        /// <summary>转换为 Vector2 (XY)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector3Int v) => new Vector2(v.x, v.y);

        #endregion

        #region 方向

        /// <summary>获取六方向邻居</summary>
        public static Vector3Int[] GetNeighbors6(this Vector3Int v)
        {
            return new[]
            {
                new Vector3Int(v.x + 1, v.y, v.z),
                new Vector3Int(v.x - 1, v.y, v.z),
                new Vector3Int(v.x, v.y + 1, v.z),
                new Vector3Int(v.x, v.y - 1, v.z),
                new Vector3Int(v.x, v.y, v.z + 1),
                new Vector3Int(v.x, v.y, v.z - 1)
            };
        }

        #endregion
    }
}
