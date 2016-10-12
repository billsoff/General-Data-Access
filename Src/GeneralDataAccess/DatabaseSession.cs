#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2010 广州金微软件技术有限公司
// 版权所有
// 
//
// 文件名：DatabaseSession.cs
// 文件功能描述：表示与数据库进行交互的对象。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008132231
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 表示与数据库进行交互的对象。
    /// </summary>
    public class DatabaseSession : MarshalByRefObject, IDatabaseSession
    {
        #region 私有字段

        #region 静态

        /// <summary>
        /// 默认数据库。
        /// </summary>
        private static readonly Database m_db = DatabaseFactory.CreateDatabase();

        #endregion

        private DatabaseTraits m_traits = new SqlServerEquivalentDatabaseTraits();
        private readonly String m_databaseName;
        private readonly String m_parameterPrefix;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public DatabaseSession()
        {
        }

        /// <summary>
        /// 设置数据库名称和参数前缀。
        /// </summary>
        /// <param name="databaseName">数据库名称。</param>
        /// <param name="parameterPrefix">参数前缀。</param>
        protected DatabaseSession(String databaseName, String parameterPrefix)
        {
            if (databaseName != null)
            {
                databaseName = databaseName.Trim();
            }

            if (parameterPrefix != null)
            {
                parameterPrefix = parameterPrefix.Trim();
            }

            if (!String.IsNullOrEmpty(databaseName))
            {
                m_databaseName = databaseName;
            }

            if (!String.IsNullOrEmpty(parameterPrefix))
            {
                m_parameterPrefix = parameterPrefix;
            }
        }

        #endregion

        #region 公共方法

        #region 动作查询

        #region 非泛型

        /// <summary>
        /// 向数据库添加实体，忽略类型为 DateTime 且值为 DateTime.MinValue 的属性。
        /// </summary>
        /// <param name="entity">要添加的实体。</param>
        public void Add(Object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "向数据库添加的实体不能为空。");
            }

            DbCommand cmd = CreateInsertCommand(entity, Database, ParameterPrefix);

            Database.ExecuteNonQuery(cmd);

            // 在 Remoting 的情景下，如果不传回实体到客户端，则以下代码不起作用
            EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(entity);
        }

        /// <summary>
        /// 添加实体，并获取标识值。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <param name="idValue">标识值。</param>
        public void Add(Object entity, out Object idValue)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "向数据库添加的实体不能为空。");
            }

            DbCommand cmd = CreateInsertCommand(entity, Database, ParameterPrefix);
            EntityDefinition definition = EntityDefinitionBuilder.Build(entity.GetType());

            if (definition.NativePrimaryKeyInfo.AutoIncrement)
            {
                String idStatement = GetRetrieveIdentifierStatement(definition); ;

                if (String.IsNullOrEmpty(idStatement))
                {
                    throw new InvalidOperationException("未设置用于获取自增长（标识）字段的值的 SQL 指令，无法获取标识字段的值。");
                }

                cmd.CommandText += String.Format(";{0};", idStatement);

                idValue = Database.ExecuteScalar(cmd);
            }
            else
            {
                Database.ExecuteNonQuery(cmd);
                idValue = null;
            }

            // 在 Remoting 的情景下，如果不传回实体到客户端，则以下代码不起作用
            EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(entity);
        }

        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <returns>创建好的数据库连接。</returns>
        public DbConnection CreateConnection()
        {
            return Database.CreateConnection();
        }

        /// <summary>
        /// 创建 Database 实例。
        /// </summary>
        /// <returns>创建好的 Database 实例。</returns>
        public Database CreateDatabase()
        {
            return Database;
        }

        /// <summary>
        /// 删除实体。
        /// </summary>
        /// <param name="entity">要删除的实体。</param>
        /// <returns>如果删除成功，则返回 true；否则返回 false。</returns>
        public Boolean Delete(Object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "要删除的实体不能为空。");
            }

            DbCommand cmd = CreateDeleteCommand(entity);

            Boolean success = true;

            try
            {
                Database.ExecuteNonQuery(cmd);

                IDbEntity bo = entity as IDbEntity;

                if (bo != null)
                {
                    bo.Transient = false; // 设置对象为持久对象（因为只有持久对象才可以删除）
                    bo.RequireDelete = true;

                    EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(bo);
                }
            }
            catch (DbException de)
            {
                success = false;

                Util.Log.TraceWarning("删除实体 {0} 失败，异常消息：{1}，异常堆栈：{2}", entity, de.Message, de.StackTrace);
            }

            return success;
        }

        /// <summary>
        /// 删除所有记录。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        public void DeleteAll(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型不能为空。");
            }

            DbCommand cmd = CreateDeleteAllCommand(entityType);

            Database.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 批量删除。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        public void DeleteBatch<TEntity>(Filter filter) where TEntity : class
        {
            Type entityType = typeof(TEntity);

            DeleteBatch(entityType, filter);
        }

        /// <summary>
        /// 修改实体，忽略类型为 DateTime 且值为 DateTime.MinValue 的属性。
        /// </summary>
        /// <param name="entity">要修改的实体。</param>
        public void Modify(Object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "要修改的实体不能为空。");
            }

            DbCommand cmd = CreateUpdateCommand(entity);

            Database.ExecuteNonQuery(cmd);

            EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(entity);
        }

        #region 批量修改

        /// <summary>
        /// 批量修改，使用新建的数据库连接，UpdateBehavior 的值为 Continue，即当遇到错误时，会继续尝试更新其余的实体。
        /// </summary>
        /// <param name="entities">要更新的实体集合。</param>
        /// <returns>实际更新的实体数。</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities)
        {
            return ModifyBatch(entities, UpdateBehavior.Continue);
        }

        /// <summary>
        /// 批量修改，使用新建的数据库连接。
        /// </summary>
        /// <param name="entities">要更新的实体集合。</param>
        /// <param name="updateBehavior">更新行为。</param>
        /// <returns>实际更新的实体数。</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities, UpdateBehavior updateBehavior)
        {
            return ModifyBatch(entities, null, updateBehavior);
        }

        /// <summary>
        /// 批量修改，使用调用方提供的数据库连接，UpdateBehavior 的值为 Continue，即当遇到错误时，会继续尝试更新其余的实体。
        /// </summary>
        /// <param name="entities">要更新的实体集合。</param>
        /// <param name="connection">数据库连接， 如果为空，则新建一个连接。</param>
        /// <returns>实际更新的实体数。</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities, DbConnection connection)
        {
            return ModifyBatch(entities, connection, UpdateBehavior.Continue);
        }

        /// <summary>
        /// 批量修改，使用调用方提供的数据库连接。
        /// </summary>
        /// <param name="entities">要更新的实体集合。</param>
        /// <param name="connection">数据库连接， 如果为空，则新建一个连接。</param>
        /// <param name="updateBehavior">更新行为。</param>
        /// <returns>实际更新的实体数。</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities, DbConnection connection, UpdateBehavior updateBehavior)
        {
            WriteModifyBatchDbEntities(
                    entities,
                    String.Format(
                        "批量修改实体，使用{0}连接，UpdateBehavior 为 {1}",
                        ((connection != null) ? "调用方提供的" : "内置"),
                        updateBehavior
                    )
                );

            IList<IDbEntity> changes = GetDbEntitiesChanges(entities);

            if ((changes == null) || (changes.Count == 0))
            {
                return 0;
            }

            Type entityType = changes[0].GetType();

            BatchModificationSchema schema = GetBatchModificationSchema(entityType);

            // 创建数据源
            DataTable dataSource = GetModifyBatchDataSource(schema, changes);

            Int32 rowsAffected;

            // 使用内置连接
            if (connection == null)
            {
                DataSet dataSet = new DataSet();

                dataSet.Tables.Add(dataSource);

                rowsAffected = Database.UpdateDataSet(
                        dataSet,
                        dataSource.TableName,
                        schema.InsertCommand,
                        schema.UpdateCommand,
                        schema.DeleteCommand,
                        updateBehavior
                    );
            }
            // 使用调用方提供的连接，事务模式
            else if (updateBehavior == UpdateBehavior.Transactional)
            {
                rowsAffected = DoModifyBatchTransactional(dataSource, schema, connection);
            }
            // 使用调用方提供的连接，其他模式
            else
            {
                DbDataAdapter adapter = CreateModifyBatchDataAdapter(entityType, connection, updateBehavior);

                schema.InsertCommand.Connection = connection;
                schema.UpdateCommand.Connection = connection;
                schema.DeleteCommand.Connection = connection;

                adapter.InsertCommand = schema.InsertCommand;
                adapter.UpdateCommand = schema.UpdateCommand;
                adapter.DeleteCommand = schema.DeleteCommand;

                OpenConnection(connection);

                rowsAffected = adapter.Update(dataSource);
            }

            EtyBusinessObject.SetDbEntitiesStatusOnCommitSuccess(changes);

            return rowsAffected;
        }

        /// <summary>
        /// 以标准的方法批量修改，即当遇到错误时，不更新其余的实体。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <returns>实际更新的实体数。</returns>
        public Int32 ModifyBatchStandard(IList<IDbEntity> entities)
        {
            return ModifyBatch(entities, UpdateBehavior.Standard);
        }

        /// <summary>
        /// 以标准的方式批量修改，即当遇到错误时，不更新其余的实体。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <param name="connection">要使用的数据库连接。</param>
        /// <returns>实际更新的实体数。</returns>
        public Int32 ModifyBatchStandard(IList<IDbEntity> entities, DbConnection connection)
        {
            return ModifyBatch(entities, connection, UpdateBehavior.Standard);
        }

        /// <summary>
        /// 以事务的方式批量修改，即当遇到错误时，回滚所有的修改。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <returns>实际修改的实体数。</returns>
        public Int32 ModifyBatchTransactional(IList<IDbEntity> entities)
        {
            return ModifyBatch(entities, UpdateBehavior.Transactional);
        }

        /// <summary>
        /// 以事务的方式批量修改，即当遇到错误时，回滚的有的修改。
        /// </summary>
        /// <param name="entities">要批量修改的实体集合。</param>
        /// <param name="connection">要使用的数据库连接。</param>
        /// <returns>实际更新的实体数。</returns>
        public Int32 ModifyBatchTransactional(IList<IDbEntity> entities, DbConnection connection)
        {
            return ModifyBatch(entities, connection, UpdateBehavior.Transactional);
        }

        #region 辅助方法

        /// <summary>
        /// 向数据源加一个实体。
        /// </summary>
        /// <param name="e">实体。</param>
        /// <param name="dataSource">数据源。</param>
        /// <param name="schema">实体架构。</param>
        /// <returns>加好的行，其 RowState 为 Unchanged。</returns>
        private static DataRow AddNewRowToModifyBatchDataSource(IDbEntity e, DataTable dataSource, EntitySchema schema)
        {
            DataRow row = dataSource.NewRow();

            row.BeginEdit();

            foreach (Column col in schema.Columns)
            {
                if (col.Selected)
                {
                    row[col.Name] = col.GetDbValue(e);
                }
            }

            row.EndEdit();

            dataSource.Rows.Add(row);

            row.AcceptChanges();

            return row;
        }

        /// <summary>
        /// 向批量修改的命令添加参数。
        /// </summary>
        /// <param name="cmd">命令。</param>
        /// <param name="col">列。</param>
        private void AddParameterToModifyBatchCommand(DbCommand cmd, Column col)
        {
            Database.AddParameter(
                cmd,
                col.Name,
                col.DbType,
                0, // size
                ParameterDirection.Input,
                false, // nullable
                0, // precision
                0, // scale
                col.Name, // sourceColumn
                DataRowVersion.Default,
                DBNull.Value // value
            );
        }

        /// <summary>
        /// 创建数据库适配器。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="connection">连接。</param>
        /// <param name="updateBehavior"></param>
        /// <returns>创建好的适配器。</returns>
        private DbDataAdapter CreateModifyBatchDataAdapter(Type entityType, DbConnection connection, UpdateBehavior updateBehavior)
        {
            Type dbType = typeof(Database);

            MethodInfo getDataAdapterMethod = dbType.GetMethod("GetDataAdapter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(UpdateBehavior) }, null);

            if (getDataAdapterMethod == null)
            {
                throw new InvalidOperationException("无法获取 DataAdapter，请调用使用内置数据库连接的版本（即不要显式地传入数据库连接）。");
            }

            DbDataAdapter adapter = (DbDataAdapter)getDataAdapterMethod.Invoke(Database, new Object[] { updateBehavior });

            return adapter;
        }

        /// <summary>
        /// 创建批量修改的删除命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>删除命令。</returns>
        private DbCommand CreateModifyBatchDeleteCommand(Type entityType)
        {
            return CreateModifyBatchDeleteCommand(entityType, null);
        }

        /// <summary>
        /// 创建批量修改的删除命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="connection">数据库连接。</param>
        /// <returns>删除命令。</returns>
        private DbCommand CreateModifyBatchDeleteCommand(Type entityType, DbConnection connection)
        {
            EntitySchemaComposite composite = GetSchemaComposite(
                    entityType,
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            StringBuilder whereBuilder = new StringBuilder();

            foreach (Column col in schema.PrimaryKeyColumns)
            {
                if (whereBuilder.Length != 0)
                {
                    whereBuilder.Append(" AND ");
                }

                whereBuilder.AppendFormat("{0}={1}{0}", col.Name, ParameterPrefix);
            }

            // 构建 SQL 指令
            String sql = String.Format("DELETE FROM {0} WHERE {1}", schema.TableName, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // 设置参数
            foreach (Column col in schema.PrimaryKeyColumns)
            {
                AddParameterToModifyBatchCommand(cmd, col);
            }

            if (connection != null)
            {
                cmd.Connection = connection;
            }

            return cmd;
        }

        /// <summary>
        /// 创建批量修改插入命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>插入命令。</returns>
        private DbCommand CreateModifyBatchInsertCommand(Type entityType)
        {
            return CreateModifyBatchInsertCommand(entityType, null);
        }

        /// <summary>
        /// 创建批量修改插入命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="connection">数据库连接。</param>
        /// <returns>插入命令。</returns>
        private DbCommand CreateModifyBatchInsertCommand(Type entityType, DbConnection connection)
        {
            EntitySchemaComposite composite = GetSchemaComposite(
                    entityType,
                    (CompositeBuilderStrategy)null,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            StringBuilder fieldListBuilder = new StringBuilder();
            StringBuilder valueListBuilder = new StringBuilder();

            foreach (Column col in schema.Columns)
            {
                if (col.IsPrimaryKeyNative || !col.Selected)
                {
                    continue;
                }

                if (fieldListBuilder.Length != 0)
                {
                    fieldListBuilder.Append(",");
                    valueListBuilder.Append(",");
                }

                fieldListBuilder.Append(col.Name);
                valueListBuilder.Append(ParameterPrefix + col.Name);
            }

            // 构建 SQL 指令
            String sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", schema.TableName, fieldListBuilder, valueListBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // 设置参数
            foreach (Column col in schema.Columns)
            {
                if (col.IsPrimaryKeyNative || !col.Selected)
                {
                    continue;
                }

                AddParameterToModifyBatchCommand(cmd, col);
            }

            if (connection != null)
            {
                cmd.Connection = connection;
            }

            return cmd;
        }

        /// <summary>
        /// 创建批量修改的更新命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>更新命令。</returns>
        private DbCommand CreateModifyBatchUpdateCommand(Type entityType)
        {
            return CreateModifyBatchUpdateCommand(entityType, null);
        }

        /// <summary>
        /// 创建批量修改的更新命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="connection">数据库连接。</param>
        /// <returns>更新命令。</returns>
        private DbCommand CreateModifyBatchUpdateCommand(Type entityType, DbConnection connection)
        {
            EntitySchemaComposite composite = GetSchemaComposite(
                    entityType,
                    (CompositeBuilderStrategy)null,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            StringBuilder setListBuilder = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();

            foreach (Column col in schema.Columns)
            {
                if (!col.Selected)
                {
                    continue;
                }

                if (col.IsPrimaryKey)
                {
                    if (whereBuilder.Length != 0)
                    {
                        whereBuilder.Append(" AND ");
                    }

                    whereBuilder.AppendFormat("{0}={1}{0}", col.Name, ParameterPrefix);
                }
                else
                {
                    if (setListBuilder.Length != 0)
                    {
                        setListBuilder.Append(",");
                    }

                    setListBuilder.AppendFormat("{0}={1}{0}", col.Name, ParameterPrefix);
                }
            }

            // 构建 SQL 指令
            String sql = String.Format("UPDATE {0} SET {1} WHERE {2}", schema.TableName, setListBuilder, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // 设置参数
            foreach (Column col in schema.Columns)
            {
                if (!col.Selected)
                {
                    continue;
                }

                AddParameterToModifyBatchCommand(cmd, col);
            }

            if (connection != null)
            {
                cmd.Connection = connection;
            }

            return cmd;
        }

        /// <summary>
        /// 在给定的数据连接上以事务的方式执行批量修改。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="schema">架构信息。</param>
        /// <param name="connection">数据库连接。</param>
        private Int32 DoModifyBatchTransactional(DataTable dataSource, BatchModificationSchema schema, DbConnection connection)
        {
            DataSet dataSet = new DataSet();

            dataSet.Tables.Add(dataSource);

            Int32 rowsAffected;

            OpenConnection(connection);

            DbTransaction transaction = connection.BeginTransaction();

            try
            {
                rowsAffected = Database.UpdateDataSet(
                        dataSet,
                        dataSource.TableName,
                        schema.InsertCommand,
                        schema.UpdateCommand,
                        schema.DeleteCommand,
                        transaction
                    );

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
            }

            return rowsAffected;
        }

        /// <summary>
        /// 获取指定实体类型的批量修改架构信息。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>批量修改的架构信息。</returns>
        private BatchModificationSchema GetBatchModificationSchema(Type entityType)
        {
            BatchModificationSchema schema = BatchModificationSchemaCache.GetSchema(entityType, Database);

            if (schema == null)
            {
                // 创建数据源
                EntitySchemaComposite composite = GetSchemaComposite(entityType);

                DataTable dataSource = new DataTable(composite.Target.TableName);

                foreach (Column col in composite.Target.Columns)
                {
                    if (col.Selected)
                    {
                        dataSource.Columns.Add(new DataColumn(col.Name, col.Type));
                    }
                }

                // 创建数据库命令
                DbCommand insertCommand = CreateModifyBatchInsertCommand(entityType);
                DbCommand updateCommand = CreateModifyBatchUpdateCommand(entityType);
                DbCommand deleteCommand = CreateModifyBatchDeleteCommand(entityType);

                schema = new BatchModificationSchema(entityType, dataSource, insertCommand, updateCommand, deleteCommand);

                BatchModificationSchemaCache.SetSchema(schema, Database);
            }

            return schema;
        }

        /// <summary>
        /// 获取实体的更新操作。
        /// </summary>
        /// <param name="e">实体。</param>
        /// <returns>实体的更新操作。</returns>
        private static UpdateAction GetDbEntityUpdateAction(IDbEntity e)
        {
            if (e.Transient)
            {
                return UpdateAction.Add;
            }
            else if (!e.Deleted)
            {
                if (e.RequireDelete)
                {
                    return UpdateAction.Delete;
                }
                else if (e.Dirty)
                {
                    return UpdateAction.Modify;
                }
            }

            return UpdateAction.None;
        }

        /// <summary>
        /// 获取要修改的实体.
        /// </summary>
        /// <param name="entities">实体集合.</param>
        /// <returns>要修改的实体集合.</returns>
        private static IList<IDbEntity> GetDbEntitiesChanges(IList<IDbEntity> entities)
        {
            if ((entities == null) || (entities.Count == 0))
            {
                return null;
            }

            List<IDbEntity> changes = new List<IDbEntity>();

            foreach (IDbEntity e in entities)
            {
                UpdateAction action = GetDbEntityUpdateAction(e);

                if (action != UpdateAction.None)
                {
                    changes.Add(e);
                }
            }

            return changes;
        }

        /// <summary>
        /// 创建批量修改的数据源。
        /// </summary>
        /// <param name="schema">架构信息。</param>
        /// <param name="entities">要修改的实体集合。</param>
        /// <returns>创建好的数据源。</returns>
        private static DataTable GetModifyBatchDataSource(BatchModificationSchema schema, IList<IDbEntity> entities)
        {
            EntitySchemaComposite composite = GetSchemaComposite(schema.EntityType);

            DataTable dataSource = schema.DataSource;

            foreach (IDbEntity e in entities)
            {
                UpdateAction action = GetDbEntityUpdateAction(e);

                if (action == UpdateAction.None)
                {
                    continue;
                }

                DataRow row = AddNewRowToModifyBatchDataSource(e, dataSource, composite.Target);

                switch (action)
                {
                    case UpdateAction.Add:
                        row.SetAdded();
                        break;

                    case UpdateAction.Modify:
                        row.SetModified();
                        break;

                    case UpdateAction.Delete:
                        row.Delete();
                        break;

                    // 忽略其余情形
                    case UpdateAction.None:
                        break;
                    default:
                        break;
                }
            }

            return dataSource;
        }

        /// <summary>
        /// 打开数据库连接。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        private static void OpenConnection(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        #endregion

        #region 用于调试

        /// <summary>
        /// 写批量修改的日志。
        /// </summary>
        /// <param name="dbEntities">数据库实体集合。</param>
        /// <param name="prompt">提示。</param>
        [Conditional("DEBUG")]
        private static void WriteModifyBatchDbEntities(IList<IDbEntity> dbEntities, String prompt)
        {
            if ((dbEntities == null) || (dbEntities.Count == 0))
            {
                Util.Log.TraceInformation("批量修改实体，执行批量修改的实体集合为空或不包含实体项。");

                return;
            }

            Type entityType = dbEntities[0].GetType();

            EntitySchemaComposite composite = GetSchemaComposite(entityType);
            EntitySchema schema = composite.Target;

            List<IDbEntity> sortedDbEntities = new List<IDbEntity>(dbEntities);

            #region 对数据库实体进行排序，将要修改的实体放在最前

            sortedDbEntities.Sort(
                    delegate(IDbEntity left, IDbEntity right)
                    {
                        UpdateAction leftAction = GetDbEntityUpdateAction(left);
                        UpdateAction rightAction = GetDbEntityUpdateAction(right);

                        if ((leftAction != UpdateAction.None) && (rightAction == UpdateAction.None))
                        {
                            return -1;
                        }

                        if ((leftAction == UpdateAction.None) && (rightAction != UpdateAction.None))
                        {
                            return 1;
                        }

                        return 0;
                    }
                );

            #endregion

            List<String> headers = new List<String>();

            headers.AddRange(new String[] { "持久对象?", "要求删除?", "已被修改?", "已被删除?", "更新操作" });

            foreach (Column col in schema.PrimaryKeyColumns)
            {
                headers.Add(col.Name);
            }

            TabularWriter writer = new TabularWriter(headers.Count);

            writer.WriteLine(headers.ToArray());

            foreach (IDbEntity e in sortedDbEntities)
            {
                Int32 index = 0;

                String[] line = writer.NewLine();

                line[index++] = DbEntityDebugger.GetTristateBoolean(!e.Transient);
                line[index++] = DbEntityDebugger.GetTristateBoolean(e.RequireDelete);
                line[index++] = DbEntityDebugger.GetTristateBoolean(e.Dirty);
                line[index++] = DbEntityDebugger.GetTristateBoolean(e.Deleted);
                line[index++] = GetDbEntityUpdateAction(e).ToString();

                foreach (Column col in schema.PrimaryKeyColumns)
                {
                    Object dbValue = col.GetDbValue(e);
                    String valueStr;

                    if (Convert.IsDBNull(dbValue))
                    {
                        valueStr = "NULL";
                    }
                    else if (dbValue is Byte[])
                    {
                        valueStr = DbEntityDebugger.Dump((Byte[])dbValue);
                    }
                    else
                    {
                        valueStr = dbValue.ToString();
                    }

                    line[index++] = valueStr;
                }

                writer.WriteLine(line);
            }

            // 写到日志
            Util.Log.TraceInformation("{0}\r\n\r\n{1}", prompt, writer);
        }

        #endregion

        #endregion

        /// <summary>
        /// 提交数据库事务。
        /// </summary>
        /// <param name="scope">事务域，封装了要进行操作的所有实体。</param>
        public void CommitTransaction(TransactionScope scope)
        {
            if ((scope == null) || (scope.Entities.Length == 0))
            {
                throw new ArgumentNullException("scope", "事务域为空或不包含实体。");
            }

            List<DbCommand> allCmds = new List<DbCommand>();

            foreach (ActionQueryEntity queryEntity in scope.Entities)
            {
                DbCommand cmd = null;

                switch (queryEntity.ActionQueryType)
                {
                    case ActionQueryType.Add:
                        cmd = CreateInsertCommand(queryEntity.Entity, Database, ParameterPrefix);
                        break;

                    case ActionQueryType.Delete:
                        cmd = CreateDeleteCommand(queryEntity.Entity);
                        break;

                    case ActionQueryType.Modify:
                        cmd = CreateUpdateCommand(queryEntity.Entity);
                        break;

                    default:
                        throw new InvalidOperationException("在事务操作中指定的操作类型不可识别。");
                }

                allCmds.Add(cmd);
            }

            DbConnection conn = Database.CreateConnection();

            conn.Open();

            DbTransaction transaction;

            if (scope.UseDefaultIsolationLevel)
            {
                transaction = conn.BeginTransaction();
            }
            else
            {
                transaction = conn.BeginTransaction(scope.IsolationLevel);
            }

            try
            {
                foreach (DbCommand cmd in allCmds)
                {
                    Database.ExecuteNonQuery(cmd, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
            finally
            {
                transaction.Dispose();
                conn.Dispose();
            }

            #region 在 Remoting 的情景下，如果不传回实体到客户端，则以下代码不起作用

            // 设置实体的状态
            List<IDbEntity> dbEntities = new List<IDbEntity>();

            foreach (ActionQueryEntity queryEntity in scope.Entities)
            {
                IDbEntity e = queryEntity.Entity as IDbEntity;

                if (e != null)
                {
                    // 持久对象
                    e.Transient = false;

                    if (queryEntity.ActionQueryType == ActionQueryType.Delete)
                    {
                        // 已请求删除
                        e.RequireDelete = true;
                    }

                    dbEntities.Add(e);
                }
            }

            if (dbEntities.Count != 0)
            {
                EtyBusinessObject.SetDbEntitiesStatusOnCommitSuccess(dbEntities);
            }

            #endregion
        }

        #region 执行存储过程

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName)
        {
            return DoExecuteStoredProcedure(storedProcedureName, null);
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="storedProcedureParameters">存储过程参数，如果不存在参数，可以为 null。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(DbStoredProcedureParameters storedProcedureParameters)
        {
            if (storedProcedureParameters == null)
            {
                throw new ArgumentNullException("storedProcedureParameters", "存储过程参数集合对象不能为空，此方法需要从中获取存储过程名称。");
            }

            return DoExecuteStoredProcedure(storedProcedureParameters.StoredProcedureName, storedProcedureParameters);
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称。</param>
        /// <param name="storedProcedureParameters">存储过程参数，如果不存在参数，可以为 null。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName, DbStoredProcedureParameters storedProcedureParameters)
        {
            return DoExecuteStoredProcedure(storedProcedureName, storedProcedureParameters);
        }

        #endregion

        #endregion

        #region 泛型

        /// <summary>
        /// 删除所有记录。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        public void DeleteAll<TEntity>() where TEntity : class
        {
            Type entityType = typeof(TEntity);

            DeleteAll(entityType);
        }

        /// <summary>
        /// 批量删除。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        public void DeleteBatch(Type entityType, Filter filter)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型不能为空。");
            }

            if (filter == null)
            {
                throw new ArgumentNullException("filter", "过滤器不能为空。");
            }

            DbCommand cmd = CreateDeleteBatchCommand(entityType, filter);

            Database.ExecuteNonQuery(cmd);
        }

        #endregion

        #region SQL Server 批量复制

        /// <summary>
        /// 向 SQL Server 数据库服务器批量复制实体。
        /// </summary>
        /// <param name="entities">实体集合。</param>
        public void SqlBulkCopy(IList entities)
        {
            SqlBulkCopy(entities, SqlBulkCopyOptions.Default);
        }

        /// <summary>
        /// 向 SQL Server 数据库服务器批量复制实体。
        /// </summary>
        /// <param name="entities">实体集合。</param>
        /// <param name="copyOptions">复制选项。</param>
        public void SqlBulkCopy(IList entities, SqlBulkCopyOptions copyOptions)
        {
            if ((entities == null) || (entities.Count == 0))
            {
                return;
            }

            if (!(Database.DbProviderFactory is SqlClientFactory))
            {
                throw new InvalidOperationException("目标数据库不是 SQL Server，不能调用此方法。");
            }

            // 获取实体架构
            EntitySchemaComposite composite = GetSchemaComposite(
                    entities[0].GetType(),
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            // 创建源表
            DataTable dataSource = CreateSqlBulkCopyDataSource(entities, schema);

            // 创建连接
            SqlConnection conn = (SqlConnection)Database.CreateConnection();

            // 创建 SqlBulkCopy 对象并调用其 WriteToServer 进行批量复制
            using (SqlBulkCopy bcp = CreateSqlBulkCopyInstance(conn, copyOptions, schema))
            {
                conn.Open();

                try
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    bcp.WriteToServer(dataSource);

                    sw.Stop();

                    Util.Log.TraceInformation(
                                "批量复制 {0} 个 {1} 实体到表 {2}，方法 SqlBulkCopy.WriteToServer 用时 {3:#,###} ms。",
                                entities.Count,
                                schema.Type.FullName,
                                schema.TableName,
                                sw.ElapsedMilliseconds
                        );
                }
                catch (Exception ex)
                {
                    Util.Log.TraceError(
                                "批量复制实体 {0} 到表 {1} 出错，错误消息：{2}\r\n堆栈：{3}",
                                schema.Type.FullName,
                                schema.TableName,
                                ex.Message,
                                ex.StackTrace
                        );

                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 创建批量复制数据源。
        /// </summary>
        /// <param name="entities">实体集合。</param>
        /// <param name="schema">实体架构。</param>
        /// <returns>创建好的数据源。</returns>
        private static DataTable CreateSqlBulkCopyDataSource(IList entities, EntitySchema schema)
        {
            Stopwatch sw = Stopwatch.StartNew();

            DataTable dataSource = new DataTable();

            foreach (Column col in schema.Columns)
            {
                dataSource.Columns.Add(new DataColumn(col.Name, col.Type));
            }

            sw.Stop();

            Util.Log.TraceInformation("完成数据源架构创建，用时 {0:#,##0} ms", sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();

            // 填充数据
            foreach (Object e in entities)
            {
                DataRow row = dataSource.NewRow();

                foreach (Column col in schema.Columns)
                {
                    Object dbValue = col.GetDbValue(e);

                    row[col.Name] = dbValue;
                }

                dataSource.Rows.Add(row);
            }

            sw.Stop();

            Util.Log.TraceInformation("完成数据源数据填充，用时 {0:#,##0} ms", sw.ElapsedMilliseconds);

            return dataSource;
        }

        /// <summary>
        /// 创建 <see cref="System.Data.SqlClient.SqlBulkCopy"/> 实例。
        /// </summary>
        /// <param name="conn">SQL Server 连接实例。</param>
        /// <param name="copyOptions">复制选项。</param>
        /// <param name="schema">实体架构。</param>
        /// <returns>创建好的 <see cref="System.Data.SqlClient.SqlBulkCopy"/> 实例。</returns>
        private static SqlBulkCopy CreateSqlBulkCopyInstance(SqlConnection conn, SqlBulkCopyOptions copyOptions, EntitySchema schema)
        {
            Stopwatch sw = Stopwatch.StartNew();

            SqlBulkCopy bcp = new SqlBulkCopy(conn, copyOptions, null);

            bcp.DestinationTableName = schema.TableName;

            // 加列映射
            foreach (Column col in schema.Columns)
            {
                bcp.ColumnMappings.Add(col.Name, col.Name);
            }

            sw.Stop();

            Util.Log.TraceInformation("创建 SqlBulkCopy 实例，用时 {0:#,##0} ms", sw.ElapsedMilliseconds);

            return bcp;
        }

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
        public Object[] Load(Type entityType)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        public Object[] Load(Type entityType, Filter filter)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, Sorter sorter)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, Filter filter, Sorter sorter)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount)
        {
            return Load(
                    entityType,
                    (CompositeBuilderStrategy)null,
                    filter,
                    sorter,
                    (AssemblyPolicy)null,
                    startIndex,
                    recordCount
                );
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>全部实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy)
        {
            return Load(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter)
        {
            return Load(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return Load(entityType, loadStrategy, null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter)
        {
            return Load(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

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
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount)
        {
            return Load(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null, startIndex, recordCount);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, filter, sorter, childrenPolicy, -1, -1);
        }

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
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount)
        {
            // TODO: 性能计数
            //PerformenceMetric metric = new PerformenceMetric();

            //metric.Start();

            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "要加载的实体类型不能为空。");
            }

            EntitySchemaComposite composite = GetSchemaComposite(entityType, loadStrategy, filter, sorter, childrenPolicy);
            EntitySchema schema = composite.Target;

            #region 如果为存储过程，则执行并返回结果集，忽略过滤器与排序器

            if (schema.IsStoredProcedure)
            {
                DbStoredProcedureParameters storedProcedureParameters = new DbStoredProcedureParameters(schema.TableName);

                StoredProcedureExecutionResult<Object> executionResult = Load(entityType, storedProcedureParameters, startIndex, recordCount);

                return executionResult.Entities;
            }

            #endregion

            DbCommand cmd = CreateSelectCommand(schema, filter, sorter);

            //metric.logSw(String.Format("完成查询 {0} 的 SQL 命令的构造", entityType.FullName));

            List<Object> results = new List<Object>();

            Int32 currentIndex = -1;

            Boolean partially = (startIndex >= 0) && (recordCount >= 1);
            Int32 maxIndex = startIndex + recordCount;

            using (IDataReader reader = Database.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    currentIndex++;

                    if (partially)
                    {
                        if (currentIndex < startIndex)
                        {
                            continue;
                        }
                        else if (currentIndex >= maxIndex)
                        {
                            break;
                        }
                    }

                    // 合成实体
                    Object entity = composite.Compose(reader);

                    results.Add(entity);
                }
            }

            Object[] entities = MakeEntityArray(entityType, results);

            if (childrenPolicy != null)
            {
                childrenPolicy.Enforce(entityType, entities, filter, this);
            }

            //metric.logSw(String.Format("完成 {0} 实体的合成", entityType.FullName));

            return entities;
        }

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters)
        {
            return Load(entityType, storedProcedureParameters, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount)
        {
            // TODO: 性能计数
            //PerformenceMetric metric = new PerformenceMetric();

            //metric.Start();

            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "要加载的实体类型不能为空。");
            }

            EntitySchemaComposite composite = GetSchemaComposite(
                    entityType,
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            if (!schema.IsStoredProcedure)
            {
                throw new InvalidOperationException(String.Format("数据源 {0} 不是存储过程，不能调用此方法。", schema.TableName));
            }

            DbCommand cmd = CreateSelectCommand(schema, storedProcedureParameters);

            //metric.logSw(String.Format("完成查询 {0} 的 SQL 命令的构造", entityType.FullName));

            List<Object> results = new List<Object>();

            Int32 currentIndex = -1;

            Boolean partially = (startIndex >= 0) && (recordCount >= 1);
            Int32 maxIndex = startIndex + recordCount;

            using (IDataReader reader = Database.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    currentIndex++;

                    if (partially)
                    {
                        if (currentIndex < startIndex)
                        {
                            continue;
                        }
                        else if (currentIndex >= maxIndex)
                        {
                            break;
                        }
                    }

                    // 合成实体
                    Object entity = composite.Compose(reader);

                    results.Add(entity);
                }
            }

            Object[] entities = MakeEntityArray(entityType, results);

            //metric.logSw(String.Format("完成 {0} 实体的合成", entityType.FullName));

            SetOutputParameterValues(storedProcedureParameters, cmd);

            StoredProcedureExecutionResult<Object> executionResult = new StoredProcedureExecutionResult<Object>(storedProcedureParameters, entities);

            return executionResult;
        }

        #endregion

        #endregion

        #region 泛型

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>全部实体集合。</returns>
        public TEntity[] Load<TEntity>() where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        public TEntity[] Load<TEntity>(Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空，如果 startIndex 小于零或 recordCount 小于 1，则取全部符合过过滤条件的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null, startIndex, recordCount);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>全部实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件的实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, (Filter)null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

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
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, filter, sorter, (AssemblyPolicy)null, startIndex, recordCount);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(Filter filter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, filter, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的实体集合。</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, filter, sorter, childrenPolicy, -1, -1);
        }

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
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, filter, sorter, childrenPolicy, startIndex, recordCount);
        }

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (StoredProcedureExecutionResult<TEntity>)Load(entityType, storedProcedureParameters, -1, -1);
        }

        /// <summary>
        /// 从数据库中加载指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="recordCount">要获取的记录数。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (StoredProcedureExecutionResult<TEntity>)Load(entityType, storedProcedureParameters, startIndex, recordCount);
        }

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
        public Object LoadFirst(Type entityType)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, Filter filter)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, Sorter sorter)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, Filter filter, Sorter sorter)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter)
        {
            return LoadFirst(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter)
        {
            return LoadFirst(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, sorter, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, loadStrategy, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            Object[] results = Load(entityType, loadStrategy, filter, sorter, childrenPolicy, 0, 1);

            if (results.Length == 1)
            {
                return results[0];
            }
            else
            {
                return null;
            }
        }

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<Object> LoadFirst(Type entityType, DbStoredProcedureParameters storedProcedureParameters)
        {
            StoredProcedureExecutionResult<Object> results = Load(entityType, storedProcedureParameters, 0, 1);

            return results;
        }

        #endregion

        #endregion

        #region 泛型

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>第一个实体。</returns>
        public TEntity LoadFirst<TEntity>() where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, filter, sorter, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，过滤器和排序器皆可以为空。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>符合过滤条件并按排序器指定的排序列进行排序后的第一个实体。</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, filter, sorter, childrenPolicy);
        }

        #region 执行存储过程

        /// <summary>
        /// 从数据库中加载第一个指定类型的实体，此方法只适用于存储过程指令。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="storedProcedureParameters">存储过程参数集合，如果没有参数，则传 null。</param>
        /// <returns>存储过程执行结果。</returns>
        public StoredProcedureExecutionResult<TEntity> LoadFirst<TEntity>(DbStoredProcedureParameters storedProcedureParameters)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (StoredProcedureExecutionResult<TEntity>)LoadFirst(entityType, storedProcedureParameters);
        }

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
        public GroupResult[] Aggregate(Type groupResultType)
        {
            return Aggregate(groupResultType, (Filter)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果集合。</returns>
        public GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter)
        {
            return Aggregate(groupResultType, whereFilter, havingFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        public GroupResult[] Aggregate(Type groupResultType, Sorter sorter)
        {
            return Aggregate(groupResultType, (Filter)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        public GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter)
        {
            return Aggregate(groupResultType, whereFilter, havingFilter, sorter, -1, -1);
        }

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
        public GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
        {
            #region 性能计数

            Timing.Start("加载分组结果", "DatabaseSession.Aggregate {1F0A0628-D299-435e-987A-4D0A9AC22A7F}");

            #endregion

            GroupSchemaBuilder builder = new GroupSchemaBuilder(groupResultType);

            builder.ExtendWhereFilter(whereFilter);
            builder.ExtendHavingFilter(havingFilter);
            builder.ExtendSorter(sorter);

            GroupSchema schema = builder.Build();
            WriteDebugInfo("分组实体架构", schema);

            DbCommand cmd = CreateAggregateSelectCommand(schema, whereFilter, havingFilter, sorter);

            List<Object> results = new List<Object>();

            Int32 currentIndex = -1;

            Boolean partially = (startIndex >= 0) && (recordCount >= 1);
            Int32 maxIndex = startIndex + recordCount;

            using (IDataReader reader = Database.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    currentIndex++;

                    if (partially)
                    {
                        if (currentIndex < startIndex)
                        {
                            continue;
                        }
                        else if (currentIndex >= maxIndex)
                        {
                            break;
                        }
                    }

                    results.Add(schema.Compose(reader));
                }
            }

            GroupResult[] entities = (GroupResult[])MakeEntityArray(groupResultType, results);

            #region 性能计数

            Timing.Stop(String.Format("获取了 {0} 条分组结果", results.Count), "DatabaseSession.Aggregate {1F0A0628-D299-435e-987A-4D0A9AC22A7F}");

            #endregion

            return entities;
        }

        #endregion

        #region 泛型

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <returns>查询结果集合。</returns>
        public TGroupResult[] Aggregate<TGroupResult>()
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>((Filter)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果集合。</returns>
        public TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter)
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>(whereFilter, havingFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        public TGroupResult[] Aggregate<TGroupResult>(Sorter sorter)
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>((Filter)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 分组查询。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果集合。</returns>
        public TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter)
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>(whereFilter, havingFilter, sorter, -1, -1);
        }

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
        public TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
            where TGroupResult : GroupResult, new()
        {
            return (TGroupResult[])Aggregate(typeof(TGroupResult), whereFilter, havingFilter, sorter, startIndex, recordCount);
        }

        #endregion

        #endregion

        #region AggregateOne

        #region 非泛型

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <returns>查询结果。</returns>
        public GroupResult AggregateOne(Type groupResultType)
        {
            return AggregateOne(groupResultType, (Filter)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果。</returns>
        public GroupResult AggregateOne(Type groupResultType, Filter whereFilter, Filter havingFilter)
        {
            return AggregateOne(groupResultType, whereFilter, havingFilter, (Sorter)null);
        }

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        public GroupResult AggregateOne(Type groupResultType, Sorter sorter)
        {
            return AggregateOne(groupResultType, (Filter)null, (Filter)null, sorter);
        }

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <param name="groupResultType">分组结果实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        public GroupResult AggregateOne(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter)
        {
            GroupResult[] results = Aggregate(groupResultType, whereFilter, havingFilter, sorter, 0, 1);

            if (results.Length == 1)
            {
                return results[0];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 泛型

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <returns>查询结果。</returns>
        public TGroupResult AggregateOne<TGroupResult>()
            where TGroupResult : GroupResult, new()
        {
            return AggregateOne<TGroupResult>((Filter)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <returns>查询结果。</returns>
        public TGroupResult AggregateOne<TGroupResult>(Filter whereFilter, Filter havingFilter)
            where TGroupResult : GroupResult, new()
        {
            return AggregateOne<TGroupResult>(whereFilter, havingFilter, (Sorter)null);
        }

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        public TGroupResult AggregateOne<TGroupResult>(Sorter sorter)
            where TGroupResult : GroupResult, new()
        {
            return AggregateOne<TGroupResult>((Filter)null, (Filter)null, sorter);
        }

        /// <summary>
        /// 分组查询，获取一个结果。
        /// </summary>
        /// <typeparam name="TGroupResult">分组结果实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>查询结果。</returns>
        public TGroupResult AggregateOne<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter)
            where TGroupResult : GroupResult, new()
        {
            TGroupResult[] results = Aggregate<TGroupResult>(whereFilter, havingFilter, sorter, 0, 1);

            if (results.Length == 1)
            {
                return results[0];
            }
            else
            {
                return null;
            }
        }

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
        public Object Avg(Type entityType, String propertyName)
        {
            return Avg(entityType, null, null, propertyName);
        }

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回 null。</returns>
        public Object Avg(Type entityType, String entityPropertyName, String propertyName)
        {
            return Avg(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回 null。</returns>
        public Object Avg(Type entityType, Filter filter, String propertyName)
        {
            return Avg(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回 null。</returns>
        public Object Avg(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型不能为空。");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "值属性名称不能为空。");
            }

            ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

            EntitySchemaComposite composite = GetAggregateSchemaComposite(entityType, filter, colLocator);
            EntitySchema schema = composite.Target;

            DbCommand cmd = CreateAggregateSelectCommand("AVG", schema, filter, colLocator);

            Object result = Database.ExecuteScalar(cmd);

            if (Convert.IsDBNull(result))
            {
                return null;
            }

            Column col = schema[colLocator][0];

            return DbConverter.ConvertFrom(result, col);
        }

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        public TResult Avg<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Avg<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        public TResult Avg<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Avg<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        public TResult Avg<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Avg<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// 计算指定列的平均值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的平均值，如果数据库值为空值，则返回默认值。</returns>
        public TResult Avg<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            Object result = Avg(entityType, filter, entityPropertyName, propertyName);

            if (result != null)
            {
                return (TResult)Convert.ChangeType(result, typeof(TResult));
            }
            else
            {
                return default(TResult);
            }
        }

        #endregion

        #region COUNT

        /// <summary>
        /// 计算记录数。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count(Type entityType)
        {
            return Count(entityType, null, null, null);
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count(Type entityType, String propertyName)
        {
            return Count(entityType, (Filter)null, (String)null, propertyName);
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count(Type entityType, String entityPropertyName, String propertyName)
        {
            return Count(entityType, (Filter)null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 计算记录数。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count(Type entityType, Filter filter)
        {
            return Count(entityType, filter, null, null);
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count(Type entityType, Filter filter, String propertyName)
        {
            return Count(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型不能为空。");
            }

            ColumnLocator colLocator;

            if (propertyName != null)
            {
                colLocator = new ColumnLocator(entityPropertyName, propertyName);
            }
            else
            {
                colLocator = null;
            }

            EntitySchemaComposite composite = GetAggregateSchemaComposite(entityType, filter, colLocator);
            EntitySchema schema = composite.Target;

            DbCommand cmd = CreateAggregateSelectCommand("COUNT", schema, filter, colLocator);

            Int32 recordCount = Convert.ToInt32(Database.ExecuteScalar(cmd));

            return recordCount;
        }

        /// <summary>
        /// 计算记录的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>计算结果。</returns>
        public Int32 Count<TEntity>() where TEntity : class, new()
        {
            return Count(typeof(TEntity));
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count<TEntity>(String propertyName) where TEntity : class, new()
        {
            return Count<TEntity>((Filter)null, (String)null, propertyName);
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count<TEntity>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Count<TEntity>((Filter)null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 计算过滤后的记录的的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count<TEntity>(Filter filter) where TEntity : class, new()
        {
            return Count<TEntity>(filter, (String)null, (String)null);
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count<TEntity>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Count<TEntity>(filter, (String)null, propertyName);
        }

        /// <summary>
        /// 计算与指定的属性相映射的列的数量。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>计算结果。</returns>
        public Int32 Count<TEntity>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return Count(entityType, filter, entityPropertyName, propertyName);
        }

        #endregion

        #region MAX

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        public Object Max(Type entityType, String propertyName)
        {
            return Max(entityType, null, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        public Object Max(Type entityType, String entityPropertyName, String propertyName)
        {
            return Max(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        public Object Max(Type entityType, Filter filter, String propertyName)
        {
            return Max(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回 null。</returns>
        public Object Max(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型不能为空。");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "值属性名称不能为空。");
            }

            ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

            EntitySchemaComposite composite = GetAggregateSchemaComposite(entityType, filter, colLocator);
            EntitySchema schema = composite.Target;

            DbCommand cmd = CreateAggregateSelectCommand("MAX", schema, filter, colLocator);

            Object result = Database.ExecuteScalar(cmd);

            if (Convert.IsDBNull(result))
            {
                return null;
            }

            Column col = schema[colLocator][0];

            return DbConverter.ConvertFrom(result, col);
        }

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        public TResult Max<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Max<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        public TResult Max<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Max<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        public TResult Max<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Max<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最大值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最大值，如果为数据库空值，则返回默认值。</returns>
        public TResult Max<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            Object result = Max(entityType, filter, entityPropertyName, propertyName);

            if (result != null)
            {
                return (TResult)Convert.ChangeType(result, typeof(TResult));
            }
            else
            {
                return default(TResult);
            }
        }

        #endregion

        #region MIN

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        public Object Min(Type entityType, String propertyName)
        {
            return Min(entityType, null, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        public Object Min(Type entityType, String entityPropertyName, String propertyName)
        {
            return Min(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        public Object Min(Type entityType, Filter filter, String propertyName)
        {
            return Min(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回 null。</returns>
        public Object Min(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型不能为空。");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "值属性名称不能为空。");
            }

            ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

            EntitySchemaComposite composite = GetAggregateSchemaComposite(entityType, filter, colLocator);
            EntitySchema schema = composite.Target;

            DbCommand cmd = CreateAggregateSelectCommand("MIN", schema, filter, colLocator);

            Object result = Database.ExecuteScalar(cmd);

            if (Convert.IsDBNull(result))
            {
                return null;
            }

            Column col = schema[colLocator][0];

            return DbConverter.ConvertFrom(result, col);
        }

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        public TResult Min<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Min<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        public TResult Min<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Min<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        public TResult Min<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Min<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// 求指定列的最小值。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的最小值，如果为数据库空值，则返回默认值。</returns>
        public TResult Min<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            Object result = Min(entityType, filter, entityPropertyName, propertyName);

            if (result != null)
            {
                return (TResult)Convert.ChangeType(result, typeof(TResult));
            }
            else
            {
                return default(TResult);
            }
        }

        #endregion

        #region SUM

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        public Object Sum(Type entityType, String propertyName)
        {
            return Sum(entityType, null, null, propertyName);
        }

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        public Object Sum(Type entityType, String entityPropertyName, String propertyName)
        {
            return Sum(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        public Object Sum(Type entityType, Filter filter, String propertyName)
        {
            return Sum(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回 null。</returns>
        public Object Sum(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型不能为空。");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "值属性名称不能为空。");
            }

            ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

            EntitySchemaComposite composite = GetAggregateSchemaComposite(entityType, filter, colLocator);
            EntitySchema schema = composite.Target;

            DbCommand cmd = CreateAggregateSelectCommand("SUM", schema, filter, colLocator);

            Object result = Database.ExecuteScalar(cmd);

            if (Convert.IsDBNull(result))
            {
                return null;
            }

            Column col = schema[colLocator][0];

            return DbConverter.ConvertFrom(result, col);
        }

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        public TResult Sum<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Sum<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        public TResult Sum<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Sum<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        public TResult Sum<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Sum<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// 求指定列的和。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <param name="entityPropertyName">实体属性名称。</param>
        /// <param name="propertyName">值属性名称。</param>
        /// <returns>列的和，如果为数据库空值，则返回默认值。</returns>
        public TResult Sum<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            Object result = Sum(entityType, filter, entityPropertyName, propertyName);

            if (result != null)
            {
                return (TResult)Convert.ChangeType(result, typeof(TResult));
            }
            else
            {
                return default(TResult);
            }
        }

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
        public CompositeResult[] Compose(Type compositeResultType)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, Filter whereFilter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
        {
            return Compose(compositeResultType, loadStrategy, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
        {
            return Compose(compositeResultType, loadStrategy, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, settings, whereFilter, sorter, -1, -1);
        }

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
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
        {
            #region 性能计数

            Timing.Start("加载复合实体", "Compose {DA5BBDB4-039F-4b2d-8773-AB47414416BA}");

            #endregion

            CompositeSchemaBuilder builder = new CompositeSchemaBuilder(compositeResultType, ParameterPrefix);

            builder.LoadStrategy = loadStrategy;
            builder.Settings = settings;
            builder.Where = whereFilter;
            builder.OrderBy = sorter;

            CompositeSchema schema = builder.Build();
            WriteDebugInfo("复合实体架构", schema);

            DbCommand cmd = CreateCompositeSelectCommand(schema);

            List<Object> results = new List<Object>();

            Int32 currentIndex = -1;

            Boolean partially = (startIndex >= 0) && (recordCount >= 1);
            Int32 maxIndex = startIndex + recordCount;

            using (IDataReader reader = Database.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    currentIndex++;

                    if (partially)
                    {
                        if (currentIndex < startIndex)
                        {
                            continue;
                        }
                        else if (currentIndex >= maxIndex)
                        {
                            break;
                        }
                    }

                    results.Add(schema.Compose(reader));
                }
            }

            CompositeResult[] entities = (CompositeResult[])MakeEntityArray(compositeResultType, results);

            #region 性能计数

            Timing.Stop(String.Format("获得 {0:#,##0} 项复合实体", entities.Length), "Compose {DA5BBDB4-039F-4b2d-8773-AB47414416BA}");

            #endregion

            return entities;
        }

        #endregion

        #region 泛型

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>()
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// 加载复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体集合。</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, whereFilter, sorter, -1, -1);
        }

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
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
            where TCompositeResult : CompositeResult, new()
        {
            Type compositeResultType = typeof(TCompositeResult);

            return (TCompositeResult[])Compose(compositeResultType, loadStrategy, settings, whereFilter, sorter, startIndex, recordCount);
        }

        #endregion

        #endregion

        #region ComposeOne

        #region 非泛型

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
        {
            return ComposeOne(compositeResultType, loadStrategy, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, loadStrategy, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
        {
            return ComposeOne(compositeResultType, loadStrategy, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <param name="compositeResultType">复合实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
        {
            CompositeResult[] results = Compose(compositeResultType, loadStrategy, settings, whereFilter, sorter, 0, 1);

            if (results.Length == 1)
            {
                return results[0];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 泛型

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>()
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// 加载一个复合实体。
        /// </summary>
        /// <typeparam name="TCompositeResult">复合实体类型。</typeparam>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="settings">复合实体过滤器列表。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>复合实体。</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            TCompositeResult[] results = Compose<TCompositeResult>(loadStrategy, settings, whereFilter, sorter, 0, 1);

            if (results.Length == 1)
            {
                return results[0];
            }
            else
            {
                return null;
            }
        }

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
        public Boolean Exists(Type entityType, Filter filter)
        {
            Int32 recordCount = Count(entityType, filter);

            return (recordCount != 0);
        }

        /// <summary>
        /// 判断给定条件的实体是否存在（过滤器如果为空表示查找全部）。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="filter">过滤器。</param>
        /// <returns>如果实体存在，则返回 true；否则返回 false。</returns>
        public Boolean Exists<TEntity>(Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return Exists(entityType, filter);
        }

        #endregion

        #region 创建命令

        /// <summary>
        /// 创建插入命令，公开此方法的目的是给系统日志服务使用，因该服务使用专有的连接进行数据库操作，因此不使用企业库的执行命令的方法。
        /// </summary>
        /// <param name="entity">要插入的实体。</param>
        /// <returns>创建好的命令。</returns>
        public static DbCommand CreateInsertCommand(Object entity)
        {
            return CreateInsertCommand(entity, m_db, Filter.PARAMETER_PREFIX);
        }

        #endregion

        #region 其他服务

        /// <summary>
        /// 获取服务器的当前时间。
        /// </summary>
        /// <returns>服务器的当前时间。</returns>
        public DateTime Now()
        {
            return DateTime.Now;
        }

        #endregion

        #endregion

        #region 受保护的属性

        /// <summary>
        /// 获取 Database 实例，默认实现返回默认的数据库实例。
        /// </summary>
        protected virtual Database Database
        {
            get
            {
                if (m_databaseName == null)
                {
                    return DatabaseFactory.CreateDatabase();
                }
                else
                {
                    return DatabaseFactory.CreateDatabase(m_databaseName);
                }
            }
        }

        /// <summary>
        /// 获取数据库特性。
        /// </summary>
        protected virtual DatabaseTraits Traits
        {
            get
            {
                return m_traits;
            }
        }

        /// <summary>
        /// 获取参数前缀，默认为“@”。
        /// </summary>
        protected virtual String ParameterPrefix
        {
            get
            {
                if (m_parameterPrefix == null)
                {
                    return Filter.PARAMETER_PREFIX;
                }
                else
                {
                    return m_parameterPrefix;
                }
            }
        }

        #endregion

        #region 创建命令

        /// <summary>
        /// 创建分组查询命令。
        /// </summary>
        /// <param name="schema">分组结果实体架构。</param>
        /// <param name="whereFilter">WHERE 过滤器。</param>
        /// <param name="havingFilter">HAVING 过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>分组查询命令。</returns>
        private DbCommand CreateAggregateSelectCommand(GroupSchema schema, Filter whereFilter, Filter havingFilter, Sorter sorter)
        {
            #region 前置条件

            Debug.Assert(!String.IsNullOrEmpty(schema.SelectList), "选择列表不能为空。");

            #endregion

            #region 性能计数

            Timing.Start("构造分组查询命令", "DatabaseSession.CreateAggregateSelectCommand {6C84F7A0-974A-4bc6-B270-FDA800A03C88}");

            #endregion

            SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

            sqlBuilder.Select = schema.SelectList;
            sqlBuilder.From = schema.FromList;
            sqlBuilder.GroupBy = schema.GroupList;

            FilterCompilationResult whereCompilationResult = schema.Composite.ComposeFilterExpression(whereFilter, ParameterPrefix);
            FilterCompilationResult havingCompilationResult = schema.ComposeFilterExpression(havingFilter, ParameterPrefix);

            sqlBuilder.Where = (whereCompilationResult != null) ? whereCompilationResult.WhereClause : null;
            sqlBuilder.Having = (havingCompilationResult != null) ? havingCompilationResult.WhereClause : null;

            String sortExpression = Sorter.ComposeSortExpression(schema, sorter);
            sqlBuilder.OrderBy = sortExpression;

            String sql = sqlBuilder.Build();

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            if (whereCompilationResult != null)
            {
                foreach (QueryParameter parameter in whereCompilationResult.Parameters)
                {
                    Database.AddInParameter(cmd, parameter.Name, parameter.DbType, parameter.Value);
                }
            }

            if (!String.IsNullOrEmpty(schema.GroupList) && (havingCompilationResult != null))
            {
                foreach (QueryParameter parameter in havingCompilationResult.Parameters)
                {
                    Database.AddInParameter(cmd, parameter.Name, parameter.DbType, parameter.Value);
                }
            }

            #region 性能计数

            Timing.Stop("DatabaseSession.CreateAggregateSelectCommand {6C84F7A0-974A-4bc6-B270-FDA800A03C88}");

            #endregion

            WriteDbCommandContent("分组查询", cmd);

            return cmd;
        }

        /// <summary>
        /// 创建复合实体查询命令。
        /// </summary>
        /// <param name="schema">复合实体架构。</param>
        /// <returns>创建好的命令。</returns>
        private DbCommand CreateCompositeSelectCommand(CompositeSchema schema)
        {
            #region 性能计数

            Timing.Start("构造复合实体查询命令", "DatabaseSession.CreateCompositeSelectCommand {9A8FBC3D-95FB-43d8-9456-6171DFF87A40}");

            #endregion

            DbCommand cmd = Database.GetSqlStringCommand(schema.SqlExpression);

            foreach (QueryParameter parameter in schema.Parameters)
            {
                Database.AddInParameter(cmd, parameter.Name, parameter.DbType, parameter.Value);
            }

            #region 性能计数

            Timing.Stop("DatabaseSession.CreateCompositeSelectCommand {9A8FBC3D-95FB-43d8-9456-6171DFF87A40}");

            #endregion

            WriteDbCommandContent("复合实体查询", cmd);

            return cmd;
        }

        /// <summary>
        /// 创建插入命令。
        /// </summary>
        /// <param name="entity">要插入的实体。</param>
        /// <param name="db">数据库。</param>
        /// <param name="parameterPrefix">参数前缀。</param>
        /// <returns>创建好的命令。</returns>
        private static DbCommand CreateInsertCommand(Object entity, Database db, String parameterPrefix)
        {
            EntitySchemaComposite composite = GetSchemaComposite(
                    entity.GetType(),
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            List<QueryParameter> parameters = new List<QueryParameter>();
            List<String> skippedColumns = new List<String>();

            foreach (Column col in schema.Columns)
            {
                Object dbValue = col.GetDbValue(entity);

                if (!DbConverter.IsDBEmpty(dbValue) && !col.IsPrimaryKeyNative)
                {
                    parameters.Add(new QueryParameter(col.Name, col.DbType, dbValue));
                }
                else
                {
                    skippedColumns.Add(col.Name);
                }
            }

            StringBuilder fieldListBuilder = new StringBuilder();
            StringBuilder valueListBuilder = new StringBuilder();

            foreach (Column col in schema.Columns)
            {
                if (skippedColumns.Contains(col.Name))
                {
                    continue;
                }

                if (fieldListBuilder.Length != 0)
                {
                    fieldListBuilder.Append(",");
                    valueListBuilder.Append(",");
                }

                fieldListBuilder.Append(col.Name);
                valueListBuilder.Append(parameterPrefix + col.Name);
            }

            // 构建 SQL 指令
            String sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", schema.TableName, fieldListBuilder, valueListBuilder);

            DbCommand cmd = db.GetSqlStringCommand(sql);

            // 设置参数
            foreach (QueryParameter p in parameters)
            {
                db.AddInParameter(cmd, p.Name, p.DbType, p.Value);
            }

            WriteDbCommandContent("插入记录", cmd, parameterPrefix);

            return cmd;
        }

        /// <summary>
        /// 创建删除命令。
        /// </summary>
        /// <param name="entity">要删除的实体。</param>
        /// <returns>创建好的删除命令。</returns>
        private DbCommand CreateDeleteCommand(Object entity)
        {
            EntitySchemaComposite compsoite = GetSchemaComposite(
                    entity.GetType(),
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = compsoite.Target;

            StringBuilder whereBuilder = new StringBuilder();

            foreach (Column col in schema.PrimaryKeyColumns)
            {
                if (whereBuilder.Length != 0)
                {
                    whereBuilder.Append(" AND ");
                }

                whereBuilder.AppendFormat("{0}={1}{0}", col.Name, ParameterPrefix);
            }

            // 构建 SQL 指令
            String sql = String.Format("DELETE FROM {0} WHERE {1}", schema.TableName, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // 设置参数
            foreach (Column col in schema.PrimaryKeyColumns)
            {
                if (col.IsPrimaryKey)
                {
                    Object dbValue = col.GetDbValue(entity);

                    Database.AddInParameter(cmd, col.Name, col.DbType, dbValue);
                }
            }

            WriteDbCommandContent("删除记录", cmd);

            return cmd;
        }

        /// <summary>
        /// 创建删除全部记录的命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>创建好的删除全部记录的命令。</returns>
        private DbCommand CreateDeleteAllCommand(Type entityType)
        {
            EntitySchemaComposite composite = GetSchemaComposite(
                    entityType,
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            String sql = String.Format("DELETE FROM {0}", schema.TableName);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            WriteDbCommandContent(((cmd != null) ? "删除所有记录" : "删除所有记录，未指定过滤条件"), cmd);

            return cmd;
        }

        /// <summary>
        /// 创建批量删除记录的命令。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>创建好的批量删除记录的命令。</returns>
        private DbCommand CreateDeleteBatchCommand(Type entityType, Filter filter)
        {
            EntitySchemaComposite composite = GetSchemaComposite(
                    entityType,
                    CompositeBuilderStrategyFactory.Nothing,
                    filter,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            FilterCompilationResult compilationResult = composite.ComposeFilterExpression(filter, ParameterPrefix);

            DbCommand cmd;

            if (compilationResult != null)
            {
                String fromList = composite.FromList;

                StringBuilder whereClauseBuilder = new StringBuilder();

                for (Int32 i = 0; i < schema.PrimaryKeyColumns.Count; i++)
                {
                    if (i > 0)
                    {
                        whereClauseBuilder.Append(" AND ");
                    }

                    Column col = schema.PrimaryKeyColumns[i];

                    whereClauseBuilder.AppendFormat(
                            "{0} IN (SELECT {1} FROM {2} WHERE {3})",
                            col.Name,
                            col.FullName,
                            fromList,
                            compilationResult.WhereClause
                        );
                }

                String sql = String.Format("DELETE FROM {0} WHERE {1}", schema.TableName, whereClauseBuilder);

                cmd = Database.GetSqlStringCommand(sql);

                foreach (QueryParameter parameter in compilationResult.Parameters)
                {
                    Database.AddInParameter(cmd, parameter.Name, parameter.DbType, parameter.Value);
                }
            }
            else
            {
                cmd = null;
            }

            WriteDbCommandContent("批量删除记录", cmd);

            return cmd;
        }

        /// <summary>
        /// 创建更新命令。
        /// </summary>
        /// <param name="entity">要更新的实体。</param>
        /// <returns>创建好的更新命令。</returns>
        private DbCommand CreateUpdateCommand(Object entity)
        {
            EntitySchemaComposite composite = GetSchemaComposite(
                    entity.GetType(),
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            List<QueryParameter> parameters = new List<QueryParameter>();
            List<String> skippedColumns = new List<String>();

            foreach (Column col in schema.Columns)
            {
                Object value = col.GetDbValue(entity);

                if (!DbConverter.IsDBEmpty(value))
                {
                    parameters.Add(new QueryParameter(col.Name, col.DbType, value));
                }
                else
                {
                    skippedColumns.Add(col.Name);
                }
            }

            StringBuilder setListBuilder = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();

            foreach (Column col in schema.Columns)
            {
                if (col.IsPrimaryKey)
                {
                    if (whereBuilder.Length != 0)
                    {
                        whereBuilder.Append(" AND ");
                    }

                    whereBuilder.AppendFormat("{0}={1}{0}", col.Name, ParameterPrefix);
                }
                else
                {
                    if (skippedColumns.Contains(col.Name))
                    {
                        continue;
                    }

                    if (setListBuilder.Length != 0)
                    {
                        setListBuilder.Append(",");
                    }

                    setListBuilder.AppendFormat("{0}={1}{0}", col.Name, ParameterPrefix);
                }
            }

            // 构建 SQL 指令
            String sql = String.Format("UPDATE {0} SET {1} WHERE {2}", schema.TableName, setListBuilder, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // 设置参数
            foreach (QueryParameter p in parameters)
            {
                Database.AddInParameter(cmd, p.Name, p.DbType, p.Value);
            }

            WriteDbCommandContent("修改记录", cmd);

            return cmd;
        }

        /// <summary>
        /// 创建查询指令。
        /// </summary>
        /// <param name="schema">实体架构。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>创建好的查询指令。</returns>
        private DbCommand CreateSelectCommand(EntitySchema schema, Filter filter, Sorter sorter)
        {
            #region 前置条件

            Debug.Assert(!String.IsNullOrEmpty(schema.Composite.SelectList), "在加载实体时应至少选择一项属性。");

            #endregion

            SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

            sqlBuilder.Select = schema.Composite.SelectList;
            sqlBuilder.From = schema.Composite.FromList;

            FilterCompilationResult compilationResult = schema.Composite.ComposeFilterExpression(filter, ParameterPrefix);
            sqlBuilder.Where = (compilationResult != null) ? compilationResult.WhereClause : null;

            String sortExpression = Sorter.ComposeSortExpression(schema, sorter);
            sqlBuilder.OrderBy = sortExpression;

            String sql = sqlBuilder.Build();

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            if (compilationResult != null)
            {
                foreach (QueryParameter parameter in compilationResult.Parameters)
                {
                    Database.AddInParameter(cmd, parameter.Name, parameter.DbType, parameter.Value);
                }
            }

            WriteDbCommandContent("查询", cmd);

            return cmd;
        }

        /// <summary>
        /// 创建查询指令，只用于存储过程。
        /// </summary>
        /// <param name="schema">实体架构。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合。</param>
        /// <returns>创建好的查询指令。</returns>
        private DbCommand CreateSelectCommand(EntitySchema schema, DbStoredProcedureParameters storedProcedureParameters)
        {
            String name;

            if ((storedProcedureParameters != null) && !String.IsNullOrEmpty(storedProcedureParameters.StoredProcedureName))
            {
                name = storedProcedureParameters.StoredProcedureName;
            }
            else
            {
                name = schema.TableName;
            }

            DbCommand cmd = Database.GetStoredProcCommand(name);

            AddParametersToCommand(cmd, storedProcedureParameters);

            return cmd;
        }

        /// <summary>
        /// 创建聚合查询指令。
        /// </summary>
        /// <param name="functionName">聚合函数名（如“SUM”）。</param>
        /// <param name="schema">实体架构。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="colLocator">列定位符。</param>
        /// <returns>创建好的聚合查询指令。</returns>
        private DbCommand CreateAggregateSelectCommand(String functionName, EntitySchema schema, Filter filter, ColumnLocator colLocator)
        {
            String fromList = schema.Composite.FromList;

            FilterCompilationResult compilationResult = schema.Composite.ComposeFilterExpression(filter, ParameterPrefix);

            String columnFullName;

            if (colLocator != null)
            {
                Column col = schema[colLocator][0];
                columnFullName = col.FullName;
            }
            else
            {
                columnFullName = "*";
            }

            // 开始构造 SQL 指令
            StringBuilder buffer = new StringBuilder();

            buffer.AppendFormat("SELECT {0}({1}) FROM {2}", functionName, columnFullName, fromList);

            if (compilationResult != null)
            {
                buffer.AppendFormat(" WHERE {0}", compilationResult.WhereClause);
            }

            String sql = buffer.ToString();

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            if (compilationResult != null)
            {
                foreach (QueryParameter parameter in compilationResult.Parameters)
                {
                    Database.AddInParameter(cmd, parameter.Name, parameter.DbType, parameter.Value);
                }
            }

            WriteDbCommandContent("聚合操作", cmd);

            return cmd;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 向 DbCommand 对象添加参数。
        /// </summary>
        /// <param name="cmd">数据库命令。</param>
        /// <param name="storedProcedureParameters">存储过程参数集合。</param>
        private void AddParametersToCommand(DbCommand cmd, DbStoredProcedureParameters storedProcedureParameters)
        {
            if (storedProcedureParameters != null)
            {
                QueryParameter[] allParameters = storedProcedureParameters.GetDbParameters();

                foreach (QueryParameter parameter in allParameters)
                {
                    Database.AddParameter(
                            cmd,
                            parameter.Name,
                            parameter.DbType,
                            parameter.Size,
                            parameter.Direction,
                            parameter.Nullable,
                            parameter.Precision,
                            parameter.Scale,
                            null,  // sourceColumn
                            DataRowVersion.Default,
                            MakeDbValue(parameter.Value)
                        );

                    if (!String.IsNullOrEmpty(parameter.DbTypePropertyName))
                    {
                        // 通过反射设置参数的数据库类型
                        SetDbTypeValue(cmd, parameter);
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程。
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称。</param>
        /// <param name="storedProcedureParameters">存储过程参数，如果不存在参数，可以为 null。</param>
        /// <returns>存储过程执行结果。</returns>
        private StoredProcedureExecutionResult<Object> DoExecuteStoredProcedure(String storedProcedureName, DbStoredProcedureParameters storedProcedureParameters)
        {
            if (storedProcedureName != null)
            {
                storedProcedureName = storedProcedureName.Trim();
            }

            if (String.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName", "存储过程名称不能为空或空字符串。");
            }

            DbCommand cmd = Database.GetStoredProcCommand(storedProcedureName);

            AddParametersToCommand(cmd, storedProcedureParameters);

            Int32 rowsAffected = Database.ExecuteNonQuery(cmd);

            SetOutputParameterValues(storedProcedureParameters, cmd);

            StoredProcedureExecutionResult<Object> executionResult = new StoredProcedureExecutionResult<Object>(storedProcedureParameters, rowsAffected);

            return executionResult;
        }

        /// <summary>
        /// 获取聚合指令的实体架构组合。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="colLocator">要聚合的列。</param>
        /// <returns>创建好的实体架构组合。</returns>
        private static EntitySchemaComposite GetAggregateSchemaComposite(Type entityType, Filter filter, ColumnLocator colLocator)
        {
            CompositeBuilderStrategy loadStrategy;

            if (colLocator != null)
            {
                loadStrategy = Select.Properties(colLocator.Create(entityType));
            }
            else
            {
                loadStrategy = CompositeBuilderStrategyFactory.Nothing;
            }

            EntitySchemaComposite composite = GetSchemaComposite(
                    entityType,
                    loadStrategy,
                    filter,
                    (Sorter)null
                );

            return composite;
        }

        /// <summary>
        /// 获取实体架构组合。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>实体架构组合。</returns>
        private static EntitySchemaComposite GetSchemaComposite(Type entityType)
        {
            return GetSchemaComposite(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// 获取实体架构组合。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <returns>实体架构组合。</returns>
        private static EntitySchemaComposite GetSchemaComposite(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter)
        {
            return GetSchemaComposite(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// 获取实体架构组合。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="loadStrategy">加载策略。</param>
        /// <param name="filter">过滤器。</param>
        /// <param name="sorter">排序器。</param>
        /// <param name="childrenPolicy">子实体装配方针。</param>
        /// <returns>实体架构组合。</returns>
        private static EntitySchemaComposite GetSchemaComposite(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            #region 前置条件

            Debug.Assert((entityType != null), "实体类型参数 entityType 不能为空。");

            #endregion

            List<PropertySelector> additionalSelectors = null;

            if ((filter != null) || (sorter != null) || (childrenPolicy != null))
            {
                additionalSelectors = new List<PropertySelector>();

                if (filter != null)
                {
                    additionalSelectors.AddRange(filter.GetAllSelectors(entityType));
                }

                if (sorter != null)
                {
                    additionalSelectors.AddRange(sorter.GetAllSelectors(entityType));
                }

                if (childrenPolicy != null)
                {
                    additionalSelectors.AddRange(childrenPolicy.GetAllSelectors(entityType));
                }
            }

            CompositeBuilderStrategy finalStrategy = CompositeBuilderStrategyFactory.Compose(entityType, loadStrategy, additionalSelectors);

            EntitySchemaComposite composite = EntitySchemaCompositeFactory.Create(entityType, finalStrategy);

            WriteSchemaComposite(composite);

            return composite;
        }

        /// <summary>
        /// 获取用于得到标识值的 SQL 指令。
        /// </summary>
        /// <param name="definition">实体定义。</param>
        /// <returns>SQL 指令。</returns>
        private String GetRetrieveIdentifierStatement(EntityDefinition definition)
        {
            if (!String.IsNullOrEmpty(definition.NativePrimaryKeyInfo.RetrieveIdentifierStatement))
            {
                return definition.NativePrimaryKeyInfo.RetrieveIdentifierStatement;
            }
            else if (!String.IsNullOrEmpty(Traits.RetrieveIdentifierStatement))
            {
                return Traits.RetrieveIdentifierStatement;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将用户提供的值转换为数据库值，当提供值为 null 或未初始化的 DateTime（即 DateTime.MinValue）时，转换为 DBNull.Value，其他情况原样返回。
        /// </summary>
        /// <param name="value">用户提供的值。</param>
        /// <returns>数据库值。</returns>
        private static Object MakeDbValue(Object value)
        {
            if ((value == null) || DbConverter.IsDateTimeAndUninitialized(value))
            {
                return DBNull.Value;
            }

            return value;
        }

        /// <summary>
        /// 构造实体数组。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// 
        /// <param name="results">实体集合。</param>
        /// <returns>构造好的实体数据。</returns>
        private Object[] MakeEntityArray(Type entityType, List<Object> results)
        {
            Object[] entities = (Object[])Array.CreateInstance(entityType, results.Count);

            results.CopyTo(entities);

            return entities;
        }

        /// <summary>
        /// 为数据库命令的参数设置数据库类型，调用此方法前要求数据库命令的参数集合中已包含此参数。
        /// </summary>
        /// <param name="cmd">数据库命令。</param>
        /// <param name="parameterInfo">参数信息。</param>
        private void SetDbTypeValue(DbCommand cmd, QueryParameter parameterInfo)
        {
            String parameterName = Database.BuildParameterName(parameterInfo.Name);
            DbParameter parameter = cmd.Parameters[parameterName];
            Type parameterType = parameter.GetType();
            PropertyInfo dbTypeProperty = parameterType.GetProperty(
                    parameterInfo.DbTypePropertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase
                );

            if (dbTypeProperty == null)
            {
                throw new InvalidOperationException(String.Format("数据库 {0}（数据库提供者为 {1}）不支持参数的数据库类型 {2}。", Database.ConnectionStringWithoutCredentials, Database.DbProviderFactory.ToString(), parameterInfo.DbTypePropertyName));
            }

            Object parameterValue = Enum.Parse(dbTypeProperty.PropertyType, parameterInfo.DbTypePropertyValue.ToString());

            dbTypeProperty.SetValue(parameter, parameterValue, null);
        }

        /// <summary>
        /// 设置存储过程输出参数的值。
        /// </summary>
        /// <param name="storedProcedureParameters">存储过程参数集合。</param>
        /// <param name="cmd">数据库命令。</param>
        private void SetOutputParameterValues(DbStoredProcedureParameters storedProcedureParameters, DbCommand cmd)
        {
            if (storedProcedureParameters != null)
            {
                // 获取非输入参数属性
                storedProcedureParameters.SetNonInputParameterValues(
                        delegate(String parameterName)
                        {
                            return Database.GetParameterValue(cmd, parameterName);
                        }
                    );
            }
        }

        #region 调试方法

        /// <summary>
        /// 将命令的文本和参数信息写入日志以供调试。
        /// </summary>
        /// <param name="operation">操作名称。</param>
        /// <param name="cmd">命令。</param>
        [Conditional("DEBUG")]
        private void WriteDbCommandContent(String operation, DbCommand cmd)
        {
            WriteDbCommandContent(operation, cmd, this.ParameterPrefix);
        }

        /// <summary>
        /// 将命令的文本和参数信息写入日志以供调试。
        /// </summary>
        /// <param name="operation">操作名称。</param>
        /// <param name="cmd">命令。</param>
        /// <param name="parameterPrefix">查询参数前缀。</param>
        [Conditional("DEBUG")]
        private static void WriteDbCommandContent(String operation, DbCommand cmd, String parameterPrefix)
        {
            if (cmd == null)
            {
                Util.Log.TraceWarning("{0}，未生成命令", operation);

                return;
            }

            String[] headers = new String[] { "名称", "类型", "值" };

            TabularWriter writer = new TabularWriter(headers.Length);

            writer.WriteLine(headers);

            foreach (DbParameter p in cmd.Parameters)
            {
                String valueStr;

                if (Convert.IsDBNull(p.Value))
                {
                    valueStr = "NULL";
                }
                else if (p.Value is Byte[])
                {
                    valueStr = DbEntityDebugger.Dump((Byte[])p.Value);
                }
                else
                {
                    valueStr = p.Value.ToString();
                }

                writer.WriteLine(
                        new String[] 
							{ 
								p.ParameterName, 
								p.DbType.ToString(), 
								valueStr
							}
                    );
            }

            // 格式化
            String formattedStatement = DbEntityDebugger.FormatSqlStatement(cmd.CommandText);
            Dictionary<String, String> parameterValues = GetParameterValuePresentations(cmd);

            Regex exParameter = new Regex(Regex.Escape(parameterPrefix) + "[_a-zA-Z0-9]+");

            formattedStatement = exParameter.Replace(
                    formattedStatement,
                    delegate(Match m)
                    {
                        string paramName = m.Value;

                        if (parameterValues.ContainsKey(paramName))
                        {
                            return parameterValues[paramName];
                        }

                        paramName = paramName.Substring(parameterPrefix.Length);

                        if (parameterValues.ContainsKey(paramName))
                        {
                            return parameterValues[paramName];
                        }

                        return String.Empty;
                    }
                );

            Util.Log.TraceInformation(
                        "{0}，命令文本：{1}{1}{2}{1}{1}{1}命令参数：{1}{1}{3}{1}{1}格式化命令文本（对于字节或文本类型，参数值截断为 100）：{1}{4}",
                        operation,
                        Environment.NewLine,
                        cmd.CommandText,
                        writer.ToString(),
                        formattedStatement
                );
        }

        /// <summary>
        /// 获取参数值集合，键为参数名称，值为参数值的字符串形式。
        /// </summary>
        /// <param name="cmd">数据库命令。</param>
        /// <returns>字典。</returns>
        private static Dictionary<String, String> GetParameterValuePresentations(DbCommand cmd)
        {
            const Int32 MAX_LENGTH = 100;
            Dictionary<String, String> results = new Dictionary<String, String>();

            foreach (DbParameter p in cmd.Parameters)
            {
                String valueStr;

                if (Convert.IsDBNull(p.Value))
                {
                    valueStr = "NULL";

                    results.Add(p.ParameterName, valueStr);

                    continue;
                }

                switch (p.DbType)
                {
                    case DbType.Binary:
                        Boolean appendDigest = false;
                        valueStr = DbEntityDebugger.Dump((Byte[])p.Value, MAX_LENGTH, appendDigest);
                        break;

                    case DbType.Decimal:
                        valueStr = Convert.ToDecimal(p.Value).ToString();
                        break;

                    case DbType.Byte:
                        valueStr = Convert.ToByte(p.Value).ToString();
                        break;

                    case DbType.Int16:
                        valueStr = Convert.ToInt16(p.Value).ToString();
                        break;

                    case DbType.Int32:
                        valueStr = Convert.ToInt32(p.Value).ToString();
                        break;

                    case DbType.Int64:
                        valueStr = Convert.ToInt64(p.Value).ToString();
                        break;

                    case DbType.UInt16:
                        valueStr = Convert.ToUInt16(p.Value).ToString();
                        break;

                    case DbType.UInt32:
                        valueStr = Convert.ToUInt32(p.Value).ToString();
                        break;

                    case DbType.UInt64:
                        valueStr = Convert.ToUInt64(p.Value).ToString();
                        break;

                    default:
                    case DbType.String:
                        valueStr = p.Value.ToString();
                        valueStr = valueStr.Substring(0, Math.Min(valueStr.Length, MAX_LENGTH));
                        valueStr = valueStr.Replace("'", "''");
                        valueStr = String.Format("'{0}'", valueStr);
                        break;
                }

                results.Add(p.ParameterName, valueStr);
            }

            return results;
        }

        /// <summary>
        /// 将过滤器的结果写入日志以供调试。
        /// </summary>
        /// <param name="compilationResult">过滤器编译结果。</param>
        [Conditional("DEBUG")]
        private static void WriteWhereClause(FilterCompilationResult compilationResult)
        {
            if (compilationResult == null || String.IsNullOrEmpty(compilationResult.WhereClause))
            {
                return;
            }

            Util.Log.TraceInformation("过滤条件：{0}", compilationResult.WhereClause);
        }

        /// <summary>
        /// 将实体架构写入日志以供调试。
        /// </summary>
        /// <param name="composite">实体架构组合。</param>
        [Conditional("DEBUG")]
        private static void WriteSchemaComposite(EntitySchemaComposite composite)
        {
            Util.Log.TraceInformation(
                        "实体架构组合（{0}）的详细信息:\r\n{1}\r\n生成策略：{2}",
                        composite.Target.Type.Name,
                        composite.Dump(),
                        composite.BuilderStrategy.Dump()
                );
        }

        /// <summary>
        /// 将调试信息写入日志以供调试。
        /// </summary>
        /// <param name="displayName">显示名称。</param>
        /// <param name="provider">调试信息提供者。</param>
        [Conditional("DEBUG")]
        private static void WriteDebugInfo(String displayName, IDebugInfoProvider provider)
        {
            Util.Log.TraceInformation(displayName + "：\r\n" + provider.Dump());
        }

        #endregion

        #endregion
    }
}