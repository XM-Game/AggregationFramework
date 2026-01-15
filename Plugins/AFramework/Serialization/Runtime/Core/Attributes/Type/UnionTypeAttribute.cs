// ==========================================================
// 文件名：UnionTypeAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 联合类型特性
    /// <para>定义多态序列化的派生类型映射</para>
    /// <para>用于接口、抽象类或基类的多态序列化支持</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>定义基类型与派生类型的映射关系</item>
    ///   <item>为每个派生类型分配唯一的类型标识符</item>
    ///   <item>支持运行时类型恢复和多态反序列化</item>
    ///   <item>支持接口、抽象类、普通基类</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 接口多态示例
    /// [UnionType(0, typeof(DogData))]
    /// [UnionType(1, typeof(CatData))]
    /// [UnionType(2, typeof(BirdData))]
    /// public interface IAnimalData
    /// {
    ///     string Name { get; }
    /// }
    /// 
    /// [Serializable]
    /// public class DogData : IAnimalData
    /// {
    ///     public string Name { get; set; }
    ///     public string Breed { get; set; }
    /// }
    /// 
    /// // 抽象类多态示例
    /// [UnionType(0, typeof(FireSpell))]
    /// [UnionType(1, typeof(IceSpell))]
    /// [UnionType(2, typeof(LightningSpell))]
    /// public abstract class SpellBase
    /// {
    ///     public abstract void Cast();
    /// }
    /// 
    /// // 使用类型名称作为标识
    /// [UnionType("player", typeof(PlayerEntity))]
    /// [UnionType("enemy", typeof(EnemyEntity))]
    /// [UnionType("npc", typeof(NpcEntity))]
    /// public abstract class EntityBase { }
    /// </code>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>类型 ID 在同一基类型下必须唯一</item>
    ///   <item>派生类型必须标记 [Serializable] 特性</item>
    ///   <item>类型 ID 一旦分配不应更改（影响兼容性）</item>
    ///   <item>建议使用连续的整数 ID 以优化存储</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class UnionTypeAttribute : Attribute
    {
        #region 常量

        /// <summary>
        /// 无效的类型 ID
        /// </summary>
        public const int InvalidTypeId = -1;

        /// <summary>
        /// 最小有效类型 ID
        /// </summary>
        public const int MinTypeId = 0;

        /// <summary>
        /// 最大有效类型 ID（使用 VarInt 编码时的最优范围）
        /// </summary>
        public const int MaxTypeId = 127;

        /// <summary>
        /// 扩展最大类型 ID
        /// </summary>
        public const int ExtendedMaxTypeId = 65535;

        #endregion

        #region 字段

        private readonly int _typeId;
        private readonly string _typeTag;
        private readonly Type _derivedType;
        private readonly bool _useStringTag;
        private string _alias;
        private bool _isDeprecated;
        private string _deprecationMessage;
        private int _replacementTypeId = InvalidTypeId;

        #endregion

        #region 属性

        /// <summary>
        /// 获取类型 ID
        /// </summary>
        public int TypeId => _typeId;

        /// <summary>
        /// 获取类型标签
        /// </summary>
        public string TypeTag => _typeTag;

        /// <summary>
        /// 获取派生类型
        /// </summary>
        public Type DerivedType => _derivedType;

        /// <summary>
        /// 获取是否使用字符串标签
        /// </summary>
        public bool UseStringTag => _useStringTag;

        /// <summary>
        /// 获取或设置类型别名（用于版本迁移）
        /// </summary>
        public string Alias
        {
            get => _alias;
            set => _alias = value;
        }

        /// <summary>
        /// 获取或设置是否已弃用
        /// </summary>
        public bool IsDeprecated
        {
            get => _isDeprecated;
            set => _isDeprecated = value;
        }

        /// <summary>
        /// 获取或设置弃用消息
        /// </summary>
        public string DeprecationMessage
        {
            get => _deprecationMessage;
            set => _deprecationMessage = value;
        }

        /// <summary>
        /// 获取或设置替代类型 ID
        /// </summary>
        public int ReplacementTypeId
        {
            get => _replacementTypeId;
            set => _replacementTypeId = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="UnionTypeAttribute"/> 的新实例
        /// </summary>
        /// <param name="typeId">类型 ID（0-127 为最优范围）</param>
        /// <param name="derivedType">派生类型</param>
        /// <exception cref="ArgumentNullException">derivedType 为 null</exception>
        /// <exception cref="ArgumentOutOfRangeException">typeId 小于 0</exception>
        public UnionTypeAttribute(int typeId, Type derivedType)
        {
            if (derivedType == null)
                throw new ArgumentNullException(nameof(derivedType));
            if (typeId < MinTypeId)
                throw new ArgumentOutOfRangeException(nameof(typeId),
                    $"类型 ID 必须大于等于 {MinTypeId}");

            _typeId = typeId;
            _derivedType = derivedType;
            _typeTag = derivedType.Name;
            _useStringTag = false;
        }

        /// <summary>
        /// 初始化 <see cref="UnionTypeAttribute"/> 的新实例
        /// </summary>
        /// <param name="typeTag">类型标签（字符串标识）</param>
        /// <param name="derivedType">派生类型</param>
        /// <exception cref="ArgumentNullException">typeTag 或 derivedType 为 null</exception>
        public UnionTypeAttribute(string typeTag, Type derivedType)
        {
            if (string.IsNullOrEmpty(typeTag))
                throw new ArgumentNullException(nameof(typeTag));
            if (derivedType == null)
                throw new ArgumentNullException(nameof(derivedType));

            _typeTag = typeTag;
            _derivedType = derivedType;
            _typeId = typeTag.GetHashCode() & 0x7FFFFFFF; // 确保为正数
            _useStringTag = true;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取有效的类型标识符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEffectiveTypeId() => _typeId;

        /// <summary>
        /// 获取类型标识符的字符串表示
        /// </summary>
        public string GetIdentifierString() => _useStringTag ? _typeTag : _typeId.ToString();

        /// <summary>
        /// 检查类型 ID 是否在最优范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInOptimalRange() => _typeId >= MinTypeId && _typeId <= MaxTypeId;

        /// <summary>
        /// 检查类型 ID 是否在扩展范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInExtendedRange() => _typeId >= MinTypeId && _typeId <= ExtendedMaxTypeId;

        /// <summary>
        /// 检查是否有替代类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasReplacement() => _isDeprecated && _replacementTypeId != InvalidTypeId;

        /// <summary>
        /// 检查是否有别名
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAlias() => !string.IsNullOrEmpty(_alias);

        /// <summary>
        /// 验证派生类型是否有效
        /// </summary>
        /// <param name="baseType">基类型</param>
        public bool ValidateDerivedType(Type baseType)
        {
            if (baseType == null || _derivedType == null)
                return false;

            if (baseType.IsInterface)
                return baseType.IsAssignableFrom(_derivedType);

            return _derivedType.IsSubclassOf(baseType) || _derivedType == baseType;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public string GetSummary()
        {
            var identifier = _useStringTag ? $"\"{_typeTag}\"" : _typeId.ToString();
            var deprecated = _isDeprecated ? " [Deprecated]" : "";
            return $"UnionType({identifier}, {_derivedType.Name}){deprecated}";
        }

        /// <inheritdoc/>
        public override string ToString() => GetSummary();

        #endregion
    }
}
