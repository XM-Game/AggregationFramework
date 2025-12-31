# VContainer 依赖注入框架 API 使用手册

## 目录

1. [基础 API](#基础-api)
2. [注册 API](#注册-api)
3. [解析 API](#解析-api)
4. [生命周期 API](#生命周期-api)
5. [Unity 集成 API](#unity-集成-api)
6. [Entry Points API](#entry-points-api)
7. [参数注入 API](#参数注入-api)
8. [诊断 API](#诊断-api)
9. [扩展 API](#扩展-api)

---

## 基础 API

### ContainerBuilder

容器构建器，用于配置和构建依赖注入容器。

```csharp
using VContainer;

// 创建容器构建器
var builder = new ContainerBuilder();

// 配置注册
builder.Register<MyService>(Lifetime.Singleton);

// 构建容器
var container = builder.Build();

// 使用容器
var service = container.Resolve<MyService>();

// 释放容器
container.Dispose();
```

### IContainerBuilder 接口

```csharp
public interface IContainerBuilder
{
    object ApplicationOrigin { get; set; }
    DiagnosticsCollector Diagnostics { get; set; }
    int Count { get; }
    
    T Register<T>(T registrationBuilder) where T : RegistrationBuilder;
    void RegisterBuildCallback(Action<IObjectResolver> container);
    bool Exists(Type type, bool includeInterfaceTypes = false, bool findParentScopes = false);
}
```

### IObjectResolver 接口

对象解析器接口，用于从容器中解析依赖。

```csharp
public interface IObjectResolver : IDisposable
{
    object ApplicationOrigin { get; }
    DiagnosticsCollector Diagnostics { get; set; }
    
    object Resolve(Type type, object key = null);
    bool TryResolve(Type type, out object resolved, object key = null);
    object Resolve(Registration registration);
    IScopedObjectResolver CreateScope(Action<IContainerBuilder> installation = null);
    void Inject(object instance);
    bool TryGetRegistration(Type type, out Registration registration, object key = null);
}
```

---

## 注册 API

### 基本注册

#### Register - 类型注册

```csharp
// 注册具体类型
builder.Register<MyService>(Lifetime.Singleton);

// 注册接口到实现
builder.Register<IMyService, MyService>(Lifetime.Singleton);

// 注册多个接口
builder.Register<IService1, IService2, MyService>(Lifetime.Singleton);

// 注册开放泛型
builder.Register(typeof(IRepository<>), typeof(Repository<>), Lifetime.Transient);
```

#### RegisterInstance - 实例注册

```csharp
// 注册已创建的实例
var service = new MyService();
builder.RegisterInstance<IMyService>(service);

// 注册为多个接口
builder.RegisterInstance<IService1, IService2>(service);
```

#### Register - 工厂注册

```csharp
// 使用工厂函数注册
builder.Register<IMyService>(resolver => 
{
    var dependency = resolver.Resolve<IDependency>();
    return new MyService(dependency);
}, Lifetime.Singleton);
```

### 注册构建器 API

#### RegistrationBuilder

```csharp
// 链式配置
builder.Register<MyService>(Lifetime.Singleton)
    .As<IMyService>()  // 注册为接口
    .AsSelf()          // 同时注册为自身类型
    .AsImplementedInterfaces();  // 注册为所有实现的接口

// 使用键值
builder.Register<MyService>(Lifetime.Singleton)
    .Keyed("service1");

// 注入参数
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter("name", "MyService")
    .WithParameter<int>(42);
```

#### As 方法

```csharp
// 注册为单个接口
builder.Register<MyService>(Lifetime.Singleton).As<IMyService>();

// 注册为多个接口
builder.Register<MyService>(Lifetime.Singleton)
    .As<IService1, IService2>();

// 注册为多个接口（最多4个）
builder.Register<MyService>(Lifetime.Singleton)
    .As<IService1, IService2, IService3, IService4>();

// 注册为所有实现的接口
builder.Register<MyService>(Lifetime.Singleton)
    .AsImplementedInterfaces();
```

#### WithParameter 方法

```csharp
// 按名称注入参数
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter("name", "MyService");

// 按类型注入参数
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter<string>("MyService")
    .WithParameter<int>(42);

// 使用工厂函数注入参数
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter("name", resolver => resolver.Resolve<IConfig>().ServiceName);

// 使用类型和工厂函数
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter<string>(resolver => resolver.Resolve<IConfig>().ServiceName);
```

#### Keyed 方法

```csharp
// 使用键值注册
builder.Register<ServiceA>(Lifetime.Singleton).Keyed("A");
builder.Register<ServiceB>(Lifetime.Singleton).Keyed("B");

// 解析时使用键值
var serviceA = container.Resolve<IService>("A");
var serviceB = container.Resolve<IService>("B");
```

### 工厂注册

#### RegisterFactory

```csharp
// 无参数工厂
builder.RegisterFactory<IMyService>(() => new MyService());

// 单参数工厂
builder.RegisterFactory<string, IMyService>(name => new MyService(name));

// 多参数工厂（最多4个参数）
builder.RegisterFactory<string, int, IMyService>((name, id) => new MyService(name, id));

// 使用容器解析的工厂
builder.RegisterFactory<IMyService>(
    resolver => () => resolver.Resolve<IMyService>(),
    Lifetime.Singleton);
```

---

## 解析 API

### Resolve 方法

```csharp
// 解析类型
var service = container.Resolve<IMyService>();

// 使用键值解析
var service = container.Resolve<IMyService>("key");

// 解析非泛型类型
var service = container.Resolve(typeof(IMyService));
```

### TryResolve 方法

```csharp
// 尝试解析，失败返回 false
if (container.TryResolve<IMyService>(out var service))
{
    // 使用 service
}

// 使用键值
if (container.TryResolve<IMyService>(out var service, "key"))
{
    // 使用 service
}
```

### ResolveOrDefault 方法

```csharp
// 解析失败时返回默认值
var service = container.ResolveOrDefault<IMyService>(defaultService);

// 使用键值
var service = container.ResolveOrDefault<IMyService>(defaultService, "key");
```

### Inject 方法

```csharp
// 手动注入已创建的对象
var obj = new MyClass();
container.Inject(obj);
```

### CreateScope 方法

```csharp
// 创建子作用域
using (var scope = container.CreateScope(builder =>
{
    builder.Register<ScopedService>(Lifetime.Scoped);
}))
{
    var service = scope.Resolve<ScopedService>();
    // scope 销毁时自动清理
}
```

---

## 生命周期 API

### Lifetime 枚举

```csharp
public enum Lifetime
{
    Transient,   // 瞬态，每次解析创建新实例
    Singleton,   // 单例，整个容器生命周期内只有一个实例
    Scoped       // 作用域，在作用域内保持单例
}
```

### 使用示例

```csharp
// 瞬态 - 每次解析都创建新实例
builder.Register<TransientService>(Lifetime.Transient);

// 单例 - 整个容器只有一个实例
builder.Register<SingletonService>(Lifetime.Singleton);

// 作用域 - 在作用域内保持单例
builder.Register<ScopedService>(Lifetime.Scoped);
```

---

## Unity 集成 API

### LifetimeScope

Unity 组件，用于在场景中管理容器。

```csharp
using VContainer.Unity;

public class MyLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // 配置注册
        builder.Register<MyService>(Lifetime.Singleton);
        builder.RegisterComponent<MyComponent>();
    }
}
```

#### LifetimeScope 静态方法

```csharp
// 创建 LifetimeScope
var scope = LifetimeScope.Create(builder =>
{
    builder.Register<MyService>(Lifetime.Singleton);
}, "MyScope");

// 使用安装器创建
var scope = LifetimeScope.Create(new MyInstaller(), "MyScope");

// 查找 LifetimeScope
var scope = LifetimeScope.Find<MyLifetimeScope>(scene);
var scope = LifetimeScope.Find<MyLifetimeScope>();

// 创建子作用域
var childScope = parentScope.CreateChild<ChildScope>(builder =>
{
    builder.Register<ChildService>(Lifetime.Scoped);
});

// 从预制体创建子作用域
var childScope = parentScope.CreateChildFromPrefab(prefab, builder =>
{
    builder.Register<ChildService>(Lifetime.Scoped);
});
```

#### LifetimeScope 属性

```csharp
public IObjectResolver Container { get; }  // 容器实例
public LifetimeScope Parent { get; }      // 父作用域
public bool IsRoot { get; }               // 是否为根作用域
```

### 组件注册

#### RegisterComponent

```csharp
// 注册现有组件
builder.RegisterComponent<IMyComponent>(existingComponent);

// 在场景层次结构中查找组件
builder.RegisterComponentInHierarchy<MyComponent>();

// 在新 GameObject 上创建组件
builder.RegisterComponentOnNewGameObject<MyComponent>(
    Lifetime.Singleton, 
    "NewGameObject");

// 从预制体实例化组件
builder.RegisterComponentInNewPrefab(prefab, Lifetime.Singleton);
```

#### ComponentRegistrationBuilder

```csharp
// 设置父对象
builder.RegisterComponentOnNewGameObject<MyComponent>(Lifetime.Singleton)
    .UnderTransform(parentTransform);

// 使用函数查找父对象
builder.RegisterComponentOnNewGameObject<MyComponent>(Lifetime.Singleton)
    .UnderTransform(() => FindObjectOfType<Parent>().transform);

// 使用解析器查找父对象
builder.RegisterComponentOnNewGameObject<MyComponent>(Lifetime.Singleton)
    .UnderTransform(resolver => resolver.Resolve<Parent>().transform);

// 设置 DontDestroyOnLoad
builder.RegisterComponentOnNewGameObject<MyComponent>(Lifetime.Singleton)
    .DontDestroyOnLoad();
```

### UseComponents 方法

```csharp
// 使用组件构建器
builder.UseComponents(components =>
{
    components.AddInstance<IMyComponent>(existingComponent);
    components.AddInHierarchy<MyComponent>();
    components.AddOnNewGameObject<MyComponent>(Lifetime.Singleton, "NewObject");
    components.AddInNewPrefab(prefab, Lifetime.Singleton);
});

// 指定根对象
builder.UseComponents(rootTransform, components =>
{
    components.AddOnNewGameObject<MyComponent>(Lifetime.Singleton)
        .UnderTransform(rootTransform);
});
```

### GameObject 注入

```csharp
// 注入 GameObject 及其所有子对象
container.InjectGameObject(gameObject);
```

### Prefab 实例化

```csharp
// 实例化 Component
var instance = container.Instantiate(prefab);

// 指定位置和旋转
var instance = container.Instantiate(prefab, position, rotation);

// 指定父对象
var instance = container.Instantiate(prefab, parent);

// 实例化 GameObject
var instance = container.Instantiate(gameObjectPrefab, position, rotation);
```

---

## Entry Points API

### IInitializable

```csharp
using VContainer.Unity;

public class MyInitializable : IInitializable
{
    public void Initialize()
    {
        // 初始化逻辑
    }
}

// 注册
builder.RegisterEntryPoint<MyInitializable>(Lifetime.Singleton);
```

### IPostInitializable

```csharp
public class MyPostInitializable : IPostInitializable
{
    public void PostInitialize()
    {
        // 初始化后逻辑
    }
}
```

### IStartable

```csharp
public class MyStartable : IStartable
{
    public void Start()
    {
        // Start 时执行
    }
}
```

### ITickable

```csharp
public class MyTickable : ITickable
{
    public void Tick()
    {
        // 每帧执行（Update）
    }
}
```

### IFixedTickable

```csharp
public class MyFixedTickable : IFixedTickable
{
    public void FixedTick()
    {
        // 每帧执行（FixedUpdate）
    }
}
```

### ILateTickable

```csharp
public class MyLateTickable : ILateTickable
{
    public void LateTick()
    {
        // 每帧执行（LateUpdate）
    }
}
```

### IAsyncStartable

```csharp
#if VCONTAINER_UNITASK_INTEGRATION || UNITY_2021_3_OR_NEWER
public class MyAsyncStartable : IAsyncStartable
{
    public async Awaitable StartAsync(CancellationToken cancellation = default)
    {
        // 异步启动逻辑
        await SomeAsyncOperation();
    }
}
#endif
```

### Post 系列接口

```csharp
public interface IPostStartable { void PostStart(); }
public interface IPostTickable { void PostTick(); }
public interface IPostFixedTickable { void PostFixedTick(); }
public interface IPostLateTickable { void PostLateTick(); }
```

### UseEntryPoints 方法

```csharp
// 使用 Entry Points 构建器
builder.UseEntryPoints(entryPoints =>
{
    entryPoints.Add<MyInitializable>();
    entryPoints.Add<MyStartable>();
    entryPoints.Add<MyTickable>();
});

// 指定生命周期
builder.UseEntryPoints(Lifetime.Scoped, entryPoints =>
{
    entryPoints.Add<MyService>();
});

// 注册异常处理器
builder.UseEntryPoints(entryPoints =>
{
    entryPoints.Add<MyStartable>();
    entryPoints.OnException(ex => Debug.LogException(ex));
});
```

### RegisterEntryPoint 方法

```csharp
// 注册单个 Entry Point
builder.RegisterEntryPoint<MyInitializable>(Lifetime.Singleton);

// 使用工厂注册
builder.RegisterEntryPoint<IMyService>(
    resolver => new MyService(resolver.Resolve<IDependency>()),
    Lifetime.Singleton);
```

---

## 参数注入 API

### WithParameter - 按名称

```csharp
public class MyService
{
    public MyService(string name, int id) { }
}

builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter("name", "MyService")
    .WithParameter("id", 42);
```

### WithParameter - 按类型

```csharp
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter<string>("MyService")
    .WithParameter<int>(42);
```

### WithParameter - 使用工厂

```csharp
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter("name", resolver => resolver.Resolve<IConfig>().ServiceName)
    .WithParameter<string>(resolver => resolver.Resolve<IConfig>().ServiceName);
```

### KeyAttribute

```csharp
public class MyService
{
    [Inject]
    [Key("service1")]
    public IService Service1 { get; set; }
    
    [Inject]
    [Key("service2")]
    public IService Service2 { get; set; }
}

// 注册
builder.Register<ServiceA>(Lifetime.Singleton).Keyed("service1");
builder.Register<ServiceB>(Lifetime.Singleton).Keyed("service2");
```

---

## 诊断 API

### 启用诊断

```csharp
// 在构建器中启用诊断
var builder = new ContainerBuilder
{
    Diagnostics = new DiagnosticsCollector()
};

// 或在 VContainerSettings 中启用
// 通过菜单 Assets/Create/VContainer/VContainer Settings 创建
```

### 诊断窗口

```csharp
// 在 Unity 编辑器中打开诊断窗口
// 菜单：Window/VContainer Diagnostics
```

### DiagnosticsCollector

```csharp
var collector = new DiagnosticsCollector();
builder.Diagnostics = collector;

// 构建容器后，收集器会记录所有注册和解析信息
var container = builder.Build();

// 可以通过诊断窗口查看信息
```

---

## 扩展 API

### IInstaller 接口

```csharp
using VContainer.Unity;

public class MyInstaller : IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.Register<MyService>(Lifetime.Singleton);
        builder.Register<MyRepository>(Lifetime.Singleton);
    }
}

// 使用
builder.RegisterBuildCallback(container =>
{
    var installer = new MyInstaller();
    installer.Install(builder);
});
```

### ActionInstaller

```csharp
var installer = new ActionInstaller(builder =>
{
    builder.Register<MyService>(Lifetime.Singleton);
});

// 在 LifetimeScope 中使用
var scope = LifetimeScope.Create(installer, "MyScope");
```

### RegisterBuildCallback

```csharp
// 注册构建回调
builder.RegisterBuildCallback(container =>
{
    // 容器构建后执行
    var service = container.Resolve<IMyService>();
    service.Initialize();
});
```

### RegisterDisposeCallback

```csharp
// 注册销毁回调
builder.RegisterDisposeCallback(container =>
{
    // 容器销毁时执行
    Debug.Log("Container disposed");
});
```

### Exists 方法

```csharp
// 检查类型是否已注册
if (builder.Exists(typeof(IMyService)))
{
    // 已注册
}

// 检查接口类型
if (builder.Exists(typeof(IMyService), includeInterfaceTypes: true))
{
    // 已注册（包括接口）
}

// 检查父作用域
if (builder.Exists(typeof(IMyService), findParentScopes: true))
{
    // 在当前或父作用域中已注册
}
```

---

## 完整示例

### 示例 1：基础使用

```csharp
using VContainer;
using VContainer.Unity;

// 定义接口和实现
public interface IUserService
{
    void SaveUser(User user);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public void SaveUser(User user)
    {
        _repository.Save(user);
    }
}

// 配置容器
public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IUserRepository, UserRepository>(Lifetime.Singleton);
        builder.Register<IUserService, UserService>(Lifetime.Singleton);
    }
}

// 使用
public class GameController : MonoBehaviour
{
    private IUserService _userService;
    
    [Inject]
    void Construct(IUserService userService)
    {
        _userService = userService;
    }
    
    void Start()
    {
        var user = new User { Name = "Player" };
        _userService.SaveUser(user);
    }
}
```

### 示例 2：使用 Entry Points

```csharp
public class GameInitializer : IInitializable, IStartable, ITickable
{
    private readonly IUserService _userService;
    
    public GameInitializer(IUserService userService)
    {
        _userService = userService;
    }
    
    public void Initialize()
    {
        Debug.Log("Game Initialized");
    }
    
    public void Start()
    {
        Debug.Log("Game Started");
    }
    
    public void Tick()
    {
        // 每帧执行
    }
}

// 注册
public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IUserService, UserService>(Lifetime.Singleton);
        builder.RegisterEntryPoint<GameInitializer>(Lifetime.Singleton);
    }
}
```

### 示例 3：使用作用域

```csharp
public class SceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // 场景级别的服务
        builder.Register<SceneService>(Lifetime.Scoped);
    }
}

// 创建子作用域
var childScope = parentScope.CreateScope(builder =>
{
    builder.Register<ChildService>(Lifetime.Scoped);
});

using (childScope)
{
    var service = childScope.Resolve<ChildService>();
    // 使用服务
}
```

### 示例 4：使用参数注入

```csharp
public class ConfigService
{
    public string ServiceName { get; set; } = "DefaultService";
}

public class MyService
{
    private readonly string _name;
    
    public MyService([Key("serviceName")] string name)
    {
        _name = name;
    }
}

// 注册
builder.Register<ConfigService>(Lifetime.Singleton);
builder.Register<MyService>(Lifetime.Singleton)
    .WithParameter("serviceName", resolver => 
        resolver.Resolve<ConfigService>().ServiceName);
```

### 示例 5：使用工厂

```csharp
public class ServiceFactory
{
    public IMyService CreateService(string name)
    {
        return new MyService(name);
    }
}

// 注册工厂
builder.Register<ServiceFactory>(Lifetime.Singleton);
builder.RegisterFactory<string, IMyService>(
    resolver => name => resolver.Resolve<ServiceFactory>().CreateService(name),
    Lifetime.Singleton);

// 使用
var factory = container.Resolve<Func<string, IMyService>>();
var service = factory("MyService");
```

---

## 注意事项

1. **生命周期选择**：根据对象的用途选择合适的生命周期，避免不必要的单例。
2. **循环依赖**：避免循环依赖，使用事件或回调解决相互依赖。
3. **作用域管理**：合理使用作用域，实现依赖隔离。
4. **性能考虑**：大量使用反射可能影响性能，合理使用缓存。
5. **线程安全**：容器本身是线程安全的，但需要注意注入对象的线程安全。
6. **资源释放**：实现 IDisposable 的对象会在容器销毁时自动释放。

---

## 总结

VContainer 提供了丰富的 API 来满足各种依赖注入需求。通过合理使用这些 API，可以构建清晰、可维护、可测试的 Unity 项目架构。本手册涵盖了所有主要的 API 使用方法，可以作为开发参考。

