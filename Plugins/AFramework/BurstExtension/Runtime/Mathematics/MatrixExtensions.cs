// ==========================================================
// 文件名：MatrixExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：float4x4扩展方法，提供便捷的矩阵操作
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// float4x4扩展方法类
    /// 提供便捷的矩阵操作和常用变换
    /// </summary>
    [BurstCompile]
    public static class MatrixExtensions
    {
        #region 常量

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static readonly float4x4 Identity = float4x4.identity;

        /// <summary>
        /// 零矩阵
        /// </summary>
        public static readonly float4x4 Zero = float4x4.zero;

        #endregion

        #region 基础操作

        /// <summary>
        /// 计算矩阵的转置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Transposed(this float4x4 m)
        {
            return math.transpose(m);
        }

        /// <summary>
        /// 计算矩阵的逆
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Inverse(this float4x4 m)
        {
            return math.inverse(m);
        }

        /// <summary>
        /// 计算矩阵的行列式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Determinant(this float4x4 m)
        {
            return math.determinant(m);
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Multiply(this float4x4 a, float4x4 b)
        {
            return math.mul(a, b);
        }

        #endregion

        #region 分解操作

        /// <summary>
        /// 获取平移分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetTranslation(this float4x4 m)
        {
            return m.c3.xyz;
        }

        /// <summary>
        /// 获取缩放分量（假设无剪切）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetScale(this float4x4 m)
        {
            return new float3(
                math.length(m.c0.xyz),
                math.length(m.c1.xyz),
                math.length(m.c2.xyz));
        }

        /// <summary>
        /// 获取旋转分量（假设正交矩阵）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion GetRotation(this float4x4 m)
        {
            float3 scale = GetScale(m);
            float3x3 rotMatrix = new float3x3(
                m.c0.xyz / scale.x,
                m.c1.xyz / scale.y,
                m.c2.xyz / scale.z);
            return new quaternion(rotMatrix);
        }

        /// <summary>
        /// 分解TRS矩阵
        /// </summary>
        /// <param name="m">变换矩阵</param>
        /// <param name="translation">输出平移</param>
        /// <param name="rotation">输出旋转</param>
        /// <param name="scale">输出缩放</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decompose(this float4x4 m, out float3 translation, out quaternion rotation, out float3 scale)
        {
            translation = m.c3.xyz;
            scale = new float3(
                math.length(m.c0.xyz),
                math.length(m.c1.xyz),
                math.length(m.c2.xyz));
            
            float3x3 rotMatrix = new float3x3(
                m.c0.xyz / scale.x,
                m.c1.xyz / scale.y,
                m.c2.xyz / scale.z);
            rotation = new quaternion(rotMatrix);
        }

        /// <summary>
        /// 获取3x3旋转缩放矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 GetRotationScale(this float4x4 m)
        {
            return new float3x3(m.c0.xyz, m.c1.xyz, m.c2.xyz);
        }

        #endregion

        #region 变换点和向量

        /// <summary>
        /// 变换点（应用完整变换，包括平移）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 TransformPoint(this float4x4 m, float3 point)
        {
            return math.transform(m, point);
        }

        /// <summary>
        /// 变换方向（不应用平移）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 TransformDirection(this float4x4 m, float3 direction)
        {
            return math.rotate(m, direction);
        }

        /// <summary>
        /// 变换向量（应用缩放和旋转，不应用平移）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 TransformVector(this float4x4 m, float3 vector)
        {
            return math.mul(new float3x3(m.c0.xyz, m.c1.xyz, m.c2.xyz), vector);
        }

        /// <summary>
        /// 逆变换点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 InverseTransformPoint(this float4x4 m, float3 point)
        {
            return math.transform(math.inverse(m), point);
        }

        /// <summary>
        /// 逆变换方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 InverseTransformDirection(this float4x4 m, float3 direction)
        {
            return math.rotate(math.inverse(m), direction);
        }

        /// <summary>
        /// 变换float4（完整矩阵乘法）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Transform(this float4x4 m, float4 v)
        {
            return math.mul(m, v);
        }

        #endregion

        #region 设置操作

        /// <summary>
        /// 设置平移分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 WithTranslation(this float4x4 m, float3 translation)
        {
            m.c3 = new float4(translation, 1f);
            return m;
        }

        /// <summary>
        /// 设置缩放分量（保持旋转）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 WithScale(this float4x4 m, float3 scale)
        {
            float3 currentScale = GetScale(m);
            float3 scaleRatio = scale / currentScale;
            m.c0 *= new float4(scaleRatio.x, scaleRatio.x, scaleRatio.x, 1f);
            m.c1 *= new float4(scaleRatio.y, scaleRatio.y, scaleRatio.y, 1f);
            m.c2 *= new float4(scaleRatio.z, scaleRatio.z, scaleRatio.z, 1f);
            return m;
        }

        #endregion

        #region 创建变换矩阵

        /// <summary>
        /// 创建平移矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Translate(float3 translation)
        {
            return float4x4.Translate(translation);
        }

        /// <summary>
        /// 创建旋转矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Rotate(quaternion rotation)
        {
            return new float4x4(rotation, float3.zero);
        }

        /// <summary>
        /// 创建缩放矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Scale(float3 scale)
        {
            return float4x4.Scale(scale);
        }

        /// <summary>
        /// 创建统一缩放矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Scale(float scale)
        {
            return float4x4.Scale(scale);
        }

        /// <summary>
        /// 创建TRS变换矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 TRS(float3 translation, quaternion rotation, float3 scale)
        {
            return float4x4.TRS(translation, rotation, scale);
        }

        /// <summary>
        /// 创建绕X轴旋转的矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 RotateX(float radians)
        {
            return float4x4.RotateX(radians);
        }

        /// <summary>
        /// 创建绕Y轴旋转的矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 RotateY(float radians)
        {
            return float4x4.RotateY(radians);
        }

        /// <summary>
        /// 创建绕Z轴旋转的矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 RotateZ(float radians)
        {
            return float4x4.RotateZ(radians);
        }

        /// <summary>
        /// 创建欧拉角旋转矩阵（XYZ顺序）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerXYZ(float3 radians)
        {
            return float4x4.EulerXYZ(radians);
        }

        /// <summary>
        /// 创建绕任意轴旋转的矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 AxisAngle(float3 axis, float radians)
        {
            return new float4x4(quaternion.AxisAngle(axis, radians), float3.zero);
        }

        #endregion

        #region 视图和投影矩阵

        /// <summary>
        /// 创建LookAt视图矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 LookAt(float3 eye, float3 target, float3 up)
        {
            return float4x4.LookAt(eye, target, up);
        }

        /// <summary>
        /// 创建透视投影矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 PerspectiveFov(float fovRadians, float aspect, float near, float far)
        {
            return float4x4.PerspectiveFov(fovRadians, aspect, near, far);
        }

        /// <summary>
        /// 创建正交投影矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Ortho(float width, float height, float near, float far)
        {
            return float4x4.Ortho(width, height, near, far);
        }

        /// <summary>
        /// 创建正交投影矩阵（指定边界）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 OrthoOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            return float4x4.OrthoOffCenter(left, right, bottom, top, near, far);
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(this float4x4 a, float4x4 b, float epsilon = 1e-6f)
        {
            return math.all(math.abs(a.c0 - b.c0) < epsilon) &&
                   math.all(math.abs(a.c1 - b.c1) < epsilon) &&
                   math.all(math.abs(a.c2 - b.c2) < epsilon) &&
                   math.all(math.abs(a.c3 - b.c3) < epsilon);
        }

        /// <summary>
        /// 检查是否为单位矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentity(this float4x4 m, float epsilon = 1e-6f)
        {
            return Approximately(m, float4x4.identity, epsilon);
        }

        /// <summary>
        /// 检查是否为正交矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOrthogonal(this float4x4 m, float epsilon = 1e-6f)
        {
            float4x4 mT = math.transpose(m);
            float4x4 product = math.mul(m, mT);
            return Approximately(product, float4x4.identity, epsilon);
        }

        /// <summary>
        /// 检查是否包含NaN
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNaN(this float4x4 m)
        {
            return math.any(math.isnan(m.c0)) ||
                   math.any(math.isnan(m.c1)) ||
                   math.any(math.isnan(m.c2)) ||
                   math.any(math.isnan(m.c3));
        }

        /// <summary>
        /// 检查是否可逆
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvertible(this float4x4 m, float epsilon = 1e-6f)
        {
            return math.abs(math.determinant(m)) > epsilon;
        }

        #endregion

        #region 行列访问

        /// <summary>
        /// 获取指定行
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetRow(this float4x4 m, int row)
        {
            return new float4(m.c0[row], m.c1[row], m.c2[row], m.c3[row]);
        }

        /// <summary>
        /// 获取指定列
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetColumn(this float4x4 m, int col)
        {
            switch (col)
            {
                case 0: return m.c0;
                case 1: return m.c1;
                case 2: return m.c2;
                case 3: return m.c3;
                default: return float4.zero;
            }
        }

        /// <summary>
        /// 设置指定列
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 WithColumn(this float4x4 m, int col, float4 value)
        {
            switch (col)
            {
                case 0: m.c0 = value; break;
                case 1: m.c1 = value; break;
                case 2: m.c2 = value; break;
                case 3: m.c3 = value; break;
            }
            return m;
        }

        #endregion
    }
}
