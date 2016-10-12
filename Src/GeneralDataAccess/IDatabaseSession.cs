#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IDatabaseSession.cs
// 文件功能描述：表示数据库操作。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20100906
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 表示数据库操作。
    /// </summary>
    public interface IDatabaseSession
    {
        #region 动作查询

        #region 非泛型

        /// <summary>
        /// 向数据库添加实体，忽略类型为 DateTime 且值为 DateTime.MinValue 的属性。
        /// </summary>
        /// <param name="entity">要添加的实体。</param>
        void Add(Object entity);

        /// <summary>
        /// 添加实体，并获取标识值。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <param name="idValue">标识值，如果不存在标识值，则返回 null。</param>
        void Add(Object entity, out Object idValue);

        /// <summary>
        /// 提交数据库事务。
        /// </summary>
        /// <param name="scope">事务域，封装了要进行操作的所有实体。</param>
        void CommitTransaction(TransactionScope scope);

        /// <summary>
        /// 删除实体。
        /// </summary>
        /// <param name="entity">要删除的实体。</param>
        /// <returns>如果删除成功，则返回 true；否则返回 false。</returns>
        Boolean Delete(Object entity);

        /// <summary>
        /// 删除所有记录。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        void DeleteAll(Type entityType);

        /// <summary>
        /// 批量删除。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        void DeleteBatch(Type entityType, Filter filter);

        /// <summary>
        /// 修改实体，忽略类型为 DateTime 且值为 DateTime.MinValue 的属性。
        /// </summary>
        /// <param name="entity">要修改的实体。</param>
        void Modify(Object entity);

        #region 批量修改

        /// <summary>
        /// 批量修改，使用新建的数据库连接，UpdateBehavior 的值为 Continue，即当遇到错误时，会继续尝试更新其余的实体。
        /// </summary>
        /// <param name="entities">要更新的实体集合。</param>
        /// <returns>实际更新的实体数。</returns>
        Int32 ModifyBatch(IList<IDbEntity> entities);

        /// <summary>
        /// 批量修改，使用调用方提供的数据库连接，UpdateBehavior 的值为 Continue，即当遇到错误时，会继续尝试更新其余的实体。
        /// </summary>
        /// <param name="entities">要更新的实体集合。</param>
        /// <param name="connection">数据库连接， 如果为空，则新建一个连接。</param>
        /// <returns>实际更新的实体数。</returns>
        Int32 ModifyBatch(IList<IDbEntity> entities, DbConnection connection);

        /// <summary>
        /// 以标准的方法批量修改，即当遇到错误时，不更新其余的实体。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <returns>实际更新的实体数。</returns>
        Int32 ModifyBatchStandard(IList<IDbEntity> entities);

        /// <summary>
        /// 以标准的方式批量修改，即当遇到错误时，不更新其余的实体。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <param name="connection">要使用的数据库连接。</param>
        /// <returns>实际更新的实体数。</returns>
        Int32 ModifyBatchStandard(IList<IDbEntity> entities, DbConnection connection);

        /// <summary>
        /// 以事务的方式批量修改，即当遇到错误时，回滚所有的修改。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <returns>实际修改的实体数。</returns>
        Int32 ModifyBatchTransactional(IList<IDbEntity> entities);

        /// <summary>
        /// 以事务的方式批量修改，即当遇到错误时，回滚的有的修改。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <param name="connection">要使用的数据库连接。</param>
        /// <returns>实际更新的实体数。</returns>
        Int32 ModifyBatchTransactional(IList<IDbEntity> entities, DbConnection connection);

        #endregion

        #region 执行存储过程

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="storedProcedureParameters">存储过程参数，如果不存在参数，可以为 null。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(DbStoredProcedureParameters storedProcedureParameters);

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称。</param>
        /// <param name="storedProcedureParameters">存储过程参数，如果不存在参数，可以为 null。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName, DbStoredProcedureParameters storedProcedureParameters);

        #endregion

        #endregion

        #region 泛型

        /// <summary>
        /// 删除所有记录。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        void DeleteAll<TEntity>() where TEntity : class;

        /// <summary>
        /// 批量删除。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        void DeleteBatch<TEntity>(Filter filter) where TEntity : class;

        #endregion

        #endregion

        #region 加载数据

        #region Load

        #region 非泛型

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>全部实体集合。</returns>
        Object[] Load(Type entityType);

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        Object[] Load(Type entityType, Filter filter);

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, Sorter sorter);

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, Filter filter, Sorter sorter);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>全部实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter);

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount);

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters);

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount);

        #endregion

        #endregion

        #region 泛型

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>全部实体集合。</returns>
        TEntity[] Load<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        TEntity[] Load<TEntity>(Filter filter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>全部实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(Filter filter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount)
                   where TEntity : class, new();

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount) where TEntity : class, new();

        #endregion

        #endregion

        #endregion

        #region LoadFirst

        #region 非泛型

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>第一个实体。</returns>
        Object LoadFirst(Type entityType);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, Filter filter);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, Sorter sorter);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, Filter filter, Sorter sorter);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter);
        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<Object> LoadFirst(Type entityType, DbStoredProcedureParameters storedProcedureParameters);

        #endregion

        #endregion

        #region 泛型

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>第一个实体。</returns>
        TEntity LoadFirst<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(Filter filter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        StoredProcedureExecutionResult<TEntity> LoadFirst<TEntity>(DbStoredProcedureParameters storedProcedureParameters) where TEntity : class, new();

        #endregion

        #endregion

        #endregion

        #region 聚合操作

        #region 分组查询

        #region Aggregate

        #region 非泛型

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <returns>查询结果集合。</returns>
        GroupResult[] Aggregate(Type groupResultType);

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果集合。</returns>
        GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter);

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        GroupResult[] Aggregate(Type groupResultType, Sorter sorter);

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter);

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">记录数。</param>
        /// <returns>查询结果集合。</returns>
        GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        #endregion

        #region 泛型

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <returns>查询结果集合。</returns>
        TGroupResult[] Aggregate<TGroupResult>()
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果集合。</returns>
        TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        TGroupResult[] Aggregate<TGroupResult>(Sorter sorter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">记录数。</param>
        /// <returns>查询结果集合。</returns>
        TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
                   where TGroupResult : GroupResult, new();

        #endregion

        #endregion

        #region AggregateOne

        #region 非泛型

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <returns>查询结果。</returns>
        GroupResult AggregateOne(Type groupResultType);

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果。</returns>
        GroupResult AggregateOne(Type groupResultType, Filter whereFilter, Filter havingFilter);

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        GroupResult AggregateOne(Type groupResultType, Sorter sorter);

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        GroupResult AggregateOne(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter);

        #endregion

        #region 泛型

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <returns>查询结果。</returns>
        TGroupResult AggregateOne<TGroupResult>()
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果。</returns>
        TGroupResult AggregateOne<TGroupResult>(Filter whereFilter, Filter havingFilter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        TGroupResult AggregateOne<TGroupResult>(Sorter sorter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        TGroupResult AggregateOne<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter)
                   where TGroupResult : GroupResult, new();

        #endregion

        #endregion

        #endregion

        #region AVG

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回 null。</returns>
        Object Avg(Type entityType, String propertyName);

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回 null。</returns>
        Object Avg(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回 null。</returns>
        Object Avg(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回 null。</returns>
        Object Avg(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        TResult Avg<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        TResult Avg<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        TResult Avg<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        TResult Avg<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region COUNT

        /// <summary>
        /// 计算记录数。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>计算结果。</returns>
        Int32 Count(Type entityType);

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count(Type entityType, String propertyName);

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// 计算记录数。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>计算结果。</returns>
        Int32 Count(Type entityType, Filter filter);

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// 计算记录的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>计算结果。</returns>
        Int32 Count<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count<TEntity>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count<TEntity>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 计算过滤后的记录的的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>计算结果。</returns>
        Int32 Count<TEntity>(Filter filter) where TEntity : class, new();

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count<TEntity>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        Int32 Count<TEntity>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region MAX

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        Object Max(Type entityType, String propertyName);

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        Object Max(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        Object Max(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        Object Max(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        TResult Max<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        TResult Max<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        TResult Max<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        TResult Max<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region MIN

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        Object Min(Type entityType, String propertyName);

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        Object Min(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        Object Min(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        Object Min(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        TResult Min<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        TResult Min<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        TResult Min<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        TResult Min<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region SUM

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        Object Sum(Type entityType, String propertyName);

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        Object Sum(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        Object Sum(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        Object Sum(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        TResult Sum<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        TResult Sum<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        TResult Sum<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        TResult Sum<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #endregion

        #region 复合实体

        #region Compose

        #region 非泛型

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, Filter whereFilter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引，基于零。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>复合实体集合。</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        #endregion

        #region 泛型

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>()
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引，基于零。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>复合实体集合。</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
                   where TCompositeResult : CompositeResult, new();

        #endregion

        #endregion

        #region ComposeOne

        #region 非泛型

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, Sorter sorter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter);

        #endregion

        #region 泛型

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>()
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        #endregion

        #endregion

        #endregion

        #endregion

        #region 存在性判断

        /// <summary>
        /// 判断给定条件的实体是否存在（过滤器如果为空表示查找全部）。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>如果实体存在，则返回 true；否则返回 false。</returns>
        Boolean Exists(Type entityType, Filter filter);

        /// <summary>
        /// 判断给定条件的实体是否存在（过滤器如果为空表示查找全部）。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>如果实体存在，则返回 true；否则返回 false。</returns>
        Boolean Exists<TEntity>(Filter filter) where TEntity : class, new();

        #endregion

        #region 其他服务

        /// <summary>
        /// 获取服务器的当前时间。
        /// </summary>
        /// <returns>服务器的当前时间。</returns>
        DateTime Now();

        #endregion

        #region 直接访问数据库

        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <returns>创建好的数据库连接。</returns>
        DbConnection CreateConnection();

        /// <summary>
        /// 创建 Database 实例。
        /// </summary>
        /// <returns>创建好的 Database 实例。</returns>
        Database CreateDatabase();

        #endregion
    }
}