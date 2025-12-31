# R3 响应式扩展框架 API 使用手册

## 目录

1. [基础 API](#基础-api)
2. [Observable 创建 API](#observable-创建-api)
3. [操作符 API](#操作符-api)
4. [Unity 集成 API](#unity-集成-api)
5. [响应式属性 API](#响应式属性-api)
6. [触发器 API](#触发器-api)
7. [时间提供者 API](#时间提供者-api)
8. [资源管理 API](#资源管理-api)

---

## 基础 API

### Observable

核心接口，表示一个可观察的事件序列。

```csharp
using R3;

// 创建空的 Observable
var empty = Observable.Empty<int>();

// 创建返回单个值的 Observable
var single = Observable.Return(42);

// 创建抛出异常的 Observable
var error = Observable.Throw<int>(new Exception("Error"));

// 创建永不完成的 Observable
var never = Observable.Never<int>();

// 订阅 Observable
var disposable = Observable.Return(42)
    .Subscribe(
        value => Debug.Log($"Value: {value}"),
        error => Debug.LogError($"Error: {error}"),
        () => Debug.Log("Completed")
    );

// 取消订阅
disposable.Dispose();
```

### Subject

Subject 是一个既是 Observable 又是 Observer 的类型。

```csharp
using R3;

// 创建 Subject
var subject = new Subject<int>();

// 订阅
var subscription = subject.Subscribe(value => Debug.Log($"Received: {value}"));

// 发送值
subject.OnNext(1);
subject.OnNext(2);
subject.OnNext(3);

// 完成
subject.OnCompleted();

// 取消订阅
subscription.Dispose();
```

### Unit

Unit 类型表示没有值的事件。

```csharp
using R3;

// 使用 Unit 表示没有值的事件
var unitObservable = Observable.Return(Unit.Default);
```

---

## Observable 创建 API

### FromEvent

从事件创建 Observable。

```csharp
using R3;
using System;

// 从 .NET 事件创建
public class EventSource
{
    public event Action<int> ValueChanged;
    
    public void Trigger(int value)
    {
        ValueChanged?.Invoke(value);
    }
}

var source = new EventSource();
var observable = Observable.FromEvent<int>(
    h => source.ValueChanged += h,
    h => source.ValueChanged -= h
);

observable.Subscribe(value => Debug.Log($"Value changed: {value}"));
source.Trigger(42);
```

### Create

自定义创建 Observable。

```csharp
using R3;

var observable = Observable.Create<int>(observer =>
{
    observer.OnNext(1);
    observer.OnNext(2);
    observer.OnNext(3);
    observer.OnCompleted();
    return Disposable.Empty;
});

observable.Subscribe(value => Debug.Log($"Value: {value}"));
```

### Range

创建范围序列。

```csharp
using R3;

// 创建从 1 到 10 的序列
var range = Observable.Range(1, 10);
range.Subscribe(value => Debug.Log($"Value: {value}"));
```

### Interval

创建定时序列。

```csharp
using R3;
using System;

// 每 1 秒发送一个值
var interval = Observable.Interval(TimeSpan.FromSeconds(1));
interval.Subscribe(value => Debug.Log($"Tick: {value}"));
```

### Timer

创建定时器序列。

```csharp
using R3;
using System;

// 延迟 1 秒后发送值
var timer = Observable.Timer(TimeSpan.FromSeconds(1));
timer.Subscribe(_ => Debug.Log("Timer fired"));

// 延迟 1 秒后，每 0.5 秒发送一个值
var periodic = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.5));
periodic.Subscribe(value => Debug.Log($"Tick: {value}"));
```

---

## 操作符 API

### 筛选操作符

#### Where

筛选满足条件的值。

```csharp
using R3;

var numbers = Observable.Range(1, 10);
var evens = numbers.Where(x => x % 2 == 0);
evens.Subscribe(value => Debug.Log($"Even: {value}"));
```

#### Distinct

去除重复值。

```csharp
using R3;

var values = Observable.Return(1)
    .Concat(Observable.Return(2))
    .Concat(Observable.Return(1))
    .Concat(Observable.Return(3));

var distinct = values.Distinct();
distinct.Subscribe(value => Debug.Log($"Distinct: {value}"));
```

#### Take

取前 N 个值。

```csharp
using R3;

var numbers = Observable.Range(1, 100);
var first5 = numbers.Take(5);
first5.Subscribe(value => Debug.Log($"Value: {value}"));
```

#### Skip

跳过前 N 个值。

```csharp
using R3;

var numbers = Observable.Range(1, 10);
var after5 = numbers.Skip(5);
after5.Subscribe(value => Debug.Log($"Value: {value}"));
```

### 转换操作符

#### Select

转换值。

```csharp
using R3;

var numbers = Observable.Range(1, 10);
var squares = numbers.Select(x => x * x);
squares.Subscribe(value => Debug.Log($"Square: {value}"));
```

#### SelectMany

扁平化转换。

```csharp
using R3;

var numbers = Observable.Range(1, 3);
var expanded = numbers.SelectMany(x => Observable.Range(1, x));
expanded.Subscribe(value => Debug.Log($"Value: {value}"));
```

#### Scan

累积转换。

```csharp
using R3;

var numbers = Observable.Range(1, 10);
var sum = numbers.Scan(0, (acc, x) => acc + x);
sum.Subscribe(value => Debug.Log($"Sum: {value}"));
```

### 组合操作符

#### Merge

合并多个 Observable。

```csharp
using R3;

var source1 = Observable.Range(1, 3);
var source2 = Observable.Range(4, 3);
var merged = source1.Merge(source2);
merged.Subscribe(value => Debug.Log($"Value: {value}"));
```

#### Concat

连接多个 Observable。

```csharp
using R3;

var source1 = Observable.Range(1, 3);
var source2 = Observable.Range(4, 3);
var concatenated = source1.Concat(source2);
concatenated.Subscribe(value => Debug.Log($"Value: {value}"));
```

#### Zip

压缩多个 Observable。

```csharp
using R3;

var source1 = Observable.Range(1, 3);
var source2 = Observable.Range(10, 3);
var zipped = source1.Zip(source2, (a, b) => a + b);
zipped.Subscribe(value => Debug.Log($"Sum: {value}"));
```

#### CombineLatest

组合最新的值。

```csharp
using R3;

var source1 = Observable.Range(1, 3);
var source2 = Observable.Range(10, 3);
var combined = source1.CombineLatest(source2, (a, b) => a + b);
combined.Subscribe(value => Debug.Log($"Sum: {value}"));
```

### 时间操作符

#### Throttle

节流，在指定时间内只取最后一个值。

```csharp
using R3;
using System;

var source = Observable.Interval(TimeSpan.FromMilliseconds(100));
var throttled = source.Throttle(TimeSpan.FromSeconds(1));
throttled.Subscribe(value => Debug.Log($"Throttled: {value}"));
```

#### Debounce

防抖，在指定时间内没有新值时才发送。

```csharp
using R3;
using System;

var source = Observable.Interval(TimeSpan.FromMilliseconds(100));
var debounced = source.Debounce(TimeSpan.FromSeconds(1));
debounced.Subscribe(value => Debug.Log($"Debounced: {value}"));
```

#### Delay

延迟发送值。

```csharp
using R3;
using System;

var source = Observable.Return(42);
var delayed = source.Delay(TimeSpan.FromSeconds(1));
delayed.Subscribe(value => Debug.Log($"Delayed: {value}"));
```

---

## Unity 集成 API

### Unity 生命周期触发器

#### OnDestroy

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅销毁事件
        this.OnDestroyAsObservable()
            .Subscribe(_ => Debug.Log("Object destroyed"));
    }
}
```

#### OnEnable/OnDisable

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅启用事件
        this.OnEnableAsObservable()
            .Subscribe(_ => Debug.Log("Object enabled"));
        
        // 订阅禁用事件
        this.OnDisableAsObservable()
            .Subscribe(_ => Debug.Log("Object disabled"));
    }
}
```

#### Update

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅 Update 事件
        this.UpdateAsObservable()
            .Subscribe(_ => Debug.Log("Update"));
    }
}
```

#### FixedUpdate

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅 FixedUpdate 事件
        this.FixedUpdateAsObservable()
            .Subscribe(_ => Debug.Log("FixedUpdate"));
    }
}
```

### 物理系统触发器

#### 碰撞检测

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅碰撞进入事件
        this.OnCollisionEnterAsObservable()
            .Subscribe(collision => 
            {
                Debug.Log($"Collision with: {collision.gameObject.name}");
            });
        
        // 订阅碰撞退出事件
        this.OnCollisionExitAsObservable()
            .Subscribe(collision => 
            {
                Debug.Log($"Collision exit: {collision.gameObject.name}");
            });
    }
}
```

#### 触发器检测

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅触发器进入事件
        this.OnTriggerEnterAsObservable()
            .Subscribe(collider => 
            {
                Debug.Log($"Trigger enter: {collider.name}");
            });
    }
}
```

### uGUI 集成

#### Button 点击

```csharp
using R3;
using UnityEngine;
using UnityEngine.UI;

public class MyComponent : MonoBehaviour
{
    public Button button;
    
    void Start()
    {
        // 订阅按钮点击事件
        button.OnClickAsObservable()
            .Subscribe(_ => Debug.Log("Button clicked"));
    }
}
```

#### Toggle 状态变化

```csharp
using R3;
using UnityEngine;
using UnityEngine.UI;

public class MyComponent : MonoBehaviour
{
    public Toggle toggle;
    
    void Start()
    {
        // 订阅 Toggle 状态变化
        toggle.OnValueChangedAsObservable()
            .Subscribe(isOn => Debug.Log($"Toggle: {isOn}"));
    }
}
```

#### InputField 文本输入

```csharp
using R3;
using UnityEngine;
using UnityEngine.UI;

public class MyComponent : MonoBehaviour
{
    public InputField inputField;
    
    void Start()
    {
        // 订阅文本变化
        inputField.OnValueChangedAsObservable()
            .Subscribe(text => Debug.Log($"Text: {text}"));
        
        // 订阅文本提交
        inputField.OnEndEditAsObservable()
            .Subscribe(text => Debug.Log($"Submitted: {text}"));
    }
}
```

#### Slider 值变化

```csharp
using R3;
using UnityEngine;
using UnityEngine.UI;

public class MyComponent : MonoBehaviour
{
    public Slider slider;
    
    void Start()
    {
        // 订阅 Slider 值变化
        slider.OnValueChangedAsObservable()
            .Subscribe(value => Debug.Log($"Slider value: {value}"));
    }
}
```

### UnityEvent 集成

```csharp
using R3;
using UnityEngine;
using UnityEngine.Events;

public class MyComponent : MonoBehaviour
{
    public UnityEvent<int> onValueChanged;
    
    void Start()
    {
        // 将 UnityEvent 转换为 Observable
        onValueChanged.AsObservable()
            .Subscribe(value => Debug.Log($"Value changed: {value}"));
    }
}
```

---

## 响应式属性 API

### ReactiveProperty

```csharp
using R3;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    private ReactiveProperty<int> health = new ReactiveProperty<int>(100);
    
    void Start()
    {
        // 订阅值变化
        health.Subscribe(value => Debug.Log($"Health: {value}"));
        
        // 修改值（会自动通知订阅者）
        health.Value = 80;
    }
}
```

### SerializableReactiveProperty

```csharp
using R3;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    // 可以在 Unity 编辑器中序列化
    [SerializeField]
    private SerializableReactiveProperty<int> score = new SerializableReactiveProperty<int>(0);
    
    void Start()
    {
        // 订阅值变化
        score.Subscribe(value => Debug.Log($"Score: {value}"));
        
        // 修改值
        score.Value = 100;
    }
}
```

### 响应式属性操作

```csharp
using R3;

var property = new ReactiveProperty<int>(0);

// 订阅值变化
var subscription = property.Subscribe(value => Debug.Log($"Value: {value}"));

// 修改值
property.Value = 42;

// 只读访问
IReadOnlyReactiveProperty<int> readOnly = property;

// 获取当前值
int currentValue = property.Value;
```

---

## 触发器 API

### 动画触发器

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅 Animator IK 事件
        this.OnAnimatorIKAsObservable()
            .Subscribe(layerIndex => Debug.Log($"IK: {layerIndex}"));
        
        // 订阅 Animator Move 事件
        this.OnAnimatorMoveAsObservable()
            .Subscribe(_ => Debug.Log("Animator Move"));
    }
}
```

### 粒子系统触发器

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅粒子系统事件
        this.OnParticleSystemStoppedAsObservable()
            .Subscribe(_ => Debug.Log("Particles stopped"));
    }
}
```

### 可见性触发器

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅可见性变化
        this.OnBecameVisibleAsObservable()
            .Subscribe(_ => Debug.Log("Became visible"));
        
        this.OnBecameInvisibleAsObservable()
            .Subscribe(_ => Debug.Log("Became invisible"));
    }
}
```

---

## 时间提供者 API

### UnityTimeProvider

```csharp
using R3;
using System;

// 使用 Unity 时间提供者
var timer = Observable.Timer(
    TimeSpan.FromSeconds(1),
    UnityTimeProvider.Update
);

// 使用忽略时间缩放的时间提供者
var unscaledTimer = Observable.Timer(
    TimeSpan.FromSeconds(1),
    UnityTimeProvider.UpdateIgnoreTimeScale
);

// 使用真实时间提供者
var realtimeTimer = Observable.Timer(
    TimeSpan.FromSeconds(1),
    UnityTimeProvider.UpdateRealtime
);
```

### UnityFrameProvider

```csharp
using R3;

// 在特定帧执行操作
var observable = Observable.Return(42)
    .ObserveOn(UnityFrameProvider.Update);

// 在 FixedUpdate 执行
var fixedObservable = Observable.Return(42)
    .ObserveOn(UnityFrameProvider.FixedUpdate);
```

---

## 资源管理 API

### AddTo

将订阅绑定到 GameObject 的生命周期。

```csharp
using R3;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        // 订阅会自动在 GameObject 销毁时取消
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(value => Debug.Log($"Tick: {value}"))
            .AddTo(this.gameObject);
    }
}
```

### GetDestroyCancellationToken

获取 GameObject 的销毁取消令牌。

```csharp
using R3;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    void Start()
    {
        var cancellationToken = this.GetDestroyCancellationToken();
        
        // 使用取消令牌
        Observable.Interval(TimeSpan.FromSeconds(1))
            .TakeUntilCanceled(cancellationToken)
            .Subscribe(value => Debug.Log($"Tick: {value}"));
    }
}
```

### DisposableBag

管理多个订阅。

```csharp
using R3;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    private DisposableBag disposables = new DisposableBag();
    
    void Start()
    {
        // 添加订阅到 DisposableBag
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(value => Debug.Log($"Tick: {value}"))
            .AddTo(disposables);
        
        Observable.Return(42)
            .Subscribe(value => Debug.Log($"Value: {value}"))
            .AddTo(disposables);
    }
    
    void OnDestroy()
    {
        // 清理所有订阅
        disposables.Dispose();
    }
}
```

---

## 完整示例

### 示例 1：UI 数据绑定

```csharp
using R3;
using UnityEngine;
using UnityEngine.UI;

public class UIBindingExample : MonoBehaviour
{
    public Text healthText;
    public Slider healthSlider;
    
    private ReactiveProperty<int> health = new ReactiveProperty<int>(100);
    
    void Start()
    {
        // 绑定到 Text
        health.SubscribeToText(healthText);
        
        // 绑定到 Slider
        health.Subscribe(value => healthSlider.value = value);
        
        // Slider 值变化时更新 health
        healthSlider.OnValueChangedAsObservable()
            .Subscribe(value => health.Value = (int)value)
            .AddTo(this.gameObject);
    }
}
```

### 示例 2：事件处理

```csharp
using R3;
using R3.Triggers;
using UnityEngine;

public class EventHandlerExample : MonoBehaviour
{
    void Start()
    {
        // 处理碰撞事件
        this.OnCollisionEnterAsObservable()
            .Where(collision => collision.gameObject.CompareTag("Enemy"))
            .Subscribe(collision => 
            {
                Debug.Log("Hit enemy!");
                TakeDamage(10);
            })
            .AddTo(this.gameObject);
    }
    
    void TakeDamage(int damage)
    {
        // 处理伤害逻辑
    }
}
```

### 示例 3：响应式状态机

```csharp
using R3;
using UnityEngine;

public class StateMachineExample : MonoBehaviour
{
    private ReactiveProperty<GameState> currentState = 
        new ReactiveProperty<GameState>(GameState.Menu);
    
    void Start()
    {
        // 根据状态执行不同逻辑
        currentState
            .DistinctUntilChanged()
            .Subscribe(state => 
            {
                switch (state)
                {
                    case GameState.Menu:
                        ShowMenu();
                        break;
                    case GameState.Playing:
                        StartGame();
                        break;
                    case GameState.Paused:
                        PauseGame();
                        break;
                }
            })
            .AddTo(this.gameObject);
    }
    
    public void ChangeState(GameState newState)
    {
        currentState.Value = newState;
    }
}

enum GameState
{
    Menu,
    Playing,
    Paused
}
```

### 示例 4：组合操作

```csharp
using R3;
using UnityEngine;
using UnityEngine.UI;

public class CombinedExample : MonoBehaviour
{
    public Button button;
    public InputField inputField;
    
    void Start()
    {
        // 组合按钮点击和输入框文本
        button.OnClickAsObservable()
            .WithLatestFrom(inputField.OnValueChangedAsObservable(), (_, text) => text)
            .Where(text => !string.IsNullOrEmpty(text))
            .Subscribe(text => 
            {
                Debug.Log($"Submitted: {text}");
                ProcessInput(text);
            })
            .AddTo(this.gameObject);
    }
    
    void ProcessInput(string text)
    {
        // 处理输入
    }
}
```

---

## 注意事项

1. **资源管理**：使用 AddTo 或 DisposableBag 管理订阅，避免内存泄漏。
2. **取消令牌**：为长时间运行的 Observable 提供 CancellationToken。
3. **异常处理**：使用 Catch 或 OnErrorResumeNext 处理异常。
4. **性能考虑**：避免在热路径中创建大量 Observable，考虑使用对象池。
5. **线程安全**：Observable 序列默认在主线程执行，使用 ObserveOn 切换到其他线程。
6. **Unity 生命周期**：使用 AddTo 将订阅绑定到 GameObject 的生命周期。

---

## 总结

R3 提供了丰富的 API 来满足各种响应式编程需求。通过合理使用这些 API，可以构建高性能、易维护的 Unity 响应式应用。本手册涵盖了所有主要的 API 使用方法，可以作为开发参考。

