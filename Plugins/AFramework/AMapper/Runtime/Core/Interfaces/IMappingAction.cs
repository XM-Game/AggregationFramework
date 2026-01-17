// ==========================================================
// 文件名：IMappingAction.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义映射动作接口，用于在映射前后执行自定义逻辑
// ==========================================================

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射动作接口
    /// <para>用于在映射前后执行自定义逻辑</para>
    /// <para>Mapping action interface for custom logic before/after mapping</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <remarks>
    /// 映射动作用于在映射执行前后添加自定义处理逻辑。
    /// 支持依赖注入，可以访问外部服务。
    /// 
    /// 使用示例：
    /// <code>
    /// public class AuditAction : IMappingAction&lt;Entity, EntityDto&gt;
    /// {
    ///     private readonly IAuditService _auditService;
    ///     
    ///     public AuditAction(IAuditService auditService)
    ///     {
    ///         _auditService = auditService;
    ///     }
    ///     
    ///     public void Process(Entity source, EntityDto destination, ResolutionContext context)
    ///     {
    ///         _auditService.LogMapping(source.Id, destination.GetType().Name);
    ///     }
    /// }
    /// 
    /// // 配置
    /// CreateMap&lt;Entity, EntityDto&gt;()
    ///     .AfterMap&lt;AuditAction&gt;();
    /// </code>
    /// 
    /// BeforeMap vs AfterMap：
    /// <list type="bullet">
    /// <item>BeforeMap：在成员映射之前执行，目标对象已创建但未填充</item>
    /// <item>AfterMap：在成员映射之后执行，目标对象已完全填充</item>
    /// </list>
    /// </remarks>
    public interface IMappingAction<in TSource, in TDestination>
    {
        /// <summary>
        /// 处理映射动作
        /// <para>Process the mapping action</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        void Process(TSource source, TDestination destination, ResolutionContext context);
    }

    /// <summary>
    /// 非泛型映射动作接口
    /// <para>Non-generic mapping action interface</para>
    /// </summary>
    public interface IMappingAction
    {
        /// <summary>
        /// 处理映射动作
        /// <para>Process the mapping action</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        void Process(object source, object destination, ResolutionContext context);
    }
}
