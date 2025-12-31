# Alchemy 框架功能自述文档

## 概述

Alchemy 是一个强大的 Unity 编辑器增强框架，旨在通过声明式属性（Attributes）系统来简化和增强 Unity Inspector 和 Hierarchy 窗口的显示与交互。该框架提供了丰富的属性标记，让开发者能够以最少的代码实现复杂的编辑器界面定制，显著提升开发效率和用户体验。

## 核心特性

### 一、Inspector 增强系统

Alchemy 的核心功能是增强 Unity Inspector 窗口的显示能力。通过简单的属性标记，开发者可以：

#### 1. 显示控制
- **隐藏脚本字段**：通过 `[HideScriptField]` 属性隐藏默认的脚本引用字段
- **隐藏标签**：使用 `[HideLabel]` 隐藏字段的标签显示
- **自定义标签文本**：通过 `[LabelText]` 自定义字段的显示名称
- **标签宽度控制**：使用 `[LabelWidth]` 精确控制标签的宽度
- **缩进控制**：通过 `[Indent]` 属性调整字段的缩进级别

#### 2. 可见性控制
- **运行时/编辑时显示控制**：
  - `[HideInPlayMode]` - 仅在编辑模式下显示
  - `[HideInEditMode]` - 仅在运行时显示
  - `[DisableInPlayMode]` - 运行时禁用
  - `[DisableInEditMode]` - 编辑时禁用
- **条件显示**：
  - `[ShowIf]` - 当条件为真时显示
  - `[HideIf]` - 当条件为真时隐藏
  - `[EnableIf]` - 当条件为真时启用
  - `[DisableIf]` - 当条件为真时禁用

#### 3. 只读与验证
- **只读字段**：`[ReadOnly]` 属性使字段在 Inspector 中不可编辑
- **必填验证**：`[Required]` 属性确保对象引用字段不为空，并提供错误提示
- **输入验证**：`[ValidateInput]` 属性允许通过方法验证字段值的有效性

#### 4. 分组系统
Alchemy 提供了多种分组方式，帮助组织 Inspector 中的字段：

- **普通分组** (`[Group]`)：简单的视觉分组容器
- **盒式分组** (`[BoxGroup]`)：带边框的分组容器，提供更明显的视觉区分
- **标签页分组** (`[TabGroup]`)：将字段组织到不同的标签页中，支持标签页切换
- **折叠分组** (`[FoldoutGroup]`)：可折叠的分组，节省 Inspector 空间
- **水平分组** (`[HorizontalGroup]`)：将多个字段水平排列在同一行
- **内联分组** (`[InlineGroup]`)：将字段内联显示，不添加额外的容器

所有分组都支持嵌套，通过 `GroupPath` 参数可以创建复杂的分组层次结构。

#### 5. 辅助信息显示
- **帮助框** (`[HelpBox]`)：显示信息、警告或错误提示框
- **标题** (`[Title]`)：显示带副标题的标题文本
- **引用块** (`[Blockquote]`)：以引用块样式显示文本信息
- **水平线** (`[HorizontalLine]`)：添加水平分隔线，支持自定义颜色
- **预览** (`[Preview]`)：为对象引用字段显示预览图像

#### 6. 方法按钮
- **按钮属性** (`[Button]`)：将方法转换为 Inspector 中的按钮，支持无参数方法直接调用，有参数方法会显示参数输入界面

#### 7. 值变化回调
- **值变化事件** (`[OnValueChanged]`)：当字段值发生变化时自动调用指定的方法
- **Inspector 生命周期回调**：
  - `[OnInspectorEnable]` - Inspector 启用时调用
  - `[OnInspectorDisable]` - Inspector 禁用时调用
  - `[OnInspectorDestroy]` - Inspector 销毁时调用

#### 8. 列表视图增强
- **列表视图设置** (`[ListViewSettings]`)：自定义列表视图的显示选项，包括：
  - 显示添加/删除按钮
  - 交替行背景
  - 显示边框
  - 显示集合大小
  - 显示折叠标题
  - 选择类型
  - 可重排序性
- **列表视图事件** (`[OnListViewChanged]`)：监听列表视图的各种事件，包括：
  - 项目变化
  - 项目索引变化
  - 项目添加
  - 项目删除
  - 项目选择
  - 选择变化

#### 9. 特殊字段支持
- **仅资源引用** (`[AssetsOnly]`)：限制对象字段只能引用项目中的资源，不能引用场景中的对象
- **内联编辑器** (`[InlineEditor]`)：在 Inspector 中内联显示对象的编辑器
- **显示非序列化字段** (`[ShowInInspector]`)：在 Inspector 中显示非序列化的字段或属性

#### 10. 排序控制
- **排序属性** (`[Order]`)：控制字段、属性或方法在 Inspector 中的显示顺序

### 二、Hierarchy 窗口增强系统

Alchemy 提供了强大的 Hierarchy 窗口增强功能，让场景层次结构更加清晰和易于管理。

#### 1. Hierarchy 对象组件
- **HierarchyObject**：基础组件，用于装饰 Hierarchy 中的 GameObject 显示
  - 支持多种模式：使用设置、无效果、运行时移除、构建时移除
- **HierarchyHeader**：在 Hierarchy 中显示标题，用于组织场景结构
- **HierarchySeparator**：在 Hierarchy 中显示分隔线，用于视觉分组

#### 2. 树形图显示
- **TreeMap**：在 Hierarchy 窗口左侧显示树形结构图，清晰展示 GameObject 的层级关系
  - 支持自定义颜色
  - 显示当前节点、最后节点、层级线和水平线

#### 3. 行分隔符和着色
- **行分隔符**：在 Hierarchy 中显示行之间的分隔线
- **行着色**：为偶数行和奇数行设置不同的背景颜色，提高可读性

#### 4. 组件图标
- 在 Hierarchy 中显示附加到 GameObject 上的组件图标，快速识别组件类型

#### 5. 切换按钮
- 在 Hierarchy 中显示切换按钮，快速控制 GameObject 的激活状态

### 三、序列化系统（可选功能）

当项目中包含 Unity.Serialization 包时，Alchemy 提供额外的序列化支持：

#### 1. JSON 序列化
- **AlchemySerialize**：标记类或结构体支持 Alchemy 序列化
- **AlchemySerializeField**：标记需要序列化的字段
- **ShowAlchemySerializationData**：显示序列化数据用于调试

#### 2. Unity 对象序列化
- 支持序列化 Unity 对象引用，通过索引引用列表实现
- 自动处理对象引用的序列化和反序列化

#### 3. 特殊类型序列化
- **AnimationCurve**：完整支持动画曲线的序列化
- **Gradient**：完整支持渐变的序列化，包括颜色键和透明度键

#### 4. 序列化回调
- **IAlchemySerializationCallbackReceiver**：提供序列化前后的回调接口
  - `OnBeforeSerialize` - 序列化前调用
  - `OnAfterDeserialize` - 反序列化后调用

### 四、扩展性系统

Alchemy 提供了强大的扩展机制，允许开发者创建自定义的属性绘制器和分组绘制器。

#### 1. 自定义属性绘制器
- 继承 `AlchemyAttributeDrawer` 基类
- 使用 `[CustomAttributeDrawer]` 属性标记目标属性类型
- 实现 `OnCreateElement` 方法来自定义绘制逻辑

#### 2. 自定义分组绘制器
- 继承 `AlchemyGroupDrawer` 基类
- 使用 `[CustomGroupDrawer]` 属性标记目标分组属性类型
- 实现 `CreateRootElement` 方法创建分组根元素
- 可选实现 `GetGroupElement` 方法处理特殊分组逻辑

#### 3. 编辑器窗口支持
- **AlchemyEditorWindow**：基类，支持使用 Alchemy 属性构建编辑器窗口
- 自动保存和加载窗口数据
- 支持序列化窗口状态

### 五、设置系统

Alchemy 提供了项目级别的设置系统，通过 Unity 的 Settings Provider 访问：

#### 1. Hierarchy 设置
- **Hierarchy 对象模式**：控制 HierarchyObject 组件在运行时的行为
- **显示切换按钮**：控制是否在 Hierarchy 中显示切换按钮
- **显示组件图标**：控制是否显示组件图标
- **显示树形图**：控制是否显示树形结构图
  - 树形图颜色设置
- **显示行分隔符**：控制是否显示行分隔符
  - 分隔符颜色设置
  - 行着色设置
    - 偶数行颜色
    - 奇数行颜色

#### 2. 设置持久化
- 设置自动保存到 `ProjectSettings/AlchemySettings.json`
- 支持项目级别的配置管理

## 技术架构

### 1. 基于 UI Toolkit
Alchemy 完全基于 Unity 的 UI Toolkit（UIElements）构建，提供了现代化的编辑器界面体验，支持：
- 响应式布局
- 样式系统
- 事件处理
- 数据绑定

### 2. 反射系统
框架大量使用反射来：
- 发现和调用标记的方法
- 读取字段和属性值
- 评估条件表达式
- 动态创建类型实例

### 3. 属性系统
通过 C# 属性系统实现声明式编程：
- 编译时检查
- 元数据驱动
- 零运行时开销（属性本身）

### 4. 序列化集成
- 与 Unity 的序列化系统深度集成
- 支持 SerializedProperty 和 SerializedObject
- 兼容 Unity 的多对象编辑

## 使用场景

### 1. 游戏配置数据
使用分组、验证和条件显示来创建清晰的配置界面

### 2. 组件开发
通过属性快速创建用户友好的组件 Inspector

### 3. 工具开发
使用 AlchemyEditorWindow 快速构建编辑器工具窗口

### 4. 场景组织
使用 Hierarchy 增强功能组织复杂的场景结构

### 5. 数据序列化
使用序列化系统保存和加载游戏数据

## 优势特点

1. **声明式编程**：通过属性标记实现功能，代码简洁清晰
2. **零学习成本**：属性名称直观，易于理解和使用
3. **高度可扩展**：支持自定义绘制器和分组器
4. **性能优化**：基于 UI Toolkit，性能优异
5. **完全兼容**：与 Unity 原生系统完全兼容
6. **灵活配置**：提供丰富的设置选项
7. **文档完善**：属性功能明确，易于查阅

## 总结

Alchemy 框架为 Unity 开发者提供了一个强大而灵活的编辑器增强解决方案。通过简单的属性标记，开发者可以快速创建专业、美观、易用的 Inspector 界面，显著提升开发效率和代码可维护性。无论是简单的字段显示控制，还是复杂的条件逻辑和分组组织，Alchemy 都能提供优雅的解决方案。

