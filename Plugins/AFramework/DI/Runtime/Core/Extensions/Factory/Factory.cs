// ==========================================================
// 文件名：Factory.cs
// 命名空间: AFramework.DI
// 依赖: System
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 工厂实现
    /// <para>使用容器创建服务实例</para>
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    public sealed class Factory<T> : IFactory<T> where T : class
    {
        private readonly IObjectResolver _resolver;

        public Factory(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public T Create() => _resolver.Instantiate<T>();
    }

    /// <summary>
    /// 带参数的工厂实现
    /// </summary>
    public sealed class Factory<TParam, T> : IFactory<TParam, T> where T : class
    {
        private readonly IObjectResolver _resolver;

        public Factory(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public T Create(TParam param)
        {
            var parameters = new List<IInjectParameter>
            {
                new TypedParameter(typeof(TParam), param)
            };
            return _resolver.Instantiate<T>(parameters);
        }
    }

    /// <summary>
    /// 带两个参数的工厂实现
    /// </summary>
    public sealed class Factory<TParam1, TParam2, T> : IFactory<TParam1, TParam2, T> 
        where T : class
    {
        private readonly IObjectResolver _resolver;

        public Factory(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public T Create(TParam1 param1, TParam2 param2)
        {
            var parameters = new List<IInjectParameter>
            {
                new TypedParameter(typeof(TParam1), param1),
                new TypedParameter(typeof(TParam2), param2)
            };
            return _resolver.Instantiate<T>(parameters);
        }
    }

    /// <summary>
    /// 带三个参数的工厂实现
    /// </summary>
    public sealed class Factory<TParam1, TParam2, TParam3, T> : IFactory<TParam1, TParam2, TParam3, T>
        where T : class
    {
        private readonly IObjectResolver _resolver;

        public Factory(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public T Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            var parameters = new List<IInjectParameter>
            {
                new TypedParameter(typeof(TParam1), param1),
                new TypedParameter(typeof(TParam2), param2),
                new TypedParameter(typeof(TParam3), param3)
            };
            return _resolver.Instantiate<T>(parameters);
        }
    }
}
