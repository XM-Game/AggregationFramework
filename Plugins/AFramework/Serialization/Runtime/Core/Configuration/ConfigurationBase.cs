// ==========================================================
// 文件名：ConfigurationBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 配置基类
    /// <para>提供配置类的通用功能：只读模式、克隆、验证</para>
    /// </summary>
    /// <remarks>
    /// <para><b>设计说明：</b></para>
    /// <list type="bullet">
    ///   <item>统一只读模式实现，避免代码重复</item>
    ///   <item>提供模板方法用于子类扩展</item>
    ///   <item>遵循开闭原则，对扩展开放</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">配置类型</typeparam>
    public abstract class ConfigurationBase<T> where T : ConfigurationBase<T>, new()
    {
        #region 字段

        /// <summary>
        /// 只读标记
        /// </summary>
        protected bool _isReadOnly;

        #endregion

        #region 属性

        /// <summary>
        /// 获取配置是否为只读
        /// </summary>
        public bool IsReadOnly => _isReadOnly;

        #endregion

        #region 公共方法

        /// <summary>
        /// 将配置设为只读
        /// </summary>
        /// <returns>当前配置实例</returns>
        public T AsReadOnly()
        {
            _isReadOnly = true;
            OnAsReadOnly();
            return (T)this;
        }

        /// <summary>
        /// 创建可修改的副本
        /// </summary>
        /// <returns>新的配置实例</returns>
        public T Clone()
        {
            var clone = new T();
            CopyTo(clone);
            return clone;
        }

        /// <summary>
        /// 验证配置有效性
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <returns>如果有效返回 true</returns>
        public bool Validate(out string error)
        {
            error = null;
            return OnValidate(ref error);
        }

        /// <summary>
        /// 获取配置摘要
        /// </summary>
        /// <returns>摘要字符串</returns>
        public abstract string GetSummary();

        #endregion

        #region 保护方法

        /// <summary>
        /// 检查只读状态，如果只读则抛出异常
        /// </summary>
        /// <exception cref="InvalidOperationException">配置为只读时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfReadOnly()
        {
            if (_isReadOnly)
                throw new InvalidOperationException("配置为只读，无法修改 / Configuration is read-only");
        }

        /// <summary>
        /// 设为只读时的回调
        /// </summary>
        protected virtual void OnAsReadOnly() { }

        /// <summary>
        /// 验证配置的回调
        /// </summary>
        /// <param name="error">错误信息引用</param>
        /// <returns>如果有效返回 true</returns>
        protected virtual bool OnValidate(ref string error) => true;

        /// <summary>
        /// 复制配置到目标实例
        /// </summary>
        /// <param name="target">目标配置</param>
        protected abstract void CopyTo(T target);

        #endregion
    }
}
