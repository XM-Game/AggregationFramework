// ==========================================================
// 文件名：FormatConstants.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化格式常量定义
    /// <para>提供各种序列化格式的配置常量</para>
    /// <para>包含二进制、YAML、JSON 格式的相关常量</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用二进制格式常量
    /// var header = FormatConstants.Binary.HeaderSize;
    /// 
    /// // 使用 YAML 格式常量
    /// var indent = FormatConstants.Yaml.DefaultIndent;
    /// 
    /// // 使用 JSON 格式常量
    /// var maxDepth = FormatConstants.Json.MaxNestingDepth;
    /// </code>
    /// </remarks>
    public static class FormatConstants
    {
        #region 通用标记常量

        /// <summary>
        /// Null 值标记字节
        /// </summary>
        public const byte NullMarker = 0x00;

        /// <summary>
        /// 非 Null 值标记字节
        /// </summary>
        public const byte NotNullMarker = 0x01;

        #endregion

        #region 二进制格式常量

        /// <summary>
        /// 二进制格式常量集合
        /// <para>定义二进制序列化格式的相关常量</para>
        /// </summary>
        public static class Binary
        {
            /// <summary>数据头大小 (字节)</summary>
            public const int HeaderSize = 12;

            /// <summary>最小数据头大小 (字节)</summary>
            public const int MinHeaderSize = 8;

            /// <summary>扩展头大小 (字节)</summary>
            public const int ExtendedHeaderSize = 24;

            /// <summary>类型码大小 (字节)</summary>
            public const int TypeCodeSize = 1;

            /// <summary>扩展类型码大小 (字节)</summary>
            public const int ExtendedTypeCodeSize = 2;

            /// <summary>长度前缀最大字节数 (VarInt)</summary>
            public const int MaxLengthPrefixSize = 5;

            /// <summary>对象 ID 大小 (字节)</summary>
            public const int ObjectIdSize = 4;

            /// <summary>校验和大小 (字节)</summary>
            public const int ChecksumSize = 4;

            /// <summary>版本号大小 (字节)</summary>
            public const int VersionSize = 2;

            /// <summary>标志位大小 (字节)</summary>
            public const int FlagsSize = 2;

            /// <summary>对齐边界 (字节)</summary>
            public const int Alignment = 8;

            /// <summary>最大内联字符串长度 (字节)</summary>
            public const int MaxInlineStringLength = 127;

            /// <summary>最大内联数组长度</summary>
            public const int MaxInlineArrayLength = 15;
        }

        #endregion

        #region VarInt 编码常量

        /// <summary>
        /// VarInt 编码常量集合
        /// <para>定义变长整数编码的相关常量</para>
        /// </summary>
        public static class VarInt
        {
            /// <summary>单字节最大值</summary>
            public const int MaxSingleByte = 0x7F; // 127

            /// <summary>双字节最大值</summary>
            public const int MaxTwoByte = 0x3FFF; // 16383

            /// <summary>三字节最大值</summary>
            public const int MaxThreeByte = 0x1FFFFF; // 2097151

            /// <summary>四字节最大值</summary>
            public const int MaxFourByte = 0x0FFFFFFF; // 268435455

            /// <summary>继续位掩码</summary>
            public const byte ContinuationMask = 0x80;

            /// <summary>数据位掩码</summary>
            public const byte DataMask = 0x7F;

            /// <summary>数据位数</summary>
            public const int DataBits = 7;

            /// <summary>最大编码字节数 (32位)</summary>
            public const int MaxBytes32 = 5;

            /// <summary>最大编码字节数 (64位)</summary>
            public const int MaxBytes64 = 10;

            /// <summary>负数标记位</summary>
            public const byte NegativeFlag = 0x40;
        }

        #endregion

        #region 类型码常量

        /// <summary>
        /// 类型码常量集合
        /// <para>定义序列化数据中的类型标识码</para>
        /// </summary>
        public static class TypeCode
        {
            // 基础类型 (0x00 - 0x1F)
            /// <summary>空值</summary>
            public const byte Null = 0x00;

            /// <summary>布尔值 false</summary>
            public const byte False = 0x01;

            /// <summary>布尔值 true</summary>
            public const byte True = 0x02;

            /// <summary>8位有符号整数</summary>
            public const byte Int8 = 0x03;

            /// <summary>8位无符号整数</summary>
            public const byte UInt8 = 0x04;

            /// <summary>16位有符号整数</summary>
            public const byte Int16 = 0x05;

            /// <summary>16位无符号整数</summary>
            public const byte UInt16 = 0x06;

            /// <summary>32位有符号整数</summary>
            public const byte Int32 = 0x07;

            /// <summary>32位无符号整数</summary>
            public const byte UInt32 = 0x08;

            /// <summary>64位有符号整数</summary>
            public const byte Int64 = 0x09;

            /// <summary>64位无符号整数</summary>
            public const byte UInt64 = 0x0A;

            /// <summary>单精度浮点数</summary>
            public const byte Float32 = 0x0B;

            /// <summary>双精度浮点数</summary>
            public const byte Float64 = 0x0C;

            /// <summary>十进制数</summary>
            public const byte Decimal = 0x0D;

            /// <summary>字符</summary>
            public const byte Char = 0x0E;

            /// <summary>半精度浮点数</summary>
            public const byte Float16 = 0x0F;

            // 字符串类型 (0x10 - 0x1F)
            /// <summary>空字符串</summary>
            public const byte EmptyString = 0x10;

            /// <summary>短字符串 (长度 1-31)</summary>
            public const byte ShortString = 0x11;

            /// <summary>中等字符串 (长度 32-255)</summary>
            public const byte MediumString = 0x12;

            /// <summary>长字符串 (长度 256+)</summary>
            public const byte LongString = 0x13;

            /// <summary>UTF-16 字符串</summary>
            public const byte Utf16String = 0x14;

            /// <summary>内化字符串引用</summary>
            public const byte InternedString = 0x15;

            /// <summary>压缩字符串</summary>
            public const byte CompressedString = 0x16;

            // 集合类型 (0x20 - 0x3F)
            /// <summary>空数组</summary>
            public const byte EmptyArray = 0x20;

            /// <summary>数组</summary>
            public const byte Array = 0x21;

            /// <summary>列表</summary>
            public const byte List = 0x22;

            /// <summary>字典</summary>
            public const byte Dictionary = 0x23;

            /// <summary>哈希集合</summary>
            public const byte HashSet = 0x24;

            /// <summary>队列</summary>
            public const byte Queue = 0x25;

            /// <summary>栈</summary>
            public const byte Stack = 0x26;

            /// <summary>链表</summary>
            public const byte LinkedList = 0x27;

            /// <summary>多维数组</summary>
            public const byte MultiDimensionalArray = 0x28;

            /// <summary>交错数组</summary>
            public const byte JaggedArray = 0x29;

            /// <summary>只读集合</summary>
            public const byte ReadOnlyCollection = 0x2A;

            /// <summary>不可变集合</summary>
            public const byte ImmutableCollection = 0x2B;

            // 特殊类型 (0x40 - 0x5F)
            /// <summary>日期时间</summary>
            public const byte DateTime = 0x40;

            /// <summary>时间间隔</summary>
            public const byte TimeSpan = 0x41;

            /// <summary>GUID</summary>
            public const byte Guid = 0x42;

            /// <summary>日期时间偏移</summary>
            public const byte DateTimeOffset = 0x43;

            /// <summary>仅日期</summary>
            public const byte DateOnly = 0x44;

            /// <summary>仅时间</summary>
            public const byte TimeOnly = 0x45;

            /// <summary>URI</summary>
            public const byte Uri = 0x46;

            /// <summary>版本号</summary>
            public const byte Version = 0x47;

            /// <summary>大整数</summary>
            public const byte BigInteger = 0x48;

            /// <summary>复数</summary>
            public const byte Complex = 0x49;

            /// <summary>类型信息</summary>
            public const byte Type = 0x4A;

            // 对象类型 (0x60 - 0x7F)
            /// <summary>对象开始</summary>
            public const byte ObjectStart = 0x60;

            /// <summary>对象结束</summary>
            public const byte ObjectEnd = 0x61;

            /// <summary>对象引用</summary>
            public const byte ObjectReference = 0x62;

            /// <summary>多态对象</summary>
            public const byte PolymorphicObject = 0x63;

            /// <summary>匿名对象</summary>
            public const byte AnonymousObject = 0x64;

            /// <summary>动态对象</summary>
            public const byte DynamicObject = 0x65;

            /// <summary>元组</summary>
            public const byte Tuple = 0x66;

            /// <summary>值元组</summary>
            public const byte ValueTuple = 0x67;

            /// <summary>可空类型</summary>
            public const byte Nullable = 0x68;

            /// <summary>枚举</summary>
            public const byte Enum = 0x69;

            // Unity 类型 (0x80 - 0x9F)
            /// <summary>Vector2</summary>
            public const byte Vector2 = 0x80;

            /// <summary>Vector3</summary>
            public const byte Vector3 = 0x81;

            /// <summary>Vector4</summary>
            public const byte Vector4 = 0x82;

            /// <summary>Vector2Int</summary>
            public const byte Vector2Int = 0x83;

            /// <summary>Vector3Int</summary>
            public const byte Vector3Int = 0x84;

            /// <summary>Quaternion</summary>
            public const byte Quaternion = 0x85;

            /// <summary>Color</summary>
            public const byte Color = 0x86;

            /// <summary>Color32</summary>
            public const byte Color32 = 0x87;

            /// <summary>Rect</summary>
            public const byte Rect = 0x88;

            /// <summary>RectInt</summary>
            public const byte RectInt = 0x89;

            /// <summary>Bounds</summary>
            public const byte Bounds = 0x8A;

            /// <summary>BoundsInt</summary>
            public const byte BoundsInt = 0x8B;

            /// <summary>Matrix4x4</summary>
            public const byte Matrix4x4 = 0x8C;

            /// <summary>AnimationCurve</summary>
            public const byte AnimationCurve = 0x8D;

            /// <summary>Gradient</summary>
            public const byte Gradient = 0x8E;

            /// <summary>LayerMask</summary>
            public const byte LayerMask = 0x8F;

            // ECS 类型 (0xA0 - 0xBF)
            /// <summary>Entity</summary>
            public const byte Entity = 0xA0;

            /// <summary>NativeArray</summary>
            public const byte NativeArray = 0xA1;

            /// <summary>NativeList</summary>
            public const byte NativeList = 0xA2;

            /// <summary>NativeHashMap</summary>
            public const byte NativeHashMap = 0xA3;

            /// <summary>NativeHashSet</summary>
            public const byte NativeHashSet = 0xA4;

            /// <summary>DynamicBuffer</summary>
            public const byte DynamicBuffer = 0xA5;

            /// <summary>BlobAssetReference</summary>
            public const byte BlobAssetReference = 0xA6;

            /// <summary>FixedString</summary>
            public const byte FixedString = 0xA7;

            // 扩展类型 (0xC0 - 0xDF)
            /// <summary>扩展类型标记</summary>
            public const byte Extension = 0xC0;

            /// <summary>自定义类型</summary>
            public const byte Custom = 0xC1;

            /// <summary>压缩数据</summary>
            public const byte Compressed = 0xC2;

            /// <summary>加密数据</summary>
            public const byte Encrypted = 0xC3;

            // 控制码 (0xE0 - 0xFF)
            /// <summary>流结束标记</summary>
            public const byte EndOfStream = 0xFE;

            /// <summary>无效/保留</summary>
            public const byte Invalid = 0xFF;
        }

        #endregion

        #region YAML 格式常量

        /// <summary>
        /// YAML 格式常量集合
        /// <para>定义 YAML 序列化格式的相关常量</para>
        /// </summary>
        public static class Yaml
        {
            /// <summary>默认缩进空格数</summary>
            public const int DefaultIndent = 2;

            /// <summary>最大缩进空格数</summary>
            public const int MaxIndent = 8;

            /// <summary>默认行宽限制</summary>
            public const int DefaultLineWidth = 80;

            /// <summary>最大行宽限制</summary>
            public const int MaxLineWidth = 1000;

            /// <summary>最大嵌套深度</summary>
            public const int MaxNestingDepth = 64;

            /// <summary>最大锚点数量</summary>
            public const int MaxAnchorCount = 10000;

            /// <summary>最大别名数量</summary>
            public const int MaxAliasCount = 10000;

            /// <summary>文档开始标记</summary>
            public const string DocumentStart = "---";

            /// <summary>文档结束标记</summary>
            public const string DocumentEnd = "...";

            /// <summary>锚点前缀</summary>
            public const char AnchorPrefix = '&';

            /// <summary>别名前缀</summary>
            public const char AliasPrefix = '*';

            /// <summary>标签前缀</summary>
            public const char TagPrefix = '!';

            /// <summary>注释前缀</summary>
            public const char CommentPrefix = '#';

            /// <summary>映射键值分隔符</summary>
            public const string KeyValueSeparator = ": ";

            /// <summary>序列项前缀</summary>
            public const string SequenceItemPrefix = "- ";

            /// <summary>流式序列开始</summary>
            public const char FlowSequenceStart = '[';

            /// <summary>流式序列结束</summary>
            public const char FlowSequenceEnd = ']';

            /// <summary>流式映射开始</summary>
            public const char FlowMappingStart = '{';

            /// <summary>流式映射结束</summary>
            public const char FlowMappingEnd = '}';

            /// <summary>流式分隔符</summary>
            public const string FlowSeparator = ", ";

            /// <summary>多行字符串标记 (保留换行)</summary>
            public const char LiteralBlockIndicator = '|';

            /// <summary>多行字符串标记 (折叠换行)</summary>
            public const char FoldedBlockIndicator = '>';

            /// <summary>空值表示</summary>
            public const string NullValue = "null";

            /// <summary>空值表示 (波浪号)</summary>
            public const string NullTilde = "~";

            /// <summary>布尔真值</summary>
            public const string TrueValue = "true";

            /// <summary>布尔假值</summary>
            public const string FalseValue = "false";

            /// <summary>正无穷</summary>
            public const string PositiveInfinity = ".inf";

            /// <summary>负无穷</summary>
            public const string NegativeInfinity = "-.inf";

            /// <summary>非数字</summary>
            public const string NotANumber = ".nan";
        }

        #endregion

        #region JSON 格式常量

        /// <summary>
        /// JSON 格式常量集合
        /// <para>定义 JSON 序列化格式的相关常量</para>
        /// </summary>
        public static class Json
        {
            /// <summary>默认缩进空格数</summary>
            public const int DefaultIndent = 2;

            /// <summary>最大缩进空格数</summary>
            public const int MaxIndent = 8;

            /// <summary>最大嵌套深度</summary>
            public const int MaxNestingDepth = 64;

            /// <summary>最大字符串长度</summary>
            public const int MaxStringLength = 1024 * 1024 * 100; // 100MB

            /// <summary>对象开始</summary>
            public const char ObjectStart = '{';

            /// <summary>对象结束</summary>
            public const char ObjectEnd = '}';

            /// <summary>数组开始</summary>
            public const char ArrayStart = '[';

            /// <summary>数组结束</summary>
            public const char ArrayEnd = ']';

            /// <summary>键值分隔符</summary>
            public const char KeyValueSeparator = ':';

            /// <summary>元素分隔符</summary>
            public const char ElementSeparator = ',';

            /// <summary>字符串引号</summary>
            public const char StringQuote = '"';

            /// <summary>转义字符</summary>
            public const char EscapeChar = '\\';

            /// <summary>空值</summary>
            public const string NullValue = "null";

            /// <summary>布尔真值</summary>
            public const string TrueValue = "true";

            /// <summary>布尔假值</summary>
            public const string FalseValue = "false";

            /// <summary>换行符 (美化输出)</summary>
            public const string NewLine = "\n";

            /// <summary>Windows 换行符</summary>
            public const string WindowsNewLine = "\r\n";

            /// <summary>Unicode 转义前缀</summary>
            public const string UnicodeEscapePrefix = "\\u";

            /// <summary>类型属性名 (多态)</summary>
            public const string TypePropertyName = "$type";

            /// <summary>引用属性名 (循环引用)</summary>
            public const string RefPropertyName = "$ref";

            /// <summary>ID 属性名 (循环引用)</summary>
            public const string IdPropertyName = "$id";

            /// <summary>值属性名 (包装)</summary>
            public const string ValuePropertyName = "$value";
        }

        #endregion

        #region 字符编码常量

        /// <summary>
        /// 字符编码常量集合
        /// <para>定义字符编码相关的常量</para>
        /// </summary>
        public static class Encoding
        {
            /// <summary>UTF-8 BOM</summary>
            public static readonly byte[] Utf8Bom = { 0xEF, 0xBB, 0xBF };

            /// <summary>UTF-16 LE BOM</summary>
            public static readonly byte[] Utf16LeBom = { 0xFF, 0xFE };

            /// <summary>UTF-16 BE BOM</summary>
            public static readonly byte[] Utf16BeBom = { 0xFE, 0xFF };

            /// <summary>UTF-32 LE BOM</summary>
            public static readonly byte[] Utf32LeBom = { 0xFF, 0xFE, 0x00, 0x00 };

            /// <summary>UTF-32 BE BOM</summary>
            public static readonly byte[] Utf32BeBom = { 0x00, 0x00, 0xFE, 0xFF };

            /// <summary>UTF-8 最大字节数 (单字符)</summary>
            public const int Utf8MaxBytesPerChar = 4;

            /// <summary>UTF-16 最大字节数 (单字符)</summary>
            public const int Utf16MaxBytesPerChar = 4;

            /// <summary>ASCII 最大值</summary>
            public const int AsciiMax = 127;

            /// <summary>UTF-8 单字节最大值</summary>
            public const int Utf8SingleByteMax = 0x7F;

            /// <summary>UTF-8 双字节最大值</summary>
            public const int Utf8TwoByteMax = 0x7FF;

            /// <summary>UTF-8 三字节最大值</summary>
            public const int Utf8ThreeByteMax = 0xFFFF;

            /// <summary>代理对起始值</summary>
            public const int SurrogateStart = 0xD800;

            /// <summary>代理对结束值</summary>
            public const int SurrogateEnd = 0xDFFF;

            /// <summary>高代理起始值</summary>
            public const int HighSurrogateStart = 0xD800;

            /// <summary>高代理结束值</summary>
            public const int HighSurrogateEnd = 0xDBFF;

            /// <summary>低代理起始值</summary>
            public const int LowSurrogateStart = 0xDC00;

            /// <summary>低代理结束值</summary>
            public const int LowSurrogateEnd = 0xDFFF;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 获取 VarInt 编码所需的字节数
        /// </summary>
        /// <param name="value">要编码的值</param>
        /// <returns>所需字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarIntSize(int value)
        {
            if (value < 0) return VarInt.MaxBytes32;
            if (value <= VarInt.MaxSingleByte) return 1;
            if (value <= VarInt.MaxTwoByte) return 2;
            if (value <= VarInt.MaxThreeByte) return 3;
            if (value <= VarInt.MaxFourByte) return 4;
            return 5;
        }

        /// <summary>
        /// 获取 VarInt 编码所需的字节数 (64位)
        /// </summary>
        /// <param name="value">要编码的值</param>
        /// <returns>所需字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarIntSize(long value)
        {
            if (value < 0) return VarInt.MaxBytes64;
            
            int size = 1;
            while (value > VarInt.DataMask)
            {
                value >>= VarInt.DataBits;
                size++;
            }
            return size;
        }

        /// <summary>
        /// 检查类型码是否为基础类型
        /// </summary>
        /// <param name="typeCode">类型码</param>
        /// <returns>如果是基础类型返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrimitiveTypeCode(byte typeCode)
        {
            return typeCode >= TypeCode.Null && typeCode <= TypeCode.Float16;
        }

        /// <summary>
        /// 检查类型码是否为字符串类型
        /// </summary>
        /// <param name="typeCode">类型码</param>
        /// <returns>如果是字符串类型返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStringTypeCode(byte typeCode)
        {
            return typeCode >= TypeCode.EmptyString && typeCode <= TypeCode.CompressedString;
        }

        /// <summary>
        /// 检查类型码是否为集合类型
        /// </summary>
        /// <param name="typeCode">类型码</param>
        /// <returns>如果是集合类型返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCollectionTypeCode(byte typeCode)
        {
            return typeCode >= TypeCode.EmptyArray && typeCode <= TypeCode.ImmutableCollection;
        }

        /// <summary>
        /// 检查类型码是否为 Unity 类型
        /// </summary>
        /// <param name="typeCode">类型码</param>
        /// <returns>如果是 Unity 类型返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnityTypeCode(byte typeCode)
        {
            return typeCode >= TypeCode.Vector2 && typeCode <= TypeCode.LayerMask;
        }

        /// <summary>
        /// 检查类型码是否为 ECS 类型
        /// </summary>
        /// <param name="typeCode">类型码</param>
        /// <returns>如果是 ECS 类型返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEcsTypeCode(byte typeCode)
        {
            return typeCode >= TypeCode.Entity && typeCode <= TypeCode.FixedString;
        }

        /// <summary>
        /// 检查字符是否需要 JSON 转义
        /// </summary>
        /// <param name="c">要检查的字符</param>
        /// <returns>如果需要转义返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NeedsJsonEscape(char c)
        {
            return c < 32 || c == '"' || c == '\\';
        }

        /// <summary>
        /// 获取 JSON 转义字符
        /// </summary>
        /// <param name="c">原始字符</param>
        /// <returns>转义后的字符，如果不需要转义返回 '\0'</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char GetJsonEscapeChar(char c)
        {
            return c switch
            {
                '"' => '"',
                '\\' => '\\',
                '\b' => 'b',
                '\f' => 'f',
                '\n' => 'n',
                '\r' => 'r',
                '\t' => 't',
                _ => '\0'
            };
        }

        /// <summary>
        /// 检查字符是否为 YAML 特殊字符
        /// </summary>
        /// <param name="c">要检查的字符</param>
        /// <returns>如果是特殊字符返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsYamlSpecialChar(char c)
        {
            return c == ':' || c == '#' || c == '&' || c == '*' || 
                   c == '!' || c == '|' || c == '>' || c == '\'' || 
                   c == '"' || c == '%' || c == '@' || c == '`' ||
                   c == '[' || c == ']' || c == '{' || c == '}' ||
                   c == ',' || c == '-' || c == '?';
        }

        /// <summary>
        /// 检查是否为 UTF-8 续字节
        /// </summary>
        /// <param name="b">要检查的字节</param>
        /// <returns>如果是续字节返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUtf8ContinuationByte(byte b)
        {
            return (b & 0xC0) == 0x80;
        }

        /// <summary>
        /// 获取 UTF-8 字符的字节数
        /// </summary>
        /// <param name="firstByte">首字节</param>
        /// <returns>字符的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetUtf8CharByteCount(byte firstByte)
        {
            if ((firstByte & 0x80) == 0) return 1;
            if ((firstByte & 0xE0) == 0xC0) return 2;
            if ((firstByte & 0xF0) == 0xE0) return 3;
            if ((firstByte & 0xF8) == 0xF0) return 4;
            return 1; // 无效字节，按单字节处理
        }

        #endregion
    }
}
