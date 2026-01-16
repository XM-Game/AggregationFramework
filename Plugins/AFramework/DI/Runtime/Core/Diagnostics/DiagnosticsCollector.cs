// ==========================================================
// 文件名：DiagnosticsCollector.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Diagnostics
// ==========================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AFramework.DI
{
    /// <summary>
    /// 诊断信息收集器
    /// <para>收集容器运行时的诊断信息</para>
    /// </summary>
    public sealed class DiagnosticsCollector : IDisposable
    {
        #region 字段

        private readonly DiagnosticsInfo _info;
        private readonly bool _collectCallStack;
        private readonly int _maxRecords;
        private readonly object _lock = new();
        private bool _isDisposed;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建诊断收集器
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <param name="collectCallStack">是否收集调用堆栈</param>
        /// <param name="maxRecords">最大记录数量</param>
        public DiagnosticsCollector(
            string containerName = null,
            bool collectCallStack = false,
            int maxRecords = 1000)
        {
            _info = new DiagnosticsInfo
            {
                ContainerName = containerName ?? "Container",
                CreatedAt = DateTime.Now
            };
            _collectCallStack = collectCallStack;
            _maxRecords = maxRecords;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        public DiagnosticsInfo Info => _info;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        #endregion

        #region 注册收集

        /// <summary>
        /// 收集注册信息
        /// </summary>
        /// <param name="registration">注册信息</param>
        /// <param name="source">注册来源</param>
        public void CollectRegistration(IRegistration registration, string source = null)
        {
            if (!IsEnabled || _isDisposed) return;

            lock (_lock)
            {
                var diag = new RegistrationDiagnostics
                {
                    ServiceType = registration.ServiceType,
                    ImplementationType = registration.ImplementationType,
                    Lifetime = registration.Lifetime,
                    Key = registration.Key,
                    Source = source
                };

                // 分析依赖
                if (registration.ImplementationType != null)
                {
                    AnalyzeDependencies(registration.ImplementationType, diag.Dependencies);
                }

                _info.Registrations.Add(diag);

                // 更新统计
                switch (registration.Lifetime)
                {
                    case Lifetime.Singleton:
                        _info.SingletonCount++;
                        break;
                    case Lifetime.Scoped:
                        _info.ScopedCount++;
                        break;
                    case Lifetime.Transient:
                        _info.TransientCount++;
                        break;
                }
            }
        }

        /// <summary>
        /// 批量收集注册信息
        /// </summary>
        public void CollectRegistrations(IEnumerable<IRegistration> registrations, string source = null)
        {
            foreach (var reg in registrations)
            {
                CollectRegistration(reg, source);
            }
        }

        #endregion

        #region 解析记录

        /// <summary>
        /// 记录解析开始
        /// </summary>
        /// <returns>解析记录</returns>
        public ResolutionRecord BeginResolution(Type serviceType)
        {
            if (!IsEnabled || _isDisposed) return null;

            var record = new ResolutionRecord
            {
                ServiceType = serviceType,
                Timestamp = DateTime.Now
            };

            if (_collectCallStack)
            {
                record.CallStack = new StackTrace(2, true).ToString();
            }

            return record;
        }

        /// <summary>
        /// 记录解析完成
        /// </summary>
        public void EndResolution(ResolutionRecord record, bool success, bool cacheHit, string error = null)
        {
            if (record == null || !IsEnabled || _isDisposed) return;

            record.Success = success;
            record.CacheHit = cacheHit;
            record.Error = error;
            record.DurationMs = (DateTime.Now - record.Timestamp).TotalMilliseconds;

            lock (_lock)
            {
                // 限制记录数量
                if (_info.ResolutionRecords.Count >= _maxRecords)
                {
                    _info.ResolutionRecords.RemoveAt(0);
                }
                _info.ResolutionRecords.Add(record);
            }
        }

        #endregion

        #region 生命周期问题

        /// <summary>
        /// 报告生命周期问题
        /// </summary>
        public void ReportLifetimeIssue(LifetimeIssue issue)
        {
            if (!IsEnabled || _isDisposed) return;

            lock (_lock)
            {
                _info.LifetimeIssues.Add(issue);
            }
        }

        /// <summary>
        /// 报告俘获依赖问题
        /// </summary>
        public void ReportCaptiveDependency(Type singletonType, Type scopedDependency)
        {
            ReportLifetimeIssue(new LifetimeIssue
            {
                Severity = IssueSeverity.Warning,
                IssueType = LifetimeIssueType.CaptiveDependency,
                ServiceType = singletonType,
                DependencyType = scopedDependency,
                Message = $"单例服务 '{singletonType.Name}' 依赖作用域服务 '{scopedDependency.Name}'，可能导致作用域服务被意外延长生命周期",
                Suggestion = $"考虑将 '{singletonType.Name}' 改为作用域生命周期，或将 '{scopedDependency.Name}' 改为单例"
            });
        }

        /// <summary>
        /// 报告循环依赖问题
        /// </summary>
        public void ReportCircularDependency(IEnumerable<Type> dependencyChain)
        {
            var chain = string.Join(" -> ", dependencyChain);
            ReportLifetimeIssue(new LifetimeIssue
            {
                Severity = IssueSeverity.Error,
                IssueType = LifetimeIssueType.CircularDependency,
                Message = $"检测到循环依赖: {chain}",
                Suggestion = "考虑使用 Lazy<T>、工厂模式或重构依赖关系来打破循环"
            });
        }

        #endregion

        #region 私有方法

        private static void AnalyzeDependencies(Type type, List<Type> dependencies)
        {
            // 分析构造函数依赖
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var ctor in constructors)
            {
                foreach (var param in ctor.GetParameters())
                {
                    if (!dependencies.Contains(param.ParameterType))
                    {
                        dependencies.Add(param.ParameterType);
                    }
                }
            }

            // 分析字段和属性注入
            var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var member in members)
            {
                if (member.GetCustomAttribute<InjectAttribute>() == null) continue;

                Type memberType = member switch
                {
                    FieldInfo field => field.FieldType,
                    PropertyInfo prop => prop.PropertyType,
                    _ => null
                };

                if (memberType != null && !dependencies.Contains(memberType))
                {
                    dependencies.Add(memberType);
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            IsEnabled = false;
        }

        #endregion
    }
}
