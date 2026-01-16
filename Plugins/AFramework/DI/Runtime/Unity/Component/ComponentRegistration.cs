// ==========================================================
// 文件名：ComponentRegistration.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: Unity 组件注册信息，描述如何注册 Unity 组件到容器
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 组件注册类型
    /// <para>定义 Unity 组件的注册方式</para>
    /// <para>Defines how Unity components are registered</para>
    /// </summary>
    public enum ComponentRegistrationType
    {
        /// <summary>
        /// 注册现有组件实例
        /// <para>Register an existing component instance</para>
        /// </summary>
        Instance,

        /// <summary>
        /// 在层级中查找组件
        /// <para>Find component in hierarchy</para>
        /// </summary>
        FindInHierarchy,

        /// <summary>
        /// 在新 GameObject 上创建组件
        /// <para>Create component on a new GameObject</para>
        /// </summary>
        NewGameObject,

        /// <summary>
        /// 从预制体实例化
        /// <para>Instantiate from prefab</para>
        /// </summary>
        FromPrefab
    }

    /// <summary>
    /// 组件注册信息
    /// <para>描述 Unity 组件的注册配置</para>
    /// <para>Describes Unity component registration configuration</para>
    /// </summary>
    public sealed class ComponentRegistration
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取组件类型
        /// <para>Get the component type</para>
        /// </summary>
        public Type ComponentType { get; }

        /// <summary>
        /// 获取服务类型列表
        /// <para>Get the list of service types</para>
        /// </summary>
        public Type[] ServiceTypes { get; internal set; }

        /// <summary>
        /// 获取注册类型
        /// <para>Get the registration type</para>
        /// </summary>
        public ComponentRegistrationType RegistrationType { get; }

        /// <summary>
        /// 获取生命周期
        /// <para>Get the lifetime</para>
        /// </summary>
        public Lifetime Lifetime { get; internal set; } = Lifetime.Singleton;

        /// <summary>
        /// 获取现有组件实例（用于 Instance 类型）
        /// <para>Get the existing component instance (for Instance type)</para>
        /// </summary>
        public Component ExistingInstance { get; }

        /// <summary>
        /// 获取预制体（用于 FromPrefab 类型）
        /// <para>Get the prefab (for FromPrefab type)</para>
        /// </summary>
        public GameObject Prefab { get; }

        /// <summary>
        /// 获取搜索根 Transform（用于 FindInHierarchy 类型）
        /// <para>Get the search root Transform (for FindInHierarchy type)</para>
        /// </summary>
        public Transform SearchRoot { get; }

        /// <summary>
        /// 获取是否包含子对象（用于 FindInHierarchy 类型）
        /// <para>Get whether to include children (for FindInHierarchy type)</para>
        /// </summary>
        public bool IncludeChildren { get; internal set; } = true;

        /// <summary>
        /// 获取新 GameObject 的名称（用于 NewGameObject 类型）
        /// <para>Get the name for new GameObject (for NewGameObject type)</para>
        /// </summary>
        public string GameObjectName { get; internal set; }

        /// <summary>
        /// 获取父 Transform（用于 NewGameObject 和 FromPrefab 类型）
        /// <para>Get the parent Transform (for NewGameObject and FromPrefab types)</para>
        /// </summary>
        public Transform ParentTransform { get; internal set; }

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建实例注册
        /// </summary>
        internal ComponentRegistration(Component instance)
        {
            ComponentType = instance.GetType();
            RegistrationType = ComponentRegistrationType.Instance;
            ExistingInstance = instance;
            ServiceTypes = new[] { ComponentType };
        }

        /// <summary>
        /// 创建层级查找注册
        /// </summary>
        internal ComponentRegistration(Type componentType, Transform searchRoot)
        {
            ComponentType = componentType;
            RegistrationType = ComponentRegistrationType.FindInHierarchy;
            SearchRoot = searchRoot;
            ServiceTypes = new[] { componentType };
        }

        /// <summary>
        /// 创建新 GameObject 注册
        /// </summary>
        internal ComponentRegistration(Type componentType, string gameObjectName)
        {
            ComponentType = componentType;
            RegistrationType = ComponentRegistrationType.NewGameObject;
            GameObjectName = gameObjectName ?? componentType.Name;
            ServiceTypes = new[] { componentType };
        }

        /// <summary>
        /// 创建预制体注册
        /// </summary>
        internal ComponentRegistration(Type componentType, GameObject prefab)
        {
            ComponentType = componentType;
            RegistrationType = ComponentRegistrationType.FromPrefab;
            Prefab = prefab;
            ServiceTypes = new[] { componentType };
        }

        #endregion
    }
}
