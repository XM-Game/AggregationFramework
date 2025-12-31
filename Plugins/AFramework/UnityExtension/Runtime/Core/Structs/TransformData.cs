// ==========================================================
// 文件名：TransformData.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Transform 数据结构
    /// <para>存储 Transform 的位置、旋转、缩放数据</para>
    /// <para>可用于快照保存、插值计算、序列化等场景</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 从 Transform 创建快照
    /// var data = TransformData.FromTransform(transform);
    /// 
    /// // 应用到 Transform
    /// data.ApplyTo(transform);
    /// 
    /// // 插值
    /// var lerped = TransformData.Lerp(dataA, dataB, 0.5f);
    /// </code>
    /// </remarks>
    [Serializable]
    public struct TransformData : IEquatable<TransformData>
    {
        #region 字段

        /// <summary>位置</summary>
        public Vector3 Position;

        /// <summary>旋转 (四元数)</summary>
        public Quaternion Rotation;

        /// <summary>缩放</summary>
        public Vector3 Scale;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 TransformData
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="scale">缩放</param>
        public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// 创建 TransformData (仅位置和旋转)
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        public TransformData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = Vector3.one;
        }

        /// <summary>
        /// 创建 TransformData (仅位置)
        /// </summary>
        /// <param name="position">位置</param>
        public TransformData(Vector3 position)
        {
            Position = position;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
        }

        #endregion

        #region 静态属性

        /// <summary>默认 TransformData (原点，无旋转，单位缩放)</summary>
        public static TransformData Identity => new TransformData(Vector3.zero, Quaternion.identity, Vector3.one);

        #endregion

        #region 属性

        /// <summary>欧拉角旋转</summary>
        public Vector3 EulerAngles
        {
            get => Rotation.eulerAngles;
            set => Rotation = Quaternion.Euler(value);
        }

        /// <summary>前方向</summary>
        public Vector3 Forward => Rotation * Vector3.forward;

        /// <summary>右方向</summary>
        public Vector3 Right => Rotation * Vector3.right;

        /// <summary>上方向</summary>
        public Vector3 Up => Rotation * Vector3.up;

        /// <summary>变换矩阵</summary>
        public Matrix4x4 Matrix => Matrix4x4.TRS(Position, Rotation, Scale);

        #endregion

        #region 工厂方法

        /// <summary>
        /// 从 Transform 创建 (世界空间)
        /// </summary>
        /// <param name="transform">Transform 组件</param>
        /// <returns>TransformData</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TransformData FromTransform(Transform transform)
        {
            return new TransformData(
                transform.position,
                transform.rotation,
                transform.lossyScale
            );
        }

        /// <summary>
        /// 从 Transform 创建 (本地空间)
        /// </summary>
        /// <param name="transform">Transform 组件</param>
        /// <returns>TransformData</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TransformData FromTransformLocal(Transform transform)
        {
            return new TransformData(
                transform.localPosition,
                transform.localRotation,
                transform.localScale
            );
        }

        /// <summary>
        /// 从矩阵创建
        /// </summary>
        /// <param name="matrix">变换矩阵</param>
        /// <returns>TransformData</returns>
        public static TransformData FromMatrix(Matrix4x4 matrix)
        {
            return new TransformData(
                matrix.GetPosition(),
                matrix.rotation,
                matrix.lossyScale
            );
        }

        #endregion

        #region 应用方法

        /// <summary>
        /// 应用到 Transform (世界空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        public void ApplyTo(Transform transform)
        {
            transform.position = Position;
            transform.rotation = Rotation;
            transform.localScale = Scale;
        }

        /// <summary>
        /// 应用到 Transform (本地空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        public void ApplyToLocal(Transform transform)
        {
            transform.localPosition = Position;
            transform.localRotation = Rotation;
            transform.localScale = Scale;
        }

        /// <summary>
        /// 仅应用位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        public void ApplyPosition(Transform transform)
        {
            transform.position = Position;
        }

        /// <summary>
        /// 仅应用旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        public void ApplyRotation(Transform transform)
        {
            transform.rotation = Rotation;
        }

        /// <summary>
        /// 仅应用缩放
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        public void ApplyScale(Transform transform)
        {
            transform.localScale = Scale;
        }

        #endregion

        #region 插值方法

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="a">起始值</param>
        /// <param name="b">结束值</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>插值结果</returns>
        public static TransformData Lerp(TransformData a, TransformData b, float t)
        {
            return new TransformData(
                Vector3.Lerp(a.Position, b.Position, t),
                Quaternion.Slerp(a.Rotation, b.Rotation, t),
                Vector3.Lerp(a.Scale, b.Scale, t)
            );
        }

        /// <summary>
        /// 无限制线性插值
        /// </summary>
        /// <param name="a">起始值</param>
        /// <param name="b">结束值</param>
        /// <param name="t">插值参数</param>
        /// <returns>插值结果</returns>
        public static TransformData LerpUnclamped(TransformData a, TransformData b, float t)
        {
            return new TransformData(
                Vector3.LerpUnclamped(a.Position, b.Position, t),
                Quaternion.SlerpUnclamped(a.Rotation, b.Rotation, t),
                Vector3.LerpUnclamped(a.Scale, b.Scale, t)
            );
        }

        /// <summary>
        /// 向目标移动
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="maxDelta">最大变化量</param>
        /// <returns>移动后的值</returns>
        public static TransformData MoveTowards(TransformData current, TransformData target, float maxDelta)
        {
            return new TransformData(
                Vector3.MoveTowards(current.Position, target.Position, maxDelta),
                Quaternion.RotateTowards(current.Rotation, target.Rotation, maxDelta * Mathf.Rad2Deg),
                Vector3.MoveTowards(current.Scale, target.Scale, maxDelta)
            );
        }

        #endregion

        #region 变换方法

        /// <summary>
        /// 变换点 (从本地空间到世界空间)
        /// </summary>
        /// <param name="point">本地空间点</param>
        /// <returns>世界空间点</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 TransformPoint(Vector3 point)
        {
            return Position + Rotation * Vector3.Scale(point, Scale);
        }

        /// <summary>
        /// 逆变换点 (从世界空间到本地空间)
        /// </summary>
        /// <param name="point">世界空间点</param>
        /// <returns>本地空间点</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InverseTransformPoint(Vector3 point)
        {
            Vector3 localPoint = Quaternion.Inverse(Rotation) * (point - Position);
            return new Vector3(
                localPoint.x / Scale.x,
                localPoint.y / Scale.y,
                localPoint.z / Scale.z
            );
        }

        /// <summary>
        /// 变换方向
        /// </summary>
        /// <param name="direction">本地方向</param>
        /// <returns>世界方向</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 TransformDirection(Vector3 direction)
        {
            return Rotation * direction;
        }

        /// <summary>
        /// 逆变换方向
        /// </summary>
        /// <param name="direction">世界方向</param>
        /// <returns>本地方向</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InverseTransformDirection(Vector3 direction)
        {
            return Quaternion.Inverse(Rotation) * direction;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>
        /// 判断是否相等
        /// </summary>
        public bool Equals(TransformData other)
        {
            return Position.Equals(other.Position) &&
                   Rotation.Equals(other.Rotation) &&
                   Scale.Equals(other.Scale);
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is TransformData other && Equals(other);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Rotation, Scale);
        }

        #endregion

        #region 运算符重载

        /// <summary>相等运算符</summary>
        public static bool operator ==(TransformData left, TransformData right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(TransformData left, TransformData right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"TransformData(Pos:{Position}, Rot:{EulerAngles}, Scale:{Scale})";
        }

        #endregion
    }
}
