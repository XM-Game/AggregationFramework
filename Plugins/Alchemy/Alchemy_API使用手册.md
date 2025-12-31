# Alchemy 框架 API 使用手册

## 目录

1. [Inspector 属性 API](#inspector-属性-api)
2. [分组属性 API](#分组属性-api)
3. [Hierarchy 组件 API](#hierarchy-组件-api)
4. [序列化 API](#序列化-api)
5. [扩展 API](#扩展-api)
6. [设置 API](#设置-api)

---

## Inspector 属性 API

### 显示控制属性

#### HideScriptFieldAttribute

隐藏 Inspector 中的脚本字段。

```csharp
[HideScriptField]
public class MyComponent : MonoBehaviour
{
    // 脚本字段将被隐藏
}
```

#### HideLabelAttribute

隐藏字段的标签显示。

```csharp
[HideLabel]
public int value;
```

#### LabelTextAttribute

自定义字段的显示标签文本。

```csharp
[LabelText("自定义标签")]
public int health;

[LabelText("玩家名称")]
public string playerName;
```

#### LabelWidthAttribute

设置标签的宽度（像素）。

```csharp
[LabelWidth(150)]
public Vector3 position;
```

#### IndentAttribute

设置字段的缩进级别。

```csharp
[Indent(1)]
public int level1;

[Indent(2)]
public int level2;
```

### 可见性控制属性

#### HideInPlayModeAttribute / HideInEditModeAttribute

根据运行状态隐藏字段。

```csharp
[HideInPlayMode]
public int editorOnlyValue;

[HideInEditMode]
public int runtimeOnlyValue;
```

#### DisableInPlayModeAttribute / DisableInEditModeAttribute

根据运行状态禁用字段。

```csharp
[DisableInPlayMode]
public int editorEditableValue;

[DisableInEditMode]
public int runtimeEditableValue;
```

#### ShowIfAttribute / HideIfAttribute

基于条件显示或隐藏字段。

```csharp
public bool showAdvanced;

[ShowIf("showAdvanced")]
public int advancedValue;

[HideIf("showAdvanced")]
public int basicValue;
```

#### EnableIfAttribute / DisableIfAttribute

基于条件启用或禁用字段。

```csharp
public bool enableFeature;

[EnableIf("enableFeature")]
public float featureValue;

[DisableIf("enableFeature")]
public float alternativeValue;
```

**条件表达式支持**：
- 字段名：`"fieldName"` - 检查布尔字段
- 属性名：`"PropertyName"` - 检查布尔属性
- 方法调用：`"MethodName()"` - 调用返回布尔值的方法
- 比较表达式：`"fieldName > 10"` - 数值比较（需要方法支持）

### 只读与验证属性

#### ReadOnlyAttribute

使字段在 Inspector 中只读。

```csharp
[ReadOnly]
public int readOnlyValue;

[ReadOnly]
public string computedValue;
```

#### RequiredAttribute

确保对象引用字段不为空。

```csharp
[Required]
public GameObject target;

[Required("目标对象不能为空！")]
public Transform parent;
```

#### ValidateInputAttribute

通过方法验证字段值。

```csharp
[ValidateInput("ValidateHealth")]
public int health;

bool ValidateHealth(int value)
{
    return value >= 0 && value <= 100;
}

// 带自定义错误消息
[ValidateInput("ValidateName", "名称长度必须在3-20个字符之间")]
public string playerName;

bool ValidateName(string name)
{
    return name != null && name.Length >= 3 && name.Length <= 20;
}
```

### 分组属性

#### GroupAttribute

创建简单的视觉分组。

```csharp
[Group("基础设置")]
public int value1;
public int value2;

[Group("高级设置")]
public float advanced1;
public float advanced2;
```

#### BoxGroupAttribute

创建带边框的分组。

```csharp
[BoxGroup("玩家信息")]
public string playerName;
public int level;

[BoxGroup("战斗属性")]
public int attack;
public int defense;
```

#### TabGroupAttribute

创建标签页分组。

```csharp
[TabGroup("基础", "基本信息")]
public string name;

[TabGroup("基础", "详细信息")]
public string description;

[TabGroup("战斗", "攻击属性")]
public int attack;

[TabGroup("战斗", "防御属性")]
public int defense;
```

#### FoldoutGroupAttribute

创建可折叠的分组。

```csharp
[FoldoutGroup("基础设置")]
public int value1;
public int value2;

[FoldoutGroup("高级设置")]
public float advanced1;
public float advanced2;
```

#### HorizontalGroupAttribute

水平排列字段。

```csharp
[HorizontalGroup("位置")]
public float x;
public float y;
public float z;
```

#### InlineGroupAttribute

内联分组，不添加额外容器。

```csharp
[InlineGroup("内联")]
public int a;
public int b;
```

**嵌套分组示例**：

```csharp
[BoxGroup("主分组")]
public class MyComponent : MonoBehaviour
{
    [FoldoutGroup("主分组/子分组1")]
    public int value1;
    
    [TabGroup("主分组/子分组2", "标签1")]
    public int value2;
    
    [TabGroup("主分组/子分组2", "标签2")]
    public int value3;
}
```

### 辅助信息属性

#### HelpBoxAttribute

显示帮助信息框。

```csharp
[HelpBox("这是一个信息提示", HelpBoxMessageType.Info)]
public int value;

[HelpBox("警告：此值不能为负数", HelpBoxMessageType.Warning)]
public float speed;

[HelpBox("错误：必须设置此值", HelpBoxMessageType.Error)]
public GameObject target;
```

#### TitleAttribute

显示标题和副标题。

```csharp
[Title("玩家配置")]
public class PlayerConfig : MonoBehaviour
{
    [Title("基础属性", "玩家的基础数值")]
    public int health;
    public int mana;
    
    [Title("战斗属性")]
    public int attack;
    public int defense;
}
```

#### BlockquoteAttribute

显示引用块样式的文本。

```csharp
[Blockquote("这是重要的说明信息，将以引用块样式显示")]
public int value;
```

#### HorizontalLineAttribute

添加水平分隔线。

```csharp
public int value1;

[HorizontalLine]
public int value2;

[HorizontalLine(1, 0, 0)] // 红色分隔线
public int value3;

[HorizontalLine(0, 1, 0, 0.5f)] // 半透明绿色分隔线
public int value4;
```

#### PreviewAttribute

为对象引用显示预览图像。

```csharp
[Preview]
public Texture2D texture;

[Preview(60)] // 指定预览大小
public Sprite sprite;

[Preview(80, Align.FlexStart)] // 指定大小和对齐方式
public Material material;
```

### 方法按钮

#### ButtonAttribute

将方法转换为 Inspector 按钮。

```csharp
[Button]
public void DoSomething()
{
    Debug.Log("执行操作");
}

[Button]
[LabelText("重置数据")]
public void ResetData()
{
    // 重置逻辑
}

// 带参数的方法
[Button]
public void SetValue(int value, string name)
{
    // 在 Inspector 中会显示参数输入界面
    Debug.Log($"设置值: {value}, 名称: {name}");
}
```

### 值变化回调

#### OnValueChangedAttribute

字段值变化时调用方法。

```csharp
[OnValueChanged("OnHealthChanged")]
public int health;

void OnHealthChanged(int newHealth)
{
    Debug.Log($"生命值变为: {newHealth}");
}

// 无参数版本
[OnValueChanged("UpdateUI")]
public bool isActive;

void UpdateUI()
{
    // 更新 UI
}
```

#### OnInspectorEnableAttribute / OnInspectorDisableAttribute / OnInspectorDestroyAttribute

Inspector 生命周期回调。

```csharp
[OnInspectorEnable]
void OnEnable()
{
    Debug.Log("Inspector 已启用");
}

[OnInspectorDisable]
void OnDisable()
{
    Debug.Log("Inspector 已禁用");
}

[OnInspectorDestroy]
void OnDestroy()
{
    Debug.Log("Inspector 已销毁");
}
```

### 列表视图属性

#### ListViewSettingsAttribute

自定义列表视图的显示选项。

```csharp
[ListViewSettings(
    ShowAddRemoveFooter = true,
    ShowAlternatingRowBackgrounds = AlternatingRowBackground.All,
    ShowBorder = true,
    ShowBoundCollectionSize = true,
    ShowFoldoutHeader = true,
    SelectionType = SelectionType.Multiple,
    Reorderable = true,
    ReorderMode = ListViewReorderMode.Animated
)]
public List<int> numbers;
```

#### OnListViewChangedAttribute

监听列表视图事件。

```csharp
[OnListViewChanged(
    OnItemChanged = "OnItemChanged",
    OnItemIndexChanged = "OnItemIndexChanged",
    OnItemsAdded = "OnItemsAdded",
    OnItemsRemoved = "OnItemsRemoved",
    OnItemsChosen = "OnItemsChosen",
    OnItemsSourceChanged = "OnItemsSourceChanged",
    OnSelectionChanged = "OnSelectionChanged",
    OnSelectedIndicesChanged = "OnSelectedIndicesChanged"
)]
public List<string> items;

void OnItemChanged(object item) { }
void OnItemIndexChanged(int oldIndex, int newIndex) { }
void OnItemsAdded(IEnumerable<int> indices) { }
void OnItemsRemoved(IEnumerable<int> indices) { }
void OnItemsChosen(IEnumerable<object> items) { }
void OnItemsSourceChanged() { }
void OnSelectionChanged(IEnumerable<object> selection) { }
void OnSelectedIndicesChanged(IEnumerable<int> indices) { }
```

### 特殊字段属性

#### AssetsOnlyAttribute

限制对象字段只能引用项目资源。

```csharp
[AssetsOnly]
public GameObject prefab; // 只能引用项目中的预制体

[AssetsOnly]
public ScriptableObject config; // 只能引用 ScriptableObject 资源
```

#### InlineEditorAttribute

在 Inspector 中内联显示对象编辑器。

```csharp
[InlineEditor]
public MyScriptableObject config; // 在 Inspector 中直接编辑
```

#### ShowInInspectorAttribute

显示非序列化的字段或属性。

```csharp
[ShowInInspector]
private int privateValue; // 私有字段也会显示

[ShowInInspector]
public int ComputedProperty { get; set; } // 属性也会显示
```

### 排序属性

#### OrderAttribute

控制字段、属性或方法的显示顺序。

```csharp
[Order(0)]
public int first;

[Order(1)]
public int second;

[Order(2)]
public int third;
```

---

## 分组属性 API

### PropertyGroupAttribute

所有分组属性的基类。

```csharp
public abstract class PropertyGroupAttribute : Attribute
{
    public string GroupPath { get; }
}
```

### 分组路径语法

使用 `/` 分隔符创建嵌套分组：

```csharp
[BoxGroup("主分组")]
public class MyComponent : MonoBehaviour
{
    [FoldoutGroup("主分组/子分组1")]
    public int value1;
    
    [TabGroup("主分组/子分组2", "标签A")]
    public int value2;
    
    [TabGroup("主分组/子分组2", "标签B")]
    public int value3;
}
```

---

## Hierarchy 组件 API

### HierarchyObject

基础 Hierarchy 装饰组件。

```csharp
public class HierarchyObject : MonoBehaviour
{
    public enum Mode
    {
        UseSettings,      // 使用项目设置
        None,             // 无效果
        RemoveInPlayMode, // 运行时移除
        RemoveInBuild     // 构建时移除
    }
    
    public Mode HierarchyObjectMode { get; }
}
```

**使用示例**：

```csharp
// 添加到 GameObject 上
[AddComponentMenu("Alchemy/Hierarchy Object")]
public class MyHeader : HierarchyObject
{
    // 组件会在 Hierarchy 中显示为装饰对象
}
```

### HierarchyHeader

在 Hierarchy 中显示标题。

```csharp
[AddComponentMenu("Alchemy/Hierarchy Header")]
public class MyHeader : HierarchyHeader
{
    // 继承自 HierarchyObject
    // 在 Hierarchy 中显示为标题样式
}
```

### HierarchySeparator

在 Hierarchy 中显示分隔线。

```csharp
[AddComponentMenu("Alchemy/Hierarchy Separator")]
public class MySeparator : HierarchySeparator
{
    // 继承自 HierarchyObject
    // 在 Hierarchy 中显示为分隔线样式
}
```

### HierarchyObjectMode 枚举

```csharp
public enum HierarchyObjectMode
{
    None = 0,              // 作为普通 GameObject 处理
    RemoveInPlayMode = 1,  // 运行时移除
    RemoveInBuild = 2      // 构建时移除
}
```

---

## 序列化 API

> **注意**：序列化功能需要项目中包含 `com.unity.serialization` 包。

### AlchemySerializeAttribute

标记类或结构体支持 Alchemy 序列化。

```csharp
#if ALCHEMY_SUPPORT_SERIALIZATION
using Alchemy.Serialization;

[AlchemySerialize]
public class GameData
{
    [AlchemySerializeField]
    public int score;
    
    [AlchemySerializeField]
    public string playerName;
    
    [AlchemySerializeField]
    public GameObject reference;
}
#endif
```

### AlchemySerializeFieldAttribute

标记需要序列化的字段。

```csharp
#if ALCHEMY_SUPPORT_SERIALIZATION
[AlchemySerialize]
public class MyData
{
    [AlchemySerializeField]
    public int value; // 会被序列化
    
    public int ignored; // 不会被序列化
}
#endif
```

### ShowAlchemySerializationDataAttribute

显示序列化数据用于调试。

```csharp
#if ALCHEMY_SUPPORT_SERIALIZATION
[AlchemySerialize]
[ShowAlchemySerializationData]
public class DebugData
{
    [AlchemySerializeField]
    public int value;
}
#endif
```

### IAlchemySerializationCallbackReceiver

序列化回调接口。

```csharp
#if ALCHEMY_SUPPORT_SERIALIZATION
using Alchemy.Serialization;

[AlchemySerialize]
public class MyData : IAlchemySerializationCallbackReceiver
{
    [AlchemySerializeField]
    public int value;
    
    public void OnBeforeSerialize()
    {
        Debug.Log("序列化前");
        // 准备序列化数据
    }
    
    public void OnAfterDeserialize()
    {
        Debug.Log("反序列化后");
        // 恢复数据状态
    }
}
#endif
```

### SerializationHelper

序列化辅助类（内部使用）。

```csharp
#if ALCHEMY_SUPPORT_SERIALIZATION
using Alchemy.Serialization.Internal;

// 序列化为 JSON
string json = SerializationHelper.ToJson(data, unityObjectReferences);

// 从 JSON 反序列化
MyData data = SerializationHelper.FromJson<MyData>(json, unityObjectReferences);

// 覆盖反序列化
SerializationHelper.FromJsonOverride(json, ref data, unityObjectReferences);
#endif
```

---

## 扩展 API

### 自定义属性绘制器

创建自定义属性绘制器：

```csharp
using Alchemy.Editor;
using Alchemy.Inspector;
using UnityEngine.UIElements;

[CustomAttributeDrawer(typeof(MyCustomAttribute))]
public class MyCustomAttributeDrawer : AlchemyAttributeDrawer
{
    public override void OnCreateElement()
    {
        // 访问属性信息
        var attribute = (MyCustomAttribute)Attribute;
        var target = Target;
        var property = SerializedProperty;
        var element = TargetElement;
        
        // 自定义绘制逻辑
        element.style.backgroundColor = Color.red;
    }
}
```

**可访问的属性**：
- `SerializedObject` - 序列化对象
- `SerializedProperty` - 序列化属性
- `Target` - 目标对象实例
- `MemberInfo` - 成员信息
- `Attribute` - 属性实例
- `TargetElement` - 目标视觉元素

### 自定义分组绘制器

创建自定义分组绘制器：

```csharp
using Alchemy.Editor;
using Alchemy.Inspector;
using UnityEngine.UIElements;

[CustomGroupDrawer(typeof(MyCustomGroupAttribute))]
public class MyCustomGroupDrawer : AlchemyGroupDrawer
{
    public override VisualElement CreateRootElement(string label)
    {
        // 创建分组根元素
        var root = new VisualElement();
        root.style.backgroundColor = Color.blue;
        return root;
    }
    
    public override VisualElement GetGroupElement(Attribute attribute)
    {
        // 可选：根据属性值返回不同的元素
        return null; // 返回 null 使用根元素
    }
}
```

### AlchemyEditorWindow

创建使用 Alchemy 属性的编辑器窗口：

```csharp
using Alchemy.Editor;
using Alchemy.Inspector;
using UnityEngine;

public class MyEditorWindow : AlchemyEditorWindow
{
    [Title("窗口设置")]
    [Group("基础设置")]
    public string windowName = "我的窗口";
    
    [Group("基础设置")]
    public int windowWidth = 400;
    
    [Button]
    void SaveSettings()
    {
        Debug.Log("保存设置");
    }
    
    [MenuItem("Tools/我的窗口")]
    static void ShowWindow()
    {
        GetWindow<MyEditorWindow>("我的窗口");
    }
    
    // 可选：自定义数据保存路径
    protected override string GetWindowDataPath()
    {
        return $"ProjectSettings/MyWindow.json";
    }
}
```

---

## 设置 API

### AlchemySettings

访问项目设置：

```csharp
using Alchemy.Editor;

// 获取设置实例
var settings = AlchemySettings.GetOrCreateSettings();

// 访问 Hierarchy 设置
var hierarchyMode = settings.HierarchyObjectMode;
var showToggles = settings.ShowHierarchyToggles;
var showIcons = settings.ShowComponentIcons;
var showTreeMap = settings.ShowTreeMap;
var treeMapColor = settings.TreeMapColor;
var showSeparator = settings.ShowSeparator;
var separatorColor = settings.SeparatorColor;
var showRowShading = settings.ShowRowShading;
var evenRowColor = settings.EvenRowColor;
var oddRowColor = settings.OddRowColor;

// 保存设置
AlchemySettings.SaveSettings();
```

### 设置访问路径

在 Unity 编辑器中：
- **菜单路径**：`Project/Alchemy`
- **设置文件**：`ProjectSettings/AlchemySettings.json`

---

## 完整示例

### 示例 1：游戏配置组件

```csharp
using Alchemy.Inspector;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    [Title("游戏配置", "基础游戏设置")]
    
    [BoxGroup("玩家设置")]
    [Required]
    public GameObject playerPrefab;
    
    [BoxGroup("玩家设置")]
    [Range(1, 100)]
    [OnValueChanged("OnHealthChanged")]
    public int maxHealth = 100;
    
    [BoxGroup("玩家设置")]
    [ShowIf("showAdvanced")]
    public float healthRegenRate = 1.0f;
    
    [BoxGroup("战斗设置")]
    [TabGroup("战斗设置/攻击", "基础攻击")]
    public int baseAttack = 10;
    
    [TabGroup("战斗设置/攻击", "技能攻击")]
    public int skillAttack = 20;
    
    [TabGroup("战斗设置/防御", "基础防御")]
    public int baseDefense = 5;
    
    [FoldoutGroup("高级设置")]
    public bool showAdvanced;
    
    [FoldoutGroup("高级设置")]
    [EnableIf("showAdvanced")]
    public float advancedValue;
    
    [Button]
    [LabelText("重置配置")]
    void ResetConfig()
    {
        maxHealth = 100;
        baseAttack = 10;
    }
    
    void OnHealthChanged(int health)
    {
        Debug.Log($"最大生命值已设置为: {health}");
    }
}
```

### 示例 2：使用 Hierarchy 组件

```csharp
using Alchemy.Hierarchy;
using UnityEngine;

// 在场景中创建 Hierarchy 标题
public class SceneSection : MonoBehaviour
{
    void Start()
    {
        // 通过代码创建 Hierarchy 标题
        var header = new GameObject("=== 玩家区域 ===");
        header.AddComponent<HierarchyHeader>();
        
        // 创建分隔线
        var separator = new GameObject("---");
        separator.AddComponent<HierarchySeparator>();
    }
}
```

### 示例 3：自定义属性绘制器

```csharp
using Alchemy.Editor;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UIElements;

// 自定义属性
public class ColorFieldAttribute : System.Attribute
{
    public Color DefaultColor { get; set; } = Color.white;
}

// 自定义绘制器
[CustomAttributeDrawer(typeof(ColorFieldAttribute))]
public class ColorFieldDrawer : AlchemyAttributeDrawer
{
    public override void OnCreateElement()
    {
        var attribute = (ColorFieldAttribute)Attribute;
        var property = SerializedProperty;
        
        if (property.propertyType == SerializedPropertyType.Color)
        {
            TargetElement.style.backgroundColor = attribute.DefaultColor;
        }
    }
}

// 使用
public class MyComponent : MonoBehaviour
{
    [ColorField(DefaultColor = Color.red)]
    public Color myColor;
}
```

---

## 注意事项

1. **条件表达式**：`ShowIf`、`HideIf` 等属性的条件表达式需要是有效的 C# 表达式，框架会通过反射评估
2. **性能考虑**：大量使用条件属性可能会影响 Inspector 刷新性能
3. **序列化限制**：`[AlchemySerializeField]` 标记的字段在多对象编辑时可能有限制
4. **版本兼容**：确保 Unity 版本支持 UI Toolkit（Unity 2021.2+）
5. **命名空间**：所有 Alchemy API 都在 `Alchemy.Inspector`、`Alchemy.Editor`、`Alchemy.Hierarchy` 等命名空间中

---

## 总结

Alchemy 框架提供了丰富的 API 来增强 Unity 编辑器体验。通过声明式的属性标记，开发者可以快速创建专业、美观的 Inspector 界面，显著提升开发效率。本手册涵盖了所有主要的 API 使用方法，可以作为开发参考。

