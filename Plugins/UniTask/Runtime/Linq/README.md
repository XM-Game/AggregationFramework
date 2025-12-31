# UniTask LINQ 功能自述

本文档描述了 UniTask 异步 LINQ 模块的功能和作用。该模块提供了完整的异步 LINQ 操作符集合，允许以声明式方式处理异步数据流，同时与 Unity 引擎深度集成。

## 目录

- [概述](#概述)
- [创建操作符](#创建操作符)
- [投影和过滤操作符](#投影和过滤操作符)
- [排序和分组操作符](#排序和分组操作符)
- [聚合操作符](#聚合操作符)
- [转换操作符](#转换操作符)
- [元素操作符](#元素操作符)
- [集合操作符](#集合操作符)
- [连接操作符](#连接操作符)
- [时间相关操作符](#时间相关操作符)
- [条件操作符](#条件操作符)
- [缓冲和窗口操作符](#缓冲和窗口操作符)
- [订阅和观察者模式](#订阅和观察者模式)
- [Unity 专用扩展](#unity-专用扩展)
- [类型转换和兼容性](#类型转换和兼容性)

---

## 概述

UniTask LINQ 模块实现了完整的异步 LINQ（Language Integrated Query）功能，提供了与标准 LINQ 类似的 API，但专门为异步场景设计。所有操作符都返回 `IUniTaskAsyncEnumerable<T>` 接口，支持延迟执行和流式处理。

**核心特性：**
- 完整的 LINQ 操作符集合，支持异步操作
- 与 Unity PlayerLoop 深度集成
- 零分配优化，减少 GC 压力
- 支持取消令牌（CancellationToken）
- 提供 Await 和 AwaitWithCancellation 变体，支持异步谓词和选择器

---

## 创建操作符

### Return（返回单个值）

创建一个只包含单个值的异步序列。用于将单个值包装为异步序列。

**使用场景：**
- 将单个值转换为异步序列
- 在异步管道中插入固定值

### Range（范围序列）

生成指定范围内的整数序列。支持起始值和数量参数。

**特性：**
- 延迟执行，按需生成元素
- 支持大范围值（使用 long 计算，避免溢出）

### Repeat（重复值）

创建一个包含指定元素重复指定次数的序列。

**特性：**
- 支持重复任意类型元素
- 可指定重复次数

### Empty（空序列）

创建一个不包含任何元素的空序列。使用单例模式，避免重复分配。

### Never（永不完成）

创建一个永远不会产生值或完成的序列。只有在取消令牌被触发时才会结束。

**使用场景：**
- 等待取消操作
- 创建无限等待的序列

### Throw（抛出异常）

创建一个在枚举时立即抛出指定异常的序列。

**使用场景：**
- 错误处理测试
- 在异步管道中传播错误

### Create（自定义创建）

通过提供自定义的创建函数来生成异步序列。使用 IAsyncWriter 接口来产生值。

**特性：**
- 完全控制序列的生成过程
- 支持异步值产生
- 支持取消操作

---

## 投影和过滤操作符

### Select（投影）

将序列中的每个元素投影到新形式。支持同步和异步选择器。

**变体：**
- Select：同步选择器
- SelectAwait：异步选择器（返回 UniTask）
- SelectAwaitWithCancellation：支持取消的异步选择器
- 所有变体都支持带索引的选择器

**特性：**
- 延迟执行
- 支持索引参数（从 0 开始）

### SelectMany（扁平化投影）

将序列的每个元素投影到序列，然后将结果序列合并为一个序列。

**特性：**
- 支持一对多投影
- 支持异步集合选择器
- 支持结果选择器，将源元素和集合元素组合

### Where（过滤）

基于谓词函数过滤序列中的元素。

**变体：**
- Where：同步谓词
- WhereAwait：异步谓词（返回 UniTask<bool>）
- WhereAwaitWithCancellation：支持取消的异步谓词
- 所有变体都支持带索引的谓词

**特性：**
- 延迟执行，只处理需要的元素
- 支持索引参数

### OfType（类型过滤）

根据指定类型过滤序列中的元素。

### Cast（类型转换）

将序列中的元素转换为指定类型。

---

## 排序和分组操作符

### OrderBy / OrderByDescending（排序）

根据键选择器对序列进行升序或降序排序。

**变体：**
- OrderBy：同步键选择器
- OrderByAwait：异步键选择器
- OrderByAwaitWithCancellation：支持取消的异步键选择器
- 支持自定义比较器

**特性：**
- 返回 IUniTaskOrderedAsyncEnumerable，支持 ThenBy 链式排序
- 延迟执行，但需要缓冲所有元素进行排序

### ThenBy / ThenByDescending（次要排序）

在已排序的序列上执行次要排序。

### GroupBy（分组）

根据键选择器对序列中的元素进行分组。

**变体：**
- 支持元素选择器，转换分组内的元素
- 支持结果选择器，自定义分组结果
- 支持自定义相等比较器
- 提供 Await 和 AwaitWithCancellation 变体

**特性：**
- 返回 IGrouping<TKey, TElement> 序列
- 延迟执行，但需要缓冲所有元素进行分组

---

## 聚合操作符

### Count / LongCount（计数）

返回序列中元素的数量。

**变体：**
- CountAsync：返回 int
- LongCountAsync：返回 long（用于大序列）
- 支持带谓词的计数（条件计数）
- 提供 Await 和 AwaitWithCancellation 变体

### Sum（求和）

计算数值序列的总和。

**特性：**
- 支持所有数值类型（int, long, float, double, decimal 等）
- 支持可空类型
- 提供 Await 和 AwaitWithCancellation 变体，支持异步选择器

### Min / Max（最小值/最大值）

返回序列中的最小或最大元素。

**特性：**
- 支持所有可比较类型
- 支持键选择器（根据键比较）
- 支持可空类型
- 提供 Await 和 AwaitWithCancellation 变体

### Average（平均值）

计算数值序列的平均值。

**特性：**
- 支持所有数值类型
- 支持可空类型
- 提供 Await 和 AwaitWithCancellation 变体

### Aggregate（聚合）

对序列应用累加器函数。这是最灵活的聚合操作，可以实现任意聚合逻辑。

**变体：**
- 无种子值：使用第一个元素作为初始值
- 有种子值：指定初始累加值
- 带结果选择器：在累加后转换结果
- 提供 Await 和 AwaitWithCancellation 变体

---

## 转换操作符

### ToArray（转换为数组）

将异步序列转换为数组。

**特性：**
- 使用数组池优化内存分配
- 自动处理容量扩展
- 智能判断是否需要清空数组（基于类型是否包含引用）

### ToList（转换为列表）

将异步序列转换为 List<T>。

**特性：**
- 延迟执行，在枚举时构建列表

### ToDictionary（转换为字典）

将序列转换为字典，使用键选择器确定键。

**变体：**
- 支持元素选择器（值选择器）
- 支持自定义相等比较器
- 提供 Await 和 AwaitWithCancellation 变体

### ToHashSet（转换为哈希集合）

将序列转换为 HashSet<T>。

**变体：**
- 支持自定义相等比较器

### ToLookup（转换为查找表）

将序列转换为 ILookup<TKey, TElement>，类似于字典但支持一个键对应多个值。

**特性：**
- 支持元素选择器
- 支持自定义相等比较器
- 提供 Await 和 AwaitWithCancellation 变体

### ToObservable（转换为可观察序列）

将异步序列转换为 IObservable<T>，实现与 Reactive Extensions 的互操作。

---

## 元素操作符

### First / FirstOrDefault（第一个元素）

返回序列的第一个元素。

**变体：**
- FirstAsync：如果序列为空则抛出异常
- FirstOrDefaultAsync：如果序列为空则返回默认值
- 支持带谓词的版本（条件查找）
- 提供 Await 和 AwaitWithCancellation 变体

### Last / LastOrDefault（最后一个元素）

返回序列的最后一个元素。

**特性：**
- 需要枚举整个序列（延迟执行但需要缓冲）

### Single / SingleOrDefault（单个元素）

返回序列的单个元素。如果序列为空或包含多个元素，则抛出异常或返回默认值。

### ElementAt / ElementAtOrDefault（索引元素）

返回序列中指定索引处的元素。

**特性：**
- 支持负数索引（从末尾开始）
- 需要枚举到指定位置

---

## 集合操作符

### Concat（连接）

将两个序列连接成一个序列。

### Union（并集）

返回两个序列的并集（去重）。

**变体：**
- 支持自定义相等比较器

### Intersect（交集）

返回两个序列的交集。

**变体：**
- 支持自定义相等比较器

### Except（差集）

返回第一个序列中存在但第二个序列中不存在的元素。

**变体：**
- 支持自定义相等比较器

### Distinct（去重）

返回序列中的不同元素。

**变体：**
- 支持自定义相等比较器

### DistinctUntilChanged（连续去重）

返回序列中的元素，但跳过与上一个元素相同的连续元素。

**特性：**
- 只比较相邻元素
- 支持自定义相等比较器

---

## 连接操作符

### Join（内连接）

基于匹配键将两个序列的元素关联起来。

**变体：**
- 支持结果选择器
- 支持自定义相等比较器
- 提供 Await 和 AwaitWithCancellation 变体

### GroupJoin（分组连接）

基于匹配键将两个序列的元素关联起来，并将第二个序列的元素分组。

**特性：**
- 类似于 SQL 的 LEFT OUTER JOIN
- 支持结果选择器
- 支持自定义相等比较器

### Zip（压缩）

将两个序列的对应元素合并为一个序列。

**变体：**
- 默认返回元组 (TFirst, TSecond)
- 支持结果选择器
- 提供 Await 和 AwaitWithCancellation 变体

**特性：**
- 当任一序列结束时，Zip 操作结束

---

## 时间相关操作符

### Take（取前 N 个）

返回序列的前 N 个元素。

### TakeWhile（条件取元素）

返回序列中满足条件的连续元素，直到遇到不满足条件的元素。

**变体：**
- 支持带索引的谓词
- 提供 Await 和 AwaitWithCancellation 变体

### TakeUntil（直到条件）

返回序列中的元素，直到另一个序列产生值。

**特性：**
- 支持异步条件序列
- 当条件序列产生值时停止

### TakeUntilCanceled（直到取消）

返回序列中的元素，直到取消令牌被触发。

### TakeLast（取后 N 个）

返回序列的最后 N 个元素。

**特性：**
- 需要缓冲最后 N 个元素

### Skip（跳过前 N 个）

跳过序列的前 N 个元素，返回剩余元素。

### SkipWhile（条件跳过）

跳过序列中满足条件的连续元素，返回剩余元素。

**变体：**
- 支持带索引的谓词
- 提供 Await 和 AwaitWithCancellation 变体

### SkipUntil（直到条件）

跳过序列中的元素，直到另一个序列产生值。

### SkipUntilCanceled（直到取消）

跳过序列中的元素，直到取消令牌被触发。

### SkipLast（跳过后 N 个）

跳过序列的最后 N 个元素，返回剩余元素。

**特性：**
- 需要缓冲最后 N 个元素以确定何时开始返回

---

## 条件操作符

### Any（是否存在）

确定序列中是否存在任何元素，或是否存在满足条件的元素。

**变体：**
- 支持谓词
- 提供 Await 和 AwaitWithCancellation 变体

**特性：**
- 短路求值，找到第一个满足条件的元素即返回

### All（是否全部满足）

确定序列中的所有元素是否都满足条件。

**变体：**
- 提供 Await 和 AwaitWithCancellation 变体

**特性：**
- 短路求值，遇到第一个不满足条件的元素即返回

### Contains（是否包含）

确定序列是否包含指定元素。

**变体：**
- 支持自定义相等比较器

### SequenceEqual（序列相等）

确定两个序列是否相等（元素相同且顺序相同）。

**变体：**
- 支持自定义相等比较器

### DefaultIfEmpty（默认值）

如果序列为空，返回包含默认值的序列；否则返回原序列。

---

## 缓冲和窗口操作符

### Buffer（缓冲）

将序列的元素分组到固定大小的缓冲区中。

**变体：**
- Buffer(count)：每 count 个元素一组
- Buffer(count, skip)：每 skip 个元素取 count 个元素一组（支持重叠）

**特性：**
- 返回 IList<TSource> 序列
- 最后一个缓冲区可能包含少于 count 个元素

---

## 订阅和观察者模式

### Subscribe（订阅）

订阅异步序列，对每个元素执行操作。

**变体：**
- Subscribe(action)：同步操作
- Subscribe(func)：异步操作（返回 UniTaskVoid）
- SubscribeAwait(func)：异步操作（返回 UniTask）
- 支持取消令牌
- 返回 IDisposable，用于取消订阅

**特性：**
- 支持 OnNext、OnError、OnCompleted 回调
- 自动处理异常和完成状态

### Do（副作用操作）

对序列中的每个元素执行副作用操作，然后传递原元素。

**特性：**
- 支持 OnNext、OnError、OnCompleted 回调
- 不影响序列的元素和顺序

---

## Unity 专用扩展

### EveryUpdate（每帧更新）

创建一个在 Unity 的每个更新循环中产生值的序列。

**特性：**
- 支持所有 PlayerLoopTiming（Update、FixedUpdate、LateUpdate 等）
- 支持立即取消选项
- 返回 AsyncUnit 序列

**使用场景：**
- 帧率相关的操作
- 每帧检查条件

### EveryValueChanged（值变化监听）

监听对象属性的变化，当值改变时产生新值。

**特性：**
- 支持 Unity 对象和标准 C# 对象
- Unity 对象使用直接引用，标准对象使用弱引用
- 支持自定义相等比较器
- 支持所有 PlayerLoopTiming
- 首次枚举时立即返回当前值

**使用场景：**
- 监听 Transform 位置变化
- 监听组件属性变化
- 响应式编程

### Timer（定时器）

创建一个在指定时间后产生值的序列，可选择周期性产生。

**变体：**
- Timer(dueTime)：单次触发
- Timer(dueTime, period)：周期性触发
- Interval(period)：立即开始，周期性触发
- TimerFrame：基于帧数的定时器
- IntervalFrame：基于帧数的间隔定时器

**特性：**
- 支持忽略 Time.timeScale（使用 unscaledDeltaTime）
- 支持所有 PlayerLoopTiming
- 支持立即取消选项
- 首次触发时跳过初始帧

**使用场景：**
- 延迟执行
- 周期性任务
- 帧率相关的定时操作

---

## 类型转换和兼容性

### ToUniTaskAsyncEnumerable（转换为异步序列）

将其他类型的序列转换为 IUniTaskAsyncEnumerable<T>。

**支持的源类型：**
- IEnumerable<T>：同步集合
- Task<T>：单个任务
- UniTask<T>：单个异步任务
- IObservable<T>：可观察序列（Reactive Extensions）

**特性：**
- 无缝集成现有代码
- 支持取消令牌

### AsUniTaskAsyncEnumerable（作为异步序列）

将异步序列转换为自身（主要用于类型转换场景）。

---

## 其他操作符

### Reverse（反转）

反转序列中元素的顺序。

**特性：**
- 需要缓冲所有元素

### Append / Prepend（追加/前置）

在序列的开头或末尾添加元素。

### Pairwise（成对）

将序列中的相邻元素组合成对。

**特性：**
- 返回 (Previous, Current) 元组序列
- 第一个元素没有 Previous，使用默认值

### Publish（发布）

将序列转换为可共享的序列，多个订阅者共享同一个枚举。

**特性：**
- 避免多次枚举源序列
- 支持多播

### Queue（队列）

将序列的元素放入队列，按顺序处理。

### ForEach（遍历）

对序列中的每个元素执行操作。

**变体：**
- 支持同步和异步操作
- 支持取消令牌

---

## 总结

UniTask LINQ 模块提供了完整的异步数据流处理能力，包括：

1. **完整的 LINQ 操作符**：覆盖了标准 LINQ 的所有主要操作
2. **异步支持**：所有操作符都支持异步操作，提供 Await 和 AwaitWithCancellation 变体
3. **Unity 集成**：专门的 Unity 扩展，与 PlayerLoop 深度集成
4. **性能优化**：零分配设计，使用对象池减少 GC 压力
5. **类型安全**：完整的泛型支持，编译时类型检查
6. **取消支持**：所有操作符都支持 CancellationToken
7. **延迟执行**：大多数操作符采用延迟执行策略，提高效率

该模块使得在 Unity 中进行异步数据流处理变得简单、高效且类型安全，是构建响应式异步系统的强大工具。

