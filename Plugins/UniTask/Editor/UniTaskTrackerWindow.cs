#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEditor.IMGUI.Controls;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks.Editor
{
    /// <summary>
    /// 任务跟踪器窗口类
    /// </summary>
    /// <remarks>
    /// 继承自 EditorWindow，用于在编辑器中显示任务跟踪器窗口
    /// </remarks>
    public class UniTaskTrackerWindow : EditorWindow
    {
        /// <summary>
        /// 更新间隔
        /// </summary>
        static int interval;

        /// <summary>
        /// 单例实例
        /// </summary>
        static UniTaskTrackerWindow window;

        /// <summary>
        /// 打开任务跟踪器窗口
        /// </summary>
        /// <remarks>
        /// 使用 [MenuItem] 属性标记，使得在编辑器菜单中显示
        /// </remarks>
        [MenuItem("Window/UniTask Tracker")]
        public static void OpenWindow()
        {
            if (window != null)
            {
                window.Close();
            }

            // 调用 OnEnable(单例实例将被设置)
            GetWindow<UniTaskTrackerWindow>("UniTask Tracker").Show();
        }

        /// <summary>
        /// 空布局选项
        /// </summary>
        static readonly GUILayoutOption[] EmptyLayoutOption = new GUILayoutOption[0];

        /// <summary>
        /// 任务跟踪器树视图
        /// </summary>
        UniTaskTrackerTreeView treeView;

        /// <summary>
        /// 分割状态
        /// </summary>
        object splitterState;

        /// <summary>
        /// 启用时调用
        /// </summary>
        /// <remarks>
        /// 设置单例实例、创建分割状态、创建任务跟踪器树视图、加载编辑器启用状态
        /// </remarks>
        void OnEnable()
        {
            window = this; // set singleton.
            splitterState = SplitterGUILayout.CreateSplitterState(new float[] { 75f, 25f }, new int[] { 32, 32 }, null);
            treeView = new UniTaskTrackerTreeView();
            TaskTracker.EditorEnableState.EnableAutoReload = EditorPrefs.GetBool(TaskTracker.EnableAutoReloadKey, false);
            TaskTracker.EditorEnableState.EnableTracking = EditorPrefs.GetBool(TaskTracker.EnableTrackingKey, false);
            TaskTracker.EditorEnableState.EnableStackTrace = EditorPrefs.GetBool(TaskTracker.EnableStackTraceKey, false);
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <remarks>
        /// 绘制头部面板、绘制分割布局、绘制表格、绘制详细面板
        /// </remarks>
        void OnGUI()
        {
            // Head
            RenderHeadPanel();

            // Splittable
            SplitterGUILayout.BeginVerticalSplit(this.splitterState, EmptyLayoutOption);
            {
                // Column Tabble
                RenderTable();

                // StackTrace details
                RenderDetailsPanel();
            }
            SplitterGUILayout.EndVerticalSplit();
        }

        #region HeadPanel

        public static bool EnableAutoReload => TaskTracker.EditorEnableState.EnableAutoReload;
        public static bool EnableTracking => TaskTracker.EditorEnableState.EnableTracking;
        public static bool EnableStackTrace => TaskTracker.EditorEnableState.EnableStackTrace;
        static readonly GUIContent EnableAutoReloadHeadContent = EditorGUIUtility.TrTextContent("启用自动重载 Enable AutoReload", "Reload automatically.", (Texture)null);
        static readonly GUIContent ReloadHeadContent = EditorGUIUtility.TrTextContent("Reload", "Reload View.", (Texture)null);
        static readonly GUIContent GCHeadContent = EditorGUIUtility.TrTextContent("垃圾回收 GC.Collect", "调用 GC.Collect。", (Texture)null);
        static readonly GUIContent EnableTrackingHeadContent = EditorGUIUtility.TrTextContent("启用跟踪 Enable Tracking", "开始跟踪 async/await UniTask。性能影响：低", (Texture)null);
        static readonly GUIContent EnableStackTraceHeadContent = EditorGUIUtility.TrTextContent("启用堆栈跟踪 Enable StackTrace", "当任务开始时捕获堆栈跟踪。性能影响：高", (Texture)null);

        // [启用跟踪] | [启用堆栈跟踪]
        void RenderHeadPanel()
        {
            EditorGUILayout.BeginVertical(EmptyLayoutOption);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, EmptyLayoutOption);

            if (GUILayout.Toggle(EnableAutoReload, EnableAutoReloadHeadContent, EditorStyles.toolbarButton, EmptyLayoutOption) != EnableAutoReload)
            {
                TaskTracker.EditorEnableState.EnableAutoReload = !EnableAutoReload;
            }

            if (GUILayout.Toggle(EnableTracking, EnableTrackingHeadContent, EditorStyles.toolbarButton, EmptyLayoutOption) != EnableTracking)
            {
                TaskTracker.EditorEnableState.EnableTracking = !EnableTracking;
            }

            if (GUILayout.Toggle(EnableStackTrace, EnableStackTraceHeadContent, EditorStyles.toolbarButton, EmptyLayoutOption) != EnableStackTrace)
            {
                TaskTracker.EditorEnableState.EnableStackTrace = !EnableStackTrace;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(ReloadHeadContent, EditorStyles.toolbarButton, EmptyLayoutOption))
            {
                // 检查并重置脏状态
                TaskTracker.CheckAndResetDirty();
                // 重新加载并排序
                treeView.ReloadAndSort();
                // 重绘窗口
                Repaint();
            }

            if (GUILayout.Button(GCHeadContent, EditorStyles.toolbarButton, EmptyLayoutOption))
            {
                GC.Collect(0);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region TableColumn

        Vector2 tableScroll;
        GUIStyle tableListStyle;

        void RenderTable()
        {
            if (tableListStyle == null)
            {
                tableListStyle = new GUIStyle("CN Box");
                tableListStyle.margin.top = 0;
                tableListStyle.padding.left = 3;
            }

            EditorGUILayout.BeginVertical(tableListStyle, EmptyLayoutOption);

            this.tableScroll = EditorGUILayout.BeginScrollView(this.tableScroll, new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.MaxWidth(2000f)
            });
            var controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true)
            });


            treeView?.OnGUI(controlRect);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void Update()
        {
            if (EnableAutoReload)
            {
                if (interval++ % 120 == 0)
                {
                    if (TaskTracker.CheckAndResetDirty())
                    {
                        treeView.ReloadAndSort();
                        Repaint();
                    }
                }
            }
        }

        #endregion

        #region Details

        static GUIStyle detailsStyle;
        Vector2 detailsScroll;

        void RenderDetailsPanel()
        {
            if (detailsStyle == null)
            {
                detailsStyle = new GUIStyle("CN Message");
                detailsStyle.wordWrap = false;
                detailsStyle.stretchHeight = true;
                detailsStyle.margin.right = 15;
            }

            string message = "";
            var selected = treeView.state.selectedIDs;
            if (selected.Count > 0)
            {
                var first = selected[0];
                var item = treeView.CurrentBindingItems.FirstOrDefault(x => x.id == first) as UniTaskTrackerViewItem;
                if (item != null)
                {
                    message = item.Position;
                }
            }

            detailsScroll = EditorGUILayout.BeginScrollView(this.detailsScroll, EmptyLayoutOption);
            var vector = detailsStyle.CalcSize(new GUIContent(message));
            EditorGUILayout.SelectableLabel(message, detailsStyle, new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true),
                GUILayout.MinWidth(vector.x),
                GUILayout.MinHeight(vector.y)
            });
            EditorGUILayout.EndScrollView();
        }

        #endregion
    }
}

