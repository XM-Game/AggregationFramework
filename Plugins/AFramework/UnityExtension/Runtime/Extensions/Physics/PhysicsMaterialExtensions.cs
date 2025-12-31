// ==========================================================
// 文件名：PhysicsMaterialExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// PhysicMaterial 扩展方法
    /// <para>提供 3D 物理材质的实用功能扩展</para>
    /// </summary>
    public static class PhysicsMaterialExtensions
    {
        #region 属性设置

        /// <summary>
        /// 设置动态摩擦力
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicMaterial WithDynamicFriction(this PhysicMaterial material, float friction)
        {
            material.dynamicFriction = friction;
            return material;
        }

        /// <summary>
        /// 设置静态摩擦力
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicMaterial WithStaticFriction(this PhysicMaterial material, float friction)
        {
            material.staticFriction = friction;
            return material;
        }

        /// <summary>
        /// 设置弹性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicMaterial WithBounciness(this PhysicMaterial material, float bounciness)
        {
            material.bounciness = bounciness;
            return material;
        }

        /// <summary>
        /// 设置摩擦力组合模式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicMaterial WithFrictionCombine(this PhysicMaterial material, PhysicMaterialCombine combine)
        {
            material.frictionCombine = combine;
            return material;
        }

        /// <summary>
        /// 设置弹性组合模式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicMaterial WithBounceCombine(this PhysicMaterial material, PhysicMaterialCombine combine)
        {
            material.bounceCombine = combine;
            return material;
        }

        #endregion

        #region 预设配置

        /// <summary>
        /// 配置为冰面材质 (低摩擦)
        /// </summary>
        public static PhysicMaterial ConfigureAsIce(this PhysicMaterial material)
        {
            material.dynamicFriction = 0.05f;
            material.staticFriction = 0.05f;
            material.bounciness = 0f;
            material.frictionCombine = PhysicMaterialCombine.Minimum;
            material.bounceCombine = PhysicMaterialCombine.Average;
            return material;
        }

        /// <summary>
        /// 配置为橡胶材质 (高摩擦高弹性)
        /// </summary>
        public static PhysicMaterial ConfigureAsRubber(this PhysicMaterial material)
        {
            material.dynamicFriction = 0.8f;
            material.staticFriction = 0.9f;
            material.bounciness = 0.8f;
            material.frictionCombine = PhysicMaterialCombine.Maximum;
            material.bounceCombine = PhysicMaterialCombine.Maximum;
            return material;
        }

        /// <summary>
        /// 配置为金属材质 (中等摩擦低弹性)
        /// </summary>
        public static PhysicMaterial ConfigureAsMetal(this PhysicMaterial material)
        {
            material.dynamicFriction = 0.4f;
            material.staticFriction = 0.5f;
            material.bounciness = 0.1f;
            material.frictionCombine = PhysicMaterialCombine.Average;
            material.bounceCombine = PhysicMaterialCombine.Average;
            return material;
        }

        /// <summary>
        /// 配置为木材材质
        /// </summary>
        public static PhysicMaterial ConfigureAsWood(this PhysicMaterial material)
        {
            material.dynamicFriction = 0.5f;
            material.staticFriction = 0.6f;
            material.bounciness = 0.2f;
            material.frictionCombine = PhysicMaterialCombine.Average;
            material.bounceCombine = PhysicMaterialCombine.Average;
            return material;
        }

        /// <summary>
        /// 配置为弹力球材质
        /// </summary>
        public static PhysicMaterial ConfigureAsBouncy(this PhysicMaterial material)
        {
            material.dynamicFriction = 0.6f;
            material.staticFriction = 0.6f;
            material.bounciness = 1f;
            material.frictionCombine = PhysicMaterialCombine.Average;
            material.bounceCombine = PhysicMaterialCombine.Maximum;
            return material;
        }

        /// <summary>
        /// 配置为无摩擦材质
        /// </summary>
        public static PhysicMaterial ConfigureAsFrictionless(this PhysicMaterial material)
        {
            material.dynamicFriction = 0f;
            material.staticFriction = 0f;
            material.bounciness = 0f;
            material.frictionCombine = PhysicMaterialCombine.Minimum;
            material.bounceCombine = PhysicMaterialCombine.Average;
            return material;
        }

        #endregion

        #region 复制

        /// <summary>
        /// 复制物理材质
        /// </summary>
        public static PhysicMaterial Clone(this PhysicMaterial material)
        {
            return new PhysicMaterial(material.name + "_Clone")
            {
                dynamicFriction = material.dynamicFriction,
                staticFriction = material.staticFriction,
                bounciness = material.bounciness,
                frictionCombine = material.frictionCombine,
                bounceCombine = material.bounceCombine
            };
        }

        /// <summary>
        /// 从另一个材质复制属性
        /// </summary>
        public static void CopyFrom(this PhysicMaterial material, PhysicMaterial source)
        {
            material.dynamicFriction = source.dynamicFriction;
            material.staticFriction = source.staticFriction;
            material.bounciness = source.bounciness;
            material.frictionCombine = source.frictionCombine;
            material.bounceCombine = source.bounceCombine;
        }

        #endregion
    }

    /// <summary>
    /// PhysicsMaterial2D 扩展方法
    /// <para>提供 2D 物理材质的实用功能扩展</para>
    /// </summary>
    public static class PhysicsMaterial2DExtensions
    {
        #region 属性设置

        /// <summary>
        /// 设置摩擦力
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicsMaterial2D WithFriction(this PhysicsMaterial2D material, float friction)
        {
            material.friction = friction;
            return material;
        }

        /// <summary>
        /// 设置弹性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicsMaterial2D WithBounciness(this PhysicsMaterial2D material, float bounciness)
        {
            material.bounciness = bounciness;
            return material;
        }

        #endregion

        #region 预设配置

        /// <summary>
        /// 配置为冰面材质 (低摩擦)
        /// </summary>
        public static PhysicsMaterial2D ConfigureAsIce(this PhysicsMaterial2D material)
        {
            material.friction = 0.05f;
            material.bounciness = 0f;
            return material;
        }

        /// <summary>
        /// 配置为橡胶材质 (高摩擦高弹性)
        /// </summary>
        public static PhysicsMaterial2D ConfigureAsRubber(this PhysicsMaterial2D material)
        {
            material.friction = 0.8f;
            material.bounciness = 0.8f;
            return material;
        }

        /// <summary>
        /// 配置为金属材质 (中等摩擦低弹性)
        /// </summary>
        public static PhysicsMaterial2D ConfigureAsMetal(this PhysicsMaterial2D material)
        {
            material.friction = 0.4f;
            material.bounciness = 0.1f;
            return material;
        }

        /// <summary>
        /// 配置为弹力球材质
        /// </summary>
        public static PhysicsMaterial2D ConfigureAsBouncy(this PhysicsMaterial2D material)
        {
            material.friction = 0.6f;
            material.bounciness = 1f;
            return material;
        }

        /// <summary>
        /// 配置为无摩擦材质
        /// </summary>
        public static PhysicsMaterial2D ConfigureAsFrictionless(this PhysicsMaterial2D material)
        {
            material.friction = 0f;
            material.bounciness = 0f;
            return material;
        }

        #endregion

        #region 复制

        /// <summary>
        /// 复制物理材质
        /// </summary>
        public static PhysicsMaterial2D Clone(this PhysicsMaterial2D material)
        {
            return new PhysicsMaterial2D(material.name + "_Clone")
            {
                friction = material.friction,
                bounciness = material.bounciness
            };
        }

        /// <summary>
        /// 从另一个材质复制属性
        /// </summary>
        public static void CopyFrom(this PhysicsMaterial2D material, PhysicsMaterial2D source)
        {
            material.friction = source.friction;
            material.bounciness = source.bounciness;
        }

        #endregion
    }
}
