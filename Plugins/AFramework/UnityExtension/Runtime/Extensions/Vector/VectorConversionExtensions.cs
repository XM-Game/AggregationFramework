// ==========================================================
// 文件名：VectorConversionExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 向量转换扩展方法
    /// <para>提供各种向量类型之间的转换操作</para>
    /// </summary>
    public static class VectorConversionExtensions
    {
        #region Color 转换

        /// <summary>
        /// Color 转换为 Vector3 (RGB)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Color c) => new Vector3(c.r, c.g, c.b);

        /// <summary>
        /// Color 转换为 Vector4 (RGBA)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);

        /// <summary>
        /// Vector3 转换为 Color (RGB, A=1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToColor(this Vector3 v) => new Color(v.x, v.y, v.z, 1f);

        /// <summary>
        /// Vector3 转换为 Color (RGB, 指定 A)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToColor(this Vector3 v, float alpha) => new Color(v.x, v.y, v.z, alpha);

        #endregion

        #region Rect 转换

        /// <summary>
        /// Vector2 转换为 Rect (作为位置，尺寸为零)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Vector2 v) => new Rect(v.x, v.y, 0f, 0f);

        /// <summary>
        /// Vector2 转换为 Rect (作为位置，指定尺寸)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Vector2 v, Vector2 size) => new Rect(v.x, v.y, size.x, size.y);

        /// <summary>
        /// Vector4 转换为 Rect (x,y=位置, z,w=尺寸)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Vector4 v) => new Rect(v.x, v.y, v.z, v.w);

        /// <summary>
        /// Rect 转换为 Vector4 (x,y=位置, z,w=尺寸)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Rect r) => new Vector4(r.x, r.y, r.width, r.height);

        /// <summary>
        /// Rect 位置转换为 Vector2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPositionVector(this Rect r) => new Vector2(r.x, r.y);

        /// <summary>
        /// Rect 尺寸转换为 Vector2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSizeVector(this Rect r) => new Vector2(r.width, r.height);

        #endregion

        #region Bounds 转换

        /// <summary>
        /// Vector3 转换为 Bounds (作为中心，尺寸为零)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds ToBounds(this Vector3 v) => new Bounds(v, Vector3.zero);

        /// <summary>
        /// Vector3 转换为 Bounds (作为中心，指定尺寸)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds ToBounds(this Vector3 v, Vector3 size) => new Bounds(v, size);

        /// <summary>
        /// Bounds 中心转换为 Vector3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetCenterVector(this Bounds b) => b.center;

        /// <summary>
        /// Bounds 尺寸转换为 Vector3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetSizeVector(this Bounds b) => b.size;

        #endregion

        #region BoundsInt 转换

        /// <summary>
        /// Vector3Int 转换为 BoundsInt (作为位置，尺寸为零)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsInt ToBoundsInt(this Vector3Int v) => new BoundsInt(v, Vector3Int.zero);

        /// <summary>
        /// Vector3Int 转换为 BoundsInt (作为位置，指定尺寸)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsInt ToBoundsInt(this Vector3Int v, Vector3Int size) => new BoundsInt(v, size);

        #endregion

        #region RectInt 转换

        /// <summary>
        /// Vector2Int 转换为 RectInt (作为位置，尺寸为零)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt ToRectInt(this Vector2Int v) => new RectInt(v.x, v.y, 0, 0);

        /// <summary>
        /// Vector2Int 转换为 RectInt (作为位置，指定尺寸)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt ToRectInt(this Vector2Int v, Vector2Int size) => new RectInt(v.x, v.y, size.x, size.y);

        #endregion

        #region 数组转换

        /// <summary>
        /// Vector2 转换为 float 数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ToArray(this Vector2 v) => new[] { v.x, v.y };

        /// <summary>
        /// Vector3 转换为 float 数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ToArray(this Vector3 v) => new[] { v.x, v.y, v.z };

        /// <summary>
        /// Vector4 转换为 float 数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float[] ToArray(this Vector4 v) => new[] { v.x, v.y, v.z, v.w };

        /// <summary>
        /// Vector2Int 转换为 int 数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ToArray(this Vector2Int v) => new[] { v.x, v.y };

        /// <summary>
        /// Vector3Int 转换为 int 数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ToArray(this Vector3Int v) => new[] { v.x, v.y, v.z };

        #endregion

        #region 元组转换

        /// <summary>
        /// Vector2 转换为元组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float x, float y) ToTuple(this Vector2 v) => (v.x, v.y);

        /// <summary>
        /// Vector3 转换为元组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float x, float y, float z) ToTuple(this Vector3 v) => (v.x, v.y, v.z);

        /// <summary>
        /// Vector4 转换为元组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float x, float y, float z, float w) ToTuple(this Vector4 v) => (v.x, v.y, v.z, v.w);

        /// <summary>
        /// Vector2Int 转换为元组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int x, int y) ToTuple(this Vector2Int v) => (v.x, v.y);

        /// <summary>
        /// Vector3Int 转换为元组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int x, int y, int z) ToTuple(this Vector3Int v) => (v.x, v.y, v.z);

        /// <summary>
        /// 元组转换为 Vector2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this (float x, float y) t) => new Vector2(t.x, t.y);

        /// <summary>
        /// 元组转换为 Vector3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this (float x, float y, float z) t) => new Vector3(t.x, t.y, t.z);

        /// <summary>
        /// 元组转换为 Vector2Int
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2Int(this (int x, int y) t) => new Vector2Int(t.x, t.y);

        /// <summary>
        /// 元组转换为 Vector3Int
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this (int x, int y, int z) t) => new Vector3Int(t.x, t.y, t.z);

        #endregion
    }
}
