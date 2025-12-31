// ==========================================================
// 文件名：SceneExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, UnityEngine.SceneManagement, System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Scene 扩展方法
    /// <para>提供 Scene 的查询和实用功能扩展</para>
    /// </summary>
    public static class SceneExtensions
    {
        #region 状态检查

        /// <summary>
        /// 检查场景是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidScene(this Scene scene)
        {
            return scene.IsValid();
        }

        /// <summary>
        /// 检查场景是否已加载
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLoadedScene(this Scene scene)
        {
            return scene.IsValid() && scene.isLoaded;
        }

        /// <summary>
        /// 检查是否为活动场景
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveScene(this Scene scene)
        {
            return scene.IsValid() && scene == SceneManager.GetActiveScene();
        }

        /// <summary>
        /// 检查场景是否为空 (没有根对象)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this Scene scene)
        {
            return !scene.IsValid() || scene.rootCount == 0;
        }

        #endregion

        #region 对象查询

        /// <summary>
        /// 获取场景中的所有根 GameObject
        /// </summary>
        public static GameObject[] GetRootGameObjects(this Scene scene)
        {
            if (!scene.IsValid()) return Array.Empty<GameObject>();
            return scene.GetRootGameObjects();
        }

        /// <summary>
        /// 获取场景中的所有 GameObject (包括子对象)
        /// </summary>
        public static List<GameObject> GetAllGameObjects(this Scene scene)
        {
            var result = new List<GameObject>();
            if (!scene.IsValid()) return result;

            foreach (var root in scene.GetRootGameObjects())
            {
                GetAllGameObjectsRecursive(root.transform, result);
            }
            return result;
        }

        private static void GetAllGameObjectsRecursive(Transform transform, List<GameObject> result)
        {
            result.Add(transform.gameObject);
            foreach (Transform child in transform)
            {
                GetAllGameObjectsRecursive(child, result);
            }
        }

        /// <summary>
        /// 获取场景中所有指定类型的组件
        /// </summary>
        public static List<T> FindComponentsOfType<T>(this Scene scene) where T : Component
        {
            var result = new List<T>();
            if (!scene.IsValid()) return result;

            foreach (var root in scene.GetRootGameObjects())
            {
                result.AddRange(root.GetComponentsInChildren<T>(true));
            }
            return result;
        }

        /// <summary>
        /// 获取场景中第一个指定类型的组件
        /// </summary>
        public static T FindComponentOfType<T>(this Scene scene) where T : Component
        {
            if (!scene.IsValid()) return null;

            foreach (var root in scene.GetRootGameObjects())
            {
                var component = root.GetComponentInChildren<T>(true);
                if (component != null) return component;
            }
            return null;
        }

        /// <summary>
        /// 在场景中查找指定名称的 GameObject
        /// </summary>
        public static GameObject FindGameObject(this Scene scene, string name)
        {
            if (!scene.IsValid() || string.IsNullOrEmpty(name)) return null;

            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root;

                var found = FindGameObjectRecursive(root.transform, name);
                if (found != null) return found;
            }
            return null;
        }

        private static GameObject FindGameObjectRecursive(Transform transform, string name)
        {
            foreach (Transform child in transform)
            {
                if (child.name == name) return child.gameObject;

                var found = FindGameObjectRecursive(child, name);
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>
        /// 在场景中查找指定标签的 GameObject
        /// </summary>
        public static List<GameObject> FindGameObjectsWithTag(this Scene scene, string tag)
        {
            var result = new List<GameObject>();
            if (!scene.IsValid() || string.IsNullOrEmpty(tag)) return result;

            foreach (var root in scene.GetRootGameObjects())
            {
                FindGameObjectsWithTagRecursive(root.transform, tag, result);
            }
            return result;
        }

        private static void FindGameObjectsWithTagRecursive(Transform transform, string tag, List<GameObject> result)
        {
            if (transform.CompareTag(tag))
            {
                result.Add(transform.gameObject);
            }

            foreach (Transform child in transform)
            {
                FindGameObjectsWithTagRecursive(child, tag, result);
            }
        }

        #endregion

        #region 统计

        /// <summary>
        /// 获取场景中的 GameObject 总数
        /// </summary>
        public static int GetGameObjectCount(this Scene scene)
        {
            if (!scene.IsValid()) return 0;

            int count = 0;
            foreach (var root in scene.GetRootGameObjects())
            {
                count += CountGameObjectsRecursive(root.transform);
            }
            return count;
        }

        private static int CountGameObjectsRecursive(Transform transform)
        {
            int count = 1;
            foreach (Transform child in transform)
            {
                count += CountGameObjectsRecursive(child);
            }
            return count;
        }

        /// <summary>
        /// 获取场景中指定类型组件的数量
        /// </summary>
        public static int GetComponentCount<T>(this Scene scene) where T : Component
        {
            if (!scene.IsValid()) return 0;

            int count = 0;
            foreach (var root in scene.GetRootGameObjects())
            {
                count += root.GetComponentsInChildren<T>(true).Length;
            }
            return count;
        }

        #endregion

        #region 场景操作

        /// <summary>
        /// 设置为活动场景
        /// </summary>
        public static bool SetAsActive(this Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded) return false;
            return SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 将 GameObject 移动到此场景
        /// </summary>
        public static void MoveGameObjectToScene(this Scene scene, GameObject gameObject)
        {
            if (!scene.IsValid() || gameObject == null) return;
            SceneManager.MoveGameObjectToScene(gameObject, scene);
        }

        /// <summary>
        /// 将多个 GameObject 移动到此场景
        /// </summary>
        public static void MoveGameObjectsToScene(this Scene scene, IEnumerable<GameObject> gameObjects)
        {
            if (!scene.IsValid() || gameObjects == null) return;

            foreach (var go in gameObjects)
            {
                if (go != null)
                {
                    SceneManager.MoveGameObjectToScene(go, scene);
                }
            }
        }

        #endregion

        #region 清理

        /// <summary>
        /// 销毁场景中的所有 GameObject
        /// </summary>
        public static void DestroyAllGameObjects(this Scene scene)
        {
            if (!scene.IsValid()) return;

            foreach (var root in scene.GetRootGameObjects())
            {
                UnityEngine.Object.Destroy(root);
            }
        }

        /// <summary>
        /// 立即销毁场景中的所有 GameObject
        /// </summary>
        public static void DestroyAllGameObjectsImmediate(this Scene scene)
        {
            if (!scene.IsValid()) return;

            foreach (var root in scene.GetRootGameObjects())
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        /// <summary>
        /// 销毁场景中指定标签的所有 GameObject
        /// </summary>
        public static void DestroyGameObjectsWithTag(this Scene scene, string tag)
        {
            var objects = scene.FindGameObjectsWithTag(tag);
            foreach (var obj in objects)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        #endregion
    }
}
