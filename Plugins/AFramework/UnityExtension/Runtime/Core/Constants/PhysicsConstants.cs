// ==========================================================
// 文件名：PhysicsConstants.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Unity 物理常量定义
    /// <para>提供 3D/2D 物理系统的常用常量和配置值</para>
    /// <para>包含射线检测、碰撞检测、物理材质等相关常量</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用默认射线检测距离
    /// Physics.Raycast(origin, direction, PhysicsConstants.DefaultRaycastDistance);
    /// 
    /// // 使用重力常量
    /// float gravity = PhysicsConstants.Gravity.Earth;
    /// 
    /// // 使用碰撞检测缓冲区大小
    /// var hits = new RaycastHit[PhysicsConstants.DefaultHitBufferSize];
    /// </code>
    /// </remarks>
    public static class PhysicsConstants
    {
        #region 射线检测常量

        /// <summary>默认射线检测距离</summary>
        public const float DefaultRaycastDistance = 100f;

        /// <summary>无限射线检测距离</summary>
        public const float InfiniteRaycastDistance = float.MaxValue;

        /// <summary>短距离射线检测 (近距离交互)</summary>
        public const float ShortRaycastDistance = 2f;

        /// <summary>中距离射线检测 (一般交互)</summary>
        public const float MediumRaycastDistance = 10f;

        /// <summary>长距离射线检测 (远程检测)</summary>
        public const float LongRaycastDistance = 50f;

        #endregion

        #region 碰撞检测缓冲区大小

        /// <summary>默认碰撞检测结果缓冲区大小</summary>
        public const int DefaultHitBufferSize = 16;

        /// <summary>小型碰撞检测结果缓冲区大小</summary>
        public const int SmallHitBufferSize = 4;

        /// <summary>大型碰撞检测结果缓冲区大小</summary>
        public const int LargeHitBufferSize = 64;

        /// <summary>默认重叠检测结果缓冲区大小</summary>
        public const int DefaultOverlapBufferSize = 16;

        #endregion

        #region 物理精度常量

        /// <summary>默认皮肤宽度 (用于角色控制器)</summary>
        public const float DefaultSkinWidth = 0.08f;

        /// <summary>默认接触偏移量</summary>
        public const float DefaultContactOffset = 0.01f;

        /// <summary>最小移动距离 (小于此值忽略移动)</summary>
        public const float MinMoveDistance = 0.001f;

        /// <summary>地面检测距离</summary>
        public const float GroundCheckDistance = 0.1f;

        /// <summary>斜坡检测角度阈值 (度)</summary>
        public const float SlopeAngleThreshold = 45f;

        /// <summary>台阶高度阈值</summary>
        public const float StepHeightThreshold = 0.3f;

        #endregion

        #region 重力常量

        /// <summary>
        /// 重力常量集合
        /// <para>提供不同环境下的重力加速度值</para>
        /// </summary>
        public static class Gravity
        {
            /// <summary>地球重力加速度 (m/s²)</summary>
            public const float Earth = -9.81f;

            /// <summary>月球重力加速度 (m/s²)</summary>
            public const float Moon = -1.62f;

            /// <summary>火星重力加速度 (m/s²)</summary>
            public const float Mars = -3.71f;

            /// <summary>木星重力加速度 (m/s²)</summary>
            public const float Jupiter = -24.79f;

            /// <summary>零重力</summary>
            public const float Zero = 0f;

            /// <summary>Unity 默认重力 (Y轴向下)</summary>
            public static readonly Vector3 DefaultGravity = new Vector3(0f, Earth, 0f);

            /// <summary>2D 默认重力 (Y轴向下)</summary>
            public static readonly Vector2 DefaultGravity2D = new Vector2(0f, Earth);

            /// <summary>低重力 (适合跳跃游戏)</summary>
            public static readonly Vector3 LowGravity = new Vector3(0f, -4.9f, 0f);

            /// <summary>高重力 (适合快节奏游戏)</summary>
            public static readonly Vector3 HighGravity = new Vector3(0f, -19.62f, 0f);
        }

        #endregion

        #region 速度常量

        /// <summary>
        /// 速度常量集合
        /// <para>提供常用的移动速度参考值</para>
        /// </summary>
        public static class Speed
        {
            /// <summary>步行速度 (m/s)</summary>
            public const float Walk = 1.4f;

            /// <summary>慢跑速度 (m/s)</summary>
            public const float Jog = 3.0f;

            /// <summary>奔跑速度 (m/s)</summary>
            public const float Run = 5.0f;

            /// <summary>冲刺速度 (m/s)</summary>
            public const float Sprint = 8.0f;

            /// <summary>默认跳跃力</summary>
            public const float DefaultJumpForce = 5.0f;

            /// <summary>默认下落速度限制</summary>
            public const float DefaultTerminalVelocity = 50f;
        }

        #endregion

        #region 物理材质常量

        /// <summary>
        /// 物理材质摩擦力常量
        /// </summary>
        public static class Friction
        {
            /// <summary>冰面摩擦力</summary>
            public const float Ice = 0.05f;

            /// <summary>光滑表面摩擦力</summary>
            public const float Smooth = 0.1f;

            /// <summary>默认摩擦力</summary>
            public const float Default = 0.4f;

            /// <summary>粗糙表面摩擦力</summary>
            public const float Rough = 0.7f;

            /// <summary>橡胶摩擦力</summary>
            public const float Rubber = 1.0f;

            /// <summary>最大摩擦力 (完全不滑动)</summary>
            public const float Maximum = 1.0f;

            /// <summary>无摩擦力</summary>
            public const float None = 0f;
        }

        /// <summary>
        /// 物理材质弹性常量
        /// </summary>
        public static class Bounciness
        {
            /// <summary>无弹性 (完全不反弹)</summary>
            public const float None = 0f;

            /// <summary>低弹性</summary>
            public const float Low = 0.2f;

            /// <summary>中等弹性</summary>
            public const float Medium = 0.5f;

            /// <summary>高弹性</summary>
            public const float High = 0.8f;

            /// <summary>完全弹性 (理想弹性碰撞)</summary>
            public const float Perfect = 1.0f;

            /// <summary>橡皮球弹性</summary>
            public const float RubberBall = 0.8f;

            /// <summary>网球弹性</summary>
            public const float TennisBall = 0.75f;

            /// <summary>篮球弹性</summary>
            public const float Basketball = 0.6f;
        }

        #endregion

        #region 碰撞体尺寸常量

        /// <summary>
        /// 碰撞体尺寸常量
        /// </summary>
        public static class ColliderSize
        {
            /// <summary>默认球体碰撞体半径</summary>
            public const float DefaultSphereRadius = 0.5f;

            /// <summary>默认胶囊体碰撞体半径</summary>
            public const float DefaultCapsuleRadius = 0.5f;

            /// <summary>默认胶囊体碰撞体高度</summary>
            public const float DefaultCapsuleHeight = 2.0f;

            /// <summary>默认盒体碰撞体尺寸</summary>
            public static readonly Vector3 DefaultBoxSize = Vector3.one;

            /// <summary>角色控制器默认半径</summary>
            public const float CharacterRadius = 0.5f;

            /// <summary>角色控制器默认高度</summary>
            public const float CharacterHeight = 2.0f;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 获取当前物理系统的重力向量
        /// </summary>
        /// <returns>当前重力向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetCurrentGravity()
        {
            return Physics.gravity;
        }

        /// <summary>
        /// 获取当前 2D 物理系统的重力向量
        /// </summary>
        /// <returns>当前 2D 重力向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetCurrentGravity2D()
        {
            return Physics2D.gravity;
        }

        /// <summary>
        /// 设置物理系统重力
        /// </summary>
        /// <param name="gravity">重力向量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetGravity(Vector3 gravity)
        {
            Physics.gravity = gravity;
        }

        /// <summary>
        /// 设置 2D 物理系统重力
        /// </summary>
        /// <param name="gravity">重力向量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetGravity2D(Vector2 gravity)
        {
            Physics2D.gravity = gravity;
        }

        /// <summary>
        /// 计算从当前高度自由落体到地面所需的时间
        /// </summary>
        /// <param name="height">高度 (正值)</param>
        /// <param name="gravity">重力加速度 (负值)</param>
        /// <returns>下落时间 (秒)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalculateFallTime(float height, float gravity = Gravity.Earth)
        {
            if (height <= 0f || gravity >= 0f)
                return 0f;

            return Mathf.Sqrt(2f * height / -gravity);
        }

        /// <summary>
        /// 计算达到指定高度所需的初始跳跃速度
        /// </summary>
        /// <param name="height">目标高度</param>
        /// <param name="gravity">重力加速度 (负值)</param>
        /// <returns>所需的初始速度</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalculateJumpVelocity(float height, float gravity = Gravity.Earth)
        {
            if (height <= 0f || gravity >= 0f)
                return 0f;

            return Mathf.Sqrt(2f * -gravity * height);
        }

        /// <summary>
        /// 计算抛物线运动的最大高度
        /// </summary>
        /// <param name="initialVelocity">初始垂直速度</param>
        /// <param name="gravity">重力加速度 (负值)</param>
        /// <returns>最大高度</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalculateMaxHeight(float initialVelocity, float gravity = Gravity.Earth)
        {
            if (initialVelocity <= 0f || gravity >= 0f)
                return 0f;

            return (initialVelocity * initialVelocity) / (2f * -gravity);
        }

        /// <summary>
        /// 检查角度是否在可行走斜坡范围内
        /// </summary>
        /// <param name="angle">斜坡角度 (度)</param>
        /// <param name="maxSlopeAngle">最大可行走角度</param>
        /// <returns>如果可行走返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWalkableSlope(float angle, float maxSlopeAngle = SlopeAngleThreshold)
        {
            return angle <= maxSlopeAngle;
        }

        /// <summary>
        /// 从法线计算斜坡角度
        /// </summary>
        /// <param name="normal">表面法线</param>
        /// <returns>斜坡角度 (度)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSlopeAngle(Vector3 normal)
        {
            return Vector3.Angle(normal, Vector3.up);
        }

        /// <summary>
        /// 从法线计算 2D 斜坡角度
        /// </summary>
        /// <param name="normal">表面法线</param>
        /// <returns>斜坡角度 (度)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSlopeAngle2D(Vector2 normal)
        {
            return Vector2.Angle(normal, Vector2.up);
        }

        #endregion
    }
}
