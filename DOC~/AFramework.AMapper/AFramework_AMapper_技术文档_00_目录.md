# AFramework.AMapper 技术文档

## 文档总览

AMapper 是 AFramework 聚合框架中的轻量级对象-对象映射组件，专为 Unity 游戏开发设计。本文档详细介绍了 AMapper 的架构设计、核心功能、使用方法和最佳实践。

---

## 文档目录

### 第一部分：概述与核心架构
- 项目概述与设计目标
- 核心架构层次
- 核心组件详解
- 执行流程
- 目录结构
- 快速开始
- 设计原则
- 性能考虑
- Unity 版本兼容性

### 第二部分：映射配置详解
- 基础映射配置（CreateMap、MappingProfile）
- 成员映射配置（ForMember、MapFrom、Ignore）
- 扁平化映射
- 投影映射
- 嵌套映射与集合映射
- 继承映射
- 反向映射
- 命名约定
- 全局配置
- 配置验证
- 特性映射
- 开放泛型映射
- Unity 特定配置

### 第三部分：高级映射功能
- 自定义类型转换器
- 自定义值解析器
- 值转换器
- 构造函数映射
- 前置/后置映射动作
- 空值处理
- 循环引用处理
- 动态对象映射
- 枚举映射
- 运行时映射选项
- 服务定位
- 异常处理
- Unity 特定高级功能

### 第四部分：LINQ 投影与查询扩展
- ProjectTo 概述
- 基本用法
- 高级投影功能
- 显式展开
- 参数化投影
- 自定义类型转换
- 空值安全
- 递归查询
- 多态投影
- 投影限制
- 性能优化
- 调试投影
- Unity 特定投影
- 集合扩展方法

### 第五部分：依赖注入与集成
- AFramework.DI 集成
- LifetimeScope 集成
- 服务构造
- 解析器依赖注入
- Profile 与 DI
- 测试支持
- Unity 组件集成
- 第三方框架集成
- 序列化框架集成
- 数据库集成
- 日志集成
- 性能监控
- 编辑器集成
- 安装器模式

### 第六部分：内置映射器与扩展点
- 内置映射器概述
- 内置映射器详解
- 值解析器系统
- 自定义映射器
- Unity 类型扩展
- 配置验证系统
- 执行计划构建
- 对象工厂
- 内部工具类
- 并发与线程安全
- 对象池集成
- 扩展最佳实践

### 第七部分：调试、测试与最佳实践
- 理解映射执行
- 配置验证详解
- 常见问题诊断
- 单元测试策略
- 性能优化详解
- 最佳实践
- 常见模式
- 反模式
- Unity 项目特别建议
- 资源与支持

### 第八部分：成员配置与表达式系统
- 成员配置表达式概述
- 值来源配置
- 条件映射
- 空值处理
- 映射控制
- 值转换器
- 反向映射支持
- 配置应用流程
- 路径配置表达式
- 构造函数参数配置
- Unity 特定配置
- 表达式系统内部
- 快速参考

---

## 核心特性

| 特性 | 说明 |
|------|------|
| Unity 原生兼容 | 完全兼容 Unity 2022.3 LTS 至 Unity 6.x |
| 零外部依赖 | 不依赖任何第三方库，纯 C# 实现 |
| 高性能 | 表达式树编译、对象池复用、零 GC 分配优化 |
| 轻量级 | 核心代码精简，按需加载，最小化内存占用 |
| 易于集成 | 与 AFramework.DI 依赖注入框架无缝集成 |
| 游戏友好 | 支持 Unity 特有类型（Vector3、Quaternion、Color 等） |
| IL2CPP 兼容 | 完全兼容 IL2CPP 编译 |

---

## 快速开始

### 基本使用

```csharp
// 1. 创建配置
var configuration = new MapperConfiguration(cfg => 
{
    cfg.CreateMap<PlayerData, PlayerDto>();
});

// 2. 创建映射器
var mapper = configuration.CreateMapper();

// 3. 执行映射
var dto = mapper.Map<PlayerDto>(playerData);
```

### 使用 Profile

```csharp
public class GameProfile : MappingProfile
{
    public GameProfile()
    {
        CreateMap<PlayerData, PlayerDto>();
        CreateMap<Enemy, EnemyDto>()
            .ForMember(d => d.HealthPercent, 
                       opt => opt.MapFrom(s => s.CurrentHealth / s.MaxHealth));
    }
}
```

### 与 AFramework.DI 集成

```csharp
public class GameInstaller : InstallerBase
{
    public override void Install(IContainerBuilder builder)
    {
        builder.RegisterAMapper(cfg =>
        {
            cfg.AddProfile<GameProfile>();
        });
    }
}
```

---

## 命名空间

```csharp
// 核心命名空间
using AFramework.AMapper;

// 配置命名空间
using AFramework.AMapper.Configuration;

// Unity 扩展命名空间
using AFramework.AMapper.Unity;

// DI 集成命名空间
using AFramework.AMapper.DI;
```

---

## 版本信息

- **框架版本**：1.0.0
- **目标框架**：.NET Standard 2.1
- **Unity 版本**：Unity 2022.3 LTS 及以上
- **许可证**：MIT License

---

## 相关文档

- AFramework.DI 依赖注入框架技术文档
- AFramework 技术设计文档
- Unity 官方文档

---

*AFramework.AMapper - 为 Unity 游戏开发设计的轻量级对象映射框架*
