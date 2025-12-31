---
inclusion: fileMatch
fileMatchPattern: ['**/*ECS*.cs', '**/*Entity*.cs', '**/*System*.cs', '**/*Job*.cs', '**/*Burst*.cs', '**/DOTS/**/*.cs']
---

# Unity DOTS 开发规范

本项目使用 Unity DOTS（Data-Oriented Technology Stack）技术栈，包含 ECS、Burst Compiler 和 JobSystem。

## 核心概念

| 组件 | 职责 | 要点 |
|------|------|------|
| Entity | 纯 ID 标识符 | 不包含数据或逻辑 |
| Component | 纯数据容器 | 仅包含值类型，禁止引用类型 |
| System | 无状态逻辑处理 | 操作 Component 数据，实现状态转换 |

## 编码规范

### Component 设计
- 必须使用 `struct` 并实现 `IComponentData`
- 仅包含 blittable 类型（int, float, bool, 固定数组等）
- 禁止使用引用类型（string, class, 托管数组）
- 保持数据紧凑，减少内存占用

```csharp
// ✅ 正确示例
public struct MoveSpeed : IComponentData
{
    public float Value;
}

// ❌ 错误示例 - 包含引用类型
public struct BadComponent : IComponentData
{
    public string Name; // 禁止
}
```

### System 设计
- 继承 `SystemBase` 或实现 `ISystem`
- 保持无状态，所有数据通过 Component 传递
- 使用 `Entities.ForEach` 或 `IJobEntity` 处理批量数据

### Burst Compiler 使用
- 为性能关键代码添加 `[BurstCompile]` 特性
- 使用 `Unity.Mathematics` 类型（float3, float4, quaternion）
- 避免在 Burst 代码中使用托管类型和异常

```csharp
[BurstCompile]
public partial struct MoveJob : IJobEntity
{
    public float DeltaTime;
    
    void Execute(ref LocalTransform transform, in MoveSpeed speed)
    {
        transform.Position += new float3(0, 0, speed.Value * DeltaTime);
    }
}
```

### JobSystem 使用
- 使用 `IJob`、`IJobParallelFor`、`IJobEntity` 等接口
- 正确处理依赖关系（JobHandle）
- 使用 `NativeArray`、`NativeList` 等原生容器

## 适用场景

推荐使用 DOTS 的模块：
- 大规模实体处理（寻路、物理模拟）
- 渲染相关计算
- 数据密集型批量操作

不强制使用 DOTS 的模块：
- UI 系统
- 网络同步
- 复杂技能系统

## 混合架构建议

参考《守望先锋》架构：核心 Gameplay 使用 ECS，复杂子系统可保留 OOP。避免过度追求纯 ECS 实现。

## 官方文档

- [ECS](https://docs.unity3d.com/Packages/com.unity.entities@latest)
- [Burst](https://docs.unity3d.com/Packages/com.unity.burst@latest)
- [JobSystem](https://docs.unity3d.com/Manual/JobSystem.html)
- [Unity.Mathematics](https://docs.unity3d.com/Packages/com.unity.mathematics@latest)
