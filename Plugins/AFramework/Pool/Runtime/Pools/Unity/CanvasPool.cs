// ==========================================================
// 文件名：CanvasPool.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: UI Canvas 对象池，支持 UI 元素池化管理
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.Pool
{
    /// <summary>
    /// Canvas 对象池
    /// Canvas Object Pool
    /// </summary>
    /// <remarks>
    /// 专为 Unity UI Canvas 设计的对象池，支持：
    /// - Canvas 层级管理
    /// - 渲染模式配置
    /// - 排序层管理
    /// - UI 元素池化
    /// Designed specifically for Unity UI Canvas, supports:
    /// - Canvas hierarchy management
    /// - Render mode configuration
    /// - Sorting layer management
    /// - UI element pooling
    /// </remarks>
    public class CanvasPool : ComponentPool<Canvas>
    {
        #region 字段 Fields

        private readonly RenderMode _renderMode;
        private readonly int _sortingOrder;
        private readonly Camera _worldCamera;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 渲染模式
        /// Render mode
        /// </summary>
        public RenderMode RenderMode => _renderMode;

        /// <summary>
        /// 排序顺序
        /// Sorting order
        /// </summary>
        public int SortingOrder => _sortingOrder;

        /// <summary>
        /// 世界相机
        /// World camera
        /// </summary>
        public Camera WorldCamera => _worldCamera;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建 Canvas 对象池
        /// Create Canvas object pool
        /// </summary>
        /// <param name="prefab">Canvas 预制体 Canvas prefab</param>
        /// <param name="parent">父级变换（可选）Parent transform (optional)</param>
        /// <param name="initialCapacity">初始容量 Initial capacity</param>
        /// <param name="maxCapacity">最大容量 Maximum capacity</param>
        /// <param name="renderMode">渲染模式 Render mode</param>
        /// <param name="sortingOrder">排序顺序 Sorting order</param>
        /// <param name="worldCamera">世界相机（可选）World camera (optional)</param>
        public CanvasPool(
            Canvas prefab,
            Transform parent = null,
            int initialCapacity = 5,
            int maxCapacity = 20,
            RenderMode renderMode = RenderMode.ScreenSpaceOverlay,
            int sortingOrder = 0,
            Camera worldCamera = null)
            : base(prefab, parent, initialCapacity, maxCapacity)
        {
            _renderMode = renderMode;
            _sortingOrder = sortingOrder;
            _worldCamera = worldCamera;
        }

        #endregion

        #region 虚方法重写 Virtual Method Overrides

        protected override void OnCreateNew(Canvas canvas)
        {
            base.OnCreateNew(canvas);
            ConfigureCanvas(canvas);
        }

        protected override void OnGetFromPool(Canvas canvas)
        {
            base.OnGetFromPool(canvas);
            ConfigureCanvas(canvas);
        }

        #endregion

        #region 私有方法 Private Methods

        /// <summary>
        /// 配置 Canvas
        /// Configure Canvas
        /// </summary>
        private void ConfigureCanvas(Canvas canvas)
        {
            canvas.renderMode = _renderMode;
            canvas.sortingOrder = _sortingOrder;

            if (_renderMode == RenderMode.ScreenSpaceCamera || _renderMode == RenderMode.WorldSpace)
            {
                canvas.worldCamera = _worldCamera;
            }
        }

        #endregion
    }
}
