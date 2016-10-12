#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：BatchModificationSchema.cs
// 文件功能描述：批量修改架构信息。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110228
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 批量修改的架构信息。
    /// </summary>
    internal class BatchModificationSchema
    {
        #region 私有字段

        private readonly Type m_entityType;

        private readonly DataTable m_dataSource;

        private readonly DbCommand m_insertCommand;
        private readonly DbCommand m_updateCommand;
        private readonly DbCommand m_deleteCommand;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造信息，设置架构信息。。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="dataSource">数据源表。</param>
        /// <param name="insertCommand">Insert 命令。</param>
        /// <param name="updateCommand">Update 命令。</param>
        /// <param name="deleteCommand">Delete 命令。</param>
        public BatchModificationSchema(Type entityType, DataTable dataSource, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "实体类型参数不能为空。");
            }

            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource", "数据表架构信息不能为空。");
            }

            m_entityType = entityType;

            m_dataSource = dataSource;

            m_insertCommand = insertCommand;
            m_updateCommand = updateCommand;
            m_deleteCommand = deleteCommand;
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 复制当前对象。
        /// </summary>
        /// <param name="db">数据库。</param>
        /// <returns>当前对象的副本。</returns>
        public BatchModificationSchema Clone(Database db)
        {
            DataTable dataSource = m_dataSource.Clone();

            DbCommand insertCommand = CloneDbCommand(m_insertCommand, db);
            DbCommand updateCommand = CloneDbCommand(m_updateCommand, db);
            DbCommand deleteCommand = CloneDbCommand(m_deleteCommand, db);

            BatchModificationSchema clone = new BatchModificationSchema(m_entityType, dataSource, insertCommand, updateCommand, deleteCommand);

            return clone;
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取实体类型。
        /// </summary>
        public Type EntityType
        {
            get { return m_entityType; }
        }

        /// <summary>
        /// 获取数据表。
        /// </summary>
        public DataTable DataSource
        {
            get { return m_dataSource; }
        }

        /// <summary>
        /// 获取 Insert 命令。
        /// </summary>
        public DbCommand InsertCommand
        {
            get { return m_insertCommand; }
        }

        /// <summary>
        /// 获取 Update 命令。
        /// </summary>
        public DbCommand UpdateCommand
        {
            get { return m_updateCommand; }
        }

        /// <summary>
        /// 获取 Delete 命令。
        /// </summary>
        public DbCommand DeleteCommand
        {
            get { return m_deleteCommand; }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 复制数据库命令。
        /// </summary>
        /// <param name="cmd">数据库命令。</param>
        /// <param name="db">数据库。</param>
        /// <returns>复制好的命令。</returns>
        private static DbCommand CloneDbCommand(DbCommand cmd, Database db)
        {
            if (cmd == null)
            {
                return null;
            }

            DbCommand clone = db.DbProviderFactory.CreateCommand();

            clone.CommandText = cmd.CommandText;
            clone.CommandType = cmd.CommandType;

            foreach (DbParameter parameter in cmd.Parameters)
            {
                DbParameter cloneParameter = db.DbProviderFactory.CreateParameter();

                cloneParameter.ParameterName = parameter.ParameterName;
                cloneParameter.DbType = parameter.DbType;

                cloneParameter.Size = parameter.Size;
                cloneParameter.Direction = parameter.Direction;

                cloneParameter.IsNullable = parameter.IsNullable;

                ((IDbDataParameter)cloneParameter).Precision = ((IDbDataParameter)parameter).Precision;
                ((IDbDataParameter)cloneParameter).Scale = ((IDbDataParameter)parameter).Scale;

                cloneParameter.SourceColumn = parameter.SourceColumn;
                cloneParameter.SourceVersion = parameter.SourceVersion;

                clone.Parameters.Add(cloneParameter);
            }

            return clone;
        }

        #endregion
    }
}