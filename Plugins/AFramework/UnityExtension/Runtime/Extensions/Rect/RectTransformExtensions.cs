// ==========================================================
// 文件名：RectTransformExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// RectTransform 扩展方法
    /// <para>提供 RectTransform 的布局操作和实用功能扩展</para>
    /// </summary>
    public static class RectTransformExtensions
    {
        #region 锚点设置

        /// <summary>
        /// 设置锚点为左上角
        /// </summary>
        public static void SetAnchorTopLeft(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
        }

        /// <summary>
        /// 设置锚点为顶部中心
        /// </summary>
        public static void SetAnchorTopCenter(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
        }

        /// <summary>
        /// 设置锚点为右上角
        /// </summary>
        public static void SetAnchorTopRight(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
        }

        /// <summary>
        /// 设置锚点为左侧中心
        /// </summary>
        public static void SetAnchorMiddleLeft(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
        }

        /// <summary>
        /// 设置锚点为中心
        /// </summary>
        public static void SetAnchorMiddleCenter(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// 设置锚点为右侧中心
        /// </summary>
        public static void SetAnchorMiddleRight(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(1f, 0.5f);
            rt.anchorMax = new Vector2(1f, 0.5f);
        }

        /// <summary>
        /// 设置锚点为左下角
        /// </summary>
        public static void SetAnchorBottomLeft(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
        }

        /// <summary>
        /// 设置锚点为底部中心
        /// </summary>
        public static void SetAnchorBottomCenter(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
        }

        /// <summary>
        /// 设置锚点为右下角
        /// </summary>
        public static void SetAnchorBottomRight(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
        }

        /// <summary>
        /// 设置锚点为拉伸填充
        /// </summary>
        public static void SetAnchorStretch(this RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
        }

        /// <summary>
        /// 设置锚点为水平拉伸
        /// </summary>
        public static void SetAnchorStretchHorizontal(this RectTransform rt, float verticalAnchor = 0.5f)
        {
            rt.anchorMin = new Vector2(0f, verticalAnchor);
            rt.anchorMax = new Vector2(1f, verticalAnchor);
        }

        /// <summary>
        /// 设置锚点为垂直拉伸
        /// </summary>
        public static void SetAnchorStretchVertical(this RectTransform rt, float horizontalAnchor = 0.5f)
        {
            rt.anchorMin = new Vector2(horizontalAnchor, 0f);
            rt.anchorMax = new Vector2(horizontalAnchor, 1f);
        }

        #endregion

        #region 尺寸设置

        /// <summary>
        /// 设置宽度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWidth(this RectTransform rt, float width)
        {
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
        }

        /// <summary>
        /// 设置高度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHeight(this RectTransform rt, float height)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        }

        /// <summary>
        /// 设置尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSize(this RectTransform rt, Vector2 size)
        {
            rt.sizeDelta = size;
        }

        /// <summary>
        /// 设置尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSize(this RectTransform rt, float width, float height)
        {
            rt.sizeDelta = new Vector2(width, height);
        }

        /// <summary>
        /// 获取实际宽度 (考虑锚点)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetWidth(this RectTransform rt) => rt.rect.width;

        /// <summary>
        /// 获取实际高度 (考虑锚点)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetHeight(this RectTransform rt) => rt.rect.height;

        /// <summary>
        /// 获取实际尺寸 (考虑锚点)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSize(this RectTransform rt) => rt.rect.size;

        #endregion

        #region 位置设置

        /// <summary>
        /// 设置锚点位置 X
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAnchoredPositionX(this RectTransform rt, float x)
        {
            rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);
        }

        /// <summary>
        /// 设置锚点位置 Y
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAnchoredPositionY(this RectTransform rt, float y)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
        }

        /// <summary>
        /// 设置左边距 (需要锚点为左侧)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        /// <summary>
        /// 设置右边距 (需要锚点为右侧)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        /// <summary>
        /// 设置上边距 (需要锚点为顶部)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        /// <summary>
        /// 设置下边距 (需要锚点为底部)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        /// <summary>
        /// 设置所有边距 (需要锚点为拉伸)
        /// </summary>
        public static void SetMargins(this RectTransform rt, float left, float right, float top, float bottom)
        {
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        /// <summary>
        /// 设置统一边距 (需要锚点为拉伸)
        /// </summary>
        public static void SetMargins(this RectTransform rt, float margin)
        {
            rt.SetMargins(margin, margin, margin, margin);
        }

        #endregion

        #region 轴心点设置

        /// <summary>
        /// 设置轴心点 X
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPivotX(this RectTransform rt, float x)
        {
            rt.pivot = new Vector2(x, rt.pivot.y);
        }

        /// <summary>
        /// 设置轴心点 Y
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPivotY(this RectTransform rt, float y)
        {
            rt.pivot = new Vector2(rt.pivot.x, y);
        }

        /// <summary>
        /// 设置轴心点为中心
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPivotCenter(this RectTransform rt)
        {
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// 设置轴心点而不改变位置
        /// </summary>
        public static void SetPivotWithoutMoving(this RectTransform rt, Vector2 newPivot)
        {
            Vector2 deltaPivot = newPivot - rt.pivot;
            Vector2 deltaPosition = new Vector2(
                deltaPivot.x * rt.rect.width,
                deltaPivot.y * rt.rect.height);
            rt.pivot = newPivot;
            rt.anchoredPosition += deltaPosition;
        }

        #endregion

        #region 填充和适配

        /// <summary>
        /// 填充父容器
        /// </summary>
        public static void FillParent(this RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 填充父容器并保留边距
        /// </summary>
        public static void FillParentWithMargin(this RectTransform rt, float margin)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(margin, margin);
            rt.offsetMax = new Vector2(-margin, -margin);
        }

        /// <summary>
        /// 居中于父容器
        /// </summary>
        public static void CenterInParent(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 重置为默认状态
        /// </summary>
        public static void ResetToDefault(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(100f, 100f);
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }

        #endregion

        #region 坐标转换

        /// <summary>
        /// 获取世界空间中的四个角点
        /// </summary>
        public static Vector3[] GetWorldCorners(this RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return corners;
        }

        /// <summary>
        /// 获取世界空间中的矩形
        /// </summary>
        public static Rect GetWorldRect(this RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y,
                corners[2].x - corners[0].x,
                corners[2].y - corners[0].y);
        }

        /// <summary>
        /// 获取屏幕空间中的矩形
        /// </summary>
        public static Rect GetScreenRect(this RectTransform rt, Camera camera = null)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            if (camera != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    corners[i] = camera.WorldToScreenPoint(corners[i]);
                }
            }

            return new Rect(corners[0].x, corners[0].y,
                corners[2].x - corners[0].x,
                corners[2].y - corners[0].y);
        }

        /// <summary>
        /// 检查屏幕点是否在 RectTransform 内
        /// </summary>
        public static bool ContainsScreenPoint(this RectTransform rt, Vector2 screenPoint, Camera camera = null)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rt, screenPoint, camera);
        }

        /// <summary>
        /// 将屏幕点转换为本地点
        /// </summary>
        public static bool ScreenPointToLocalPoint(this RectTransform rt, Vector2 screenPoint, Camera camera, out Vector2 localPoint)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, camera, out localPoint);
        }

        #endregion

        #region 检查

        /// <summary>
        /// 检查是否与另一个 RectTransform 重叠
        /// </summary>
        public static bool Overlaps(this RectTransform rt, RectTransform other)
        {
            Rect rect1 = rt.GetWorldRect();
            Rect rect2 = other.GetWorldRect();
            return rect1.Overlaps(rect2);
        }

        /// <summary>
        /// 检查是否完全包含另一个 RectTransform
        /// </summary>
        public static bool Contains(this RectTransform rt, RectTransform other)
        {
            Rect rect1 = rt.GetWorldRect();
            Rect rect2 = other.GetWorldRect();
            return rect1.Contains(rect2.min) && rect1.Contains(rect2.max);
        }

        #endregion
    }
}
