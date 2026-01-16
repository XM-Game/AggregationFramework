// ==========================================================
// 文件名：ScopeManager.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 作用域管理器，统一管理所有作用域的生命周期
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 作用域管理器
    /// <para>统一管理依赖注入容器中所有作用域的创建、追踪和释放</para>
    /// <para>Scope manager that manages creation, tracking and disposal of all scopes</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：专注于作用域的生命周期管理</item>
    /// <item>集中管理：提供作用域的统一入口点</item>
    /// <item>诊断友好：提供作用域状态的监控能力</item>
    /// </list>
    /// 
    /// 核心功能：
    /// <list type="bullet">
    /// <item>作用域注册与注销</item>
    /// <item>作用域查找与枚举</item>
    /// <item>批量作用域释放</item>
    /// <item>作用域诊断信息</item>
    /// </list>
    /// </remarks>
    public sealed class ScopeManager : IDisposable
    {
        #region 字段 / Fields

        /// <summary>
        /// 活动作用域字典
        /// </summary>
        private readonly Dictionary<Guid, WeakReference<ScopeContainer>> _activeScopes;

        /// <summary>
        /// 命名作用域字典
        /// </summary>
        private readonly Dictionary<string, Guid> _namedScopes;

        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 是否启用诊断
        /// </summary>
        private readonly bool _enableDiagnostics;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private volatile bool _isDisposed;

        /// <summary>
        /// 作用域创建计数
        /// </summary>
        private int _scopeCreationCount;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取活动作用域数量
        /// <para>Get the count of active scopes</para>
        /// </summary>
        public int ActiveScopeCount
        {
            get
            {
                lock (_syncRoot)
                {
                    CleanupDeadReferences();
                    return _activeScopes.Count;
                }
            }
        }

        /// <summary>
        /// 获取作用域创建总数
        /// <para>Get the total count of scope creations</para>
        /// </summary>
        public int ScopeCreationCount => _scopeCreationCount;

        /// <summary>
        /// 获取是否已释放
        /// <para>Get whether the manager has been disposed</para>
        /// </summary>
        public bool IsDisposed => _isDisposed;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建作用域管理器实例
        /// </summary>
        /// <param name="enableDiagnostics">是否启用诊断 / Whether to enable diagnostics</param>
        public ScopeManager(bool enableDiagnostics = false)
        {
            _enableDiagnostics = enableDiagnostics;
            _activeScopes = new Dictionary<Guid, WeakReference<ScopeContainer>>();
            _namedScopes = new Dictionary<string, Guid>(StringComparer.Ordinal);
        }

        #endregion

        #region 作用域注册 / Scope Registration

        /// <summary>
        /// 注册作用域
        /// <para>Register a scope</para>
        /// </summary>
        /// <param name="scope">作用域容器 / Scope container</param>
        internal void RegisterScope(ScopeContainer scope)
        {
            if (_isDisposed || scope == null)
                return;

            lock (_syncRoot)
            {
                _activeScopes[scope.ScopeId] = new WeakReference<ScopeContainer>(scope);
                
                // 注册命名作用域
                if (!string.IsNullOrEmpty(scope.ScopeName))
                {
                    _namedScopes[scope.ScopeName] = scope.ScopeId;
                }

                System.Threading.Interlocked.Increment(ref _scopeCreationCount);

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"注册作用域: {scope.ScopeName} (ID: {scope.ScopeId})");
                }
            }
        }

        /// <summary>
        /// 注销作用域
        /// <para>Unregister a scope</para>
        /// </summary>
        /// <param name="scope">作用域容器 / Scope container</param>
        internal void UnregisterScope(ScopeContainer scope)
        {
            if (_isDisposed || scope == null)
                return;

            lock (_syncRoot)
            {
                _activeScopes.Remove(scope.ScopeId);
                
                // 移除命名作用域
                if (!string.IsNullOrEmpty(scope.ScopeName))
                {
                    _namedScopes.Remove(scope.ScopeName);
                }

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"注销作用域: {scope.ScopeName} (ID: {scope.ScopeId})");
                }
            }
        }

        #endregion

        #region 作用域查找 / Scope Lookup

        /// <summary>
        /// 根据ID获取作用域
        /// <para>Get scope by ID</para>
        /// </summary>
        /// <param name="scopeId">作用域ID / Scope ID</param>
        /// <returns>作用域容器，如果不存在则返回 null / Scope container or null if not found</returns>
        public ScopeContainer GetScope(Guid scopeId)
        {
            if (_isDisposed)
                return null;

            lock (_syncRoot)
            {
                if (_activeScopes.TryGetValue(scopeId, out var weakRef) && 
                    weakRef.TryGetTarget(out var scope) && 
                    !scope.IsDisposed)
                {
                    return scope;
                }
                return null;
            }
        }

        /// <summary>
        /// 根据名称获取作用域
        /// <para>Get scope by name</para>
        /// </summary>
        /// <param name="scopeName">作用域名称 / Scope name</param>
        /// <returns>作用域容器，如果不存在则返回 null / Scope container or null if not found</returns>
        public ScopeContainer GetScopeByName(string scopeName)
        {
            if (_isDisposed || string.IsNullOrEmpty(scopeName))
                return null;

            lock (_syncRoot)
            {
                if (_namedScopes.TryGetValue(scopeName, out var scopeId))
                {
                    return GetScope(scopeId);
                }
                return null;
            }
        }

        /// <summary>
        /// 尝试获取作用域
        /// <para>Try to get a scope</para>
        /// </summary>
        /// <param name="scopeId">作用域ID / Scope ID</param>
        /// <param name="scope">输出的作用域 / Output scope</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        public bool TryGetScope(Guid scopeId, out ScopeContainer scope)
        {
            scope = GetScope(scopeId);
            return scope != null;
        }

        /// <summary>
        /// 检查作用域是否存在
        /// <para>Check if a scope exists</para>
        /// </summary>
        /// <param name="scopeId">作用域ID / Scope ID</param>
        /// <returns>是否存在 / Whether exists</returns>
        public bool HasScope(Guid scopeId)
        {
            return GetScope(scopeId) != null;
        }

        /// <summary>
        /// 检查命名作用域是否存在
        /// <para>Check if a named scope exists</para>
        /// </summary>
        /// <param name="scopeName">作用域名称 / Scope name</param>
        /// <returns>是否存在 / Whether exists</returns>
        public bool HasScopeByName(string scopeName)
        {
            return GetScopeByName(scopeName) != null;
        }

        #endregion

        #region 作用域枚举 / Scope Enumeration

        /// <summary>
        /// 获取所有活动作用域
        /// <para>Get all active scopes</para>
        /// </summary>
        /// <returns>活动作用域集合 / Collection of active scopes</returns>
        public IReadOnlyList<ScopeContainer> GetAllActiveScopes()
        {
            if (_isDisposed)
                return Array.Empty<ScopeContainer>();

            lock (_syncRoot)
            {
                CleanupDeadReferences();
                
                var result = new List<ScopeContainer>();
                foreach (var kvp in _activeScopes)
                {
                    if (kvp.Value.TryGetTarget(out var scope) && !scope.IsDisposed)
                    {
                        result.Add(scope);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 获取所有作用域ID
        /// <para>Get all scope IDs</para>
        /// </summary>
        /// <returns>作用域ID集合 / Collection of scope IDs</returns>
        public IReadOnlyList<Guid> GetAllScopeIds()
        {
            if (_isDisposed)
                return Array.Empty<Guid>();

            lock (_syncRoot)
            {
                CleanupDeadReferences();
                return new List<Guid>(_activeScopes.Keys);
            }
        }

        #endregion

        #region 作用域释放 / Scope Disposal

        /// <summary>
        /// 释放指定作用域
        /// <para>Dispose a specific scope</para>
        /// </summary>
        /// <param name="scopeId">作用域ID / Scope ID</param>
        /// <returns>是否成功释放 / Whether successfully disposed</returns>
        public bool DisposeScope(Guid scopeId)
        {
            if (_isDisposed)
                return false;

            var scope = GetScope(scopeId);
            if (scope != null && !scope.IsDisposed)
            {
                scope.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 释放命名作用域
        /// <para>Dispose a named scope</para>
        /// </summary>
        /// <param name="scopeName">作用域名称 / Scope name</param>
        /// <returns>是否成功释放 / Whether successfully disposed</returns>
        public bool DisposeScopeByName(string scopeName)
        {
            if (_isDisposed || string.IsNullOrEmpty(scopeName))
                return false;

            var scope = GetScopeByName(scopeName);
            if (scope != null && !scope.IsDisposed)
            {
                scope.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 释放所有作用域
        /// <para>Dispose all scopes</para>
        /// </summary>
        public void DisposeAllScopes()
        {
            if (_isDisposed)
                return;

            lock (_syncRoot)
            {
                if (_enableDiagnostics)
                {
                    LogDiagnostic($"开始释放所有作用域，共 {_activeScopes.Count} 个");
                }

                var scopesToDispose = new List<ScopeContainer>();
                
                foreach (var kvp in _activeScopes)
                {
                    if (kvp.Value.TryGetTarget(out var scope) && !scope.IsDisposed)
                    {
                        scopesToDispose.Add(scope);
                    }
                }

                // 按创建时间逆序释放（后创建的先释放）
                scopesToDispose.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));

                foreach (var scope in scopesToDispose)
                {
                    try
                    {
                        scope.Dispose();
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogWarning(
                            $"[AFramework.DI] 释放作用域 {scope.ScopeName} 时发生异常: {ex.Message}");
                    }
                }

                _activeScopes.Clear();
                _namedScopes.Clear();

                if (_enableDiagnostics)
                {
                    LogDiagnostic("所有作用域已释放");
                }
            }
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 清理已失效的弱引用
        /// </summary>
        private void CleanupDeadReferences()
        {
            var deadKeys = new List<Guid>();
            var deadNames = new List<string>();

            foreach (var kvp in _activeScopes)
            {
                if (!kvp.Value.TryGetTarget(out var scope) || scope.IsDisposed)
                {
                    deadKeys.Add(kvp.Key);
                }
            }

            foreach (var key in deadKeys)
            {
                _activeScopes.Remove(key);
            }

            foreach (var kvp in _namedScopes)
            {
                if (!_activeScopes.ContainsKey(kvp.Value))
                {
                    deadNames.Add(kvp.Key);
                }
            }

            foreach (var name in deadNames)
            {
                _namedScopes.Remove(name);
            }
        }

        #endregion

        #region IDisposable 实现 / IDisposable Implementation

        /// <summary>
        /// 释放作用域管理器及其管理的所有作用域
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            lock (_syncRoot)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"开始释放作用域管理器，共创建过 {_scopeCreationCount} 个作用域");
                }

                DisposeAllScopes();

                if (_enableDiagnostics)
                {
                    LogDiagnostic("作用域管理器已释放");
                }
            }
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取诊断信息
        /// <para>Get diagnostic information</para>
        /// </summary>
        /// <returns>诊断信息字符串 / Diagnostic information string</returns>
        public string GetDiagnosticInfo()
        {
            lock (_syncRoot)
            {
                CleanupDeadReferences();
                return $"ScopeManager[ActiveScopes={_activeScopes.Count}, " +
                       $"NamedScopes={_namedScopes.Count}, " +
                       $"TotalCreated={_scopeCreationCount}]";
            }
        }

        /// <summary>
        /// 获取所有作用域的详细信息
        /// <para>Get detailed information of all scopes</para>
        /// </summary>
        /// <returns>作用域信息集合 / Collection of scope information</returns>
        public IReadOnlyList<string> GetAllScopeDiagnostics()
        {
            if (_isDisposed)
                return Array.Empty<string>();

            lock (_syncRoot)
            {
                CleanupDeadReferences();
                
                var result = new List<string>();
                foreach (var kvp in _activeScopes)
                {
                    if (kvp.Value.TryGetTarget(out var scope) && !scope.IsDisposed)
                    {
                        result.Add(scope.GetDiagnosticInfo());
                    }
                }
                return result;
            }
        }

        private void LogDiagnostic(string message)
        {
            if (_enableDiagnostics)
            {
                UnityEngine.Debug.Log($"[AFramework.DI.ScopeManager] {message}");
            }
        }

        #endregion
    }
}
