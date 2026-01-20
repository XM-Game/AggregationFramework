// ==========================================================
// 文件名：PoolSerializer.cs
// 命名空间: AFramework.Pool.Utilities
// 依赖: System, System.IO, System.Text, AFramework.Pool
// 功能: 对象池序列化器，提供池状态的保存和加载功能
// ==========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AFramework.Pool.Utilities
{
    /// <summary>
    /// 对象池序列化器
    /// Pool Serializer
    /// </summary>
    /// <remarks>
    /// 提供对象池状态的序列化和反序列化功能，包括：
    /// - 统计信息序列化
    /// - 配置信息序列化
    /// - JSON/二进制格式支持
    /// - 状态快照保存/加载
    /// Provides serialization and deserialization for pool state, including:
    /// - Statistics serialization
    /// - Configuration serialization
    /// - JSON/Binary format support
    /// - State snapshot save/load
    /// </remarks>
    public static class PoolSerializer
    {
        #region JSON 序列化 JSON Serialization

        /// <summary>
        /// 将池统计信息序列化为 JSON
        /// Serialize pool statistics to JSON
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        /// <returns>JSON 字符串 / JSON string</returns>
        public static string SerializeStatisticsToJson(IPoolStatistics statistics)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"totalCreated\": {statistics.TotalCreated},");
            sb.AppendLine($"  \"totalDestroyed\": {statistics.TotalDestroyed},");
            sb.AppendLine($"  \"currentActive\": {statistics.CurrentActive},");
            sb.AppendLine($"  \"currentIdle\": {statistics.CurrentIdle},");
            sb.AppendLine($"  \"currentTotal\": {statistics.CurrentTotal},");
            sb.AppendLine($"  \"totalGets\": {statistics.TotalGets},");
            sb.AppendLine($"  \"totalReturns\": {statistics.TotalReturns},");
            sb.AppendLine($"  \"hits\": {statistics.Hits},");
            sb.AppendLine($"  \"misses\": {statistics.Misses},");
            sb.AppendLine($"  \"hitRate\": {statistics.HitRate:F6},");
            sb.AppendLine($"  \"missRate\": {statistics.MissRate:F6},");
            sb.AppendLine($"  \"averageGetTime\": {statistics.AverageGetTime:F6},");
            sb.AppendLine($"  \"averageReturnTime\": {statistics.AverageReturnTime:F6},");
            sb.AppendLine($"  \"peakActive\": {statistics.PeakActive},");
            sb.AppendLine($"  \"peakTotal\": {statistics.PeakTotal},");
            sb.AppendLine($"  \"estimatedMemoryUsage\": {statistics.EstimatedMemoryUsage},");
            sb.AppendLine($"  \"createdTime\": \"{statistics.CreatedTime:O}\",");
            sb.AppendLine($"  \"uptime\": \"{statistics.Uptime}\",");
            sb.AppendLine($"  \"lastGetTime\": {FormatNullableDateTime(statistics.LastGetTime)},");
            sb.AppendLine($"  \"lastReturnTime\": {FormatNullableDateTime(statistics.LastReturnTime)}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// 将池状态序列化为 JSON
        /// Serialize pool state to JSON
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <returns>JSON 字符串 / JSON string</returns>
        public static string SerializePoolStateToJson(IObjectPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"objectType\": \"{pool.ObjectType?.FullName ?? "Unknown"}\",");
            sb.AppendLine($"  \"state\": \"{pool.State}\",");
            sb.AppendLine($"  \"availableCount\": {pool.AvailableCount},");
            sb.AppendLine($"  \"activeCount\": {pool.ActiveCount},");
            sb.AppendLine($"  \"totalCount\": {pool.TotalCount}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// 将池配置序列化为 JSON
        /// Serialize pool configuration to JSON
        /// </summary>
        /// <param name="config">池配置 / Pool configuration</param>
        /// <returns>JSON 字符串 / JSON string</returns>
        public static string SerializeConfigurationToJson(PoolConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"initialCapacity\": {config.InitialCapacity},");
            sb.AppendLine($"  \"maxCapacity\": {config.MaxCapacity},");
            sb.AppendLine($"  \"minCapacity\": {config.MinCapacity}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        #endregion

        #region 快照序列化 Snapshot Serialization

        /// <summary>
        /// 将统计快照序列化为 JSON
        /// Serialize statistics snapshot to JSON
        /// </summary>
        /// <param name="snapshot">统计快照 / Statistics snapshot</param>
        /// <returns>JSON 字符串 / JSON string</returns>
        public static string SerializeSnapshotToJson(PoolStatisticsSnapshot snapshot)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"snapshotTime\": \"{snapshot.SnapshotTime:O}\",");
            sb.AppendLine($"  \"createdTime\": \"{snapshot.CreatedTime:O}\",");
            sb.AppendLine($"  \"uptime\": \"{snapshot.Uptime}\",");
            sb.AppendLine($"  \"totalCreated\": {snapshot.TotalCreated},");
            sb.AppendLine($"  \"totalDestroyed\": {snapshot.TotalDestroyed},");
            sb.AppendLine($"  \"currentActive\": {snapshot.CurrentActive},");
            sb.AppendLine($"  \"currentIdle\": {snapshot.CurrentIdle},");
            sb.AppendLine($"  \"currentTotal\": {snapshot.CurrentTotal},");
            sb.AppendLine($"  \"totalGets\": {snapshot.TotalGets},");
            sb.AppendLine($"  \"totalReturns\": {snapshot.TotalReturns},");
            sb.AppendLine($"  \"hits\": {snapshot.Hits},");
            sb.AppendLine($"  \"misses\": {snapshot.Misses},");
            sb.AppendLine($"  \"hitRate\": {snapshot.HitRate:F6},");
            sb.AppendLine($"  \"missRate\": {snapshot.MissRate:F6},");
            sb.AppendLine($"  \"averageGetTime\": {snapshot.AverageGetTime:F6},");
            sb.AppendLine($"  \"averageReturnTime\": {snapshot.AverageReturnTime:F6},");
            sb.AppendLine($"  \"peakActive\": {snapshot.PeakActive},");
            sb.AppendLine($"  \"peakTotal\": {snapshot.PeakTotal},");
            sb.AppendLine($"  \"estimatedMemoryUsage\": {snapshot.EstimatedMemoryUsage}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// 将多个快照序列化为 JSON 数组
        /// Serialize multiple snapshots to JSON array
        /// </summary>
        /// <param name="snapshots">快照列表 / Snapshot list</param>
        /// <returns>JSON 字符串 / JSON string</returns>
        public static string SerializeSnapshotsToJson(IEnumerable<PoolStatisticsSnapshot> snapshots)
        {
            if (snapshots == null)
                throw new ArgumentNullException(nameof(snapshots));

            var sb = new StringBuilder();
            sb.AppendLine("[");

            bool first = true;
            foreach (var snapshot in snapshots)
            {
                if (!first)
                    sb.AppendLine(",");

                sb.Append(SerializeSnapshotToJson(snapshot).TrimEnd());
                first = false;
            }

            sb.AppendLine();
            sb.AppendLine("]");

            return sb.ToString();
        }

        #endregion

        #region 文件操作 File Operations

        /// <summary>
        /// 保存池统计信息到文件
        /// Save pool statistics to file
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        /// <param name="filePath">文件路径 / File path</param>
        public static void SaveStatisticsToFile(IPoolStatistics statistics, string filePath)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空 File path cannot be empty", nameof(filePath));

            var json = SerializeStatisticsToJson(statistics);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        /// <summary>
        /// 保存池状态到文件
        /// Save pool state to file
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="filePath">文件路径 / File path</param>
        public static void SavePoolStateToFile(IObjectPool pool, string filePath)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空 File path cannot be empty", nameof(filePath));

            var json = SerializePoolStateToJson(pool);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        /// <summary>
        /// 保存快照到文件
        /// Save snapshot to file
        /// </summary>
        /// <param name="snapshot">统计快照 / Statistics snapshot</param>
        /// <param name="filePath">文件路径 / File path</param>
        public static void SaveSnapshotToFile(PoolStatisticsSnapshot snapshot, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空 File path cannot be empty", nameof(filePath));

            var json = SerializeSnapshotToJson(snapshot);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        /// <summary>
        /// 保存多个快照到文件
        /// Save multiple snapshots to file
        /// </summary>
        /// <param name="snapshots">快照列表 / Snapshot list</param>
        /// <param name="filePath">文件路径 / File path</param>
        public static void SaveSnapshotsToFile(IEnumerable<PoolStatisticsSnapshot> snapshots, string filePath)
        {
            if (snapshots == null)
                throw new ArgumentNullException(nameof(snapshots));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空 File path cannot be empty", nameof(filePath));

            var json = SerializeSnapshotsToJson(snapshots);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        #endregion

        #region CSV 导出 CSV Export

        /// <summary>
        /// 将统计信息导出为 CSV 格式
        /// Export statistics to CSV format
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        /// <returns>CSV 字符串 / CSV string</returns>
        public static string ExportStatisticsToCSV(IPoolStatistics statistics)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));

            var sb = new StringBuilder();

            // CSV 头部 / CSV header
            sb.AppendLine("Metric,Value");

            // 数据行 / Data rows
            sb.AppendLine($"TotalCreated,{statistics.TotalCreated}");
            sb.AppendLine($"TotalDestroyed,{statistics.TotalDestroyed}");
            sb.AppendLine($"CurrentActive,{statistics.CurrentActive}");
            sb.AppendLine($"CurrentIdle,{statistics.CurrentIdle}");
            sb.AppendLine($"CurrentTotal,{statistics.CurrentTotal}");
            sb.AppendLine($"TotalGets,{statistics.TotalGets}");
            sb.AppendLine($"TotalReturns,{statistics.TotalReturns}");
            sb.AppendLine($"Hits,{statistics.Hits}");
            sb.AppendLine($"Misses,{statistics.Misses}");
            sb.AppendLine($"HitRate,{statistics.HitRate:F6}");
            sb.AppendLine($"MissRate,{statistics.MissRate:F6}");
            sb.AppendLine($"AverageGetTime,{statistics.AverageGetTime:F6}");
            sb.AppendLine($"AverageReturnTime,{statistics.AverageReturnTime:F6}");
            sb.AppendLine($"PeakActive,{statistics.PeakActive}");
            sb.AppendLine($"PeakTotal,{statistics.PeakTotal}");
            sb.AppendLine($"EstimatedMemoryUsage,{statistics.EstimatedMemoryUsage}");

            return sb.ToString();
        }

        /// <summary>
        /// 将多个快照导出为 CSV 格式（时间序列）
        /// Export multiple snapshots to CSV format (time series)
        /// </summary>
        /// <param name="snapshots">快照列表 / Snapshot list</param>
        /// <returns>CSV 字符串 / CSV string</returns>
        public static string ExportSnapshotsToCSV(IEnumerable<PoolStatisticsSnapshot> snapshots)
        {
            if (snapshots == null)
                throw new ArgumentNullException(nameof(snapshots));

            var sb = new StringBuilder();

            // CSV 头部 / CSV header
            sb.AppendLine("SnapshotTime,TotalCreated,TotalDestroyed,CurrentActive,CurrentIdle,CurrentTotal," +
                "TotalGets,TotalReturns,Hits,Misses,HitRate,AverageGetTime,AverageReturnTime,PeakActive,PeakTotal");

            // 数据行 / Data rows
            foreach (var snapshot in snapshots)
            {
                sb.AppendLine($"{snapshot.SnapshotTime:O}," +
                    $"{snapshot.TotalCreated}," +
                    $"{snapshot.TotalDestroyed}," +
                    $"{snapshot.CurrentActive}," +
                    $"{snapshot.CurrentIdle}," +
                    $"{snapshot.CurrentTotal}," +
                    $"{snapshot.TotalGets}," +
                    $"{snapshot.TotalReturns}," +
                    $"{snapshot.Hits}," +
                    $"{snapshot.Misses}," +
                    $"{snapshot.HitRate:F6}," +
                    $"{snapshot.AverageGetTime:F6}," +
                    $"{snapshot.AverageReturnTime:F6}," +
                    $"{snapshot.PeakActive}," +
                    $"{snapshot.PeakTotal}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 保存 CSV 到文件
        /// Save CSV to file
        /// </summary>
        /// <param name="csv">CSV 字符串 / CSV string</param>
        /// <param name="filePath">文件路径 / File path</param>
        public static void SaveCSVToFile(string csv, string filePath)
        {
            if (string.IsNullOrEmpty(csv))
                throw new ArgumentException("CSV 内容不能为空 CSV content cannot be empty", nameof(csv));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空 File path cannot be empty", nameof(filePath));

            File.WriteAllText(filePath, csv, Encoding.UTF8);
        }

        #endregion

        #region 辅助方法 Helper Methods

        /// <summary>
        /// 格式化可空日期时间
        /// Format nullable datetime
        /// </summary>
        private static string FormatNullableDateTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? $"\"{dateTime.Value:O}\"" : "null";
        }

        #endregion
    }
}
