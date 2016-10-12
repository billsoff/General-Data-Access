#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2010 ���ݽ�΢����������޹�˾
// ��Ȩ����
// 
//
// �ļ�����DatabaseSession.cs
// �ļ�������������ʾ�����ݿ���н����Ķ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008132231
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

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
    /// ��ʾ�����ݿ���н����Ķ���
    /// </summary>
    public class DatabaseSession : MarshalByRefObject, IDatabaseSession
    {
        #region ˽���ֶ�

        #region ��̬

        /// <summary>
        /// Ĭ�����ݿ⡣
        /// </summary>
        private static readonly Database m_db = DatabaseFactory.CreateDatabase();

        #endregion

        private DatabaseTraits m_traits = new SqlServerEquivalentDatabaseTraits();
        private readonly String m_databaseName;
        private readonly String m_parameterPrefix;

        #endregion

        #region ���캯��

        /// <summary>
        /// Ĭ�Ϲ��캯����
        /// </summary>
        public DatabaseSession()
        {
        }

        /// <summary>
        /// �������ݿ����ƺͲ���ǰ׺��
        /// </summary>
        /// <param name="databaseName">���ݿ����ơ�</param>
        /// <param name="parameterPrefix">����ǰ׺��</param>
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

        #region ��������

        #region ������ѯ

        #region �Ƿ���

        /// <summary>
        /// �����ݿ����ʵ�壬��������Ϊ DateTime ��ֵΪ DateTime.MinValue �����ԡ�
        /// </summary>
        /// <param name="entity">Ҫ��ӵ�ʵ�塣</param>
        public void Add(Object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "�����ݿ���ӵ�ʵ�岻��Ϊ�ա�");
            }

            DbCommand cmd = CreateInsertCommand(entity, Database, ParameterPrefix);

            Database.ExecuteNonQuery(cmd);

            // �� Remoting ���龰�£����������ʵ�嵽�ͻ��ˣ������´��벻������
            EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(entity);
        }

        /// <summary>
        /// ���ʵ�壬����ȡ��ʶֵ��
        /// </summary>
        /// <param name="entity">ʵ�塣</param>
        /// <param name="idValue">��ʶֵ��</param>
        public void Add(Object entity, out Object idValue)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "�����ݿ���ӵ�ʵ�岻��Ϊ�ա�");
            }

            DbCommand cmd = CreateInsertCommand(entity, Database, ParameterPrefix);
            EntityDefinition definition = EntityDefinitionBuilder.Build(entity.GetType());

            if (definition.NativePrimaryKeyInfo.AutoIncrement)
            {
                String idStatement = GetRetrieveIdentifierStatement(definition); ;

                if (String.IsNullOrEmpty(idStatement))
                {
                    throw new InvalidOperationException("δ�������ڻ�ȡ����������ʶ���ֶε�ֵ�� SQL ָ��޷���ȡ��ʶ�ֶε�ֵ��");
                }

                cmd.CommandText += String.Format(";{0};", idStatement);

                idValue = Database.ExecuteScalar(cmd);
            }
            else
            {
                Database.ExecuteNonQuery(cmd);
                idValue = null;
            }

            // �� Remoting ���龰�£����������ʵ�嵽�ͻ��ˣ������´��벻������
            EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(entity);
        }

        /// <summary>
        /// �������ݿ����ӡ�
        /// </summary>
        /// <returns>�����õ����ݿ����ӡ�</returns>
        public DbConnection CreateConnection()
        {
            return Database.CreateConnection();
        }

        /// <summary>
        /// ���� Database ʵ����
        /// </summary>
        /// <returns>�����õ� Database ʵ����</returns>
        public Database CreateDatabase()
        {
            return Database;
        }

        /// <summary>
        /// ɾ��ʵ�塣
        /// </summary>
        /// <param name="entity">Ҫɾ����ʵ�塣</param>
        /// <returns>���ɾ���ɹ����򷵻� true�����򷵻� false��</returns>
        public Boolean Delete(Object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Ҫɾ����ʵ�岻��Ϊ�ա�");
            }

            DbCommand cmd = CreateDeleteCommand(entity);

            Boolean success = true;

            try
            {
                Database.ExecuteNonQuery(cmd);

                IDbEntity bo = entity as IDbEntity;

                if (bo != null)
                {
                    bo.Transient = false; // ���ö���Ϊ�־ö�����Ϊֻ�г־ö���ſ���ɾ����
                    bo.RequireDelete = true;

                    EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(bo);
                }
            }
            catch (DbException de)
            {
                success = false;

                Util.Log.TraceWarning("ɾ��ʵ�� {0} ʧ�ܣ��쳣��Ϣ��{1}���쳣��ջ��{2}", entity, de.Message, de.StackTrace);
            }

            return success;
        }

        /// <summary>
        /// ɾ�����м�¼��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        public void DeleteAll(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ���Ϊ�ա�");
            }

            DbCommand cmd = CreateDeleteAllCommand(entityType);

            Database.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// ����ɾ����
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        public void DeleteBatch<TEntity>(Filter filter) where TEntity : class
        {
            Type entityType = typeof(TEntity);

            DeleteBatch(entityType, filter);
        }

        /// <summary>
        /// �޸�ʵ�壬��������Ϊ DateTime ��ֵΪ DateTime.MinValue �����ԡ�
        /// </summary>
        /// <param name="entity">Ҫ�޸ĵ�ʵ�塣</param>
        public void Modify(Object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Ҫ�޸ĵ�ʵ�岻��Ϊ�ա�");
            }

            DbCommand cmd = CreateUpdateCommand(entity);

            Database.ExecuteNonQuery(cmd);

            EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(entity);
        }

        #region �����޸�

        /// <summary>
        /// �����޸ģ�ʹ���½������ݿ����ӣ�UpdateBehavior ��ֵΪ Continue��������������ʱ����������Ը��������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ���µ�ʵ�弯�ϡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities)
        {
            return ModifyBatch(entities, UpdateBehavior.Continue);
        }

        /// <summary>
        /// �����޸ģ�ʹ���½������ݿ����ӡ�
        /// </summary>
        /// <param name="entities">Ҫ���µ�ʵ�弯�ϡ�</param>
        /// <param name="updateBehavior">������Ϊ��</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities, UpdateBehavior updateBehavior)
        {
            return ModifyBatch(entities, null, updateBehavior);
        }

        /// <summary>
        /// �����޸ģ�ʹ�õ��÷��ṩ�����ݿ����ӣ�UpdateBehavior ��ֵΪ Continue��������������ʱ����������Ը��������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ���µ�ʵ�弯�ϡ�</param>
        /// <param name="connection">���ݿ����ӣ� ���Ϊ�գ����½�һ�����ӡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities, DbConnection connection)
        {
            return ModifyBatch(entities, connection, UpdateBehavior.Continue);
        }

        /// <summary>
        /// �����޸ģ�ʹ�õ��÷��ṩ�����ݿ����ӡ�
        /// </summary>
        /// <param name="entities">Ҫ���µ�ʵ�弯�ϡ�</param>
        /// <param name="connection">���ݿ����ӣ� ���Ϊ�գ����½�һ�����ӡ�</param>
        /// <param name="updateBehavior">������Ϊ��</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        public Int32 ModifyBatch(IList<IDbEntity> entities, DbConnection connection, UpdateBehavior updateBehavior)
        {
            WriteModifyBatchDbEntities(
                    entities,
                    String.Format(
                        "�����޸�ʵ�壬ʹ��{0}���ӣ�UpdateBehavior Ϊ {1}",
                        ((connection != null) ? "���÷��ṩ��" : "����"),
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

            // ��������Դ
            DataTable dataSource = GetModifyBatchDataSource(schema, changes);

            Int32 rowsAffected;

            // ʹ����������
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
            // ʹ�õ��÷��ṩ�����ӣ�����ģʽ
            else if (updateBehavior == UpdateBehavior.Transactional)
            {
                rowsAffected = DoModifyBatchTransactional(dataSource, schema, connection);
            }
            // ʹ�õ��÷��ṩ�����ӣ�����ģʽ
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
        /// �Ա�׼�ķ��������޸ģ�������������ʱ�������������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        public Int32 ModifyBatchStandard(IList<IDbEntity> entities)
        {
            return ModifyBatch(entities, UpdateBehavior.Standard);
        }

        /// <summary>
        /// �Ա�׼�ķ�ʽ�����޸ģ�������������ʱ�������������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <param name="connection">Ҫʹ�õ����ݿ����ӡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        public Int32 ModifyBatchStandard(IList<IDbEntity> entities, DbConnection connection)
        {
            return ModifyBatch(entities, connection, UpdateBehavior.Standard);
        }

        /// <summary>
        /// ������ķ�ʽ�����޸ģ�������������ʱ���ع����е��޸ġ�
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <returns>ʵ���޸ĵ�ʵ������</returns>
        public Int32 ModifyBatchTransactional(IList<IDbEntity> entities)
        {
            return ModifyBatch(entities, UpdateBehavior.Transactional);
        }

        /// <summary>
        /// ������ķ�ʽ�����޸ģ�������������ʱ���ع����е��޸ġ�
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <param name="connection">Ҫʹ�õ����ݿ����ӡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        public Int32 ModifyBatchTransactional(IList<IDbEntity> entities, DbConnection connection)
        {
            return ModifyBatch(entities, connection, UpdateBehavior.Transactional);
        }

        #region ��������

        /// <summary>
        /// ������Դ��һ��ʵ�塣
        /// </summary>
        /// <param name="e">ʵ�塣</param>
        /// <param name="dataSource">����Դ��</param>
        /// <param name="schema">ʵ��ܹ���</param>
        /// <returns>�Ӻõ��У��� RowState Ϊ Unchanged��</returns>
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
        /// �������޸ĵ�������Ӳ�����
        /// </summary>
        /// <param name="cmd">���</param>
        /// <param name="col">�С�</param>
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
        /// �������ݿ���������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="connection">���ӡ�</param>
        /// <param name="updateBehavior"></param>
        /// <returns>�����õ���������</returns>
        private DbDataAdapter CreateModifyBatchDataAdapter(Type entityType, DbConnection connection, UpdateBehavior updateBehavior)
        {
            Type dbType = typeof(Database);

            MethodInfo getDataAdapterMethod = dbType.GetMethod("GetDataAdapter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(UpdateBehavior) }, null);

            if (getDataAdapterMethod == null)
            {
                throw new InvalidOperationException("�޷���ȡ DataAdapter�������ʹ���������ݿ����ӵİ汾������Ҫ��ʽ�ش������ݿ����ӣ���");
            }

            DbDataAdapter adapter = (DbDataAdapter)getDataAdapterMethod.Invoke(Database, new Object[] { updateBehavior });

            return adapter;
        }

        /// <summary>
        /// ���������޸ĵ�ɾ�����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>ɾ�����</returns>
        private DbCommand CreateModifyBatchDeleteCommand(Type entityType)
        {
            return CreateModifyBatchDeleteCommand(entityType, null);
        }

        /// <summary>
        /// ���������޸ĵ�ɾ�����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="connection">���ݿ����ӡ�</param>
        /// <returns>ɾ�����</returns>
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

            // ���� SQL ָ��
            String sql = String.Format("DELETE FROM {0} WHERE {1}", schema.TableName, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // ���ò���
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
        /// ���������޸Ĳ������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>�������</returns>
        private DbCommand CreateModifyBatchInsertCommand(Type entityType)
        {
            return CreateModifyBatchInsertCommand(entityType, null);
        }

        /// <summary>
        /// ���������޸Ĳ������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="connection">���ݿ����ӡ�</param>
        /// <returns>�������</returns>
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

            // ���� SQL ָ��
            String sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", schema.TableName, fieldListBuilder, valueListBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // ���ò���
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
        /// ���������޸ĵĸ������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>�������</returns>
        private DbCommand CreateModifyBatchUpdateCommand(Type entityType)
        {
            return CreateModifyBatchUpdateCommand(entityType, null);
        }

        /// <summary>
        /// ���������޸ĵĸ������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="connection">���ݿ����ӡ�</param>
        /// <returns>�������</returns>
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

            // ���� SQL ָ��
            String sql = String.Format("UPDATE {0} SET {1} WHERE {2}", schema.TableName, setListBuilder, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // ���ò���
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
        /// �ڸ���������������������ķ�ʽִ�������޸ġ�
        /// </summary>
        /// <param name="dataSource">����Դ��</param>
        /// <param name="schema">�ܹ���Ϣ��</param>
        /// <param name="connection">���ݿ����ӡ�</param>
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
        /// ��ȡָ��ʵ�����͵������޸ļܹ���Ϣ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>�����޸ĵļܹ���Ϣ��</returns>
        private BatchModificationSchema GetBatchModificationSchema(Type entityType)
        {
            BatchModificationSchema schema = BatchModificationSchemaCache.GetSchema(entityType, Database);

            if (schema == null)
            {
                // ��������Դ
                EntitySchemaComposite composite = GetSchemaComposite(entityType);

                DataTable dataSource = new DataTable(composite.Target.TableName);

                foreach (Column col in composite.Target.Columns)
                {
                    if (col.Selected)
                    {
                        dataSource.Columns.Add(new DataColumn(col.Name, col.Type));
                    }
                }

                // �������ݿ�����
                DbCommand insertCommand = CreateModifyBatchInsertCommand(entityType);
                DbCommand updateCommand = CreateModifyBatchUpdateCommand(entityType);
                DbCommand deleteCommand = CreateModifyBatchDeleteCommand(entityType);

                schema = new BatchModificationSchema(entityType, dataSource, insertCommand, updateCommand, deleteCommand);

                BatchModificationSchemaCache.SetSchema(schema, Database);
            }

            return schema;
        }

        /// <summary>
        /// ��ȡʵ��ĸ��²�����
        /// </summary>
        /// <param name="e">ʵ�塣</param>
        /// <returns>ʵ��ĸ��²�����</returns>
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
        /// ��ȡҪ�޸ĵ�ʵ��.
        /// </summary>
        /// <param name="entities">ʵ�弯��.</param>
        /// <returns>Ҫ�޸ĵ�ʵ�弯��.</returns>
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
        /// ���������޸ĵ�����Դ��
        /// </summary>
        /// <param name="schema">�ܹ���Ϣ��</param>
        /// <param name="entities">Ҫ�޸ĵ�ʵ�弯�ϡ�</param>
        /// <returns>�����õ�����Դ��</returns>
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

                    // ������������
                    case UpdateAction.None:
                        break;
                    default:
                        break;
                }
            }

            return dataSource;
        }

        /// <summary>
        /// �����ݿ����ӡ�
        /// </summary>
        /// <param name="connection">���ݿ����ӡ�</param>
        private static void OpenConnection(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        #endregion

        #region ���ڵ���

        /// <summary>
        /// д�����޸ĵ���־��
        /// </summary>
        /// <param name="dbEntities">���ݿ�ʵ�弯�ϡ�</param>
        /// <param name="prompt">��ʾ��</param>
        [Conditional("DEBUG")]
        private static void WriteModifyBatchDbEntities(IList<IDbEntity> dbEntities, String prompt)
        {
            if ((dbEntities == null) || (dbEntities.Count == 0))
            {
                Util.Log.TraceInformation("�����޸�ʵ�壬ִ�������޸ĵ�ʵ�弯��Ϊ�ջ򲻰���ʵ���");

                return;
            }

            Type entityType = dbEntities[0].GetType();

            EntitySchemaComposite composite = GetSchemaComposite(entityType);
            EntitySchema schema = composite.Target;

            List<IDbEntity> sortedDbEntities = new List<IDbEntity>(dbEntities);

            #region �����ݿ�ʵ��������򣬽�Ҫ�޸ĵ�ʵ�������ǰ

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

            headers.AddRange(new String[] { "�־ö���?", "Ҫ��ɾ��?", "�ѱ��޸�?", "�ѱ�ɾ��?", "���²���" });

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

            // д����־
            Util.Log.TraceInformation("{0}\r\n\r\n{1}", prompt, writer);
        }

        #endregion

        #endregion

        /// <summary>
        /// �ύ���ݿ�����
        /// </summary>
        /// <param name="scope">�����򣬷�װ��Ҫ���в���������ʵ�塣</param>
        public void CommitTransaction(TransactionScope scope)
        {
            if ((scope == null) || (scope.Entities.Length == 0))
            {
                throw new ArgumentNullException("scope", "������Ϊ�ջ򲻰���ʵ�塣");
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
                        throw new InvalidOperationException("�����������ָ���Ĳ������Ͳ���ʶ��");
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

            #region �� Remoting ���龰�£����������ʵ�嵽�ͻ��ˣ������´��벻������

            // ����ʵ���״̬
            List<IDbEntity> dbEntities = new List<IDbEntity>();

            foreach (ActionQueryEntity queryEntity in scope.Entities)
            {
                IDbEntity e = queryEntity.Entity as IDbEntity;

                if (e != null)
                {
                    // �־ö���
                    e.Transient = false;

                    if (queryEntity.ActionQueryType == ActionQueryType.Delete)
                    {
                        // ������ɾ��
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

        #region ִ�д洢����

        /// <summary>
        /// ִ�д洢���̡�
        /// </summary>
        /// <param name="storedProcedureName">�洢�������ơ�</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName)
        {
            return DoExecuteStoredProcedure(storedProcedureName, null);
        }

        /// <summary>
        /// ִ�д洢���̡�
        /// </summary>
        /// <param name="storedProcedureParameters">�洢���̲�������������ڲ���������Ϊ null��</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(DbStoredProcedureParameters storedProcedureParameters)
        {
            if (storedProcedureParameters == null)
            {
                throw new ArgumentNullException("storedProcedureParameters", "�洢���̲������϶�����Ϊ�գ��˷�����Ҫ���л�ȡ�洢�������ơ�");
            }

            return DoExecuteStoredProcedure(storedProcedureParameters.StoredProcedureName, storedProcedureParameters);
        }

        /// <summary>
        /// ִ�д洢���̡�
        /// </summary>
        /// <param name="storedProcedureName">�洢�������ơ�</param>
        /// <param name="storedProcedureParameters">�洢���̲�������������ڲ���������Ϊ null��</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName, DbStoredProcedureParameters storedProcedureParameters)
        {
            return DoExecuteStoredProcedure(storedProcedureName, storedProcedureParameters);
        }

        #endregion

        #endregion

        #region ����

        /// <summary>
        /// ɾ�����м�¼��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        public void DeleteAll<TEntity>() where TEntity : class
        {
            Type entityType = typeof(TEntity);

            DeleteAll(entityType);
        }

        /// <summary>
        /// ����ɾ����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        public void DeleteBatch(Type entityType, Filter filter)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ���Ϊ�ա�");
            }

            if (filter == null)
            {
                throw new ArgumentNullException("filter", "����������Ϊ�ա�");
            }

            DbCommand cmd = CreateDeleteBatchCommand(entityType, filter);

            Database.ExecuteNonQuery(cmd);
        }

        #endregion

        #region SQL Server ��������

        /// <summary>
        /// �� SQL Server ���ݿ��������������ʵ�塣
        /// </summary>
        /// <param name="entities">ʵ�弯�ϡ�</param>
        public void SqlBulkCopy(IList entities)
        {
            SqlBulkCopy(entities, SqlBulkCopyOptions.Default);
        }

        /// <summary>
        /// �� SQL Server ���ݿ��������������ʵ�塣
        /// </summary>
        /// <param name="entities">ʵ�弯�ϡ�</param>
        /// <param name="copyOptions">����ѡ�</param>
        public void SqlBulkCopy(IList entities, SqlBulkCopyOptions copyOptions)
        {
            if ((entities == null) || (entities.Count == 0))
            {
                return;
            }

            if (!(Database.DbProviderFactory is SqlClientFactory))
            {
                throw new InvalidOperationException("Ŀ�����ݿⲻ�� SQL Server�����ܵ��ô˷�����");
            }

            // ��ȡʵ��ܹ�
            EntitySchemaComposite composite = GetSchemaComposite(
                    entities[0].GetType(),
                    CompositeBuilderStrategyFactory.Nothing,
                    (Filter)null,
                    (Sorter)null
                );
            EntitySchema schema = composite.Target;

            // ����Դ��
            DataTable dataSource = CreateSqlBulkCopyDataSource(entities, schema);

            // ��������
            SqlConnection conn = (SqlConnection)Database.CreateConnection();

            // ���� SqlBulkCopy ���󲢵����� WriteToServer ������������
            using (SqlBulkCopy bcp = CreateSqlBulkCopyInstance(conn, copyOptions, schema))
            {
                conn.Open();

                try
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    bcp.WriteToServer(dataSource);

                    sw.Stop();

                    Util.Log.TraceInformation(
                                "�������� {0} �� {1} ʵ�嵽�� {2}������ SqlBulkCopy.WriteToServer ��ʱ {3:#,###} ms��",
                                entities.Count,
                                schema.Type.FullName,
                                schema.TableName,
                                sw.ElapsedMilliseconds
                        );
                }
                catch (Exception ex)
                {
                    Util.Log.TraceError(
                                "��������ʵ�� {0} ���� {1} ����������Ϣ��{2}\r\n��ջ��{3}",
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
        /// ����������������Դ��
        /// </summary>
        /// <param name="entities">ʵ�弯�ϡ�</param>
        /// <param name="schema">ʵ��ܹ���</param>
        /// <returns>�����õ�����Դ��</returns>
        private static DataTable CreateSqlBulkCopyDataSource(IList entities, EntitySchema schema)
        {
            Stopwatch sw = Stopwatch.StartNew();

            DataTable dataSource = new DataTable();

            foreach (Column col in schema.Columns)
            {
                dataSource.Columns.Add(new DataColumn(col.Name, col.Type));
            }

            sw.Stop();

            Util.Log.TraceInformation("�������Դ�ܹ���������ʱ {0:#,##0} ms", sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();

            // �������
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

            Util.Log.TraceInformation("�������Դ������䣬��ʱ {0:#,##0} ms", sw.ElapsedMilliseconds);

            return dataSource;
        }

        /// <summary>
        /// ���� <see cref="System.Data.SqlClient.SqlBulkCopy"/> ʵ����
        /// </summary>
        /// <param name="conn">SQL Server ����ʵ����</param>
        /// <param name="copyOptions">����ѡ�</param>
        /// <param name="schema">ʵ��ܹ���</param>
        /// <returns>�����õ� <see cref="System.Data.SqlClient.SqlBulkCopy"/> ʵ����</returns>
        private static SqlBulkCopy CreateSqlBulkCopyInstance(SqlConnection conn, SqlBulkCopyOptions copyOptions, EntitySchema schema)
        {
            Stopwatch sw = Stopwatch.StartNew();

            SqlBulkCopy bcp = new SqlBulkCopy(conn, copyOptions, null);

            bcp.DestinationTableName = schema.TableName;

            // ����ӳ��
            foreach (Column col in schema.Columns)
            {
                bcp.ColumnMappings.Add(col.Name, col.Name);
            }

            sw.Stop();

            Util.Log.TraceInformation("���� SqlBulkCopy ʵ������ʱ {0:#,##0} ms", sw.ElapsedMilliseconds);

            return bcp;
        }

        #endregion

        #endregion

        #region ��������

        #region Load

        #region �Ƿ���

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>ȫ��ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, Filter filter)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, Sorter sorter)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, Filter filter, Sorter sorter)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
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
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>ȫ��ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy)
        {
            return Load(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter)
        {
            return Load(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return Load(entityType, loadStrategy, null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter)
        {
            return Load(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount)
        {
            return Load(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null, startIndex, recordCount);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return Load(entityType, loadStrategy, filter, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount)
        {
            // TODO: ���ܼ���
            //PerformenceMetric metric = new PerformenceMetric();

            //metric.Start();

            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "Ҫ���ص�ʵ�����Ͳ���Ϊ�ա�");
            }

            EntitySchemaComposite composite = GetSchemaComposite(entityType, loadStrategy, filter, sorter, childrenPolicy);
            EntitySchema schema = composite.Target;

            #region ���Ϊ�洢���̣���ִ�в����ؽ���������Թ�������������

            if (schema.IsStoredProcedure)
            {
                DbStoredProcedureParameters storedProcedureParameters = new DbStoredProcedureParameters(schema.TableName);

                StoredProcedureExecutionResult<Object> executionResult = Load(entityType, storedProcedureParameters, startIndex, recordCount);

                return executionResult.Entities;
            }

            #endregion

            DbCommand cmd = CreateSelectCommand(schema, filter, sorter);

            //metric.logSw(String.Format("��ɲ�ѯ {0} �� SQL ����Ĺ���", entityType.FullName));

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

                    // �ϳ�ʵ��
                    Object entity = composite.Compose(reader);

                    results.Add(entity);
                }
            }

            Object[] entities = MakeEntityArray(entityType, results);

            if (childrenPolicy != null)
            {
                childrenPolicy.Enforce(entityType, entities, filter, this);
            }

            //metric.logSw(String.Format("��� {0} ʵ��ĺϳ�", entityType.FullName));

            return entities;
        }

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters)
        {
            return Load(entityType, storedProcedureParameters, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount)
        {
            // TODO: ���ܼ���
            //PerformenceMetric metric = new PerformenceMetric();

            //metric.Start();

            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "Ҫ���ص�ʵ�����Ͳ���Ϊ�ա�");
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
                throw new InvalidOperationException(String.Format("����Դ {0} ���Ǵ洢���̣����ܵ��ô˷�����", schema.TableName));
            }

            DbCommand cmd = CreateSelectCommand(schema, storedProcedureParameters);

            //metric.logSw(String.Format("��ɲ�ѯ {0} �� SQL ����Ĺ���", entityType.FullName));

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

                    // �ϳ�ʵ��
                    Object entity = composite.Compose(reader);

                    results.Add(entity);
                }
            }

            Object[] entities = MakeEntityArray(entityType, results);

            //metric.logSw(String.Format("��� {0} ʵ��ĺϳ�", entityType.FullName));

            SetOutputParameterValues(storedProcedureParameters, cmd);

            StoredProcedureExecutionResult<Object> executionResult = new StoredProcedureExecutionResult<Object>(storedProcedureParameters, entities);

            return executionResult;
        }

        #endregion

        #endregion

        #region ����

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <returns>ȫ��ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>() where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null, startIndex, recordCount);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>ȫ��ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, (Filter)null, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, filter, sorter, (AssemblyPolicy)null, startIndex, recordCount);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(Filter filter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>((CompositeBuilderStrategy)null, filter, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, (Filter)null, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, filter, (Sorter)null, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, (Filter)null, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
            where TEntity : class, new()
        {
            return Load<TEntity>(loadStrategy, filter, sorter, childrenPolicy, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        public TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity[])Load(entityType, loadStrategy, filter, sorter, childrenPolicy, startIndex, recordCount);
        }

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (StoredProcedureExecutionResult<TEntity>)Load(entityType, storedProcedureParameters, -1, -1);
        }

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (StoredProcedureExecutionResult<TEntity>)Load(entityType, storedProcedureParameters, startIndex, recordCount);
        }

        #endregion

        #endregion

        #endregion

        #region LoadFirst

        #region �Ƿ���

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>��һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, Filter filter)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, Sorter sorter)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, Filter filter, Sorter sorter)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>��һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter)
        {
            return LoadFirst(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter)
        {
            return LoadFirst(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, sorter, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, loadStrategy, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            return LoadFirst(entityType, loadStrategy, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
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

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<Object> LoadFirst(Type entityType, DbStoredProcedureParameters storedProcedureParameters)
        {
            StoredProcedureExecutionResult<Object> results = Load(entityType, storedProcedureParameters, 0, 1);

            return results;
        }

        #endregion

        #endregion

        #region ����

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <returns>��һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>() where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, (CompositeBuilderStrategy)null, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>��һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, (Filter)null, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, filter, (Sorter)null, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, (Filter)null, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>((CompositeBuilderStrategy)null, filter, sorter, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, (Filter)null, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, filter, (Sorter)null, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            return LoadFirst<TEntity>(loadStrategy, (Filter)null, sorter, childrenPolicy);
        }

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        public TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (TEntity)LoadFirst(entityType, loadStrategy, filter, sorter, childrenPolicy);
        }

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        public StoredProcedureExecutionResult<TEntity> LoadFirst<TEntity>(DbStoredProcedureParameters storedProcedureParameters)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return (StoredProcedureExecutionResult<TEntity>)LoadFirst(entityType, storedProcedureParameters);
        }

        #endregion

        #endregion

        #endregion

        #region �ۺϲ���

        #region �����ѯ

        #region Aggregate

        #region �Ƿ���

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <returns>��ѯ������ϡ�</returns>
        public GroupResult[] Aggregate(Type groupResultType)
        {
            return Aggregate(groupResultType, (Filter)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        public GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter)
        {
            return Aggregate(groupResultType, whereFilter, havingFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        public GroupResult[] Aggregate(Type groupResultType, Sorter sorter)
        {
            return Aggregate(groupResultType, (Filter)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        public GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter)
        {
            return Aggregate(groupResultType, whereFilter, havingFilter, sorter, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">��¼����</param>
        /// <returns>��ѯ������ϡ�</returns>
        public GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
        {
            #region ���ܼ���

            Timing.Start("���ط�����", "DatabaseSession.Aggregate {1F0A0628-D299-435e-987A-4D0A9AC22A7F}");

            #endregion

            GroupSchemaBuilder builder = new GroupSchemaBuilder(groupResultType);

            builder.ExtendWhereFilter(whereFilter);
            builder.ExtendHavingFilter(havingFilter);
            builder.ExtendSorter(sorter);

            GroupSchema schema = builder.Build();
            WriteDebugInfo("����ʵ��ܹ�", schema);

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

            #region ���ܼ���

            Timing.Stop(String.Format("��ȡ�� {0} ��������", results.Count), "DatabaseSession.Aggregate {1F0A0628-D299-435e-987A-4D0A9AC22A7F}");

            #endregion

            return entities;
        }

        #endregion

        #region ����

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <returns>��ѯ������ϡ�</returns>
        public TGroupResult[] Aggregate<TGroupResult>()
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>((Filter)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        public TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter)
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>(whereFilter, havingFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        public TGroupResult[] Aggregate<TGroupResult>(Sorter sorter)
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>((Filter)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        public TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter)
            where TGroupResult : GroupResult, new()
        {
            return Aggregate<TGroupResult>(whereFilter, havingFilter, sorter, -1, -1);
        }

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">��¼����</param>
        /// <returns>��ѯ������ϡ�</returns>
        public TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
            where TGroupResult : GroupResult, new()
        {
            return (TGroupResult[])Aggregate(typeof(TGroupResult), whereFilter, havingFilter, sorter, startIndex, recordCount);
        }

        #endregion

        #endregion

        #region AggregateOne

        #region �Ƿ���

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <returns>��ѯ�����</returns>
        public GroupResult AggregateOne(Type groupResultType)
        {
            return AggregateOne(groupResultType, (Filter)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ�����</returns>
        public GroupResult AggregateOne(Type groupResultType, Filter whereFilter, Filter havingFilter)
        {
            return AggregateOne(groupResultType, whereFilter, havingFilter, (Sorter)null);
        }

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
        public GroupResult AggregateOne(Type groupResultType, Sorter sorter)
        {
            return AggregateOne(groupResultType, (Filter)null, (Filter)null, sorter);
        }

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
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

        #region ����

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <returns>��ѯ�����</returns>
        public TGroupResult AggregateOne<TGroupResult>()
            where TGroupResult : GroupResult, new()
        {
            return AggregateOne<TGroupResult>((Filter)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ�����</returns>
        public TGroupResult AggregateOne<TGroupResult>(Filter whereFilter, Filter havingFilter)
            where TGroupResult : GroupResult, new()
        {
            return AggregateOne<TGroupResult>(whereFilter, havingFilter, (Sorter)null);
        }

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
        public TGroupResult AggregateOne<TGroupResult>(Sorter sorter)
            where TGroupResult : GroupResult, new()
        {
            return AggregateOne<TGroupResult>((Filter)null, (Filter)null, sorter);
        }

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
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
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻� null��</returns>
        public Object Avg(Type entityType, String propertyName)
        {
            return Avg(entityType, null, null, propertyName);
        }

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻� null��</returns>
        public Object Avg(Type entityType, String entityPropertyName, String propertyName)
        {
            return Avg(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻� null��</returns>
        public Object Avg(Type entityType, Filter filter, String propertyName)
        {
            return Avg(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻� null��</returns>
        public Object Avg(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ���Ϊ�ա�");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "ֵ�������Ʋ���Ϊ�ա�");
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
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Avg<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Avg<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Avg<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Avg<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Avg<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Avg<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
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
        /// �����¼����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>��������</returns>
        public Int32 Count(Type entityType)
        {
            return Count(entityType, null, null, null);
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count(Type entityType, String propertyName)
        {
            return Count(entityType, (Filter)null, (String)null, propertyName);
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count(Type entityType, String entityPropertyName, String propertyName)
        {
            return Count(entityType, (Filter)null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// �����¼����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>��������</returns>
        public Int32 Count(Type entityType, Filter filter)
        {
            return Count(entityType, filter, null, null);
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count(Type entityType, Filter filter, String propertyName)
        {
            return Count(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ���Ϊ�ա�");
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
        /// �����¼��������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <returns>��������</returns>
        public Int32 Count<TEntity>() where TEntity : class, new()
        {
            return Count(typeof(TEntity));
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count<TEntity>(String propertyName) where TEntity : class, new()
        {
            return Count<TEntity>((Filter)null, (String)null, propertyName);
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count<TEntity>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Count<TEntity>((Filter)null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ������˺�ļ�¼�ĵ�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>��������</returns>
        public Int32 Count<TEntity>(Filter filter) where TEntity : class, new()
        {
            return Count<TEntity>(filter, (String)null, (String)null);
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count<TEntity>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Count<TEntity>(filter, (String)null, propertyName);
        }

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        public Int32 Count<TEntity>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return Count(entityType, filter, entityPropertyName, propertyName);
        }

        #endregion

        #region MAX

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Max(Type entityType, String propertyName)
        {
            return Max(entityType, null, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Max(Type entityType, String entityPropertyName, String propertyName)
        {
            return Max(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Max(Type entityType, Filter filter, String propertyName)
        {
            return Max(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Max(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ���Ϊ�ա�");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "ֵ�������Ʋ���Ϊ�ա�");
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
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Max<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Max<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Max<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Max<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Max<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Max<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
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
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Min(Type entityType, String propertyName)
        {
            return Min(entityType, null, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Min(Type entityType, String entityPropertyName, String propertyName)
        {
            return Min(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Min(Type entityType, Filter filter, String propertyName)
        {
            return Min(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Min(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ���Ϊ�ա�");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "ֵ�������Ʋ���Ϊ�ա�");
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
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Min<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Min<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Min<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Min<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Min<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Min<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
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
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Sum(Type entityType, String propertyName)
        {
            return Sum(entityType, null, null, propertyName);
        }

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Sum(Type entityType, String entityPropertyName, String propertyName)
        {
            return Sum(entityType, null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Sum(Type entityType, Filter filter, String propertyName)
        {
            return Sum(entityType, filter, null, propertyName);
        }

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        public Object Sum(Type entityType, Filter filter, String entityPropertyName, String propertyName)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ���Ϊ�ա�");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName", "ֵ�������Ʋ���Ϊ�ա�");
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
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Sum<TEntity, TResult>(String propertyName) where TEntity : class, new()
        {
            return Sum<TEntity, TResult>(null, null, propertyName);
        }

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Sum<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new()
        {
            return Sum<TEntity, TResult>(null, entityPropertyName, propertyName);
        }

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        public TResult Sum<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new()
        {
            return Sum<TEntity, TResult>(filter, null, propertyName);
        }

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
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

        #region ����ʵ��

        #region Compose

        #region �Ƿ���

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, Filter whereFilter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
        {
            return Compose(compositeResultType, loadStrategy, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
        {
            return Compose(compositeResultType, loadStrategy, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
        {
            return Compose(compositeResultType, loadStrategy, settings, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ�����������㡣</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
        {
            #region ���ܼ���

            Timing.Start("���ظ���ʵ��", "Compose {DA5BBDB4-039F-4b2d-8773-AB47414416BA}");

            #endregion

            CompositeSchemaBuilder builder = new CompositeSchemaBuilder(compositeResultType, ParameterPrefix);

            builder.LoadStrategy = loadStrategy;
            builder.Settings = settings;
            builder.Where = whereFilter;
            builder.OrderBy = sorter;

            CompositeSchema schema = builder.Build();
            WriteDebugInfo("����ʵ��ܹ�", schema);

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

            #region ���ܼ���

            Timing.Stop(String.Format("��� {0:#,##0} ���ʵ��", entities.Length), "Compose {DA5BBDB4-039F-4b2d-8773-AB47414416BA}");

            #endregion

            return entities;
        }

        #endregion

        #region ����

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>()
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, (Filter)null, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, whereFilter, (Sorter)null, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, (Filter)null, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return Compose<TCompositeResult>(loadStrategy, settings, whereFilter, sorter, -1, -1);
        }

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ�����������㡣</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        public TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
            where TCompositeResult : CompositeResult, new()
        {
            Type compositeResultType = typeof(TCompositeResult);

            return (TCompositeResult[])Compose(compositeResultType, loadStrategy, settings, whereFilter, sorter, startIndex, recordCount);
        }

        #endregion

        #endregion

        #region ComposeOne

        #region �Ƿ���

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, (Filter)null, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
        {
            return ComposeOne(compositeResultType, loadStrategy, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter)
        {
            return ComposeOne(compositeResultType, (CompositeBuilderStrategy)null, settings, whereFilter, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
        {
            return ComposeOne(compositeResultType, loadStrategy, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
        {
            return ComposeOne(compositeResultType, loadStrategy, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
        {
            return ComposeOne(compositeResultType, loadStrategy, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
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

        #region ����

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>()
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, (Filter)null, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, (CompositeSettings)null, whereFilter, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>((CompositeBuilderStrategy)null, settings, whereFilter, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, settings, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, settings, whereFilter, (Sorter)null);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        public TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
            where TCompositeResult : CompositeResult, new()
        {
            return ComposeOne<TCompositeResult>(loadStrategy, settings, (Filter)null, sorter);
        }

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
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

        #region �������ж�

        /// <summary>
        /// �жϸ���������ʵ���Ƿ���ڣ����������Ϊ�ձ�ʾ����ȫ������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>���ʵ����ڣ��򷵻� true�����򷵻� false��</returns>
        public Boolean Exists(Type entityType, Filter filter)
        {
            Int32 recordCount = Count(entityType, filter);

            return (recordCount != 0);
        }

        /// <summary>
        /// �жϸ���������ʵ���Ƿ���ڣ����������Ϊ�ձ�ʾ����ȫ������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>���ʵ����ڣ��򷵻� true�����򷵻� false��</returns>
        public Boolean Exists<TEntity>(Filter filter) where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            return Exists(entityType, filter);
        }

        #endregion

        #region ��������

        /// <summary>
        /// ����������������˷�����Ŀ���Ǹ�ϵͳ��־����ʹ�ã���÷���ʹ��ר�е����ӽ������ݿ��������˲�ʹ����ҵ���ִ������ķ�����
        /// </summary>
        /// <param name="entity">Ҫ�����ʵ�塣</param>
        /// <returns>�����õ����</returns>
        public static DbCommand CreateInsertCommand(Object entity)
        {
            return CreateInsertCommand(entity, m_db, Filter.PARAMETER_PREFIX);
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ�������ĵ�ǰʱ�䡣
        /// </summary>
        /// <returns>�������ĵ�ǰʱ�䡣</returns>
        public DateTime Now()
        {
            return DateTime.Now;
        }

        #endregion

        #endregion

        #region �ܱ���������

        /// <summary>
        /// ��ȡ Database ʵ����Ĭ��ʵ�ַ���Ĭ�ϵ����ݿ�ʵ����
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
        /// ��ȡ���ݿ����ԡ�
        /// </summary>
        protected virtual DatabaseTraits Traits
        {
            get
            {
                return m_traits;
            }
        }

        /// <summary>
        /// ��ȡ����ǰ׺��Ĭ��Ϊ��@����
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

        #region ��������

        /// <summary>
        /// ���������ѯ���
        /// </summary>
        /// <param name="schema">������ʵ��ܹ���</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>�����ѯ���</returns>
        private DbCommand CreateAggregateSelectCommand(GroupSchema schema, Filter whereFilter, Filter havingFilter, Sorter sorter)
        {
            #region ǰ������

            Debug.Assert(!String.IsNullOrEmpty(schema.SelectList), "ѡ���б���Ϊ�ա�");

            #endregion

            #region ���ܼ���

            Timing.Start("��������ѯ����", "DatabaseSession.CreateAggregateSelectCommand {6C84F7A0-974A-4bc6-B270-FDA800A03C88}");

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

            #region ���ܼ���

            Timing.Stop("DatabaseSession.CreateAggregateSelectCommand {6C84F7A0-974A-4bc6-B270-FDA800A03C88}");

            #endregion

            WriteDbCommandContent("�����ѯ", cmd);

            return cmd;
        }

        /// <summary>
        /// ��������ʵ���ѯ���
        /// </summary>
        /// <param name="schema">����ʵ��ܹ���</param>
        /// <returns>�����õ����</returns>
        private DbCommand CreateCompositeSelectCommand(CompositeSchema schema)
        {
            #region ���ܼ���

            Timing.Start("���츴��ʵ���ѯ����", "DatabaseSession.CreateCompositeSelectCommand {9A8FBC3D-95FB-43d8-9456-6171DFF87A40}");

            #endregion

            DbCommand cmd = Database.GetSqlStringCommand(schema.SqlExpression);

            foreach (QueryParameter parameter in schema.Parameters)
            {
                Database.AddInParameter(cmd, parameter.Name, parameter.DbType, parameter.Value);
            }

            #region ���ܼ���

            Timing.Stop("DatabaseSession.CreateCompositeSelectCommand {9A8FBC3D-95FB-43d8-9456-6171DFF87A40}");

            #endregion

            WriteDbCommandContent("����ʵ���ѯ", cmd);

            return cmd;
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="entity">Ҫ�����ʵ�塣</param>
        /// <param name="db">���ݿ⡣</param>
        /// <param name="parameterPrefix">����ǰ׺��</param>
        /// <returns>�����õ����</returns>
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

            // ���� SQL ָ��
            String sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", schema.TableName, fieldListBuilder, valueListBuilder);

            DbCommand cmd = db.GetSqlStringCommand(sql);

            // ���ò���
            foreach (QueryParameter p in parameters)
            {
                db.AddInParameter(cmd, p.Name, p.DbType, p.Value);
            }

            WriteDbCommandContent("�����¼", cmd, parameterPrefix);

            return cmd;
        }

        /// <summary>
        /// ����ɾ�����
        /// </summary>
        /// <param name="entity">Ҫɾ����ʵ�塣</param>
        /// <returns>�����õ�ɾ�����</returns>
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

            // ���� SQL ָ��
            String sql = String.Format("DELETE FROM {0} WHERE {1}", schema.TableName, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // ���ò���
            foreach (Column col in schema.PrimaryKeyColumns)
            {
                if (col.IsPrimaryKey)
                {
                    Object dbValue = col.GetDbValue(entity);

                    Database.AddInParameter(cmd, col.Name, col.DbType, dbValue);
                }
            }

            WriteDbCommandContent("ɾ����¼", cmd);

            return cmd;
        }

        /// <summary>
        /// ����ɾ��ȫ����¼�����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>�����õ�ɾ��ȫ����¼�����</returns>
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

            WriteDbCommandContent(((cmd != null) ? "ɾ�����м�¼" : "ɾ�����м�¼��δָ����������"), cmd);

            return cmd;
        }

        /// <summary>
        /// ��������ɾ����¼�����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>�����õ�����ɾ����¼�����</returns>
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

            WriteDbCommandContent("����ɾ����¼", cmd);

            return cmd;
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="entity">Ҫ���µ�ʵ�塣</param>
        /// <returns>�����õĸ������</returns>
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

            // ���� SQL ָ��
            String sql = String.Format("UPDATE {0} SET {1} WHERE {2}", schema.TableName, setListBuilder, whereBuilder);

            DbCommand cmd = Database.GetSqlStringCommand(sql);

            // ���ò���
            foreach (QueryParameter p in parameters)
            {
                Database.AddInParameter(cmd, p.Name, p.DbType, p.Value);
            }

            WriteDbCommandContent("�޸ļ�¼", cmd);

            return cmd;
        }

        /// <summary>
        /// ������ѯָ�
        /// </summary>
        /// <param name="schema">ʵ��ܹ���</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>�����õĲ�ѯָ�</returns>
        private DbCommand CreateSelectCommand(EntitySchema schema, Filter filter, Sorter sorter)
        {
            #region ǰ������

            Debug.Assert(!String.IsNullOrEmpty(schema.Composite.SelectList), "�ڼ���ʵ��ʱӦ����ѡ��һ�����ԡ�");

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

            WriteDbCommandContent("��ѯ", cmd);

            return cmd;
        }

        /// <summary>
        /// ������ѯָ�ֻ���ڴ洢���̡�
        /// </summary>
        /// <param name="schema">ʵ��ܹ���</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϡ�</param>
        /// <returns>�����õĲ�ѯָ�</returns>
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
        /// �����ۺϲ�ѯָ�
        /// </summary>
        /// <param name="functionName">�ۺϺ��������硰SUM������</param>
        /// <param name="schema">ʵ��ܹ���</param>
        /// <param name="filter">��������</param>
        /// <param name="colLocator">�ж�λ����</param>
        /// <returns>�����õľۺϲ�ѯָ�</returns>
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

            // ��ʼ���� SQL ָ��
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

            WriteDbCommandContent("�ۺϲ���", cmd);

            return cmd;
        }

        #endregion

        #region ��������

        /// <summary>
        /// �� DbCommand ������Ӳ�����
        /// </summary>
        /// <param name="cmd">���ݿ����</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϡ�</param>
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
                        // ͨ���������ò��������ݿ�����
                        SetDbTypeValue(cmd, parameter);
                    }
                }
            }
        }

        /// <summary>
        /// ִ�д洢���̡�
        /// </summary>
        /// <param name="storedProcedureName">�洢�������ơ�</param>
        /// <param name="storedProcedureParameters">�洢���̲�������������ڲ���������Ϊ null��</param>
        /// <returns>�洢����ִ�н����</returns>
        private StoredProcedureExecutionResult<Object> DoExecuteStoredProcedure(String storedProcedureName, DbStoredProcedureParameters storedProcedureParameters)
        {
            if (storedProcedureName != null)
            {
                storedProcedureName = storedProcedureName.Trim();
            }

            if (String.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName", "�洢�������Ʋ���Ϊ�ջ���ַ�����");
            }

            DbCommand cmd = Database.GetStoredProcCommand(storedProcedureName);

            AddParametersToCommand(cmd, storedProcedureParameters);

            Int32 rowsAffected = Database.ExecuteNonQuery(cmd);

            SetOutputParameterValues(storedProcedureParameters, cmd);

            StoredProcedureExecutionResult<Object> executionResult = new StoredProcedureExecutionResult<Object>(storedProcedureParameters, rowsAffected);

            return executionResult;
        }

        /// <summary>
        /// ��ȡ�ۺ�ָ���ʵ��ܹ���ϡ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="colLocator">Ҫ�ۺϵ��С�</param>
        /// <returns>�����õ�ʵ��ܹ���ϡ�</returns>
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
        /// ��ȡʵ��ܹ���ϡ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>ʵ��ܹ���ϡ�</returns>
        private static EntitySchemaComposite GetSchemaComposite(Type entityType)
        {
            return GetSchemaComposite(entityType, (CompositeBuilderStrategy)null, (Filter)null, (Sorter)null);
        }

        /// <summary>
        /// ��ȡʵ��ܹ���ϡ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>ʵ��ܹ���ϡ�</returns>
        private static EntitySchemaComposite GetSchemaComposite(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter)
        {
            return GetSchemaComposite(entityType, loadStrategy, filter, sorter, (AssemblyPolicy)null);
        }

        /// <summary>
        /// ��ȡʵ��ܹ���ϡ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>ʵ��ܹ���ϡ�</returns>
        private static EntitySchemaComposite GetSchemaComposite(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
        {
            #region ǰ������

            Debug.Assert((entityType != null), "ʵ�����Ͳ��� entityType ����Ϊ�ա�");

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
        /// ��ȡ���ڵõ���ʶֵ�� SQL ָ�
        /// </summary>
        /// <param name="definition">ʵ�嶨�塣</param>
        /// <returns>SQL ָ�</returns>
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
        /// ���û��ṩ��ֵת��Ϊ���ݿ�ֵ�����ṩֵΪ null ��δ��ʼ���� DateTime���� DateTime.MinValue��ʱ��ת��Ϊ DBNull.Value���������ԭ�����ء�
        /// </summary>
        /// <param name="value">�û��ṩ��ֵ��</param>
        /// <returns>���ݿ�ֵ��</returns>
        private static Object MakeDbValue(Object value)
        {
            if ((value == null) || DbConverter.IsDateTimeAndUninitialized(value))
            {
                return DBNull.Value;
            }

            return value;
        }

        /// <summary>
        /// ����ʵ�����顣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// 
        /// <param name="results">ʵ�弯�ϡ�</param>
        /// <returns>����õ�ʵ�����ݡ�</returns>
        private Object[] MakeEntityArray(Type entityType, List<Object> results)
        {
            Object[] entities = (Object[])Array.CreateInstance(entityType, results.Count);

            results.CopyTo(entities);

            return entities;
        }

        /// <summary>
        /// Ϊ���ݿ�����Ĳ����������ݿ����ͣ����ô˷���ǰҪ�����ݿ�����Ĳ����������Ѱ����˲�����
        /// </summary>
        /// <param name="cmd">���ݿ����</param>
        /// <param name="parameterInfo">������Ϣ��</param>
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
                throw new InvalidOperationException(String.Format("���ݿ� {0}�����ݿ��ṩ��Ϊ {1}����֧�ֲ��������ݿ����� {2}��", Database.ConnectionStringWithoutCredentials, Database.DbProviderFactory.ToString(), parameterInfo.DbTypePropertyName));
            }

            Object parameterValue = Enum.Parse(dbTypeProperty.PropertyType, parameterInfo.DbTypePropertyValue.ToString());

            dbTypeProperty.SetValue(parameter, parameterValue, null);
        }

        /// <summary>
        /// ���ô洢�������������ֵ��
        /// </summary>
        /// <param name="storedProcedureParameters">�洢���̲������ϡ�</param>
        /// <param name="cmd">���ݿ����</param>
        private void SetOutputParameterValues(DbStoredProcedureParameters storedProcedureParameters, DbCommand cmd)
        {
            if (storedProcedureParameters != null)
            {
                // ��ȡ�������������
                storedProcedureParameters.SetNonInputParameterValues(
                        delegate(String parameterName)
                        {
                            return Database.GetParameterValue(cmd, parameterName);
                        }
                    );
            }
        }

        #region ���Է���

        /// <summary>
        /// ��������ı��Ͳ�����Ϣд����־�Թ����ԡ�
        /// </summary>
        /// <param name="operation">�������ơ�</param>
        /// <param name="cmd">���</param>
        [Conditional("DEBUG")]
        private void WriteDbCommandContent(String operation, DbCommand cmd)
        {
            WriteDbCommandContent(operation, cmd, this.ParameterPrefix);
        }

        /// <summary>
        /// ��������ı��Ͳ�����Ϣд����־�Թ����ԡ�
        /// </summary>
        /// <param name="operation">�������ơ�</param>
        /// <param name="cmd">���</param>
        /// <param name="parameterPrefix">��ѯ����ǰ׺��</param>
        [Conditional("DEBUG")]
        private static void WriteDbCommandContent(String operation, DbCommand cmd, String parameterPrefix)
        {
            if (cmd == null)
            {
                Util.Log.TraceWarning("{0}��δ��������", operation);

                return;
            }

            String[] headers = new String[] { "����", "����", "ֵ" };

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

            // ��ʽ��
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
                        "{0}�������ı���{1}{1}{2}{1}{1}{1}���������{1}{1}{3}{1}{1}��ʽ�������ı��������ֽڻ��ı����ͣ�����ֵ�ض�Ϊ 100����{1}{4}",
                        operation,
                        Environment.NewLine,
                        cmd.CommandText,
                        writer.ToString(),
                        formattedStatement
                );
        }

        /// <summary>
        /// ��ȡ����ֵ���ϣ���Ϊ�������ƣ�ֵΪ����ֵ���ַ�����ʽ��
        /// </summary>
        /// <param name="cmd">���ݿ����</param>
        /// <returns>�ֵ䡣</returns>
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
        /// ���������Ľ��д����־�Թ����ԡ�
        /// </summary>
        /// <param name="compilationResult">��������������</param>
        [Conditional("DEBUG")]
        private static void WriteWhereClause(FilterCompilationResult compilationResult)
        {
            if (compilationResult == null || String.IsNullOrEmpty(compilationResult.WhereClause))
            {
                return;
            }

            Util.Log.TraceInformation("����������{0}", compilationResult.WhereClause);
        }

        /// <summary>
        /// ��ʵ��ܹ�д����־�Թ����ԡ�
        /// </summary>
        /// <param name="composite">ʵ��ܹ���ϡ�</param>
        [Conditional("DEBUG")]
        private static void WriteSchemaComposite(EntitySchemaComposite composite)
        {
            Util.Log.TraceInformation(
                        "ʵ��ܹ���ϣ�{0}������ϸ��Ϣ:\r\n{1}\r\n���ɲ��ԣ�{2}",
                        composite.Target.Type.Name,
                        composite.Dump(),
                        composite.BuilderStrategy.Dump()
                );
        }

        /// <summary>
        /// ��������Ϣд����־�Թ����ԡ�
        /// </summary>
        /// <param name="displayName">��ʾ���ơ�</param>
        /// <param name="provider">������Ϣ�ṩ�ߡ�</param>
        [Conditional("DEBUG")]
        private static void WriteDebugInfo(String displayName, IDebugInfoProvider provider)
        {
            Util.Log.TraceInformation(displayName + "��\r\n" + provider.Dump());
        }

        #endregion

        #endregion
    }
}