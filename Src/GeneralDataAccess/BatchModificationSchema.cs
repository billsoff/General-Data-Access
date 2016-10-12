#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����BatchModificationSchema.cs
// �ļ����������������޸ļܹ���Ϣ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110228
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// �����޸ĵļܹ���Ϣ��
    /// </summary>
    internal class BatchModificationSchema
    {
        #region ˽���ֶ�

        private readonly Type m_entityType;

        private readonly DataTable m_dataSource;

        private readonly DbCommand m_insertCommand;
        private readonly DbCommand m_updateCommand;
        private readonly DbCommand m_deleteCommand;

        #endregion

        #region ���캯��

        /// <summary>
        /// ������Ϣ�����üܹ���Ϣ����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="dataSource">����Դ��</param>
        /// <param name="insertCommand">Insert ���</param>
        /// <param name="updateCommand">Update ���</param>
        /// <param name="deleteCommand">Delete ���</param>
        public BatchModificationSchema(Type entityType, DataTable dataSource, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType", "ʵ�����Ͳ�������Ϊ�ա�");
            }

            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource", "���ݱ�ܹ���Ϣ����Ϊ�ա�");
            }

            m_entityType = entityType;

            m_dataSource = dataSource;

            m_insertCommand = insertCommand;
            m_updateCommand = updateCommand;
            m_deleteCommand = deleteCommand;
        }

        #endregion

        #region ���з���

        /// <summary>
        /// ���Ƶ�ǰ����
        /// </summary>
        /// <param name="db">���ݿ⡣</param>
        /// <returns>��ǰ����ĸ�����</returns>
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

        #region ��������

        /// <summary>
        /// ��ȡʵ�����͡�
        /// </summary>
        public Type EntityType
        {
            get { return m_entityType; }
        }

        /// <summary>
        /// ��ȡ���ݱ�
        /// </summary>
        public DataTable DataSource
        {
            get { return m_dataSource; }
        }

        /// <summary>
        /// ��ȡ Insert ���
        /// </summary>
        public DbCommand InsertCommand
        {
            get { return m_insertCommand; }
        }

        /// <summary>
        /// ��ȡ Update ���
        /// </summary>
        public DbCommand UpdateCommand
        {
            get { return m_updateCommand; }
        }

        /// <summary>
        /// ��ȡ Delete ���
        /// </summary>
        public DbCommand DeleteCommand
        {
            get { return m_deleteCommand; }
        }

        #endregion

        #region ��������

        /// <summary>
        /// �������ݿ����
        /// </summary>
        /// <param name="cmd">���ݿ����</param>
        /// <param name="db">���ݿ⡣</param>
        /// <returns>���ƺõ����</returns>
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