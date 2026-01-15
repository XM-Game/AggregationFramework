// ==========================================================
// 文件名：BinaryFormat.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 二进制格式定义
    /// <para>定义二进制序列化的格式规范和类型码</para>
    /// <para>提供格式版本、类型标识、特殊标记等常量</para>
    /// </summary>
    /// <remarks>
    /// 二进制格式布局:
    /// <code>
    /// [Header]     - 数据头 (可选，32字节)
    /// [TypeCode]   - 类型码 (1-2字节)
    /// [Length]     - 长度前缀 (VarInt，1-5字节)
    /// [Data]       - 实际数据
    /// </code>
    /// 
    /// 使用示例:
    /// <code>
    /// // 检查类型码
    /// if (BinaryFormat.IsFixedSizeType(typeCode))
    ///     return BinaryFormat.GetFixedTypeSize(typeCode);
    /// 
    /// // 获取类型码
    /// byte code = BinaryFormat.GetTypeCode&lt;int&gt;();
    /// </code>
    /// </remarks>
    public static class BinaryFormat
    {
        #region 格式版本常量

        /// <summary>格式主版本号</summary>
        public const byte MajorVersion = 1;

        /// <summary>格式次版本号</summary>
        public const byte MinorVersion = 0;

        /// <summary>格式版本 (组合)</summary>
        public const ushort Version = (MajorVersion << 8) | MinorVersion;

        /// <summary>最小兼容版本</summary>
        public const ushort MinCompatibleVersion = 0x0100; // 1.0

        #endregion

        #region 类型码常量 - 基础类型 (0x00 - 0x1F)

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

        #endregion

        #region 类型码常量 - 变长整数 (0x10 - 0x17)

        /// <summary>变长32位有符号整数</summary>
        public const byte VarInt32 = 0x10;

        /// <summary>变长32位无符号整数</summary>
        public const byte VarUInt32 = 0x11;

        /// <summary>变长64位有符号整数</summary>
        public const byte VarInt64 = 0x12;

        /// <summary>变长64位无符号整数</summary>
        public const byte VarUInt64 = 0x13;

        /// <summary>ZigZag编码32位整数</summary>
        public const byte ZigZagInt32 = 0x14;

        /// <summary>ZigZag编码64位整数</summary>
        public const byte ZigZagInt64 = 0x15;

        #endregion

        #region 类型码常量 - 字符串 (0x20 - 0x2F)

        /// <summary>空字符串</summary>
        public const byte EmptyString = 0x20;

        /// <summary>短字符串 (长度 1-31，内联长度)</summary>
        public const byte ShortString = 0x21;

        /// <summary>中等字符串 (长度 32-255)</summary>
        public const byte MediumString = 0x22;

        /// <summary>长字符串 (长度 256+)</summary>
        public const byte LongString = 0x23;

        /// <summary>UTF-16 字符串</summary>
        public const byte Utf16String = 0x24;

        /// <summary>内化字符串引用</summary>
        public const byte InternedString = 0x25;

        /// <summary>压缩字符串</summary>
        public const byte CompressedString = 0x26;

        /// <summary>ASCII 字符串</summary>
        public const byte AsciiString = 0x27;

        #endregion

        #region 类型码常量 - 字节数组 (0x30 - 0x3F)

        /// <summary>空字节数组</summary>
        public const byte EmptyBytes = 0x30;

        /// <summary>短字节数组 (长度 1-255)</summary>
        public const byte ShortBytes = 0x31;

        /// <summary>长字节数组 (长度 256+)</summary>
        public const byte LongBytes = 0x32;

        /// <summary>压缩字节数组</summary>
        public const byte CompressedBytes = 0x33;

        #endregion

        #region 类型码常量 - 集合 (0x40 - 0x5F)

        /// <summary>空数组</summary>
        public const byte EmptyArray = 0x40;

        /// <summary>数组</summary>
        public const byte Array = 0x41;

        /// <summary>列表</summary>
        public const byte List = 0x42;

        /// <summary>字典</summary>
        public const byte Dictionary = 0x43;

        /// <summary>哈希集合</summary>
        public const byte HashSet = 0x44;

        /// <summary>队列</summary>
        public const byte Queue = 0x45;

        /// <summary>栈</summary>
        public const byte Stack = 0x46;

        /// <summary>链表</summary>
        public const byte LinkedList = 0x47;

        /// <summary>多维数组</summary>
        public const byte MultiDimArray = 0x48;

        /// <summary>交错数组</summary>
        public const byte JaggedArray = 0x49;

        /// <summary>只读集合</summary>
        public const byte ReadOnlyCollection = 0x4A;

        /// <summary>不可变集合</summary>
        public const byte ImmutableCollection = 0x4B;

        /// <summary>排序字典</summary>
        public const byte SortedDictionary = 0x4C;

        /// <summary>排序集合</summary>
        public const byte SortedSet = 0x4D;

        #endregion

        #region 类型码常量 - 特殊类型 (0x60 - 0x7F)

        /// <summary>日期时间</summary>
        public const byte DateTime = 0x60;

        /// <summary>时间间隔</summary>
        public const byte TimeSpan = 0x61;

        /// <summary>GUID</summary>
        public const byte Guid = 0x62;

        /// <summary>日期时间偏移</summary>
        public const byte DateTimeOffset = 0x63;

        /// <summary>仅日期</summary>
        public const byte DateOnly = 0x64;

        /// <summary>仅时间</summary>
        public const byte TimeOnly = 0x65;

        /// <summary>URI</summary>
        public const byte Uri = 0x66;

        /// <summary>版本号</summary>
        public const byte Version = 0x67;

        /// <summary>大整数</summary>
        public const byte BigInteger = 0x68;

        /// <summary>复数</summary>
        public const byte Complex = 0x69;

        /// <summary>类型信息</summary>
        public const byte Type = 0x6A;

        /// <summary>枚举</summary>
        public const byte Enum = 0x6B;

        /// <summary>可空类型</summary>
        public const byte Nullable = 0x6C;

        #endregion

        #region 类型码常量 - 对象 (0x80 - 0x9F)

        /// <summary>对象开始</summary>
        public const byte ObjectStart = 0x80;

        /// <summary>对象结束</summary>
        public const byte ObjectEnd = 0x81;

        /// <summary>对象引用</summary>
        public const byte ObjectReference = 0x82;

        /// <summary>多态对象</summary>
        public const byte PolymorphicObject = 0x83;

        /// <summary>匿名对象</summary>
        public const byte AnonymousObject = 0x84;

        /// <summary>动态对象</summary>
        public const byte DynamicObject = 0x85;

        /// <summary>元组</summary>
        public const byte Tuple = 0x86;

        /// <summary>值元组</summary>
        public const byte ValueTuple = 0x87;

        /// <summary>结构体</summary>
        public const byte Struct = 0x88;

        /// <summary>记录类型</summary>
        public const byte Record = 0x89;

        #endregion

        #region 类型码常量 - Unity 类型 (0xA0 - 0xBF)

        /// <summary>Vector2</summary>
        public const byte Vector2 = 0xA0;

        /// <summary>Vector3</summary>
        public const byte Vector3 = 0xA1;

        /// <summary>Vector4</summary>
        public const byte Vector4 = 0xA2;

        /// <summary>Vector2Int</summary>
        public const byte Vector2Int = 0xA3;

        /// <summary>Vector3Int</summary>
        public const byte Vector3Int = 0xA4;

        /// <summary>Quaternion</summary>
        public const byte Quaternion = 0xA5;

        /// <summary>Color</summary>
        public const byte Color = 0xA6;

        /// <summary>Color32</summary>
        public const byte Color32 = 0xA7;

        /// <summary>Rect</summary>
        public const byte Rect = 0xA8;

        /// <summary>RectInt</summary>
        public const byte RectInt = 0xA9;

        /// <summary>Bounds</summary>
        public const byte Bounds = 0xAA;

        /// <summary>BoundsInt</summary>
        public const byte BoundsInt = 0xAB;

        /// <summary>Matrix4x4</summary>
        public const byte Matrix4x4 = 0xAC;

        /// <summary>AnimationCurve</summary>
        public const byte AnimationCurve = 0xAD;

        /// <summary>Gradient</summary>
        public const byte Gradient = 0xAE;

        /// <summary>LayerMask</summary>
        public const byte LayerMask = 0xAF;

        #endregion

        #region 类型码常量 - 控制码 (0xF0 - 0xFF)

        /// <summary>扩展类型标记</summary>
        public const byte Extension = 0xF0;

        /// <summary>自定义类型</summary>
        public const byte Custom = 0xF1;

        /// <summary>压缩数据块</summary>
        public const byte CompressedBlock = 0xF2;

        /// <summary>加密数据块</summary>
        public const byte EncryptedBlock = 0xF3;

        /// <summary>流结束标记</summary>
        public const byte EndOfStream = 0xFE;

        /// <summary>无效/保留</summary>
        public const byte Invalid = 0xFF;

        #endregion

        #region 特殊标记常量

        /// <summary>短字符串长度掩码 (低5位)</summary>
        public const byte ShortStringLengthMask = 0x1F;

        /// <summary>短字符串最大长度</summary>
        public const int MaxShortStringLength = 31;

        /// <summary>中等字符串最大长度</summary>
        public const int MaxMediumStringLength = 255;

        /// <summary>短字节数组最大长度</summary>
        public const int MaxShortBytesLength = 255;

        /// <summary>内联数组最大长度</summary>
        public const int MaxInlineArrayLength = 15;

        /// <summary>小整数范围 (-32 到 127)</summary>
        public const int SmallIntMin = -32;
        public const int SmallIntMax = 127;

        #endregion

        #region 工具方法

        /// <summary>
        /// 检查类型码是否为固定大小类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFixedSizeType(byte typeCode)
        {
            return typeCode switch
            {
                Null or False or True => true,
                Int8 or UInt8 => true,
                Int16 or UInt16 or Char or Float16 => true,
                Int32 or UInt32 or Float32 => true,
                Int64 or UInt64 or Float64 or DateTime or TimeSpan => true,
                Decimal or Guid or DateTimeOffset => true,
                Vector2 or Vector2Int => true,
                Vector3 or Vector3Int => true,
                Vector4 or Quaternion or Color or Rect or RectInt => true,
                Color32 or LayerMask => true,
                Bounds or BoundsInt => true,
                Matrix4x4 => true,
                _ => false
            };
        }

        /// <summary>
        /// 获取固定大小类型的字节数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFixedTypeSize(byte typeCode)
        {
            return typeCode switch
            {
                Null or False or True => 0,
                Int8 or UInt8 => 1,
                Int16 or UInt16 or Char or Float16 => 2,
                Int32 or UInt32 or Float32 or Color32 or LayerMask => 4,
                Int64 or UInt64 or Float64 or DateTime or TimeSpan or Vector2 or Vector2Int => 8,
                Vector3 or Vector3Int => 12,
                Decimal or Guid or Vector4 or Quaternion or Color or Rect or RectInt => 16,
                DateTimeOffset => 12,
                Bounds or BoundsInt => 24,
                Matrix4x4 => 64,
                _ => -1
            };
        }

        /// <summary>
        /// 检查类型码是否为基础类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrimitiveType(byte typeCode)
        {
            return typeCode >= Null && typeCode <= Float16;
        }

        /// <summary>
        /// 检查类型码是否为变长整数类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVarIntType(byte typeCode)
        {
            return typeCode >= VarInt32 && typeCode <= ZigZagInt64;
        }

        /// <summary>
        /// 检查类型码是否为字符串类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStringType(byte typeCode)
        {
            return typeCode >= EmptyString && typeCode <= AsciiString;
        }

        /// <summary>
        /// 检查类型码是否为字节数组类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBytesType(byte typeCode)
        {
            return typeCode >= EmptyBytes && typeCode <= CompressedBytes;
        }

        /// <summary>
        /// 检查类型码是否为集合类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCollectionType(byte typeCode)
        {
            return typeCode >= EmptyArray && typeCode <= SortedSet;
        }

        /// <summary>
        /// 检查类型码是否为特殊类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSpecialType(byte typeCode)
        {
            return typeCode >= DateTime && typeCode <= Nullable;
        }

        /// <summary>
        /// 检查类型码是否为对象类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObjectType(byte typeCode)
        {
            return typeCode >= ObjectStart && typeCode <= Record;
        }

        /// <summary>
        /// 检查类型码是否为 Unity 类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnityType(byte typeCode)
        {
            return typeCode >= Vector2 && typeCode <= LayerMask;
        }

        /// <summary>
        /// 检查类型码是否为控制码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsControlCode(byte typeCode)
        {
            return typeCode >= Extension;
        }

        /// <summary>
        /// 获取类型码的名称
        /// </summary>
        public static string GetTypeCodeName(byte typeCode)
        {
            return typeCode switch
            {
                Null => "Null",
                False => "False",
                True => "True",
                Int8 => "Int8",
                UInt8 => "UInt8",
                Int16 => "Int16",
                UInt16 => "UInt16",
                Int32 => "Int32",
                UInt32 => "UInt32",
                Int64 => "Int64",
                UInt64 => "UInt64",
                Float32 => "Float32",
                Float64 => "Float64",
                Decimal => "Decimal",
                Char => "Char",
                Float16 => "Float16",
                VarInt32 => "VarInt32",
                VarUInt32 => "VarUInt32",
                VarInt64 => "VarInt64",
                VarUInt64 => "VarUInt64",
                ZigZagInt32 => "ZigZagInt32",
                ZigZagInt64 => "ZigZagInt64",
                EmptyString => "EmptyString",
                ShortString => "ShortString",
                MediumString => "MediumString",
                LongString => "LongString",
                Utf16String => "Utf16String",
                InternedString => "InternedString",
                CompressedString => "CompressedString",
                AsciiString => "AsciiString",
                EmptyBytes => "EmptyBytes",
                ShortBytes => "ShortBytes",
                LongBytes => "LongBytes",
                CompressedBytes => "CompressedBytes",
                EmptyArray => "EmptyArray",
                Array => "Array",
                List => "List",
                Dictionary => "Dictionary",
                HashSet => "HashSet",
                Queue => "Queue",
                Stack => "Stack",
                LinkedList => "LinkedList",
                MultiDimArray => "MultiDimArray",
                JaggedArray => "JaggedArray",
                DateTime => "DateTime",
                TimeSpan => "TimeSpan",
                Guid => "Guid",
                DateTimeOffset => "DateTimeOffset",
                DateOnly => "DateOnly",
                TimeOnly => "TimeOnly",
                Uri => "Uri",
                Version => "Version",
                BigInteger => "BigInteger",
                Complex => "Complex",
                Type => "Type",
                Enum => "Enum",
                Nullable => "Nullable",
                ObjectStart => "ObjectStart",
                ObjectEnd => "ObjectEnd",
                ObjectReference => "ObjectReference",
                PolymorphicObject => "PolymorphicObject",
                AnonymousObject => "AnonymousObject",
                DynamicObject => "DynamicObject",
                Tuple => "Tuple",
                ValueTuple => "ValueTuple",
                Struct => "Struct",
                Record => "Record",
                Vector2 => "Vector2",
                Vector3 => "Vector3",
                Vector4 => "Vector4",
                Vector2Int => "Vector2Int",
                Vector3Int => "Vector3Int",
                Quaternion => "Quaternion",
                Color => "Color",
                Color32 => "Color32",
                Rect => "Rect",
                RectInt => "RectInt",
                Bounds => "Bounds",
                BoundsInt => "BoundsInt",
                Matrix4x4 => "Matrix4x4",
                AnimationCurve => "AnimationCurve",
                Gradient => "Gradient",
                LayerMask => "LayerMask",
                Extension => "Extension",
                Custom => "Custom",
                CompressedBlock => "CompressedBlock",
                EncryptedBlock => "EncryptedBlock",
                EndOfStream => "EndOfStream",
                Invalid => "Invalid",
                _ => $"Unknown(0x{typeCode:X2})"
            };
        }

        #endregion
    }
}
