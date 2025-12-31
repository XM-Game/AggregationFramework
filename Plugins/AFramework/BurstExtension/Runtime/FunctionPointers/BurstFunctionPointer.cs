// ==========================================================
// 文件名：BurstFunctionPointer.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：Burst函数指针封装，提供类型安全的函数指针包装器
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;

namespace AFramework.Burst
{
    /// <summary>
    /// Burst函数指针封装器
    /// 提供类型安全的函数指针包装，支持空值检查和便捷调用
    /// </summary>
    /// <typeparam name="T">函数指针委托类型</typeparam>
    public struct BurstFunctionPointer<T> where T : Delegate
    {
        #region 字段

        private FunctionPointer<T> m_FunctionPointer;
        private bool m_IsInitialized;

        #endregion

        #region 属性

        /// <summary>
        /// 获取底层函数指针
        /// </summary>
        public FunctionPointer<T> Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!m_IsInitialized)
                    throw new InvalidOperationException("函数指针未初始化");
                return m_FunctionPointer;
            }
        }

        /// <summary>
        /// 检查函数指针是否已初始化
        /// </summary>
        public bool IsInitialized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_IsInitialized && m_FunctionPointer.IsCreated;
        }

        /// <summary>
        /// 检查函数指针是否有效
        /// </summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_IsInitialized && m_FunctionPointer.IsCreated;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 从委托创建函数指针封装器
        /// </summary>
        /// <param name="delegate">委托实例</param>
        public BurstFunctionPointer(T @delegate)
        {
            if (@delegate == null)
            {
                m_FunctionPointer = default;
                m_IsInitialized = false;
            }
            else
            {
                m_FunctionPointer = BurstCompiler.CompileFunctionPointer(@delegate);
                m_IsInitialized = true;
            }
        }

        /// <summary>
        /// 从函数指针创建封装器
        /// </summary>
        /// <param name="functionPointer">函数指针</param>
        public BurstFunctionPointer(FunctionPointer<T> functionPointer)
        {
            m_FunctionPointer = functionPointer;
            m_IsInitialized = functionPointer.IsCreated;
        }

        #endregion

        #region 隐式转换

        /// <summary>
        /// 从委托隐式转换为函数指针封装器
        /// </summary>
        public static implicit operator BurstFunctionPointer<T>(T @delegate)
        {
            return new BurstFunctionPointer<T>(@delegate);
        }

        /// <summary>
        /// 从函数指针隐式转换为封装器
        /// </summary>
        public static implicit operator BurstFunctionPointer<T>(FunctionPointer<T> functionPointer)
        {
            return new BurstFunctionPointer<T>(functionPointer);
        }

        /// <summary>
        /// 隐式转换为底层函数指针
        /// </summary>
        public static implicit operator FunctionPointer<T>(BurstFunctionPointer<T> wrapper)
        {
            return wrapper.Value;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化函数指针
        /// </summary>
        /// <param name="delegate">委托实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize(T @delegate)
        {
            if (@delegate == null)
            {
                m_FunctionPointer = default;
                m_IsInitialized = false;
            }
            else
            {
                m_FunctionPointer = BurstCompiler.CompileFunctionPointer(@delegate);
                m_IsInitialized = true;
            }
        }

        /// <summary>
        /// 重置函数指针
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            m_FunctionPointer = default;
            m_IsInitialized = false;
        }

        /// <summary>
        /// 验证函数指针是否有效，如果无效则抛出异常
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <exception cref="InvalidOperationException">函数指针无效时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Validate(string paramName = "functionPointer")
        {
            if (!IsValid)
                throw new InvalidOperationException($"函数指针无效或未初始化: {paramName}");
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return IsValid ? $"BurstFunctionPointer<{typeof(T).Name}>(Valid)" : $"BurstFunctionPointer<{typeof(T).Name}>(Invalid)";
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return m_IsInitialized ? m_FunctionPointer.GetHashCode() : 0;
        }

        /// <summary>
        /// 比较相等性
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is BurstFunctionPointer<T> other)
                return Equals(other);
            return false;
        }

        /// <summary>
        /// 比较相等性
        /// </summary>
        public bool Equals(BurstFunctionPointer<T> other)
        {
            if (!m_IsInitialized && !other.m_IsInitialized)
                return true;
            if (m_IsInitialized != other.m_IsInitialized)
                return false;
            return m_FunctionPointer.Equals(other.m_FunctionPointer);
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 相等性比较
        /// </summary>
        public static bool operator ==(BurstFunctionPointer<T> left, BurstFunctionPointer<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 不等性比较
        /// </summary>
        public static bool operator !=(BurstFunctionPointer<T> left, BurstFunctionPointer<T> right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}

