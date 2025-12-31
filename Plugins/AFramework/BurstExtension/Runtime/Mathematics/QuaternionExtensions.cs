// ==========================================================
// 文件名：QuaternionExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：quaternion扩展方法，提供便捷的四元数操作
// 依赖：Unity.Burst, Unity.Mathematics
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// quaternion扩展方法类
    /// 提供便捷的四元数操作和常用计算
    /// </summary>
    [BurstCompile]
    public static class QuaternionExtensions
    {
        #region 常量

        /// <summary>
        /// 单位四元数
        /// </summary>
        public static readonly quaternion Identity = quaternion.identity;

        #endregion

        #region 基础操作

        /// <summary>
        /// 获取四元数的共轭
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Conjugate(this quaternion q)
        {
            return math.conjugate(q);
        }

        /// <summary>
        /// 获取四元数的逆
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Inverse(this quaternion q)
        {
            return math.inverse(q);
        }

        /// <summary>
        /// 归一化四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Normalized(this quaternion q)
        {
            return math.normalize(q);
        }

        /// <summary>
        /// 安全归一化（零四元数返回单位四元数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion NormalizedSafe(this quaternion q)
        {
            return math.normalizesafe(q);
        }

        /// <summary>
        /// 计算四元数的长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(this quaternion q)
        {
            return math.length(q.value);
        }

        /// <summary>
        /// 计算四元数的长度平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSq(this quaternion q)
        {
            return math.lengthsq(q.value);
        }

        #endregion

        #region 旋转操作

        /// <summary>
        /// 使用四元数旋转向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Rotate(this quaternion q, float3 v)
        {
            return math.rotate(q, v);
        }

        /// <summary>
        /// 使用四元数的逆旋转向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 InverseRotate(this quaternion q, float3 v)
        {
            return math.rotate(math.inverse(q), v);
        }

        /// <summary>
        /// 组合两个旋转（先应用other，再应用this）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Multiply(this quaternion q, quaternion other)
        {
            return math.mul(q, other);
        }

        /// <summary>
        /// 获取前向向量（Z轴正方向）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Forward(this quaternion q)
        {
            return math.forward(q);
        }

        /// <summary>
        /// 获取上向量（Y轴正方向）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Up(this quaternion q)
        {
            return math.rotate(q, math.up());
        }

        /// <summary>
        /// 获取右向量（X轴正方向）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Right(this quaternion q)
        {
            return math.rotate(q, math.right());
        }

        #endregion

        #region 欧拉角转换

        /// <summary>
        /// 转换为欧拉角（弧度，XYZ顺序）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ToEulerRadians(this quaternion q)
        {
            return ToEulerRadiansXYZ(q);
        }

        /// <summary>
        /// 转换为欧拉角（角度，XYZ顺序）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ToEulerDegrees(this quaternion q)
        {
            return math.degrees(ToEulerRadiansXYZ(q));
        }

        /// <summary>
        /// 转换为欧拉角（弧度，XYZ顺序）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ToEulerRadiansXYZ(quaternion q)
        {
            float4 qv = q.value;
            float sinr_cosp = 2f * (qv.w * qv.x + qv.y * qv.z);
            float cosr_cosp = 1f - 2f * (qv.x * qv.x + qv.y * qv.y);
            float x = math.atan2(sinr_cosp, cosr_cosp);

            float sinp = 2f * (qv.w * qv.y - qv.z * qv.x);
            float y;
            if (math.abs(sinp) >= 1f)
                y = math.sign(sinp) * math.PI * 0.5f;
            else
                y = math.asin(sinp);

            float siny_cosp = 2f * (qv.w * qv.z + qv.x * qv.y);
            float cosy_cosp = 1f - 2f * (qv.y * qv.y + qv.z * qv.z);
            float z = math.atan2(siny_cosp, cosy_cosp);

            return new float3(x, y, z);
        }

        /// <summary>
        /// 从欧拉角创建四元数（弧度，XYZ顺序）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromEulerRadians(float3 euler)
        {
            return quaternion.EulerXYZ(euler);
        }

        /// <summary>
        /// 从欧拉角创建四元数（角度，XYZ顺序）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromEulerDegrees(float3 euler)
        {
            return quaternion.EulerXYZ(math.radians(euler));
        }

        #endregion

        #region 轴角转换

        /// <summary>
        /// 转换为轴角表示
        /// </summary>
        /// <param name="q">四元数</param>
        /// <param name="axis">输出旋转轴</param>
        /// <param name="angle">输出旋转角度（弧度）</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToAxisAngle(this quaternion q, out float3 axis, out float angle)
        {
            float4 qv = q.value;
            float sinHalfAngle = math.length(qv.xyz);
            
            if (sinHalfAngle < 1e-6f)
            {
                axis = math.up();
                angle = 0f;
                return;
            }

            axis = qv.xyz / sinHalfAngle;
            angle = 2f * math.atan2(sinHalfAngle, qv.w);
        }

        /// <summary>
        /// 获取旋转轴
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetAxis(this quaternion q)
        {
            ToAxisAngle(q, out float3 axis, out _);
            return axis;
        }

        /// <summary>
        /// 获取旋转角度（弧度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this quaternion q)
        {
            ToAxisAngle(q, out _, out float angle);
            return angle;
        }

        /// <summary>
        /// 获取旋转角度（角度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngleDegrees(this quaternion q)
        {
            return math.degrees(GetAngle(q));
        }

        #endregion

        #region 插值

        /// <summary>
        /// 球面线性插值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Slerp(this quaternion from, quaternion to, float t)
        {
            return math.slerp(from, to, t);
        }

        /// <summary>
        /// 归一化线性插值（比Slerp快，但不保持恒定角速度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Nlerp(this quaternion from, quaternion to, float t)
        {
            return math.nlerp(from, to, t);
        }

        /// <summary>
        /// 向目标旋转（限制最大角度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion RotateTowards(this quaternion from, quaternion to, float maxRadiansDelta)
        {
            float angle = AngleBetween(from, to);
            if (angle < 1e-6f)
                return to;
            float t = math.min(1f, maxRadiansDelta / angle);
            return math.slerp(from, to, t);
        }

        #endregion

        #region 角度计算

        /// <summary>
        /// 计算两个四元数之间的角度（弧度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleBetween(quaternion a, quaternion b)
        {
            float dot = math.abs(math.dot(a.value, b.value));
            return 2f * math.acos(math.min(dot, 1f));
        }

        /// <summary>
        /// 计算两个四元数之间的角度（角度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleBetweenDegrees(quaternion a, quaternion b)
        {
            return math.degrees(AngleBetween(a, b));
        }

        /// <summary>
        /// 计算到另一个四元数的角度（弧度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleTo(this quaternion q, quaternion other)
        {
            return AngleBetween(q, other);
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(this quaternion q, quaternion other, float epsilon = 1e-6f)
        {
            // 四元数q和-q表示相同的旋转
            float dot = math.abs(math.dot(q.value, other.value));
            return dot > 1f - epsilon;
        }

        /// <summary>
        /// 检查是否为单位四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentity(this quaternion q, float epsilon = 1e-6f)
        {
            return Approximately(q, quaternion.identity, epsilon);
        }

        /// <summary>
        /// 检查是否为有效四元数（归一化）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNormalized(this quaternion q, float epsilon = 1e-6f)
        {
            float lenSq = math.lengthsq(q.value);
            return math.abs(lenSq - 1f) < epsilon;
        }

        /// <summary>
        /// 检查是否包含NaN
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNaN(this quaternion q)
        {
            return math.any(math.isnan(q.value));
        }

        #endregion

        #region 创建辅助

        /// <summary>
        /// 创建从一个方向到另一个方向的旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromToRotation(float3 from, float3 to)
        {
            from = math.normalize(from);
            to = math.normalize(to);
            
            float dot = math.dot(from, to);
            
            if (dot > 0.99999f)
                return quaternion.identity;
            
            if (dot < -0.99999f)
            {
                // 180度旋转，需要找一个垂直轴
                float3 axis = math.cross(math.right(), from);
                if (math.lengthsq(axis) < 0.0001f)
                    axis = math.cross(math.up(), from);
                axis = math.normalize(axis);
                return quaternion.AxisAngle(axis, math.PI);
            }
            
            float3 cross = math.cross(from, to);
            float4 qv = new float4(cross, 1f + dot);
            return math.normalize(new quaternion(qv));
        }

        /// <summary>
        /// 创建看向目标的旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion LookRotation(float3 forward, float3 up)
        {
            return quaternion.LookRotation(forward, up);
        }

        /// <summary>
        /// 创建看向目标的旋转（使用默认上向量）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion LookRotation(float3 forward)
        {
            return quaternion.LookRotationSafe(forward, math.up());
        }

        #endregion

        #region 矩阵转换

        /// <summary>
        /// 转换为3x3旋转矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 ToMatrix3x3(this quaternion q)
        {
            return new float3x3(q);
        }

        /// <summary>
        /// 转换为4x4变换矩阵（仅旋转）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 ToMatrix4x4(this quaternion q)
        {
            return new float4x4(q, float3.zero);
        }

        /// <summary>
        /// 从3x3旋转矩阵创建四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromMatrix(float3x3 m)
        {
            return new quaternion(m);
        }

        /// <summary>
        /// 从4x4变换矩阵创建四元数（提取旋转部分）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromMatrix(float4x4 m)
        {
            return new quaternion(new float3x3(m.c0.xyz, m.c1.xyz, m.c2.xyz));
        }

        #endregion
    }
}
