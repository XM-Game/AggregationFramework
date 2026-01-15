// ==========================================================
// 文件名：TypeCodeMap.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// 类型码映射器
    /// <para>提供 .NET 类型与二进制类型码之间的映射</para>
    /// <para>支持快速的类型查找和类型码解析</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 使用静态字典缓存类型映射，避免重复计算
    /// 2. 支持基础类型、集合类型、Unity 类型的映射
    /// 3. 提供类型码到类型的反向映射
    /// 
    /// 使用示例:
    /// <code>
    /// // 获取类型码
    /// byte code = TypeCodeMap.GetTypeCode(typeof(int));
    /// 
    /// // 获取类型
    /// Type type = TypeCodeMap.GetType(BinaryFormat.Int32);
    /// 
    /// // 检查是否为已知类型
    /// if (TypeCodeMap.IsKnownType(typeof(Vector3)))
    ///     // 使用优化路径
    /// </code>
    /// </remarks>
    internal static class TypeCodeMap
    {
        #region 静态字段

        /// <summary>类型到类型码的映射</summary>
        private static readonly Dictionary<Type, byte> s_typeToCode;

        /// <summary>类型码到类型的映射</summary>
        private static readonly Dictionary<byte, Type> s_codeToType;

        /// <summary>类型码到固定大小的映射</summary>
        private static readonly Dictionary<byte, int> s_codeToSize;

        #endregion

        #region 静态构造函数

        /// <summary>
        /// 静态构造函数，初始化类型映射
        /// </summary>
        static TypeCodeMap()
        {
            s_typeToCode = new Dictionary<Type, byte>(64);
            s_codeToType = new Dictionary<byte, Type>(64);
            s_codeToSize = new Dictionary<byte, int>(32);

            InitializePrimitiveTypes();
            InitializeSpecialTypes();
            InitializeCollectionTypes();

#if UNITY_2022_3_OR_NEWER
            InitializeUnityTypes();
#endif
        }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化基础类型映射
        /// </summary>
        private static void InitializePrimitiveTypes()
        {
            // 布尔类型
            RegisterType(typeof(bool), BinaryFormat.True, 1);

            // 整数类型
            RegisterType(typeof(sbyte), BinaryFormat.Int8, 1);
            RegisterType(typeof(byte), BinaryFormat.UInt8, 1);
            RegisterType(typeof(short), BinaryFormat.Int16, 2);
            RegisterType(typeof(ushort), BinaryFormat.UInt16, 2);
            RegisterType(typeof(int), BinaryFormat.Int32, 4);
            RegisterType(typeof(uint), BinaryFormat.UInt32, 4);
            RegisterType(typeof(long), BinaryFormat.Int64, 8);
            RegisterType(typeof(ulong), BinaryFormat.UInt64, 8);

            // 浮点类型
            RegisterType(typeof(float), BinaryFormat.Float32, 4);
            RegisterType(typeof(double), BinaryFormat.Float64, 8);
            RegisterType(typeof(decimal), BinaryFormat.Decimal, 16);

            // 字符类型
            RegisterType(typeof(char), BinaryFormat.Char, 2);

            // 字符串类型 (变长)
            RegisterType(typeof(string), BinaryFormat.LongString, -1);

            // 字节数组 (变长)
            RegisterType(typeof(byte[]), BinaryFormat.LongBytes, -1);
        }

        /// <summary>
        /// 初始化特殊类型映射
        /// </summary>
        private static void InitializeSpecialTypes()
        {
            RegisterType(typeof(DateTime), BinaryFormat.DateTime, 8);
            RegisterType(typeof(TimeSpan), BinaryFormat.TimeSpan, 8);
            RegisterType(typeof(Guid), BinaryFormat.Guid, 16);
            RegisterType(typeof(DateTimeOffset), BinaryFormat.DateTimeOffset, 12);

#if NET6_0_OR_GREATER
            RegisterType(typeof(DateOnly), BinaryFormat.DateOnly, 4);
            RegisterType(typeof(TimeOnly), BinaryFormat.TimeOnly, 8);
#endif

            RegisterType(typeof(Uri), BinaryFormat.Uri, -1);
            RegisterType(typeof(Version), BinaryFormat.Version, -1);
            RegisterType(typeof(Type), BinaryFormat.Type, -1);
        }

        /// <summary>
        /// 初始化集合类型映射
        /// </summary>
        private static void InitializeCollectionTypes()
        {
            // 注意：泛型集合类型需要特殊处理
            // 这里只注册类型码，不注册具体类型
            s_codeToSize[BinaryFormat.EmptyArray] = 0;
            s_codeToSize[BinaryFormat.Array] = -1;
            s_codeToSize[BinaryFormat.List] = -1;
            s_codeToSize[BinaryFormat.Dictionary] = -1;
            s_codeToSize[BinaryFormat.HashSet] = -1;
            s_codeToSize[BinaryFormat.Queue] = -1;
            s_codeToSize[BinaryFormat.Stack] = -1;
        }

#if UNITY_2022_3_OR_NEWER
        /// <summary>
        /// 初始化 Unity 类型映射
        /// </summary>
        private static void InitializeUnityTypes()
        {
            RegisterType(typeof(UnityEngine.Vector2), BinaryFormat.Vector2, 8);
            RegisterType(typeof(UnityEngine.Vector3), BinaryFormat.Vector3, 12);
            RegisterType(typeof(UnityEngine.Vector4), BinaryFormat.Vector4, 16);
            RegisterType(typeof(UnityEngine.Vector2Int), BinaryFormat.Vector2Int, 8);
            RegisterType(typeof(UnityEngine.Vector3Int), BinaryFormat.Vector3Int, 12);
            RegisterType(typeof(UnityEngine.Quaternion), BinaryFormat.Quaternion, 16);
            RegisterType(typeof(UnityEngine.Color), BinaryFormat.Color, 16);
            RegisterType(typeof(UnityEngine.Color32), BinaryFormat.Color32, 4);
            RegisterType(typeof(UnityEngine.Rect), BinaryFormat.Rect, 16);
            RegisterType(typeof(UnityEngine.RectInt), BinaryFormat.RectInt, 16);
            RegisterType(typeof(UnityEngine.Bounds), BinaryFormat.Bounds, 24);
            RegisterType(typeof(UnityEngine.BoundsInt), BinaryFormat.BoundsInt, 24);
            RegisterType(typeof(UnityEngine.Matrix4x4), BinaryFormat.Matrix4x4, 64);
            RegisterType(typeof(UnityEngine.LayerMask), BinaryFormat.LayerMask, 4);
        }
#endif

        /// <summary>
        /// 注册类型映射
        /// </summary>
        private static void RegisterType(Type type, byte code, int size)
        {
            s_typeToCode[type] = code;
            s_codeToType[code] = type;
            if (size >= 0)
                s_codeToSize[code] = size;
        }

        #endregion

        #region 公共方法 - 类型到类型码

        /// <summary>
        /// 获取类型的类型码
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型码，未知类型返回 Invalid</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetTypeCode(Type type)
        {
            if (type == null)
                return BinaryFormat.Null;

            if (s_typeToCode.TryGetValue(type, out byte code))
                return code;

            // 处理可空类型
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return BinaryFormat.Nullable;

            // 处理枚举类型
            if (type.IsEnum)
                return BinaryFormat.Enum;

            // 处理数组类型
            if (type.IsArray)
                return type.GetArrayRank() == 1 ? BinaryFormat.Array : BinaryFormat.MultiDimArray;

            // 处理泛型集合类型
            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                return GetGenericCollectionTypeCode(genericDef);
            }

            // 处理对象类型
            if (type.IsClass)
                return BinaryFormat.ObjectStart;

            // 处理结构体类型
            if (type.IsValueType)
                return BinaryFormat.Struct;

            return BinaryFormat.Invalid;
        }

        /// <summary>
        /// 获取泛型类型的类型码
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>类型码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetTypeCode<T>()
        {
            return TypeCodeCache<T>.Code;
        }

        /// <summary>
        /// 获取泛型集合的类型码
        /// </summary>
        private static byte GetGenericCollectionTypeCode(Type genericDef)
        {
            if (genericDef == typeof(List<>))
                return BinaryFormat.List;
            if (genericDef == typeof(Dictionary<,>))
                return BinaryFormat.Dictionary;
            if (genericDef == typeof(HashSet<>))
                return BinaryFormat.HashSet;
            if (genericDef == typeof(Queue<>))
                return BinaryFormat.Queue;
            if (genericDef == typeof(Stack<>))
                return BinaryFormat.Stack;
            if (genericDef == typeof(LinkedList<>))
                return BinaryFormat.LinkedList;
            if (genericDef == typeof(SortedDictionary<,>))
                return BinaryFormat.SortedDictionary;
            if (genericDef == typeof(SortedSet<>))
                return BinaryFormat.SortedSet;
            if (genericDef == typeof(Nullable<>))
                return BinaryFormat.Nullable;

            // 检查元组类型
            if (IsTupleType(genericDef))
                return BinaryFormat.Tuple;
            if (IsValueTupleType(genericDef))
                return BinaryFormat.ValueTuple;

            return BinaryFormat.ObjectStart;
        }

        /// <summary>
        /// 检查是否为元组类型
        /// </summary>
        private static bool IsTupleType(Type type)
        {
            return type == typeof(Tuple<>) ||
                   type == typeof(Tuple<,>) ||
                   type == typeof(Tuple<,,>) ||
                   type == typeof(Tuple<,,,>) ||
                   type == typeof(Tuple<,,,,>) ||
                   type == typeof(Tuple<,,,,,>) ||
                   type == typeof(Tuple<,,,,,,>) ||
                   type == typeof(Tuple<,,,,,,,>);
        }

        /// <summary>
        /// 检查是否为值元组类型
        /// </summary>
        private static bool IsValueTupleType(Type type)
        {
            return type == typeof(ValueTuple<>) ||
                   type == typeof(ValueTuple<,>) ||
                   type == typeof(ValueTuple<,,>) ||
                   type == typeof(ValueTuple<,,,>) ||
                   type == typeof(ValueTuple<,,,,>) ||
                   type == typeof(ValueTuple<,,,,,>) ||
                   type == typeof(ValueTuple<,,,,,,>) ||
                   type == typeof(ValueTuple<,,,,,,,>);
        }

        #endregion

        #region 公共方法 - 类型码到类型

        /// <summary>
        /// 获取类型码对应的类型
        /// </summary>
        /// <param name="code">类型码</param>
        /// <returns>类型，未知类型码返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetType(byte code)
        {
            return s_codeToType.TryGetValue(code, out Type type) ? type : null;
        }

        /// <summary>
        /// 尝试获取类型码对应的类型
        /// </summary>
        /// <param name="code">类型码</param>
        /// <param name="type">输出类型</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetType(byte code, out Type type)
        {
            return s_codeToType.TryGetValue(code, out type);
        }

        #endregion

        #region 公共方法 - 大小查询

        /// <summary>
        /// 获取类型码对应的固定大小
        /// </summary>
        /// <param name="code">类型码</param>
        /// <returns>固定大小，变长类型返回 -1</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFixedSize(byte code)
        {
            return s_codeToSize.TryGetValue(code, out int size) ? size : -1;
        }

        /// <summary>
        /// 检查类型码是否为固定大小类型
        /// </summary>
        /// <param name="code">类型码</param>
        /// <returns>是否为固定大小类型</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFixedSize(byte code)
        {
            return s_codeToSize.TryGetValue(code, out int size) && size >= 0;
        }

        #endregion

        #region 公共方法 - 类型检查

        /// <summary>
        /// 检查是否为已知类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是否为已知类型</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKnownType(Type type)
        {
            return type != null && s_typeToCode.ContainsKey(type);
        }

        /// <summary>
        /// 检查是否为已知类型码
        /// </summary>
        /// <param name="code">类型码</param>
        /// <returns>是否为已知类型码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKnownTypeCode(byte code)
        {
            return s_codeToType.ContainsKey(code);
        }

        /// <summary>
        /// 检查类型是否为基础类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是否为基础类型</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrimitiveType(Type type)
        {
            if (type == null) return false;
            if (s_typeToCode.TryGetValue(type, out byte code))
                return BinaryFormat.IsPrimitiveType(code);
            return false;
        }

        /// <summary>
        /// 检查类型是否为非托管类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是否为非托管类型</returns>
        public static bool IsUnmanagedType(Type type)
        {
            if (type == null) return false;
            if (!type.IsValueType) return false;
            if (type.IsPrimitive) return true;
            if (type.IsEnum) return true;
            if (type.IsPointer) return true;

            // 检查结构体的所有字段
            foreach (var field in type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic))
            {
                if (!IsUnmanagedType(field.FieldType))
                    return false;
            }

            return true;
        }

        #endregion

        #region 泛型类型码缓存

        /// <summary>
        /// 泛型类型码缓存
        /// </summary>
        private static class TypeCodeCache<T>
        {
            public static readonly byte Code = GetTypeCode(typeof(T));
        }

        #endregion
    }
}
