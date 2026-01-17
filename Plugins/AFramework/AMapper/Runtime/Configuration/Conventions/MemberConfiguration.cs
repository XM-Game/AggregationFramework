// ==========================================================
// 文件名：MemberConfiguration.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Reflection
// 功能: 成员匹配配置，定义成员自动匹配的规则
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 成员匹配配置
    /// <para>定义成员自动匹配的规则</para>
    /// <para>Member configuration defining auto-matching rules</para>
    /// </summary>
    public sealed class MemberConfiguration
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取源成员命名约定
        /// <para>Get source member naming convention</para>
        /// </summary>
        public INamingConvention SourceNamingConvention { get; set; }

        /// <summary>
        /// 获取目标成员命名约定
        /// <para>Get destination member naming convention</para>
        /// </summary>
        public INamingConvention DestinationNamingConvention { get; set; }

        /// <summary>
        /// 获取源成员前缀列表
        /// <para>Get source member prefix list</para>
        /// </summary>
        public IList<string> SourcePrefixes { get; }

        /// <summary>
        /// 获取源成员后缀列表
        /// <para>Get source member postfix list</para>
        /// </summary>
        public IList<string> SourcePostfixes { get; }

        /// <summary>
        /// 获取目标成员前缀列表
        /// <para>Get destination member prefix list</para>
        /// </summary>
        public IList<string> DestinationPrefixes { get; }

        /// <summary>
        /// 获取目标成员后缀列表
        /// <para>Get destination member postfix list</para>
        /// </summary>
        public IList<string> DestinationPostfixes { get; }

        /// <summary>
        /// 获取是否启用扁平化
        /// <para>Get whether flattening is enabled</para>
        /// </summary>
        public bool EnableFlattening { get; set; } = true;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建成员匹配配置实例
        /// </summary>
        public MemberConfiguration()
        {
            SourceNamingConvention = PascalCaseNamingConvention.Instance;
            DestinationNamingConvention = PascalCaseNamingConvention.Instance;
            SourcePrefixes = new List<string>();
            SourcePostfixes = new List<string>();
            DestinationPrefixes = new List<string>();
            DestinationPostfixes = new List<string>();
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 尝试匹配源成员
        /// <para>Try to match source member</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationMemberName">目标成员名称 / Destination member name</param>
        /// <returns>匹配的源成员或 null / Matched source member or null</returns>
        public MemberInfo[] TryMatch(Type sourceType, string destinationMemberName)
        {
            if (sourceType == null || string.IsNullOrEmpty(destinationMemberName))
                return null;

            // 1. 精确匹配
            var exactMatch = FindMember(sourceType, destinationMemberName);
            if (exactMatch != null)
                return new[] { exactMatch };

            // 2. 处理前缀/后缀
            var processedName = ProcessName(destinationMemberName, DestinationPrefixes, DestinationPostfixes);
            if (processedName != destinationMemberName)
            {
                exactMatch = FindMember(sourceType, processedName);
                if (exactMatch != null)
                    return new[] { exactMatch };
            }

            // 3. 扁平化匹配
            if (EnableFlattening)
            {
                var flattenMatch = TryFlattenMatch(sourceType, destinationMemberName);
                if (flattenMatch != null)
                    return flattenMatch;
            }

            return null;
        }

        #endregion

        #region 私有方法 / Private Methods

        private MemberInfo FindMember(Type type, string name)
        {
            // 尝试属性
            var property = type.GetProperty(name, 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
                return property;

            // 尝试字段
            var field = type.GetField(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field;

            // 尝试 Get 方法
            var method = type.GetMethod($"Get{name}",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase,
                null, Type.EmptyTypes, null);
            if (method != null && method.ReturnType != typeof(void))
                return method;

            return null;
        }

        private MemberInfo[] TryFlattenMatch(Type sourceType, string destinationMemberName)
        {
            // 分词
            var tokens = DestinationNamingConvention.Split(destinationMemberName).ToList();
            if (tokens.Count < 2)
                return null;

            // 尝试逐级匹配
            var currentType = sourceType;
            var path = new List<MemberInfo>();

            for (int i = 0; i < tokens.Count; i++)
            {
                // 尝试从当前位置开始的不同长度组合
                for (int len = 1; len <= tokens.Count - i; len++)
                {
                    var combinedName = string.Concat(tokens.Skip(i).Take(len));
                    var member = FindMember(currentType, combinedName);

                    if (member != null)
                    {
                        path.Add(member);
                        currentType = GetMemberType(member);
                        i += len - 1;
                        break;
                    }
                }

                if (path.Count == 0 || (i < tokens.Count - 1 && path.Count == i))
                {
                    // 匹配失败
                    return null;
                }
            }

            return path.Count > 0 ? path.ToArray() : null;
        }

        private static string ProcessName(string name, IList<string> prefixes, IList<string> postfixes)
        {
            var result = name;

            // 移除前缀
            foreach (var prefix in prefixes)
            {
                if (result.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Substring(prefix.Length);
                    break;
                }
            }

            // 移除后缀
            foreach (var postfix in postfixes)
            {
                if (result.EndsWith(postfix, StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Substring(0, result.Length - postfix.Length);
                    break;
                }
            }

            return result;
        }

        private static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                MethodInfo method => method.ReturnType,
                _ => null
            };
        }

        #endregion
    }
}
