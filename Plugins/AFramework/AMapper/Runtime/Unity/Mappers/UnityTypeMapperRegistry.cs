// ==========================================================
// 文件名：UnityTypeMapperRegistry.cs
// 命名空间: AFramework.AMapper.Unity
// 依赖: AFramework.AMapper
// 功能: Unity 类型映射器注册表，统一管理 Unity 特定类型的映射器
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper.Unity
{
    /// <summary>
    /// Unity 类型映射器注册表
    /// <para>统一管理和注册 Unity 特定类型的映射器</para>
    /// <para>Unity type mapper registry for managing Unity-specific type mappers</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责 Unity 类型映射器的注册和管理</item>
    /// <item>外观模式：为 Unity 类型映射器提供统一的注册入口</item>
    /// <item>开闭原则：支持扩展新的 Unity 类型映射器</item>
    /// </list>
    /// 
    /// 包含的映射器类别：
    /// <list type="bullet">
    /// <item>数学类型：Vector2/3/4、Vector2Int/3Int、Quaternion、Matrix4x4、Bounds</item>
    /// <item>图形类型：Color、Color32、Rect</item>
    /// <item>物理类型：Ray、Plane</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 注册所有 Unity 类型映射器
    /// UnityTypeMapperRegistry.RegisterAll(mapperRegistry);
    /// 
    /// // 仅注册数学类型映射器
    /// UnityTypeMapperRegistry.RegisterMathMappers(mapperRegistry);
    /// </code>
    /// </remarks>
    public static class UnityTypeMapperRegistry
    {
        #region 公共方法 / Public Methods

        /// <summary>
        /// 注册所有 Unity 类型映射器
        /// <para>Register all Unity type mappers</para>
        /// </summary>
        /// <param name="registry">映射器注册表 / Mapper registry</param>
        /// <exception cref="ArgumentNullException">当 registry 为 null 时抛出</exception>
        public static void RegisterAll(MapperRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            RegisterMathMappers(registry);
            RegisterGraphicsMappers(registry);
            RegisterPhysicsMappers(registry);
        }

        /// <summary>
        /// 注册数学类型映射器
        /// <para>Register math type mappers</para>
        /// </summary>
        /// <param name="registry">映射器注册表 / Mapper registry</param>
        /// <exception cref="ArgumentNullException">当 registry 为 null 时抛出</exception>
        public static void RegisterMathMappers(MapperRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            // 向量类型映射器
            registry.Register(new Vector2Mapper());
            registry.Register(new Vector3Mapper());
            registry.Register(new Vector4Mapper());
            registry.Register(new Vector2IntMapper());
            registry.Register(new Vector3IntMapper());

            // 旋转和矩阵映射器
            registry.Register(new QuaternionMapper());
            registry.Register(new Matrix4x4Mapper());

            // 边界映射器
            registry.Register(new BoundsMapper());
        }

        /// <summary>
        /// 注册图形类型映射器
        /// <para>Register graphics type mappers</para>
        /// </summary>
        /// <param name="registry">映射器注册表 / Mapper registry</param>
        /// <exception cref="ArgumentNullException">当 registry 为 null 时抛出</exception>
        public static void RegisterGraphicsMappers(MapperRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            // 颜色类型映射器
            registry.Register(new ColorMapper());
            registry.Register(new Color32Mapper());

            // 矩形映射器
            registry.Register(new RectMapper());
        }

        /// <summary>
        /// 注册物理类型映射器
        /// <para>Register physics type mappers</para>
        /// </summary>
        /// <param name="registry">映射器注册表 / Mapper registry</param>
        /// <exception cref="ArgumentNullException">当 registry 为 null 时抛出</exception>
        public static void RegisterPhysicsMappers(MapperRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            // 射线和平面映射器
            registry.Register(new RayMapper());
            registry.Register(new PlaneMapper());
        }

        /// <summary>
        /// 获取所有 Unity 类型映射器
        /// <para>Get all Unity type mappers</para>
        /// </summary>
        /// <returns>映射器列表 / Mapper list</returns>
        public static IReadOnlyList<IObjectMapper> GetAllMappers()
        {
            var mappers = new List<IObjectMapper>();

            // 数学类型映射器
            mappers.Add(new Vector2Mapper());
            mappers.Add(new Vector3Mapper());
            mappers.Add(new Vector4Mapper());
            mappers.Add(new Vector2IntMapper());
            mappers.Add(new Vector3IntMapper());
            mappers.Add(new QuaternionMapper());
            mappers.Add(new Matrix4x4Mapper());
            mappers.Add(new BoundsMapper());

            // 图形类型映射器
            mappers.Add(new ColorMapper());
            mappers.Add(new Color32Mapper());
            mappers.Add(new RectMapper());

            // 物理类型映射器
            mappers.Add(new RayMapper());
            mappers.Add(new PlaneMapper());

            return mappers;
        }

        /// <summary>
        /// 获取数学类型映射器
        /// <para>Get math type mappers</para>
        /// </summary>
        /// <returns>映射器列表 / Mapper list</returns>
        public static IReadOnlyList<IObjectMapper> GetMathMappers()
        {
            return new List<IObjectMapper>
            {
                new Vector2Mapper(),
                new Vector3Mapper(),
                new Vector4Mapper(),
                new Vector2IntMapper(),
                new Vector3IntMapper(),
                new QuaternionMapper(),
                new Matrix4x4Mapper(),
                new BoundsMapper()
            };
        }

        /// <summary>
        /// 获取图形类型映射器
        /// <para>Get graphics type mappers</para>
        /// </summary>
        /// <returns>映射器列表 / Mapper list</returns>
        public static IReadOnlyList<IObjectMapper> GetGraphicsMappers()
        {
            return new List<IObjectMapper>
            {
                new ColorMapper(),
                new Color32Mapper(),
                new RectMapper()
            };
        }

        /// <summary>
        /// 获取物理类型映射器
        /// <para>Get physics type mappers</para>
        /// </summary>
        /// <returns>映射器列表 / Mapper list</returns>
        public static IReadOnlyList<IObjectMapper> GetPhysicsMappers()
        {
            return new List<IObjectMapper>
            {
                new RayMapper(),
                new PlaneMapper()
            };
        }

        #endregion
    }
}
