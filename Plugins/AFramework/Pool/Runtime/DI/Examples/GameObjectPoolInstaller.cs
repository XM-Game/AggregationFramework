// ==========================================================
// 文件名：GameObjectPoolInstaller.cs
// 命名空间: AFramework.Pool.DI.Examples
// 依赖: AFramework.DI, AFramework.Pool
// 功能: GameObject 对象池安装器示例
// ==========================================================

using AFramework.DI;
using AFramework.Pool.Warming;

namespace AFramework.Pool.DI.Examples
{
    /// <summary>
    /// GameObject 对象池安装器示例
    /// GameObject Pool Installer Example
    /// </summary>
    /// <remarks>
    /// 演示如何注册和配置对象池
    /// Demonstrates how to register and configure object pools
    /// 
    /// 使用方法 / Usage:
    /// 1. 在 LifetimeScope 中添加此安装器
    ///    Add this installer to LifetimeScope
    /// 2. 通过依赖注入获取对象池
    ///    Get object pool through dependency injection
    /// 
    /// 示例代码 / Example Code:
    /// <code>
    /// public class MyService
    /// {
    ///     private readonly IObjectPool&lt;MyObject&gt; _pool;
    ///     
    ///     public MyService(IObjectPool&lt;MyObject&gt; pool)
    ///     {
    ///         _pool = pool;
    ///     }
    ///     
    ///     public void DoSomething()
    ///     {
    ///         var obj = _pool.Get();
    ///         // 使用对象 / Use object
    ///         _pool.Return(obj);
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public class GameObjectPoolInstaller : PoolInstaller
    {
        protected override void InstallPools(IContainerBuilder builder)
        {
            // 示例 1: 注册简单对象池
            // Example 1: Register simple object pool
            builder.RegisterPool<MyPooledObject>(pool =>
            {
                pool.WithInitialCapacity(10)
                    .WithMaxCapacity(100)
                    .WithDiagnostics(true);
            });

            // 示例 2: 注册带工厂的对象池
            // Example 2: Register object pool with factory
            builder.RegisterPoolWithFactory<MyComplexObject>(
                factory: () => new MyComplexObject { Id = System.Guid.NewGuid() },
                configure: pool =>
                {
                    pool.WithInitialCapacity(5)
                        .WithMaxCapacity(50);
                });

            // 示例 3: 注册带预热的对象池
            // Example 3: Register object pool with warmup
            builder.RegisterPoolWithWarmup<MyGameObject>(
                warmupConfig: new WarmupConfig
                {
                    Strategy = WarmupStrategy.Immediate,
                    Count = 20,
                    AutoWarmup = true
                },
                configure: pool =>
                {
                    pool.WithInitialCapacity(20)
                        .WithMaxCapacity(200);
                });

            // 示例 4: 注册使用容器解析的对象池
            // Example 4: Register object pool using container resolution
            builder.RegisterPoolWithContainerFactory<MyServiceObject>(pool =>
            {
                pool.WithInitialCapacity(5)
                    .WithMaxCapacity(50);
            });

            // 示例 5: 批量注册对象池
            // Example 5: Batch register object pools
            builder.RegisterPools(
                typeof(PooledObject1),
                typeof(PooledObject2),
                typeof(PooledObject3)
            );
        }
    }

    #region 示例类型 Example Types

    /// <summary>
    /// 简单池化对象示例
    /// Simple pooled object example
    /// </summary>
    public class MyPooledObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void Reset()
        {
            Id = 0;
            Name = null;
        }
    }

    /// <summary>
    /// 复杂池化对象示例
    /// Complex pooled object example
    /// </summary>
    public class MyComplexObject
    {
        public System.Guid Id { get; set; }
        public System.DateTime CreatedTime { get; set; }

        public MyComplexObject()
        {
            CreatedTime = System.DateTime.UtcNow;
        }
    }

    /// <summary>
    /// GameObject 池化对象示例
    /// GameObject pooled object example
    /// </summary>
    public class MyGameObject
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// 服务对象示例（需要依赖注入）
    /// Service object example (requires dependency injection)
    /// </summary>
    public class MyServiceObject
    {
        private readonly IObjectResolver _resolver;

        public MyServiceObject(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
    }

    /// <summary>
    /// 批量注册示例对象 1
    /// Batch registration example object 1
    /// </summary>
    public class PooledObject1
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// 批量注册示例对象 2
    /// Batch registration example object 2
    /// </summary>
    public class PooledObject2
    {
        public string Text { get; set; }
    }

    /// <summary>
    /// 批量注册示例对象 3
    /// Batch registration example object 3
    /// </summary>
    public class PooledObject3
    {
        public float Number { get; set; }
    }

    #endregion
}
