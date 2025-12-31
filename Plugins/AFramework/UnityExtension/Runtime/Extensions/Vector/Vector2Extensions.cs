// ==========================================================
// 文件名：Vector2Extensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Vector2 扩展方法
    /// <para>提供 Vector2 的数学运算和实用操作扩展</para>
    /// </summary>
    public static class Vector2Extensions
    {
        #region 分量操作

        /// <summary>
        /// 设置 X 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="x">新 X 值</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);

        /// <summary>
        /// 设置 Y 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="y">新 Y 值</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

        /// <summary>
        /// 增加 X 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="x">增加的 X 值</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AddX(this Vector2 v, float x) => new Vector2(v.x + x, v.y);

        /// <summary>
        /// 增加 Y 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="y">增加的 Y 值</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AddY(this Vector2 v, float y) => new Vector2(v.x, v.y + y);

        /// <summary>
        /// 乘以 X 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="x">乘数</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 MultiplyX(this Vector2 v, float x) => new Vector2(v.x * x, v.y);

        /// <summary>
        /// 乘以 Y 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="y">乘数</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 MultiplyY(this Vector2 v, float y) => new Vector2(v.x, v.y * y);

        /// <summary>
        /// 取反 X 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NegateX(this Vector2 v) => new Vector2(-v.x, v.y);

        /// <summary>
        /// 取反 Y 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NegateY(this Vector2 v) => new Vector2(v.x, -v.y);

        /// <summary>
        /// 交换 X 和 Y 分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>交换后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Swap(this Vector2 v) => new Vector2(v.y, v.x);

        #endregion

        #region 数学运算

        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>绝对值向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));

        /// <summary>
        /// 向下取整
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>取整后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Floor(this Vector2 v) => new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));

        /// <summary>
        /// 向上取整
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>取整后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Ceil(this Vector2 v) => new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>取整后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Round(this Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));

        /// <summary>
        /// 钳制向量分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>钳制后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(this Vector2 v, float min, float max)
        {
            return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        /// <summary>
        /// 钳制向量分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="min">最小向量</param>
        /// <param name="max">最大向量</param>
        /// <returns>钳制后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
        }

        /// <summary>
        /// 钳制向量长度
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>钳制后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampMagnitude(this Vector2 v, float maxLength)
        {
            return Vector2.ClampMagnitude(v, maxLength);
        }

        /// <summary>
        /// 获取最大分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>最大分量值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxComponent(this Vector2 v) => Mathf.Max(v.x, v.y);

        /// <summary>
        /// 获取最小分量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>最小分量值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinComponent(this Vector2 v) => Mathf.Min(v.x, v.y);

        /// <summary>
        /// 获取分量之和
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>分量之和</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector2 v) => v.x + v.y;

        /// <summary>
        /// 获取分量之积
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>分量之积</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Product(this Vector2 v) => v.x * v.y;

        /// <summary>
        /// 获取分量平均值
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>分量平均值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Average(this Vector2 v) => (v.x + v.y) * 0.5f;

        #endregion

        #region 方向和角度

        /// <summary>
        /// 获取向量角度 (弧度)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>角度 (弧度)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector2 v) => Mathf.Atan2(v.y, v.x);

        /// <summary>
        /// 获取向量角度 (度)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>角度 (度)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleDegrees(this Vector2 v) => Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

        /// <summary>
        /// 获取到另一个向量的角度 (度)
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <returns>角度 (度)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleTo(this Vector2 from, Vector2 to) => Vector2.Angle(from, to);

        /// <summary>
        /// 获取到另一个向量的有符号角度 (度)
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <returns>有符号角度 (度)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngleTo(this Vector2 from, Vector2 to)
        {
            return Vector2.SignedAngle(from, to);
        }

        /// <summary>
        /// 旋转向量 (度)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="degrees">旋转角度 (度)</param>
        /// <returns>旋转后的向量</returns>
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        /// <summary>
        /// 旋转向量 (弧度)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="radians">旋转角度 (弧度)</param>
        /// <returns>旋转后的向量</returns>
        public static Vector2 RotateRadians(this Vector2 v, float radians)
        {
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        /// <summary>
        /// 获取垂直向量 (顺时针 90 度)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>垂直向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PerpendicularCW(this Vector2 v) => new Vector2(v.y, -v.x);

        /// <summary>
        /// 获取垂直向量 (逆时针 90 度)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>垂直向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PerpendicularCCW(this Vector2 v) => new Vector2(-v.y, v.x);

        /// <summary>
        /// 从角度创建方向向量
        /// </summary>
        /// <param name="degrees">角度 (度)</param>
        /// <returns>方向向量</returns>
        public static Vector2 FromAngle(float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        #endregion

        #region 距离和插值

        /// <summary>
        /// 计算到另一个点的距离
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <returns>距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Vector2 from, Vector2 to) => Vector2.Distance(from, to);

        /// <summary>
        /// 计算到另一个点的平方距离
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <returns>平方距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistanceTo(this Vector2 from, Vector2 to) => (to - from).sqrMagnitude;

        /// <summary>
        /// 计算曼哈顿距离
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <returns>曼哈顿距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ManhattanDistanceTo(this Vector2 from, Vector2 to)
        {
            return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
        }

        /// <summary>
        /// 线性插值到目标
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>插值结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LerpTo(this Vector2 from, Vector2 to, float t) => Vector2.Lerp(from, to, t);

        /// <summary>
        /// 无限制线性插值到目标
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <param name="t">插值参数</param>
        /// <returns>插值结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LerpUnclampedTo(this Vector2 from, Vector2 to, float t)
        {
            return Vector2.LerpUnclamped(from, to, t);
        }

        /// <summary>
        /// 向目标移动
        /// </summary>
        /// <param name="current">当前位置</param>
        /// <param name="target">目标位置</param>
        /// <param name="maxDistanceDelta">最大移动距离</param>
        /// <returns>移动后的位置</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 MoveTowards(this Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            return Vector2.MoveTowards(current, target, maxDistanceDelta);
        }

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否为零向量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>如果是零向量返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector2 v) => v.sqrMagnitude < float.Epsilon;

        /// <summary>
        /// 检查是否近似为零向量
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果近似为零返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector2 v, float tolerance = 0.0001f)
        {
            return v.sqrMagnitude < tolerance * tolerance;
        }

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        /// <param name="a">向量 A</param>
        /// <param name="b">向量 B</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果近似相等返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Vector2 a, Vector2 b, float tolerance = 0.0001f)
        {
            return (a - b).sqrMagnitude < tolerance * tolerance;
        }

        /// <summary>
        /// 检查是否为有效向量 (非 NaN 和非 Infinity)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>如果有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this Vector2 v)
        {
            return !float.IsNaN(v.x) && !float.IsNaN(v.y) &&
                   !float.IsInfinity(v.x) && !float.IsInfinity(v.y);
        }

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 Vector3 (Z = 0)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>Vector3</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 v) => new Vector3(v.x, v.y, 0f);

        /// <summary>
        /// 转换为 Vector3 (指定 Z)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="z">Z 值</param>
        /// <returns>Vector3</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 v, float z) => new Vector3(v.x, v.y, z);

        /// <summary>
        /// 转换为 Vector3 (XZ 平面)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <param name="y">Y 值</param>
        /// <returns>Vector3</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3XZ(this Vector2 v, float y = 0f) => new Vector3(v.x, y, v.y);

        /// <summary>
        /// 转换为 Vector2Int
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>Vector2Int</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        /// <summary>
        /// 转换为 Vector2Int (向下取整)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>Vector2Int</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int FloorToInt(this Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        /// <summary>
        /// 转换为 Vector2Int (向上取整)
        /// </summary>
        /// <param name="v">原向量</param>
        /// <returns>Vector2Int</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int CeilToInt(this Vector2 v)
        {
            return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
        }

        #endregion
    }
}
