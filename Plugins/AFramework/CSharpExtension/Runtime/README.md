# AFramework C# 扩展方法库

## 概述

本模块提供了一套完整的 C# 扩展方法集合，旨在提升开发效率，简化常见操作。所有扩展方法都经过精心设计，遵循 SOLID 原则，保持代码简洁高效。

## 模块结构

### 1. Collections（集合扩展）

#### ArrayExtensions
- 数组操作：添加、移除、查找、排序
- 转换操作：ToList、ToDictionary
- 实用工具：IsNullOrEmpty、ForEach

#### ListExtensions
- 批量操作：AddRange、RemoveRange
- 随机操作：Shuffle、GetRandom
- 查找操作：FindAll、IndexOfAll

#### DictionaryExtensions
- 安全访问：GetValueOrDefault、GetOrAdd、TryAdd
- 批量操作：AddRange、RemoveRange、RemoveWhere
- 查询操作：IsNullOrEmpty、FindKeyByValue

#### HashSetExtensions
- 集合运算：UnionWith、IntersectWith、ExceptWith
- 批量操作：AddRange、RemoveRange
- 实用工具：IsNullOrEmpty

#### QueueExtensions
- 批量操作：EnqueueRange、DequeueAll
- 转换操作：ToArray、ToList
- 实用工具：IsNullOrEmpty

#### StackExtensions
- 批量操作：PushRange、PopAll
- 转换操作：ToArray、ToList
- 实用工具：IsNullOrEmpty

#### EnumerableExtensions
- LINQ 增强：ForEach、Batch、DistinctBy
- 统计操作：IsNullOrEmpty、None
- 实用工具：JoinString、ToHashSet

#### CollectionExtensions
- 批量操作：AddRange、RemoveRange、RemoveWhere
- 查询操作：IsNullOrEmpty、ContainsAny、ContainsAll

#### ReadOnlyExtensions
- 只读集合：AsReadOnly、GetValueOrDefault
- 查询操作：IsNullOrEmpty、IndexOf

#### SpanExtensions
- Span<T> 操作：IsEmpty、GetValueOrDefault、FillWith、Reverse
- Memory<T> 操作：IsEmpty、GetValueOrDefault

### 2. Primitives（基础类型扩展）

#### BoolExtensions
- 转换操作：ToInt、ToString、ToChineseString、ToYesNo
- 条件执行：IfTrue、IfFalse、Then
- 实用工具：Toggle

#### ByteExtensions
- 进制转换：ToHexString、ToBinaryString
- 位操作：IsBitSet、SetBit、ClearBit、ToggleBit
- 范围操作：IsInRange、Clamp
- 字节数组：ToHexString、ToBase64String、FromHexString

#### IntExtensions
- 范围操作：IsInRange、Clamp
- 数学运算：Abs、IsEven、IsOdd、IsPrime
- 实用工具：Times、To、Step

#### LongExtensions
- 范围操作：IsInRange、Clamp
- 数学运算：Abs、IsEven、IsOdd
- 转换操作：ToFileSizeString、ToDateTime

#### FloatExtensions
- 范围操作：IsInRange、Clamp
- 数学运算：Abs、Approximately、IsZero
- 插值操作：Lerp、InverseLerp
- 实用工具：Round、Ceiling、Floor

#### DoubleExtensions
- 范围操作：IsInRange、Clamp
- 数学运算：Abs、Approximately、IsZero
- 插值操作：Lerp、InverseLerp
- 实用工具：Round、Ceiling、Floor、Squared、Sqrt

#### DecimalExtensions
- 范围操作：IsInRange、Clamp
- 数学运算：Abs、Round、Ceiling、Floor
- 格式化：ToCurrencyString、ToPercentageString

#### CharExtensions
- 类型判断：IsDigit、IsLetter、IsUpper、IsLower
- 转换操作：ToUpper、ToLower、ToAscii
- 实用工具：Repeat、IsVowel、IsConsonant

### 3. Strings（字符串扩展）

#### StringExtensions
- 基础操作：Trim、Split、Replace、Substring
- 转换操作：ToUpper、ToLower、Reverse
- 实用工具：IsNullOrEmpty、Contains、StartsWith

#### StringBuilderExtensions
- 批量操作：AppendLine、AppendFormat
- 条件操作：AppendIf、AppendLineIf
- 实用工具：Clear、IsEmpty

#### StringValidationExtensions
- 基础验证：IsNullOrEmpty、IsNullOrWhiteSpace、HasValue
- 格式验证：IsValidEmail、IsValidUrl、IsValidIPAddress、IsValidPhoneNumber
- 内容验证：IsNumeric、IsAlpha、IsAlphaNumeric、IsLowerCase、IsUpperCase
- 长度验证：IsLengthInRange、IsLengthEqual
- 正则验证：IsMatch

#### StringConversionExtensions
- 数值转换：ToInt、ToLong、ToFloat、ToDouble、ToDecimal、ToBool
- 日期转换：ToDateTime、ToDateTime(format)
- 枚举转换：ToEnum
- 字节转换：ToBytes、FromBase64、FromHexString
- 可空类型：ToNullableInt、ToNullableLong、ToNullableFloat

#### StringFormatExtensions
- 大小写转换：ToTitleCase、ToCamelCase、ToPascalCase、ToSnakeCase、ToKebabCase
- 截断填充：Truncate、PadLeft、PadRight、PadCenter
- 移除操作：RemoveWhiteSpace、Remove
- 重复反转：Repeat、Reverse
- 格式化：FormatWith、IfEmpty、IfWhiteSpace

#### StringSearchExtensions
- 包含判断：ContainsIgnoreCase、ContainsAny、ContainsAll
- 开始结束：StartsWithIgnoreCase、EndsWithIgnoreCase、StartsWithAny、EndsWithAny
- 索引查找：IndexOfAll、CountOccurrences
- 提取操作：Between、Before、After、BeforeLast、AfterLast
- 相似度：LevenshteinDistance

### 4. DateTime（日期时间扩展）

#### DateTimeExtensions
- 时间戳：ToUnixTimestamp、FromUnixTimestamp
- 日期判断：IsToday、IsYesterday、IsTomorrow、IsWeekend、IsWeekday
- 日期计算：StartOfMonth、EndOfMonth、StartOfWeek、EndOfWeek、StartOfYear、EndOfYear
- 年龄计算：CalculateAge
- 格式化：ToFriendlyString、ToIso8601String
- 时间设置：SetTime、SetDate

#### TimeSpanExtensions
- 转换操作：ToReadableString、ToShortString
- 数学运算：Multiply、Divide
- 实用工具：IsZero、IsPositive、IsNegative

#### DateOnlyExtensions（.NET 6+ / Unity 2022.2+）
- 日期判断：IsToday、IsWeekend、IsWeekday
- 日期计算：StartOfMonth、EndOfMonth、StartOfWeek、EndOfWeek
- 年龄计算：CalculateAge
- 转换操作：ToDateTime

#### TimeOnlyExtensions（.NET 6+ / Unity 2022.2+）
- 时间判断：IsBetween、IsAM、IsPM
- 格式化：To12HourFormat
- 时间操作：AddHours、AddMinutes、AddSeconds

### 5. Types（类型扩展）

#### ObjectExtensions
- 类型判断：IsNull、IsNotNull、IsDefault
- 转换操作：As、To、TryConvert
- 实用工具：With、Tap、Pipe

#### EnumExtensions
- 转换操作：ToInt、ToString、GetDescription
- 实用工具：GetValues、GetNames、IsDefined

## 使用示例

### 集合操作
```csharp
// 数组扩展
var array = new[] { 1, 2, 3, 4, 5 };
var shuffled = array.Shuffle();
var random = array.GetRandom();

// 字典扩展
var dict = new Dictionary<string, int>();
dict.GetOrAdd("key", k => 42);
var value = dict.GetValueOrDefault("key", 0);

// 列表扩展
var list = new List<int> { 1, 2, 3 };
list.AddRange(new[] { 4, 5, 6 });
list.Shuffle();
```

### 字符串操作
```csharp
// 验证
var email = "test@example.com";
if (email.IsValidEmail()) { }

// 转换
var number = "123".ToInt(0);
var date = "2024-01-01".ToDateTime();

// 格式化
var text = "HelloWorld".ToSnakeCase(); // "hello_world"
var truncated = "Long text...".Truncate(10); // "Long te..."

// 搜索
var count = "hello world".CountOccurrences("l"); // 3
var between = "start[content]end".Between("[", "]"); // "content"
```

### 数值操作
```csharp
// 范围检查
var value = 5;
if (value.IsInRange(1, 10)) { }

// 数学运算
var clamped = value.Clamp(0, 100);
var lerped = 0f.Lerp(100f, 0.5f); // 50

// 格式化
var size = 1024L.ToFileSizeString(); // "1.00 KB"
var percent = 0.75.ToPercentageString(); // "75.00%"
```

### 日期时间操作
```csharp
// 日期判断
var now = DateTime.Now;
if (now.IsToday()) { }
if (now.IsWeekend()) { }

// 日期计算
var startOfMonth = now.StartOfMonth();
var endOfWeek = now.EndOfWeek();

// 友好显示
var friendly = now.ToFriendlyString(); // "刚刚"、"5分钟前"等

// 年龄计算
var birthDate = new DateTime(1990, 1, 1);
var age = birthDate.CalculateAge(); // 34
```

### 布尔操作
```csharp
// 条件执行
bool condition = true;
condition.IfTrue(() => Console.WriteLine("True!"));

// 三元操作
var result = condition.Then("Yes", "No");

// 转换
var intValue = condition.ToInt(); // 1
var chinese = condition.ToChineseString(); // "是"
```

## 性能考虑

1. **零分配设计**：大部分扩展方法避免不必要的内存分配
2. **Span<T> 支持**：提供高性能的内存操作
3. **延迟计算**：使用 IEnumerable 实现延迟执行
4. **缓存优化**：合理使用缓存减少重复计算

## 版本兼容性

- Unity 2022.3 LTS 及以上
- .NET Standard 2.1
- C# 8.0+

部分功能需要更高版本：
- DateOnly/TimeOnly：Unity 2022.2+ 或 .NET 6+
- Span<T>：Unity 2021.2+ 或 .NET Standard 2.1+

## 最佳实践

1. **优先使用扩展方法**：简化代码，提高可读性
2. **注意空值检查**：大部分方法会进行空值检查并抛出异常
3. **合理使用默认值**：转换方法提供默认值参数
4. **性能敏感场景**：使用 Span<T> 相关扩展
5. **避免过度使用**：简单场景直接使用原生方法

## 贡献指南

添加新的扩展方法时请遵循：
1. 单一职责原则：每个方法只做一件事
2. 命名清晰：方法名能准确表达功能
3. 参数验证：对输入参数进行必要的验证
4. 文档注释：提供完整的 XML 文档注释
5. 性能优化：避免不必要的内存分配和计算

## 许可证

本项目遵循 MIT 许可证。
