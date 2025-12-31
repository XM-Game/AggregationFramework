# AFramework.Burst 模块

> Burst编译器集成支持，高性能计算基础模块

## 概述

AFramework.Burst 是一个高性能的 Burst 编译器扩展模块，提供：
- Burst 运行时检测与配置
- Job 系统封装与调度优化
- 常用并行算法实现
- CPU 特性检测（SIMD 支持）

## 依赖

- Unity 2022.3 LTS 或更高版本
- com.unity.burst 1.8.x+
- com.unity.collections 2.x+
- com.unity.mathematics 1.x+

## 快速开始

### 1. 检测 Burst 可用性

```csharp
using AFramework.Burst;

// 检查 Burst 是否可用
if (BurstRuntime.IsBurstAvailable)
{
    Debug.Log("Burst 编译器可用");
}

// 获取推荐的批处理大小
int batchSize = BurstRuntime.RecommendedBatchSize;
```

### 2. 使用常用 Job

```csharp
using AFramework.Burst;
using Unity.Collections;
using Unity.Jobs;

// 数据复制
var source = new NativeArray<float>(1000, Allocator.TempJob);
var dest = new NativeArray<float>(1000, Allocator.TempJob);
CopyJobUtility.Copy(source, dest).Complete();

// 数据填充
FillJobUtility.Fill(source, 1.0f).Complete();

// 数据清除
ClearJobUtility.Clear(source).Complete();
```

### 3. 使用并行算法

```csharp
// 并行排序
ParallelSort.Sort(data).Complete();

// 并行归约（求和）
var result = new NativeReference<int>(Allocator.TempJob);
ParallelReduceUtility.Reduce(data, result, new SumIntReduceOperator()).Complete();

// 并行前缀和
ParallelScanUtility.InclusiveScan(input, output, new SumIntReduceOperator()).Complete();
```

## 模块结构

```
Runtime/
├── Core/                    # 核心功能
│   ├── BurstRuntime.cs      # 运行时检测与配置
│   ├── BurstCompileOptions.cs # 编译选项
│   └── BurstFeatureDetection.cs # CPU特性检测
│
├── Jobs/
│   ├── Base/                # Job基类
│   ├── Scheduling/          # 调度器
│   ├── Common/              # 常用Job
│   └── Parallel/            # 并行算法
│
└── AFramework.Burst.asmdef  # 程序集定义
```

## 版本兼容性

支持 Unity 2022.3 LTS 至 Unity 6.x 全版本。

---
AFramework © 2025
