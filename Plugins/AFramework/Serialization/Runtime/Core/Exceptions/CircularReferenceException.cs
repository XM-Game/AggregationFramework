// ==========================================================
// 文件名：CircularReferenceException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 循环引用异常
    /// <para>当序列化过程中检测到对象循环引用时抛出</para>
    /// </summary>
    /// <remarks>
    /// 循环引用场景:
    /// <list type="bullet">
    /// <item>对象 A 引用对象 B，对象 B 又引用对象 A</item>
    /// <item>对象自引用（对象引用自身）</item>
    /// <item>多层嵌套形成的引用环</item>
    /// </list>
    /// 
    /// 解决方案:
    /// <list type="number">
    /// <item>使用 CircularReference 序列化模式</item>
    /// <item>使用 [SerializeIgnore] 标记打破循环</item>
    /// <item>重新设计数据结构避免循环</item>
    /// <item>设置最大深度限制</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     serializer.Serialize(objectWithCircularRef);
    /// }
    /// catch (CircularReferenceException ex)
    /// {
    ///     Console.WriteLine($"检测到循环引用，深度: {ex.Depth}");
    ///     Console.WriteLine($"引用路径: {ex.GetReferencePath()}");
    ///     
    ///     // 使用支持循环引用的模式重试
    ///     var options = new SerializeOptions { Mode = SerializeMode.CircularReference };
    ///     serializer.Serialize(objectWithCircularRef, options);
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public sealed class CircularReferenceException : SerializationException
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息
        /// </summary>
        private const string DefaultMessage = "检测到循环引用";

        /// <summary>
        /// 默认最大深度
        /// </summary>
        public const int DefaultMaxDepth = 64;

        #endregion

        #region 字段

        /// <summary>
        /// 引用深度
        /// </summary>
        private readonly int _depth;

        /// <summary>
        /// 最大允许深度
        /// </summary>
        private readonly int _maxDepth;

        /// <summary>
        /// 循环引用的对象类型
        /// </summary>
        private readonly Type _circularType;

        /// <summary>
        /// 引用路径（类型名称列表）
        /// </summary>
        private readonly string[] _referencePath;

        /// <summary>
        /// 是否为自引用
        /// </summary>
        private readonly bool _isSelfReference;

        #endregion

        #region 属性

        /// <summary>
        /// 获取引用深度
        /// </summary>
        /// <value>检测到循环时的嵌套深度</value>
        public int Depth => _depth;

        /// <summary>
        /// 获取最大允许深度
        /// </summary>
        /// <value>配置的最大嵌套深度</value>
        public int MaxDepth => _maxDepth;

        /// <summary>
        /// 获取循环引用的对象类型
        /// </summary>
        /// <value>形成循环的对象类型</value>
        public Type CircularType => _circularType;

        /// <summary>
        /// 获取引用路径
        /// </summary>
        /// <value>从根对象到循环点的类型路径</value>
        public string[] ReferencePath => _referencePath;

        /// <summary>
        /// 获取是否为自引用
        /// </summary>
        /// <value>true 表示对象引用自身</value>
        public bool IsSelfReference => _isSelfReference;

        /// <summary>
        /// 获取循环长度
        /// </summary>
        /// <value>循环中涉及的对象数量</value>
        public int CycleLength => _referencePath?.Length ?? 0;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建默认循环引用异常
        /// </summary>
        public CircularReferenceException()
            : this(DefaultMessage)
        {
        }

        /// <summary>
        /// 使用消息创建异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public CircularReferenceException(string message)
            : base(SerializeErrorCode.CircularReference, message ?? DefaultMessage)
        {
            _maxDepth = DefaultMaxDepth;
        }

        /// <summary>
        /// 使用循环类型创建异常
        /// </summary>
        /// <param name="circularType">循环引用的类型</param>
        public CircularReferenceException(Type circularType)
            : this(circularType, 0, DefaultMaxDepth)
        {
        }

        /// <summary>
        /// 使用循环类型和深度创建异常
        /// </summary>
        /// <param name="circularType">循环引用的类型</param>
        /// <param name="depth">当前深度</param>
        /// <param name="maxDepth">最大深度</param>
        public CircularReferenceException(Type circularType, int depth, int maxDepth)
            : base(
                SerializeErrorCode.CircularReference,
                $"检测到类型 '{circularType?.FullName ?? "unknown"}' 的循环引用，深度: {depth}",
                circularType)
        {
            _circularType = circularType;
            _depth = depth;
            _maxDepth = maxDepth;
            _isSelfReference = false;
        }

        /// <summary>
        /// 使用完整信息创建异常
        /// </summary>
        /// <param name="circularType">循环引用的类型</param>
        /// <param name="depth">当前深度</param>
        /// <param name="maxDepth">最大深度</param>
        /// <param name="referencePath">引用路径</param>
        /// <param name="isSelfReference">是否自引用</param>
        /// <param name="innerException">内部异常</param>
        public CircularReferenceException(
            Type circularType,
            int depth,
            int maxDepth,
            string[] referencePath,
            bool isSelfReference = false,
            Exception innerException = null)
            : base(
                SerializeErrorCode.CircularReference,
                FormatMessage(circularType, depth, isSelfReference),
                circularType,
                null,
                -1,
                innerException)
        {
            _circularType = circularType;
            _depth = depth;
            _maxDepth = maxDepth;
            _referencePath = referencePath;
            _isSelfReference = isSelfReference;
        }

        /// <summary>
        /// 序列化构造函数
        /// </summary>
        private CircularReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _depth = info.GetInt32(nameof(Depth));
            _maxDepth = info.GetInt32(nameof(MaxDepth));
            _isSelfReference = info.GetBoolean(nameof(IsSelfReference));
            _referencePath = (string[])info.GetValue(nameof(ReferencePath), typeof(string[]));
            
            var typeName = info.GetString(nameof(CircularType));
            if (!string.IsNullOrEmpty(typeName))
            {
                _circularType = Type.GetType(typeName);
            }
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Depth), _depth);
            info.AddValue(nameof(MaxDepth), _maxDepth);
            info.AddValue(nameof(IsSelfReference), _isSelfReference);
            info.AddValue(nameof(ReferencePath), _referencePath);
            info.AddValue(nameof(CircularType), _circularType?.AssemblyQualifiedName);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 格式化错误消息
        /// </summary>
        private static string FormatMessage(Type type, int depth, bool isSelfReference)
        {
            if (isSelfReference)
            {
                return $"检测到类型 '{type?.FullName ?? "unknown"}' 的自引用";
            }
            return $"检测到类型 '{type?.FullName ?? "unknown"}' 的循环引用，深度: {depth}";
        }

        /// <summary>
        /// 获取引用路径字符串
        /// </summary>
        /// <returns>格式化的引用路径</returns>
        public string GetReferencePath()
        {
            if (_referencePath == null || _referencePath.Length == 0)
            {
                return _circularType?.Name ?? "unknown";
            }
            return string.Join(" -> ", _referencePath);
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 循环引用诊断信息 ===");
            builder.AppendLine($"循环类型: {_circularType?.FullName ?? "unknown"}");
            builder.AppendLine($"当前深度: {_depth}");
            builder.AppendLine($"最大深度: {_maxDepth}");
            builder.AppendLine($"是否自引用: {_isSelfReference}");
            builder.AppendLine($"循环长度: {CycleLength}");
            
            if (_referencePath != null && _referencePath.Length > 0)
            {
                builder.AppendLine("引用路径:");
                for (int i = 0; i < _referencePath.Length; i++)
                {
                    builder.AppendLine($"  [{i}] {_referencePath[i]}");
                }
            }
            
            builder.AppendLine("解决方案:");
            builder.AppendLine("  1. 使用 SerializeMode.CircularReference 模式");
            builder.AppendLine("  2. 使用 [SerializeIgnore] 标记打破循环");
            builder.AppendLine("  3. 重新设计数据结构避免循环引用");
            builder.AppendLine("  4. 增加 MaxDepth 配置（如果是深度问题）");
            
            return builder.ToString();
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建自引用异常
        /// </summary>
        /// <param name="type">自引用的类型</param>
        /// <returns>循环引用异常</returns>
        public static CircularReferenceException SelfReference(Type type)
        {
            return new CircularReferenceException(
                type,
                1,
                DefaultMaxDepth,
                new[] { type?.Name ?? "unknown" },
                true);
        }

        /// <summary>
        /// 创建自引用异常（泛型版本）
        /// </summary>
        /// <typeparam name="T">自引用的类型</typeparam>
        /// <returns>循环引用异常</returns>
        public static CircularReferenceException SelfReference<T>()
        {
            return SelfReference(typeof(T));
        }

        /// <summary>
        /// 创建超出最大深度异常
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="depth">当前深度</param>
        /// <param name="maxDepth">最大深度</param>
        /// <returns>循环引用异常</returns>
        public static CircularReferenceException MaxDepthExceeded(Type type, int depth, int maxDepth)
        {
            return new CircularReferenceException(
                type,
                depth,
                maxDepth,
                null,
                false)
            {
                // 使用不同的错误码
            };
        }

        /// <summary>
        /// 从引用路径创建异常
        /// </summary>
        /// <param name="path">引用路径</param>
        /// <returns>循环引用异常</returns>
        public static CircularReferenceException FromPath(IEnumerable<Type> path)
        {
            var pathList = new List<string>();
            Type lastType = null;
            
            foreach (var type in path)
            {
                pathList.Add(type?.Name ?? "unknown");
                lastType = type;
            }
            
            return new CircularReferenceException(
                lastType,
                pathList.Count,
                DefaultMaxDepth,
                pathList.ToArray(),
                false);
        }

        /// <summary>
        /// 从引用路径创建异常（字符串版本）
        /// </summary>
        /// <param name="path">引用路径字符串数组</param>
        /// <param name="circularType">循环类型</param>
        /// <returns>循环引用异常</returns>
        public static CircularReferenceException FromPath(string[] path, Type circularType = null)
        {
            return new CircularReferenceException(
                circularType,
                path?.Length ?? 0,
                DefaultMaxDepth,
                path,
                false);
        }

        #endregion
    }
}
