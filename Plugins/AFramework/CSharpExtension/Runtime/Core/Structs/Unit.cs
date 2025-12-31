// ==========================================================
// 文件名：Unit.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 空值单元结构体
    /// <para>表示无返回值的操作结果，类似于 void 但可作为泛型参数</para>
    /// <para>常用于函数式编程中表示无意义的返回值</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 作为泛型参数替代 void
    /// Result&lt;Unit&gt; ExecuteCommand()
    /// {
    ///     DoSomething();
    ///     return Result&lt;Unit&gt;.Success(Unit.Default);
    /// }
    /// 
    /// // 在 Task 中使用
    /// Task&lt;Unit&gt; DoWorkAsync()
    /// {
    ///     await SomeAsyncOperation();
    ///     return Unit.Default;
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        #region 单例实例

        /// <summary>
        /// Unit 的默认实例
        /// <para>由于 Unit 不包含任何数据，所有实例都是等价的</para>
        /// </summary>
        public static readonly Unit Default = default;

        /// <summary>
        /// Unit 的默认实例 (别名)
        /// </summary>
        public static readonly Unit Value = default;

        #endregion

        #region IEquatable 实现

        /// <summary>
        /// 判断是否与另一个 Unit 相等
        /// <para>所有 Unit 实例都相等</para>
        /// </summary>
        /// <param name="other">另一个 Unit</param>
        /// <returns>始终返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Unit other) => true;

        /// <summary>
        /// 判断是否与另一个对象相等
        /// </summary>
        /// <param name="obj">另一个对象</param>
        /// <returns>如果是 Unit 类型返回 true</returns>
        public override bool Equals(object obj) => obj is Unit;

        /// <summary>
        /// 获取哈希码
        /// <para>所有 Unit 实例返回相同的哈希码</para>
        /// </summary>
        /// <returns>固定哈希码 0</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => 0;

        #endregion

        #region IComparable 实现

        /// <summary>
        /// 与另一个 Unit 比较
        /// <para>所有 Unit 实例相等，返回 0</para>
        /// </summary>
        /// <param name="other">另一个 Unit</param>
        /// <returns>始终返回 0</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Unit other) => 0;

        /// <summary>
        /// 与另一个对象比较
        /// </summary>
        /// <param name="obj">另一个对象</param>
        /// <returns>如果是 Unit 返回 0，否则返回 1</returns>
        public int CompareTo(object obj)
        {
            if (obj is Unit)
                return 0;
            return 1;
        }

        #endregion

        #region 运算符重载

        /// <summary>相等运算符</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Unit left, Unit right) => true;

        /// <summary>不等运算符</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Unit left, Unit right) => false;

        #endregion

        #region 字符串表示

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>返回 "()"</returns>
        public override string ToString() => "()";

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建 Unit 实例
        /// <para>等同于使用 Unit.Default</para>
        /// </summary>
        /// <returns>Unit 实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Unit Create() => default;

        /// <summary>
        /// 执行操作并返回 Unit
        /// <para>用于将 void 方法转换为返回 Unit 的方法</para>
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>Unit 实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Unit FromAction(Action action)
        {
            action?.Invoke();
            return default;
        }

        #endregion
    }
}
