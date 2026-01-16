// ==========================================================
// 文件名：EntryPointBuilderExtensions.cs
// 命名空间: AFramework.DI
// 依赖: System
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 入口点容器构建器扩展方法
    /// <para>提供便捷的入口点注册 API</para>
    /// </summary>
    public static class EntryPointBuilderExtensions
    {
        #region 注册入口点

        /// <summary>
        /// 注册入口点类型
        /// <para>自动检测并注册实现的所有入口点接口</para>
        /// </summary>
        /// <typeparam name="T">入口点类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <returns>注册构建器</returns>
        public static IRegistrationBuilder RegisterEntryPoint<T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.Register<T>()
                .Singleton()
                .AsImplementedInterfaces()
                .AsSelf();
        }

        /// <summary>
        /// 注册入口点类型（指定生命周期）
        /// </summary>
        /// <typeparam name="T">入口点类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <param name="lifetime">生命周期</param>
        /// <returns>注册构建器</returns>
        public static IRegistrationBuilder RegisterEntryPoint<T>(
            this IContainerBuilder builder,
            Lifetime lifetime)
            where T : class
        {
            return builder.Register<T>()
                .WithLifetime(lifetime)
                .AsImplementedInterfaces()
                .AsSelf();
        }

        /// <summary>
        /// 注册入口点实例
        /// </summary>
        /// <typeparam name="T">入口点类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <param name="instance">入口点实例</param>
        /// <returns>注册构建器</returns>
        public static IRegistrationBuilder RegisterEntryPointInstance<T>(
            this IContainerBuilder builder,
            T instance)
            where T : class
        {
            return builder.RegisterInstance(instance)
                .AsImplementedInterfaces()
                .AsSelf();
        }

        #endregion

        #region 特定入口点注册

        /// <summary>
        /// 注册 IInitializable 入口点
        /// </summary>
        public static IRegistrationBuilder RegisterInitializable<T>(this IContainerBuilder builder)
            where T : class, IInitializable
        {
            return builder.Register<T>()
                .Singleton()
                .As<IInitializable>()
                .AsSelf();
        }

        /// <summary>
        /// 注册 IStartable 入口点
        /// </summary>
        public static IRegistrationBuilder RegisterStartable<T>(this IContainerBuilder builder)
            where T : class, IStartable
        {
            return builder.Register<T>()
                .Singleton()
                .As<IStartable>()
                .AsSelf();
        }

        /// <summary>
        /// 注册 ITickable 入口点
        /// </summary>
        public static IRegistrationBuilder RegisterTickable<T>(this IContainerBuilder builder)
            where T : class, ITickable
        {
            return builder.Register<T>()
                .Singleton()
                .As<ITickable>()
                .AsSelf();
        }

        /// <summary>
        /// 注册 IFixedTickable 入口点
        /// </summary>
        public static IRegistrationBuilder RegisterFixedTickable<T>(this IContainerBuilder builder)
            where T : class, IFixedTickable
        {
            return builder.Register<T>()
                .Singleton()
                .As<IFixedTickable>()
                .AsSelf();
        }

        /// <summary>
        /// 注册 ILateTickable 入口点
        /// </summary>
        public static IRegistrationBuilder RegisterLateTickable<T>(this IContainerBuilder builder)
            where T : class, ILateTickable
        {
            return builder.Register<T>()
                .Singleton()
                .As<ILateTickable>()
                .AsSelf();
        }

        /// <summary>
        /// 注册 IAsyncStartable 入口点
        /// </summary>
        public static IRegistrationBuilder RegisterAsyncStartable<T>(this IContainerBuilder builder)
            where T : class, IAsyncStartable
        {
            return builder.Register<T>()
                .Singleton()
                .As<IAsyncStartable>()
                .AsSelf();
        }

        #endregion

        #region 回调注册

        /// <summary>
        /// 注册初始化回调
        /// </summary>
        /// <param name="builder">容器构建器</param>
        /// <param name="callback">初始化回调</param>
        /// <returns>容器构建器</returns>
        public static IContainerBuilder RegisterInitializeCallback(
            this IContainerBuilder builder,
            Action callback)
        {
            builder.RegisterEntryPointInstance(new CallbackInitializable(callback));
            return builder;
        }

        /// <summary>
        /// 注册启动回调
        /// </summary>
        /// <param name="builder">容器构建器</param>
        /// <param name="callback">启动回调</param>
        /// <returns>容器构建器</returns>
        public static IContainerBuilder RegisterStartCallback(
            this IContainerBuilder builder,
            Action callback)
        {
            builder.RegisterEntryPointInstance(new CallbackStartable(callback));
            return builder;
        }

        /// <summary>
        /// 注册更新回调
        /// </summary>
        /// <param name="builder">容器构建器</param>
        /// <param name="callback">更新回调</param>
        /// <returns>容器构建器</returns>
        public static IContainerBuilder RegisterTickCallback(
            this IContainerBuilder builder,
            Action callback)
        {
            builder.RegisterEntryPointInstance(new CallbackTickable(callback));
            return builder;
        }

        #endregion

        #region 内部回调类

        private sealed class CallbackInitializable : IInitializable
        {
            private readonly Action _callback;
            public CallbackInitializable(Action callback) => _callback = callback;
            public void Initialize() => _callback?.Invoke();
        }

        private sealed class CallbackStartable : IStartable
        {
            private readonly Action _callback;
            public CallbackStartable(Action callback) => _callback = callback;
            public void Start() => _callback?.Invoke();
        }

        private sealed class CallbackTickable : ITickable
        {
            private readonly Action _callback;
            public CallbackTickable(Action callback) => _callback = callback;
            public void Tick() => _callback?.Invoke();
        }

        #endregion
    }
}
