# UniTask 异步框架 API 使用手册

## 目录

1. [基础 API](#基础-api)
2. [异步操作 API](#异步操作-api)
3. [任务组合 API](#任务组合-api)
4. [Unity 集成 API](#unity-集成-api)
5. [异步 LINQ API](#异步-linq-api)
6. [响应式编程 API](#响应式编程-api)
7. [通道 API](#通道-api)
8. [基础设施 API](#基础设施-api)
9. [触发器 API](#触发器-api)

---

## 基础 API

### UniTask

核心异步任务类型。

```csharp
using Cysharp.Threading.Tasks;

// 创建已完成的 UniTask
UniTask completedTask = UniTask.CompletedTask;

// 创建带返回值的 UniTask
UniTask<int> taskWithResult = UniTask.FromResult(42);

// 创建取消的 UniTask
UniTask canceledTask = UniTask.FromCanceled(cancellationToken);

// 创建异常的 UniTask
UniTask faultedTask = UniTask.FromException(new Exception("Error"));

// 异步方法
async UniTask DoSomethingAsync()
{
    await UniTask.Delay(1000);
    Debug.Log("Done");
}

// 带返回值的异步方法
async UniTask<int> GetValueAsync()
{
    await UniTask.Delay(1000);
    return 42;
}
```

### UniTaskCompletionSource

手动控制异步操作的完成。

```csharp
// 创建完成源
var completionSource = new UniTaskCompletionSource<int>();

// 设置结果
completionSource.TrySetResult(42);

// 设置异常
completionSource.TrySetException(new Exception("Error"));

// 设置取消
completionSource.TrySetCanceled(cancellationToken);

// 获取 UniTask
UniTask<int> task = completionSource.Task;

// 使用示例
async UniTask<int> LoadDataAsync()
{
    var completionSource = new UniTaskCompletionSource<int>();
    
    // 模拟异步加载
    LoadDataFromServer(result =>
    {
        completionSource.TrySetResult(result);
    });
    
    return await completionSource.Task;
}
```

### UniTaskVoid

用于 fire-and-forget 异步操作。

```csharp
// 使用 UniTaskVoid 执行不需要等待的异步操作
UniTaskVoid FireAndForget()
{
    // 这个方法的异常不会被捕获
    // 适合用于事件处理等场景
}

// 调用
FireAndForget().Forget();
```

---

## 异步操作 API

### Delay - 延迟

```csharp
// 延迟指定时间（毫秒）
await UniTask.Delay(1000);

// 延迟指定时间（TimeSpan）
await UniTask.Delay(TimeSpan.FromSeconds(1));

// 使用取消令牌
await UniTask.Delay(1000, cancellationToken);

// 指定延迟类型
await UniTask.Delay(1000, DelayType.UnscaledDeltaTime);

// 指定 PlayerLoopTiming
await UniTask.Delay(1000, PlayerLoopTiming.FixedUpdate);
```

### Yield - 等待下一帧

```csharp
// 等待下一帧
await UniTask.Yield();

// 指定 PlayerLoopTiming
await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

// 使用取消令牌
await UniTask.Yield(cancellationToken);
```

### NextFrame - 等待下一帧（保证）

```csharp
// 保证在下一帧运行
await UniTask.NextFrame();

// 指定 PlayerLoopTiming
await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate);

// 使用取消令牌
await UniTask.NextFrame(cancellationToken);
```

### WaitUntil - 等待直到条件

```csharp
// 等待直到条件满足
await UniTask.WaitUntil(() => someCondition);

// 指定 PlayerLoopTiming
await UniTask.WaitUntil(() => someCondition, PlayerLoopTiming.FixedUpdate);

// 使用取消令牌
await UniTask.WaitUntil(() => someCondition, cancellationToken: cancellationToken);
```

### WaitWhile - 等待直到条件不满足

```csharp
// 等待直到条件不满足
await UniTask.WaitWhile(() => someCondition);

// 指定 PlayerLoopTiming
await UniTask.WaitWhile(() => someCondition, PlayerLoopTiming.FixedUpdate);
```

### WaitForEndOfFrame - 等待帧结束

```csharp
#if UNITY_2023_1_OR_NEWER
// 等待帧结束
await UniTask.WaitForEndOfFrame(cancellationToken);
#endif
```

### Run - 在线程池运行

```csharp
// 在线程池中运行
await UniTask.Run(() =>
{
    // 在后台线程执行
    HeavyComputation();
});

// 带返回值
int result = await UniTask.Run(() =>
{
    return ComputeValue();
});

// 使用取消令牌
await UniTask.Run(() => HeavyComputation(), cancellationToken);
```

### SwitchToThreadPool / SwitchToMainThread

```csharp
// 切换到线程池
await UniTask.SwitchToThreadPool();

// 切换到主线程
await UniTask.SwitchToMainThread();

// 指定 PlayerLoopTiming
await UniTask.SwitchToMainThread(PlayerLoopTiming.Update);
```

---

## 任务组合 API

### WhenAll - 等待所有任务

```csharp
// 等待多个任务
var task1 = LoadData1Async();
var task2 = LoadData2Async();
var task3 = LoadData3Async();

await UniTask.WhenAll(task1, task2, task3);

// 等待带返回值的任务
var results = await UniTask.WhenAll(
    GetValue1Async(),
    GetValue2Async(),
    GetValue3Async()
);
// results 是数组 [value1, value2, value3]

// 等待集合中的任务
var tasks = new List<UniTask>();
tasks.Add(LoadData1Async());
tasks.Add(LoadData2Async());
await UniTask.WhenAll(tasks);
```

### WhenAny - 等待任意任务

```csharp
// 等待任意任务完成
var (winIndex, result) = await UniTask.WhenAny(
    LoadData1Async(),
    LoadData2Async(),
    LoadData3Async()
);
// winIndex 是完成的任务索引
// result 是完成的任务结果

// 等待集合中的任务
var tasks = new List<UniTask<int>>();
var (index, value) = await UniTask.WhenAny(tasks);
```

### WhenEach - 等待每个任务并处理

```csharp
// 等待每个任务完成并处理结果
await UniTask.WhenEach(
    LoadData1Async(),
    LoadData2Async(),
    LoadData3Async()
).ForEachAsync((result, index) =>
{
    Debug.Log($"Task {index} completed: {result}");
});
```

---

## Unity 集成 API

### AsyncOperation 扩展

```csharp
using Cysharp.Threading.Tasks;

// 等待异步操作
var request = Resources.LoadAsync<GameObject>("Prefab");
await request;

// 带进度报告
var request = Resources.LoadAsync<GameObject>("Prefab");
await request.ToUniTask(Progress.Create<float>(progress =>
{
    Debug.Log($"Loading: {progress * 100}%");
}));

// 带取消令牌
await request.ToUniTask(cancellationToken: cancellationToken);
```

### UnityWebRequest 扩展

```csharp
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

// 发送 GET 请求
var request = UnityWebRequest.Get("https://api.example.com/data");
var response = await request.SendWebRequest();

// 发送 POST 请求
var request = UnityWebRequest.Post("https://api.example.com/data", formData);
var response = await request.SendWebRequest();

// 处理响应
if (response.result == UnityWebRequest.Result.Success)
{
    Debug.Log(response.downloadHandler.text);
}
```

### MonoBehaviour 扩展

```csharp
public class MyComponent : MonoBehaviour
{
    async void Start()
    {
        // 等待指定时间后执行
        await this.GetCancellationTokenOnDestroy().ToUniTask()
            .Delay(1000);
        
        Debug.Log("1 second passed");
    }
    
    async UniTask LoadDataAsync()
    {
        // 使用组件的取消令牌
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        
        await UniTask.Delay(1000, cancellationToken: cancellationToken);
    }
}
```

### uGUI 扩展

```csharp
using Cysharp.Threading.Tasks;

// 等待按钮点击
await button.OnClickAsync();

// 等待 Toggle 状态变化
bool isOn = await toggle.OnValueChangedAsync();

// 等待 InputField 文本输入
string text = await inputField.OnEndEditAsync();

// 等待 Slider 值变化
float value = await slider.OnValueChangedAsync();
```

### 异步实例化

```csharp
// 异步实例化预制体
GameObject instance = await InstantiateAsync(prefab);

// 指定位置和旋转
GameObject instance = await InstantiateAsync(
    prefab, 
    position, 
    rotation
);

// 指定父对象
GameObject instance = await InstantiateAsync(
    prefab, 
    parent
);
```

---

## 异步 LINQ API

### 基本操作

```csharp
using Cysharp.Threading.Tasks.Linq;

// Where - 筛选
await Enumerable.Range(1, 10)
    .ToUniTaskAsyncEnumerable()
    .Where(x => x % 2 == 0)
    .ForEachAsync(x => Debug.Log(x));

// Select - 投影
var squares = await Enumerable.Range(1, 10)
    .ToUniTaskAsyncEnumerable()
    .Select(x => x * x)
    .ToArrayAsync();

// Take - 取前 N 个
var first5 = await numbers
    .Take(5)
    .ToArrayAsync();

// Skip - 跳过前 N 个
var after5 = await numbers
    .Skip(5)
    .ToArrayAsync();
```

### 聚合操作

```csharp
// Sum - 求和
int sum = await numbers.SumAsync();

// Average - 平均值
double avg = await numbers.AverageAsync();

// Min/Max - 最小值/最大值
int min = await numbers.MinAsync();
int max = await numbers.MaxAsync();

// Count - 计数
int count = await numbers.CountAsync();

// Aggregate - 聚合
int product = await numbers.AggregateAsync(1, (acc, x) => acc * x);
```

### 查询操作

```csharp
// First - 第一个
int first = await numbers.FirstAsync();

// FirstOrDefault - 第一个或默认值
int first = await numbers.FirstOrDefaultAsync();

// Last - 最后一个
int last = await numbers.LastAsync();

// Any - 是否存在
bool hasEven = await numbers.AnyAsync(x => x % 2 == 0);

// All - 是否全部满足
bool allPositive = await numbers.AllAsync(x => x > 0);

// Contains - 是否包含
bool contains = await numbers.ContainsAsync(5);
```

### Unity 特定操作

```csharp
// EveryUpdate - 每帧更新
await UniTaskAsyncEnumerable.EveryUpdate()
    .Take(100)
    .ForEachAsync(_ =>
    {
        // 每帧执行，共 100 帧
    });

// EveryValueChanged - 值变化监听
await transform.position
    .ToUniTaskAsyncEnumerable()
    .EveryValueChanged(x => x)
    .ForEachAsync(position =>
    {
        Debug.Log($"Position changed: {position}");
    });

// Timer - 定时器
await UniTaskAsyncEnumerable.Timer(
    TimeSpan.FromSeconds(1),
    TimeSpan.FromSeconds(0.5)
)
.Take(10)
.ForEachAsync(_ =>
{
    // 每 0.5 秒执行一次，共 10 次
});
```

---

## 响应式编程 API

### AsyncReactiveProperty

```csharp
using Cysharp.Threading.Tasks;

// 创建响应式属性
var property = new AsyncReactiveProperty<int>(0);

// 设置值（会通知所有订阅者）
property.Value = 42;

// 订阅值变化
await foreach (var value in property)
{
    Debug.Log($"Value changed: {value}");
}

// 等待下一个值
int nextValue = await property.WaitAsync(cancellationToken);

// 不包含当前值的序列
await foreach (var value in property.WithoutCurrent())
{
    // 只会在值变化时产生新值
}
```

### 只读响应式属性

```csharp
// 创建只读响应式属性
IReadOnlyAsyncReactiveProperty<int> readOnlyProperty = property;

// 只能读取，不能设置
int value = readOnlyProperty.Value;
```

---

## 通道 API

### Channel - 生产者-消费者

```csharp
using Cysharp.Threading.Tasks;

// 创建通道
var channel = Channel.CreateSingleConsumerUnbounded<int>();

// 生产者
async UniTaskVoid Producer()
{
    for (int i = 0; i < 10; i++)
    {
        channel.Writer.TryWrite(i);
        await UniTask.Delay(100);
    }
    channel.Writer.Complete();
}

// 消费者
async UniTaskVoid Consumer()
{
    await foreach (var value in channel.Reader.ReadAllAsync())
    {
        Debug.Log($"Received: {value}");
    }
}

// 启动
Producer().Forget();
Consumer().Forget();
```

### 通道读写

```csharp
// 写入
channel.Writer.TryWrite(42);

// 完成写入
channel.Writer.Complete();

// 读取
if (channel.Reader.TryRead(out var value))
{
    Debug.Log($"Read: {value}");
}

// 异步读取
int value = await channel.Reader.ReadAsync();

// 等待可读
bool canRead = await channel.Reader.WaitToReadAsync();
```

---

## 基础设施 API

### AsyncLazy - 异步延迟初始化

```csharp
// 创建异步延迟初始化
var lazy = new AsyncLazy<int>(async () =>
{
    await UniTask.Delay(1000);
    return LoadExpensiveData();
});

// 获取值（首次调用时初始化）
int value = await lazy.Task;

// 后续调用直接返回缓存的值
int value2 = await lazy.Task;
```

### Progress - 进度报告

```csharp
// 创建进度报告器
var progress = Progress.Create<float>(value =>
{
    Debug.Log($"Progress: {value * 100}%");
});

// 在异步操作中使用
await LoadDataAsync(progress);

// 仅值变化时报告
var progress = Progress.CreateOnlyValueChanged<float>(value =>
{
    Debug.Log($"Progress changed: {value * 100}%");
});
```

### TimeoutController - 超时控制

```csharp
// 创建超时控制器
var timeoutController = new TimeoutController();

// 设置超时
var timeoutToken = timeoutController.Timeout(TimeSpan.FromSeconds(5));

// 在异步操作中使用
try
{
    await SomeAsyncOperation(timeoutToken);
}
catch (OperationCanceledException)
{
    if (timeoutController.IsTimeout())
    {
        Debug.Log("Operation timed out");
    }
}

// 重置超时控制器
timeoutController.Reset();
```

### PlayerLoopHelper

```csharp
// 添加 PlayerLoop 项
PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, () =>
{
    // 在 Update 阶段执行
});

// 移除 PlayerLoop 项
var action = new Action(() => { });
PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, action);
PlayerLoopHelper.RemoveAction(action);
```

---

## 触发器 API

### 生命周期触发器

```csharp
using Cysharp.Threading.Tasks.Triggers;

public class MyComponent : MonoBehaviour
{
    async void Start()
    {
        // 等待 Awake
        await this.GetAsyncAwakeTrigger().OnAwakeAsync();
        
        // 等待 Destroy
        await this.GetAsyncDestroyTrigger().OnDestroyAsync();
    }
}
```

### 更新触发器

```csharp
// 等待 Update
await foreach (var _ in this.GetAsyncUpdateTrigger())
{
    // 每帧执行
}

// 等待 FixedUpdate
await foreach (var _ in this.GetAsyncFixedUpdateTrigger())
{
    // 每个固定更新执行
}
```

### 物理触发器

```csharp
// 等待碰撞进入
await foreach (var collision in this.GetAsyncCollisionEnterTrigger())
{
    Debug.Log($"Collision: {collision.collider.name}");
}

// 等待触发器进入
await foreach (var other in this.GetAsyncTriggerEnterTrigger())
{
    Debug.Log($"Trigger: {other.name}");
}
```

### uGUI 触发器

```csharp
// 等待指针进入
await foreach (var eventData in this.GetAsyncPointerEnterTrigger())
{
    Debug.Log("Pointer entered");
}

// 等待拖拽
await foreach (var eventData in this.GetAsyncDragTrigger())
{
    Debug.Log("Dragging");
}
```

---

## 完整示例

### 示例 1：资源加载

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    async UniTaskVoid Start()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        
        try
        {
            // 加载资源
            var request = Resources.LoadAsync<GameObject>("MyPrefab");
            await request.ToUniTask(
                Progress.Create<float>(progress =>
                {
                    Debug.Log($"Loading: {progress * 100}%");
                }),
                cancellationToken: cancellationToken
            );
            
            var prefab = request.asset as GameObject;
            Instantiate(prefab);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Loading cancelled");
        }
    }
}
```

### 示例 2：网络请求

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class ApiClient
{
    async UniTask<string> GetDataAsync(string url, CancellationToken cancellationToken = default)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                throw new Exception($"Request failed: {request.error}");
            }
        }
    }
}
```

### 示例 3：任务组合

```csharp
async UniTask LoadAllDataAsync()
{
    // 并行加载多个资源
    var (data1, data2, data3) = await UniTask.WhenAll(
        LoadData1Async(),
        LoadData2Async(),
        LoadData3Async()
    );
    
    // 处理结果
    ProcessData(data1, data2, data3);
}
```

### 示例 4：异步 LINQ

```csharp
async UniTask ProcessDataAsync()
{
    // 从网络获取数据流
    var dataStream = GetDataStreamAsync();
    
    // 处理数据流
    var results = await dataStream
        .Where(x => x.IsValid)
        .Select(x => x.Process())
        .Take(100)
        .ToArrayAsync();
    
    // 使用结果
    foreach (var result in results)
    {
        Debug.Log(result);
    }
}
```

### 示例 5：响应式属性

```csharp
public class GameManager : MonoBehaviour
{
    private AsyncReactiveProperty<int> score = new AsyncReactiveProperty<int>(0);
    
    async void Start()
    {
        // 订阅分数变化
        score.Subscribe(value =>
        {
            Debug.Log($"Score: {value}");
        });
        
        // 或者使用异步枚举
        await foreach (var value in score)
        {
            UpdateUI(value);
        }
    }
    
    void AddScore(int points)
    {
        score.Value += points;  // 自动通知订阅者
    }
}
```

### 示例 6：通道通信

```csharp
public class MessageSystem
{
    private Channel<string> messageChannel = Channel.CreateSingleConsumerUnbounded<string>();
    
    async UniTaskVoid Start()
    {
        // 启动消息处理
        ProcessMessages().Forget();
    }
    
    // 发送消息
    public void SendMessage(string message)
    {
        messageChannel.Writer.TryWrite(message);
    }
    
    // 处理消息
    async UniTaskVoid ProcessMessages()
    {
        await foreach (var message in messageChannel.Reader.ReadAllAsync())
        {
            Debug.Log($"Received: {message}");
        }
    }
}
```

---

## 注意事项

1. **取消令牌**：为异步操作提供 CancellationToken，支持取消操作。
2. **PlayerLoopTiming**：根据需求选择合适的 PlayerLoopTiming。
3. **异常处理**：使用 try-catch 处理异步操作中的异常。
4. **资源释放**：使用 using 语句或 Dispose 方法释放资源。
5. **Unity 生命周期**：使用 `GetCancellationTokenOnDestroy()` 获取组件的取消令牌。
6. **性能考虑**：避免在热路径中创建大量 UniTask，考虑使用对象池。

---

## 总结

UniTask 提供了丰富的 API 来满足各种异步编程需求。通过合理使用这些 API，可以构建高性能、易维护的 Unity 异步应用。本手册涵盖了所有主要的 API 使用方法，可以作为开发参考。

