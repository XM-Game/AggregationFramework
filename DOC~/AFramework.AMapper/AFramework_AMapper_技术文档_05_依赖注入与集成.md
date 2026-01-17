# AFramework.AMapper 技术文档

## 第五部分：依赖注入与集成

---

## 1. AFramework.DI 集成

### 1.1 RegisterAMapper 扩展方法

AMapper 提供与 AFramework.DI 的深度集成。

**基本用法**：
- builder.RegisterAMapper(cfg => { ... })
- builder.RegisterAMapper(cfg => { ... }, assemblies)

**注册内容**：

| 服务 | 生命周期 | 说明 |
|------|---------|------|
| IMapperConfiguration | Singleton | 配置提供者 |
| IAMapper | Transient/Scoped | 映射器实例 |
| 值解析器 | Transient | 自动扫描注册 |
| 类型转换器 | Transient | 自动扫描注册 |
| 映射动作 | Transient | 自动扫描注册 |

### 1.2 程序集扫描

RegisterAMapper 自动扫描指定程序集。

**扫描内容**：
- MappingProfile 类
- IValueResolver\<,,\> 实现
- IMemberValueResolver\<,,,\> 实现
- ITypeConverter\<,\> 实现
- IValueConverter\<,\> 实现
- IMappingAction\<,\> 实现

**扫描规则**：
- 排除抽象类
- 只扫描公共类型
- 检查泛型接口实现

### 1.3 配置回调

支持在注册时配置 AMapper。

**IObjectResolver 访问**：
- builder.RegisterAMapper((resolver, cfg) => { ... })
- 可访问已注册的服务

### 1.4 生命周期配置

**默认生命周期**：
- IMapperConfiguration：Singleton
- IAMapper：Transient

**自定义生命周期**：
- builder.RegisterAMapper(cfg => { ... }, Lifetime.Scoped)

---

## 2. LifetimeScope 集成

### 2.1 作用域映射器

在 LifetimeScope 中使用映射器。

**使用方式**：
- 通过构造函数注入 IAMapper
- 自动使用当前作用域的服务

### 2.2 子作用域

支持子作用域中的映射器。

**特性**：
- 继承父作用域配置
- 可覆盖特定服务
- 自动生命周期管理

---

## 3. 服务构造

### 3.1 ServiceProvider 配置

配置全局服务提供者。

**配置方式**：
- cfg.ServiceProvider = container

**使用场景**：
- 自定义 DI 容器
- 特殊构造逻辑

### 3.2 动态服务定位

运行时指定服务提供者。

**配置方式**：
- new AMapper(config, serviceProvider)

**使用场景**：
- 子容器/作用域容器
- 请求级服务

---

## 4. 解析器依赖注入

### 4.1 值解析器 DI

值解析器支持构造函数注入。

**配置方式**：
- MapFrom\<TResolver\>()（类型形式）
- 容器自动解析依赖

**示例场景**：
- 注入游戏服务
- 注入配置管理器
- 注入资源加载器

### 4.2 类型转换器 DI

类型转换器同样支持 DI。

**配置方式**：
- ConvertUsing\<TConverter\>()

### 4.3 映射动作 DI

IMappingAction 支持 DI。

**配置方式**：
- AfterMap\<TAction\>()
- BeforeMap\<TAction\>()

---

## 5. Profile 与 DI

### 5.1 Profile 不支持 DI

MappingProfile 类不支持构造函数注入。

**原因**：
- Profile 在配置阶段实例化
- 此时 DI 容器可能未完全配置

**替代方案**：
- 在 IMappingAction 中使用 DI
- 在解析器中使用 DI

### 5.2 配置时访问服务

使用 IObjectResolver 回调。

**方式**：
- RegisterAMapper((resolver, cfg) => { ... })
- 可访问已注册服务

---

## 6. 测试支持

### 6.1 单元测试配置

测试中创建独立的 MapperConfiguration。

**最佳实践**：
- 每个测试类一个配置
- 使用 AssertConfigurationIsValid
- 测试映射结果

### 6.2 Mock IAMapper

可以 Mock IAMapper 进行隔离测试。

**注意事项**：
- 通常不需要 Mock
- 直接使用真实 Mapper 更可靠

### 6.3 集成测试

验证完整的映射配置。

**测试内容**：
- 配置有效性
- 映射正确性
- 边界条件

---

## 7. Unity 组件集成

### 7.1 MonoBehaviour 注入

支持在 MonoBehaviour 中注入映射器。

**使用方式**：
- 使用 [Inject] 特性
- 通过 LifetimeScope 注入

### 7.2 ScriptableObject 集成

支持 ScriptableObject 的映射配置。

**特性**：
- ScriptableObjectProfile 基类
- 编辑器配置支持
- 资源引用处理

### 7.3 Prefab 实例化集成

支持 Prefab 实例化时的映射。

**使用方式**：
- 配合 PrefabInjector 使用
- 自动映射组件数据

---

## 8. 第三方框架集成

### 8.1 UniTask 集成

支持异步映射操作。

**特性**：
- MapAsync 扩展方法
- 支持取消令牌
- 非阻塞映射

### 8.2 R3 集成

支持响应式映射。

**特性**：
- Observable 映射扩展
- 自动订阅管理
- 数据流转换

### 8.3 MessagePipe 集成

支持消息管道中的映射。

**特性**：
- 消息转换过滤器
- 自动类型转换
- 管道集成

### 8.4 VContainer 集成

支持 VContainer 依赖注入。

**配置方式**：
- 使用 VContainer 扩展方法
- 自动注册映射服务

---

## 9. 序列化框架集成

### 9.1 MemoryPack 集成

支持 MemoryPack 序列化数据的映射。

**特性**：
- 高性能二进制映射
- 零拷贝支持
- 自动类型转换

### 9.2 MessagePack 集成

支持 MessagePack 序列化数据的映射。

**特性**：
- 二进制数据映射
- 自定义格式化器
- 压缩支持

### 9.3 JsonUtility 集成

支持 Unity JsonUtility 的映射。

**特性**：
- JSON 数据映射
- Unity 类型支持
- 编辑器集成

---

## 10. 数据库集成

### 10.1 MasterMemory 集成

支持 MasterMemory 内存数据库的映射。

**特性**：
- 高性能查询映射
- 索引优化
- 类型安全

### 10.2 SQLite 集成

支持 SQLite 数据的映射。

**特性**：
- 查询结果映射
- 参数化查询
- 事务支持

---

## 11. 日志集成

### 11.1 Unity Debug 日志

AMapper 使用 Unity Debug 进行日志输出。

**日志内容**：
- 配置警告
- 映射错误
- 性能提示

### 11.2 自定义日志

支持自定义日志提供者。

**配置方式**：
- cfg.Logger = customLogger

### 11.3 日志级别

**建议配置**：
- 开发环境：Debug/Information
- 生产环境：Warning/Error

---

## 12. 性能监控

### 12.1 编译时间

监控映射编译时间。

**方法**：
- 使用 Stopwatch
- 记录 CompileMappings 时间

### 12.2 映射性能

监控运行时映射性能。

**工具**：
- Unity Profiler
- 自定义计时
- 性能标记

### 12.3 内存使用

监控内存占用。

**关注点**：
- 配置内存
- 执行计划缓存
- 大对象映射
- GC 分配

---

## 13. 编辑器集成

### 13.1 AMapper 配置窗口

提供可视化配置界面。

**功能**：
- 查看所有 TypeMap
- 查看成员映射
- 验证配置

### 13.2 执行计划查看器

查看映射执行计划。

**功能**：
- 表达式树可视化
- 性能分析
- 调试支持

### 13.3 Profile 检视器

检视 Profile 配置。

**功能**：
- 查看映射列表
- 编辑配置
- 验证状态

---

## 14. 安装器模式

### 14.1 AMapperInstaller

提供标准安装器模式。

**使用方式**：
- 继承 InstallerBase
- 在 Install 方法中配置

### 14.2 模块化配置

支持模块化的映射配置。

**特性**：
- 按模块组织 Profile
- 按需加载配置
- 配置隔离

### 14.3 配置优先级

**优先级顺序**：
1. 显式配置
2. Profile 配置
3. 全局配置
4. 默认约定

---

*下一部分：内置映射器与扩展点*
