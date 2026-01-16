// ==========================================================
// 文件名：TypeHelper.cs
// 命名空间: AFramework.DI.Internal
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.DI.Internal
{
    /// <summary>
    /// 类型辅助工具
    /// <para>提供类型相关的实用方法</para>
    /// </summary>
    internal static class TypeHelper
    {
        #region 系统接口过滤

        /// <summary>
        /// 需要排除的系统接口
        /// </summary>
        private static readonly HashSet<Type> ExcludedInterfaces = new()
        {
            typeof(IDisposable),
            typeof(IComparable),
            typeof(IFormattable),
            typeof(IConvertible),
            typeof(ICloneable),
            typeof(IEquatable<>)
        };

        #endregion

        #region 接口获取

        /// <summary>
        /// 获取类型实现的所有接口（排除系统接口）
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>接口集合</returns>
        public static IEnumerable<Type> GetImplementedInterfaces(Type type)
        {
            if (type == null) yield break;

            foreach (var iface in type.GetInterfaces())
            {
                if (ShouldIncludeInterface(iface))
                {
                    yield return iface;
                }
            }
        }

        /// <summary>
        /// 检查接口是否应该包含
        /// </summary>
        private static bool ShouldIncludeInterface(Type interfaceType)
        {
            // 排除系统命名空间的接口
            if (interfaceType.Namespace?.StartsWith("System") == true)
            {
                return false;
            }

            // 排除特定接口
            if (ExcludedInterfaces.Contains(interfaceType))
            {
                return false;
            }

            // 排除泛型定义的特定接口
            if (interfaceType.IsGenericType)
            {
                var genericDef = interfaceType.GetGenericTypeDefinition();
                if (ExcludedInterfaces.Contains(genericDef))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region 类型检查

        /// <summary>
        /// 检查类型是否可实例化
        /// </summary>
        public static bool IsInstantiable(Type type)
        {
            return type != null &&
                   !type.IsAbstract &&
                   !type.IsInterface &&
                   !type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// 检查类型是否为开放泛型
        /// </summary>
        public static bool IsOpenGeneric(Type type)
        {
            return type != null && type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// 检查类型是否为封闭泛型
        /// </summary>
        public static bool IsClosedGeneric(Type type)
        {
            return type != null && type.IsGenericType && !type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// 获取类型的友好名称
        /// </summary>
        public static string GetFriendlyName(Type type)
        {
            if (type == null) return "null";

            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var genericArgs = type.GetGenericArguments();
            var typeName = type.Name;
            var backtickIndex = typeName.IndexOf('`');
            if (backtickIndex > 0)
            {
                typeName = typeName.Substring(0, backtickIndex);
            }

            var argNames = genericArgs.Select(GetFriendlyName);
            return $"{typeName}<{string.Join(", ", argNames)}>";
        }

        #endregion

        #region 泛型处理

        /// <summary>
        /// 尝试构造封闭泛型类型
        /// </summary>
        public static bool TryMakeGenericType(Type openGenericType, Type[] typeArguments, out Type closedType)
        {
            closedType = null;

            if (openGenericType == null || !openGenericType.IsGenericTypeDefinition)
                return false;

            if (typeArguments == null || typeArguments.Length != openGenericType.GetGenericArguments().Length)
                return false;

            try
            {
                closedType = openGenericType.MakeGenericType(typeArguments);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取泛型类型的基础定义
        /// </summary>
        public static Type GetGenericTypeDefinitionSafe(Type type)
        {
            if (type == null) return null;
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        #endregion
    }
}
