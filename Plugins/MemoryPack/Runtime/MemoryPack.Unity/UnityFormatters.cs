#nullable enable

using MemoryPack.Internal;
using UnityEngine;

namespace MemoryPack
{   
    /// <summary>
    /// AnimationCurve格式化器
    /// </summary>
    [Preserve]
    internal sealed class AnimationCurveFormatter : MemoryPackFormatter<AnimationCurve>
    {
        /// <summary>
        /// 序列化AnimationCurve
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">AnimationCurve</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref AnimationCurve? value)
        {
            if (value == null)
            {
                writer.WriteNullObjectHeader();
                return;
            }

            writer.WriteUnmanagedWithObjectHeader(3, value.@preWrapMode, value.@postWrapMode);
            writer.WriteUnmanagedArray(value.@keys);
        }

        /// <summary>
        /// 反序列化AnimationCurve
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">AnimationCurve</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref AnimationCurve? value)
        {
            if (!reader.TryReadObjectHeader(out var count))
            {
                value = null;
                return;
            }

            if (count != 3) MemoryPackSerializationException.ThrowInvalidPropertyCount(3, count);

            reader.ReadUnmanaged(out WrapMode preWrapMode, out WrapMode postWrapMode);
            var keys = reader.ReadUnmanagedArray<global::UnityEngine.Keyframe>();

            if (value == null)
            {
                value = new AnimationCurve();
            }

            value.preWrapMode = preWrapMode;
            value.postWrapMode = postWrapMode;
            value.keys = keys;
        }
    }

    /// <summary>
    /// Gradient格式化器
    /// </summary>
    [Preserve]
    internal sealed class GradientFormatter : MemoryPackFormatter<Gradient>
    {
        /// <summary>
        /// 序列化Gradient
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">Gradient</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref Gradient? value)
        {
            if (value == null)
            {
                writer.WriteNullObjectHeader();
                return;
            }

            writer.WriteObjectHeader(3);
            writer.WriteUnmanagedArray(value.@colorKeys);
            writer.WriteUnmanagedArray(value.@alphaKeys);
            writer.WriteUnmanaged(value.@mode);
        }

        /// <summary>
        /// 反序列化Gradient
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">Gradient</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref Gradient? value)
        {
            if (!reader.TryReadObjectHeader(out var count))
            {
                value = null;
                return;
            }

            if (count != 3) MemoryPackSerializationException.ThrowInvalidPropertyCount(3, count);

            var colorKeys = reader.ReadUnmanagedArray<global::UnityEngine.GradientColorKey>();
            var alphaKeys = reader.ReadUnmanagedArray<global::UnityEngine.GradientAlphaKey>();
            reader.ReadUnmanaged(out GradientMode mode);

            if (value == null)
            {
                value = new Gradient();
            }

            value.colorKeys = colorKeys;
            value.alphaKeys = alphaKeys;
            value.mode = mode;
        }
    }

    /// <summary>
    /// RectOffset格式化器
    /// </summary>
    [Preserve]
    internal sealed class RectOffsetFormatter : MemoryPackFormatter<RectOffset>
    {
        /// <summary>
        /// 序列化RectOffset
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">RectOffset</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref RectOffset? value)
        {
            if (value == null)
            {
                writer.WriteNullObjectHeader();
                return;
            }

            writer.WriteUnmanagedWithObjectHeader(4, value.@left, value.@right, value.@top, value.@bottom);
        }

        /// <summary>
        /// 反序列化RectOffset
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">RectOffset</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref RectOffset? value)
        {
            if (!reader.TryReadObjectHeader(out var count))
            {
                value = null;
                return;
            }

            if (count != 4) MemoryPackSerializationException.ThrowInvalidPropertyCount(4, count);

            reader.ReadUnmanaged(out int left, out int right, out int top, out int bottom);

            if (value == null)
            {
                value = new RectOffset(left, right, top, bottom);
            }
            else
            {
                value.left = left;
                value.right = right;
                value.top = top;
                value.bottom = bottom;
            }
        }
    }
}
