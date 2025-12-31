// ==========================================================
// 文件名：BurstFeatureDetection.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：CPU特性检测，提供SIMD指令集支持检测和硬件能力查询
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;

namespace AFramework.Burst
{
    /// <summary>
    /// SIMD指令集类型
    /// </summary>
    [Flags]
    public enum SimdFeature
    {
        /// <summary>无SIMD支持</summary>
        None = 0,
        /// <summary>SSE指令集</summary>
        SSE = 1 << 0,
        /// <summary>SSE2指令集</summary>
        SSE2 = 1 << 1,
        /// <summary>SSE3指令集</summary>
        SSE3 = 1 << 2,
        /// <summary>SSSE3指令集</summary>
        SSSE3 = 1 << 3,
        /// <summary>SSE4.1指令集</summary>
        SSE41 = 1 << 4,
        /// <summary>SSE4.2指令集</summary>
        SSE42 = 1 << 5,
        /// <summary>AVX指令集</summary>
        AVX = 1 << 6,
        /// <summary>AVX2指令集</summary>
        AVX2 = 1 << 7,
        /// <summary>FMA指令集</summary>
        FMA = 1 << 8,
        /// <summary>AVX-512指令集</summary>
        AVX512 = 1 << 9,
        /// <summary>ARM NEON指令集</summary>
        NEON = 1 << 10,
        /// <summary>ARM NEON64指令集</summary>
        NEON64 = 1 << 11,
        /// <summary>WASM SIMD指令集</summary>
        WASM_SIMD = 1 << 12
    }

    /// <summary>
    /// CPU特性检测器
    /// 提供运行时SIMD指令集支持检测和硬件能力查询
    /// </summary>
    public static class BurstFeatureDetection
    {
        #region 缓存字段

        private static SimdFeature? s_CachedFeatures;
        private static int? s_CachedVectorWidth;

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取当前平台支持的SIMD特性
        /// </summary>
        public static SimdFeature SupportedFeatures
        {
            get
            {
                if (!s_CachedFeatures.HasValue)
                {
                    s_CachedFeatures = DetectSimdFeatures();
                }
                return s_CachedFeatures.Value;
            }
        }

        /// <summary>
        /// 获取最优向量宽度（以float为单位）
        /// </summary>
        public static int OptimalVectorWidth
        {
            get
            {
                if (!s_CachedVectorWidth.HasValue)
                {
                    s_CachedVectorWidth = CalculateOptimalVectorWidth();
                }
                return s_CachedVectorWidth.Value;
            }
        }

        /// <summary>是否支持SSE2</summary>
        public static bool HasSSE2 => (SupportedFeatures & SimdFeature.SSE2) != 0;

        /// <summary>是否支持SSE4.1</summary>
        public static bool HasSSE41 => (SupportedFeatures & SimdFeature.SSE41) != 0;

        /// <summary>是否支持AVX</summary>
        public static bool HasAVX => (SupportedFeatures & SimdFeature.AVX) != 0;

        /// <summary>是否支持AVX2</summary>
        public static bool HasAVX2 => (SupportedFeatures & SimdFeature.AVX2) != 0;

        /// <summary>是否支持NEON</summary>
        public static bool HasNEON => (SupportedFeatures & SimdFeature.NEON) != 0;

        /// <summary>是否支持任何SIMD指令集</summary>
        public static bool HasAnySIMD => SupportedFeatures != SimdFeature.None;

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否支持指定的SIMD特性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSupported(SimdFeature feature)
        {
            return (SupportedFeatures & feature) == feature;
        }

        /// <summary>
        /// 重置缓存的特性检测结果
        /// </summary>
        public static void ResetCache()
        {
            s_CachedFeatures = null;
            s_CachedVectorWidth = null;
        }

        /// <summary>
        /// 获取SIMD特性的友好名称
        /// </summary>
        public static string GetFeatureName(SimdFeature feature)
        {
            return feature switch
            {
                SimdFeature.SSE => "SSE",
                SimdFeature.SSE2 => "SSE2",
                SimdFeature.SSE3 => "SSE3",
                SimdFeature.SSSE3 => "SSSE3",
                SimdFeature.SSE41 => "SSE4.1",
                SimdFeature.SSE42 => "SSE4.2",
                SimdFeature.AVX => "AVX",
                SimdFeature.AVX2 => "AVX2",
                SimdFeature.FMA => "FMA",
                SimdFeature.AVX512 => "AVX-512",
                SimdFeature.NEON => "NEON",
                SimdFeature.NEON64 => "NEON64",
                SimdFeature.WASM_SIMD => "WASM SIMD",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// 获取所有支持特性的描述字符串
        /// </summary>
        public static string GetSupportedFeaturesDescription()
        {
            var features = SupportedFeatures;
            if (features == SimdFeature.None)
                return "No SIMD support";

            var result = new System.Text.StringBuilder();
            foreach (SimdFeature feature in Enum.GetValues(typeof(SimdFeature)))
            {
                if (feature != SimdFeature.None && (features & feature) == feature)
                {
                    if (result.Length > 0) result.Append(", ");
                    result.Append(GetFeatureName(feature));
                }
            }
            return result.ToString();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 检测当前平台支持的SIMD特性
        /// </summary>
        private static SimdFeature DetectSimdFeatures()
        {
            var features = SimdFeature.None;

            // X86/X64平台检测
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            // 注意：Unity Burst Intrinsics 的 X86 API 在不同版本中可能有所不同
            // 这里使用条件编译来避免编译错误
            // 对于生产环境，建议在 Burst 编译的代码中进行 CPU 特性检测
            
            // 默认假设现代 x86/x64 CPU 支持基本 SIMD 指令集
            // 这些指令集在现代 CPU（2000年后）中都是标准支持的
            features |= SimdFeature.SSE | SimdFeature.SSE2 | SimdFeature.SSE3 | SimdFeature.SSSE3;
            
            // 更高级的特性检测需要在 Burst 编译的代码中进行
            // 或者使用平台特定的 API（如 CPUID 指令）
#endif

            // ARM平台检测
#if UNITY_ANDROID || UNITY_IOS
            if (Arm.Neon.IsNeonSupported) features |= SimdFeature.NEON;
#if UNITY_64
            features |= SimdFeature.NEON64;
#endif
#endif

            // WebGL平台检测
#if UNITY_WEBGL
            if (Wasm.IsWasmSimdSupported) features |= SimdFeature.WASM_SIMD;
#endif

            return features;
        }

        /// <summary>
        /// 计算最优向量宽度
        /// </summary>
        private static int CalculateOptimalVectorWidth()
        {
            var features = SupportedFeatures;

            // AVX-512: 16个float
            if ((features & SimdFeature.AVX512) != 0) return 16;
            // AVX/AVX2: 8个float
            if ((features & (SimdFeature.AVX | SimdFeature.AVX2)) != 0) return 8;
            // SSE/NEON: 4个float
            if ((features & (SimdFeature.SSE2 | SimdFeature.NEON)) != 0) return 4;
            // 无SIMD: 1个float
            return 1;
        }

        #endregion
    }
}
