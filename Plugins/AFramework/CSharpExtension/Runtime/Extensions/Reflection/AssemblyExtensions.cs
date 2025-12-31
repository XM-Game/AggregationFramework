// ==========================================================
// 文件名：AssemblyExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Reflection, System.Linq
// ==========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Assembly 扩展方法
    /// <para>提供程序集的常用操作扩展，包括类型查找、资源获取、版本信息等功能</para>
    /// </summary>
    public static class AssemblyExtensions
    {
        #region 类型查找

        /// <summary>
        /// 获取程序集中所有公共类型
        /// </summary>
        public static Type[] GetPublicTypes(this Assembly assembly)
        {
            if (assembly == null) return Array.Empty<Type>();
            
            try
            {
                return assembly.GetExportedTypes();
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

        /// <summary>
        /// 安全获取程序集中的所有类型
        /// </summary>
        public static Type[] GetTypesSafe(this Assembly assembly)
        {
            if (assembly == null) return Array.Empty<Type>();
            
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null).ToArray();
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

        /// <summary>
        /// 查找实现指定接口的所有类型
        /// </summary>
        public static IEnumerable<Type> GetTypesImplementing<TInterface>(this Assembly assembly)
        {
            if (assembly == null) yield break;
            
            var interfaceType = typeof(TInterface);
            foreach (var type in assembly.GetTypesSafe())
            {
                if (type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type))
                    yield return type;
            }
        }

        /// <summary>
        /// 查找继承自指定基类的所有类型
        /// </summary>
        public static IEnumerable<Type> GetTypesInheritingFrom<TBase>(this Assembly assembly) where TBase : class
        {
            if (assembly == null) yield break;
            
            var baseType = typeof(TBase);
            foreach (var type in assembly.GetTypesSafe())
            {
                if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type) && type != baseType)
                    yield return type;
            }
        }

        /// <summary>
        /// 查找带有指定特性的所有类型
        /// </summary>
        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly assembly, bool inherit = true) 
            where TAttribute : Attribute
        {
            if (assembly == null) yield break;
            
            foreach (var type in assembly.GetTypesSafe())
            {
                if (Attribute.IsDefined(type, typeof(TAttribute), inherit))
                    yield return type;
            }
        }

        /// <summary>
        /// 按名称查找类型
        /// </summary>
        public static Type FindType(this Assembly assembly, string typeName, bool ignoreCase = false)
        {
            if (assembly == null || string.IsNullOrEmpty(typeName)) return null;
            
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            
            foreach (var type in assembly.GetTypesSafe())
            {
                if (type.Name.Equals(typeName, comparison) || type.FullName?.Equals(typeName, comparison) == true)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// 按命名空间查找类型
        /// </summary>
        public static IEnumerable<Type> GetTypesInNamespace(this Assembly assembly, string namespaceName)
        {
            if (assembly == null || string.IsNullOrEmpty(namespaceName)) yield break;
            
            foreach (var type in assembly.GetTypesSafe())
            {
                if (type.Namespace == namespaceName)
                    yield return type;
            }
        }

        #endregion

        #region 版本信息

        /// <summary>
        /// 获取程序集版本
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Version GetVersion(this Assembly assembly)
        {
            return assembly?.GetName().Version;
        }

        /// <summary>
        /// 获取程序集版本字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetVersionString(this Assembly assembly)
        {
            return assembly?.GetName().Version?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集文件版本
        /// </summary>
        public static string GetFileVersion(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            return attr?.Version ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集信息版本
        /// </summary>
        public static string GetInformationalVersion(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attr?.InformationalVersion ?? string.Empty;
        }

        #endregion

        #region 程序集信息

        /// <summary>
        /// 获取程序集标题
        /// </summary>
        public static string GetTitle(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            return attr?.Title ?? assembly.GetName().Name ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集描述
        /// </summary>
        public static string GetDescription(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            return attr?.Description ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集公司名称
        /// </summary>
        public static string GetCompany(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return attr?.Company ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集产品名称
        /// </summary>
        public static string GetProduct(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            return attr?.Product ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集版权信息
        /// </summary>
        public static string GetCopyright(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            return attr?.Copyright ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集配置
        /// </summary>
        public static string GetConfiguration(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var attr = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            return attr?.Configuration ?? string.Empty;
        }

        #endregion

        #region 资源操作

        /// <summary>
        /// 获取嵌入资源名称列表
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetEmbeddedResourceNames(this Assembly assembly)
        {
            return assembly?.GetManifestResourceNames() ?? Array.Empty<string>();
        }

        /// <summary>
        /// 检查是否包含指定嵌入资源
        /// </summary>
        public static bool HasEmbeddedResource(this Assembly assembly, string resourceName)
        {
            if (assembly == null || string.IsNullOrEmpty(resourceName)) return false;
            return assembly.GetManifestResourceNames().Contains(resourceName);
        }

        /// <summary>
        /// 获取嵌入资源流
        /// </summary>
        public static Stream GetEmbeddedResourceStream(this Assembly assembly, string resourceName)
        {
            if (assembly == null || string.IsNullOrEmpty(resourceName)) return null;
            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// 读取嵌入资源为字符串
        /// </summary>
        public static string ReadEmbeddedResourceAsString(this Assembly assembly, string resourceName, System.Text.Encoding encoding = null)
        {
            if (assembly == null || string.IsNullOrEmpty(resourceName)) return string.Empty;
            
            encoding = encoding ?? System.Text.Encoding.UTF8;
            
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return string.Empty;
                
                using (var reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 读取嵌入资源为字节数组
        /// </summary>
        public static byte[] ReadEmbeddedResourceAsBytes(this Assembly assembly, string resourceName)
        {
            if (assembly == null || string.IsNullOrEmpty(resourceName)) return Array.Empty<byte>();
            
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return Array.Empty<byte>();
                
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        #endregion

        #region 路径信息

        /// <summary>
        /// 获取程序集所在目录
        /// </summary>
        public static string GetDirectory(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var location = assembly.Location;
            return string.IsNullOrEmpty(location) ? string.Empty : Path.GetDirectoryName(location) ?? string.Empty;
        }

        /// <summary>
        /// 获取程序集文件名
        /// </summary>
        public static string GetFileName(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var location = assembly.Location;
            return string.IsNullOrEmpty(location) ? string.Empty : Path.GetFileName(location);
        }

        /// <summary>
        /// 获取程序集完整路径
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLocation(this Assembly assembly)
        {
            return assembly?.Location ?? string.Empty;
        }

        #endregion

        #region 引用程序集

        /// <summary>
        /// 获取引用的程序集名称
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AssemblyName[] GetReferencedAssemblyNames(this Assembly assembly)
        {
            return assembly?.GetReferencedAssemblies() ?? Array.Empty<AssemblyName>();
        }

        /// <summary>
        /// 检查是否引用了指定程序集
        /// </summary>
        public static bool ReferencesAssembly(this Assembly assembly, string assemblyName)
        {
            if (assembly == null || string.IsNullOrEmpty(assemblyName)) return false;
            
            return assembly.GetReferencedAssemblies()
                .Any(a => a.Name?.Equals(assemblyName, StringComparison.OrdinalIgnoreCase) == true);
        }

        #endregion

        #region 实例创建

        /// <summary>
        /// 创建指定类型的实例
        /// </summary>
        public static object CreateInstance(this Assembly assembly, string typeName, params object[] args)
        {
            if (assembly == null || string.IsNullOrEmpty(typeName)) return null;
            
            try
            {
                return assembly.CreateInstance(typeName, false, BindingFlags.Default, null, args, null, null);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建指定类型的实例（泛型版本）
        /// </summary>
        public static T CreateInstance<T>(this Assembly assembly, string typeName, params object[] args) where T : class
        {
            return assembly.CreateInstance(typeName, args) as T;
        }

        /// <summary>
        /// 创建实现指定接口的所有类型的实例
        /// </summary>
        public static IEnumerable<TInterface> CreateInstancesOf<TInterface>(this Assembly assembly) where TInterface : class
        {
            if (assembly == null) yield break;
            
            foreach (var type in assembly.GetTypesImplementing<TInterface>())
            {
                TInterface instance = null;
                try
                {
                    instance = Activator.CreateInstance(type) as TInterface;
                }
                catch
                {
                    // 忽略无法创建的类型
                }
                
                if (instance != null)
                    yield return instance;
            }
        }

        #endregion

        #region 程序集特性检查

        /// <summary>
        /// 检查是否为动态程序集
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDynamic(this Assembly assembly)
        {
            return assembly?.IsDynamic ?? false;
        }

        /// <summary>
        /// 检查是否为完全信任程序集
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFullyTrusted(this Assembly assembly)
        {
            return assembly?.IsFullyTrusted ?? false;
        }

        /// <summary>
        /// 获取程序集的公钥令牌
        /// </summary>
        public static string GetPublicKeyToken(this Assembly assembly)
        {
            if (assembly == null) return string.Empty;
            
            var token = assembly.GetName().GetPublicKeyToken();
            if (token == null || token.Length == 0) return string.Empty;
            
            return BitConverter.ToString(token).Replace("-", "").ToLowerInvariant();
        }

        #endregion
    }
}
