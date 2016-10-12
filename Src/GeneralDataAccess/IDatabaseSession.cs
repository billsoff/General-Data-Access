#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IDatabaseSession.cs
// �ļ�������������ʾ���ݿ������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20100906
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// ��ʾ���ݿ������
    /// </summary>
    public interface IDatabaseSession
    {
        #region ������ѯ

        #region �Ƿ���

        /// <summary>
        /// �����ݿ����ʵ�壬��������Ϊ DateTime ��ֵΪ DateTime.MinValue �����ԡ�
        /// </summary>
        /// <param name="entity">Ҫ��ӵ�ʵ�塣</param>
        void Add(Object entity);

        /// <summary>
        /// ���ʵ�壬����ȡ��ʶֵ��
        /// </summary>
        /// <param name="entity">ʵ�塣</param>
        /// <param name="idValue">��ʶֵ����������ڱ�ʶֵ���򷵻� null��</param>
        void Add(Object entity, out Object idValue);

        /// <summary>
        /// �ύ���ݿ�����
        /// </summary>
        /// <param name="scope">�����򣬷�װ��Ҫ���в���������ʵ�塣</param>
        void CommitTransaction(TransactionScope scope);

        /// <summary>
        /// ɾ��ʵ�塣
        /// </summary>
        /// <param name="entity">Ҫɾ����ʵ�塣</param>
        /// <returns>���ɾ���ɹ����򷵻� true�����򷵻� false��</returns>
        Boolean Delete(Object entity);

        /// <summary>
        /// ɾ�����м�¼��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        void DeleteAll(Type entityType);

        /// <summary>
        /// ����ɾ����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        void DeleteBatch(Type entityType, Filter filter);

        /// <summary>
        /// �޸�ʵ�壬��������Ϊ DateTime ��ֵΪ DateTime.MinValue �����ԡ�
        /// </summary>
        /// <param name="entity">Ҫ�޸ĵ�ʵ�塣</param>
        void Modify(Object entity);

        #region �����޸�

        /// <summary>
        /// �����޸ģ�ʹ���½������ݿ����ӣ�UpdateBehavior ��ֵΪ Continue��������������ʱ����������Ը��������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ���µ�ʵ�弯�ϡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        Int32 ModifyBatch(IList<IDbEntity> entities);

        /// <summary>
        /// �����޸ģ�ʹ�õ��÷��ṩ�����ݿ����ӣ�UpdateBehavior ��ֵΪ Continue��������������ʱ����������Ը��������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ���µ�ʵ�弯�ϡ�</param>
        /// <param name="connection">���ݿ����ӣ� ���Ϊ�գ����½�һ�����ӡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        Int32 ModifyBatch(IList<IDbEntity> entities, DbConnection connection);

        /// <summary>
        /// �Ա�׼�ķ��������޸ģ�������������ʱ�������������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        Int32 ModifyBatchStandard(IList<IDbEntity> entities);

        /// <summary>
        /// �Ա�׼�ķ�ʽ�����޸ģ�������������ʱ�������������ʵ�塣
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <param name="connection">Ҫʹ�õ����ݿ����ӡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        Int32 ModifyBatchStandard(IList<IDbEntity> entities, DbConnection connection);

        /// <summary>
        /// ������ķ�ʽ�����޸ģ�������������ʱ���ع����е��޸ġ�
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <returns>ʵ���޸ĵ�ʵ������</returns>
        Int32 ModifyBatchTransactional(IList<IDbEntity> entities);

        /// <summary>
        /// ������ķ�ʽ�����޸ģ�������������ʱ���ع����е��޸ġ�
        /// </summary>
        /// <param name="entities">Ҫ�����޸ĵ�ʵ�弯�ϡ�</param>
        /// <param name="connection">Ҫʹ�õ����ݿ����ӡ�</param>
        /// <returns>ʵ�ʸ��µ�ʵ������</returns>
        Int32 ModifyBatchTransactional(IList<IDbEntity> entities, DbConnection connection);

        #endregion

        #region ִ�д洢����

        /// <summary>
        /// ִ�д洢���̡�
        /// </summary>
        /// <param name="storedProcedureName">�洢�������ơ�</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName);

        /// <summary>
        /// ִ�д洢���̡�
        /// </summary>
        /// <param name="storedProcedureParameters">�洢���̲�������������ڲ���������Ϊ null��</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(DbStoredProcedureParameters storedProcedureParameters);

        /// <summary>
        /// ִ�д洢���̡�
        /// </summary>
        /// <param name="storedProcedureName">�洢�������ơ�</param>
        /// <param name="storedProcedureParameters">�洢���̲�������������ڲ���������Ϊ null��</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<Object> ExecuteStoredProcedure(String storedProcedureName, DbStoredProcedureParameters storedProcedureParameters);

        #endregion

        #endregion

        #region ����

        /// <summary>
        /// ɾ�����м�¼��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        void DeleteAll<TEntity>() where TEntity : class;

        /// <summary>
        /// ����ɾ����
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        void DeleteBatch<TEntity>(Filter filter) where TEntity : class;

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
        Object[] Load(Type entityType);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, Filter filter);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, Sorter sorter);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, Filter filter, Sorter sorter);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>ȫ��ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter);

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
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

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
        Object[] Load(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount);

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters);

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<Object> Load(Type entityType, DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount);

        #endregion

        #endregion

        #region ����

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <returns>ȫ��ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>() where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(Filter filter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�գ���� startIndex С����� recordCount С�� 1����ȡȫ�����Ϲ�����������ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>ȫ��ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new();

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
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, Int32 startIndex, Int32 recordCount)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(Filter filter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н���������ʵ�弯�ϡ�</returns>
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy)
                   where TEntity : class, new();

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
        TEntity[] Load<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy, Int32 startIndex, Int32 recordCount)
                   where TEntity : class, new();

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м���ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="recordCount">Ҫ��ȡ�ļ�¼����</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<TEntity> Load<TEntity>(DbStoredProcedureParameters storedProcedureParameters, Int32 startIndex, Int32 recordCount) where TEntity : class, new();

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
        Object LoadFirst(Type entityType);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, Filter filter);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, Sorter sorter);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, Filter filter, Sorter sorter);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>��һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter);
        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy);

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        Object LoadFirst(Type entityType, CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy);

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<Object> LoadFirst(Type entityType, DbStoredProcedureParameters storedProcedureParameters);

        #endregion

        #endregion

        #region ����

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <returns>��һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>() where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(Filter filter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>��һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬����������Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�塣
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬���������������Կ���Ϊ�ա�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="filter">��������</param>
        /// <param name="sorter">��������</param>
        /// <param name="childrenPolicy">��ʵ��װ�䷽�롣</param>
        /// <returns>���Ϲ�����������������ָ���������н��������ĵ�һ��ʵ�塣</returns>
        TEntity LoadFirst<TEntity>(CompositeBuilderStrategy loadStrategy, Filter filter, Sorter sorter, AssemblyPolicy childrenPolicy) where TEntity : class, new();

        #region ִ�д洢����

        /// <summary>
        /// �����ݿ��м��ص�һ��ָ�����͵�ʵ�壬�˷���ֻ�����ڴ洢����ָ�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="storedProcedureParameters">�洢���̲������ϣ����û�в������� null��</param>
        /// <returns>�洢����ִ�н����</returns>
        StoredProcedureExecutionResult<TEntity> LoadFirst<TEntity>(DbStoredProcedureParameters storedProcedureParameters) where TEntity : class, new();

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
        GroupResult[] Aggregate(Type groupResultType);

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter);

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        GroupResult[] Aggregate(Type groupResultType, Sorter sorter);

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter);

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
        GroupResult[] Aggregate(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        #endregion

        #region ����

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <returns>��ѯ������ϡ�</returns>
        TGroupResult[] Aggregate<TGroupResult>()
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        TGroupResult[] Aggregate<TGroupResult>(Sorter sorter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// �����ѯ��
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ������ϡ�</returns>
        TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter)
                   where TGroupResult : GroupResult, new();

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
        TGroupResult[] Aggregate<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
                   where TGroupResult : GroupResult, new();

        #endregion

        #endregion

        #region AggregateOne

        #region �Ƿ���

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <returns>��ѯ�����</returns>
        GroupResult AggregateOne(Type groupResultType);

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ�����</returns>
        GroupResult AggregateOne(Type groupResultType, Filter whereFilter, Filter havingFilter);

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
        GroupResult AggregateOne(Type groupResultType, Sorter sorter);

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <param name="groupResultType">������ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
        GroupResult AggregateOne(Type groupResultType, Filter whereFilter, Filter havingFilter, Sorter sorter);

        #endregion

        #region ����

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <returns>��ѯ�����</returns>
        TGroupResult AggregateOne<TGroupResult>()
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <returns>��ѯ�����</returns>
        TGroupResult AggregateOne<TGroupResult>(Filter whereFilter, Filter havingFilter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
        TGroupResult AggregateOne<TGroupResult>(Sorter sorter)
                   where TGroupResult : GroupResult, new();

        /// <summary>
        /// �����ѯ����ȡһ�������
        /// </summary>
        /// <typeparam name="TGroupResult">������ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="havingFilter">HAVING ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>��ѯ�����</returns>
        TGroupResult AggregateOne<TGroupResult>(Filter whereFilter, Filter havingFilter, Sorter sorter)
                   where TGroupResult : GroupResult, new();

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
        Object Avg(Type entityType, String propertyName);

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻� null��</returns>
        Object Avg(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻� null��</returns>
        Object Avg(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻� null��</returns>
        Object Avg(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Avg<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Avg<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Avg<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ����ָ���е�ƽ��ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е�ƽ��ֵ��������ݿ�ֵΪ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Avg<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region COUNT

        /// <summary>
        /// �����¼����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>��������</returns>
        Int32 Count(Type entityType);

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count(Type entityType, String propertyName);

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// �����¼����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <returns>��������</returns>
        Int32 Count(Type entityType, Filter filter);

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// �����¼��������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <returns>��������</returns>
        Int32 Count<TEntity>() where TEntity : class, new();

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count<TEntity>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count<TEntity>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ������˺�ļ�¼�ĵ�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>��������</returns>
        Int32 Count<TEntity>(Filter filter) where TEntity : class, new();

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count<TEntity>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ������ָ����������ӳ����е�������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>��������</returns>
        Int32 Count<TEntity>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region MAX

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Max(Type entityType, String propertyName);

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Max(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Max(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Max(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Max<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Max<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Max<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���е����ֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е����ֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Max<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region MIN

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Min(Type entityType, String propertyName);

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Min(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Min(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Min(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Min<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Min<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Min<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���е���Сֵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�е���Сֵ�����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Min<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

        #endregion

        #region SUM

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Sum(Type entityType, String propertyName);

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Sum(Type entityType, String entityPropertyName, String propertyName);

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Sum(Type entityType, Filter filter, String propertyName);

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻� null��</returns>
        Object Sum(Type entityType, Filter filter, String entityPropertyName, String propertyName);

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Sum<TEntity, TResult>(String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Sum<TEntity, TResult>(String entityPropertyName, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Sum<TEntity, TResult>(Filter filter, String propertyName) where TEntity : class, new();

        /// <summary>
        /// ��ָ���еĺ͡�
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <typeparam name="TResult">������͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <param name="entityPropertyName">ʵ���������ơ�</param>
        /// <param name="propertyName">ֵ�������ơ�</param>
        /// <returns>�еĺͣ����Ϊ���ݿ��ֵ���򷵻�Ĭ��ֵ��</returns>
        TResult Sum<TEntity, TResult>(Filter filter, String entityPropertyName, String propertyName) where TEntity : class, new();

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
        CompositeResult[] Compose(Type compositeResultType);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, Filter whereFilter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, Sorter sorter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter);

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
        CompositeResult[] Compose(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount);

        #endregion

        #region ����

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>()
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ���ظ���ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�弯�ϡ�</returns>
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

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
        TCompositeResult[] Compose<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter, Int32 startIndex, Int32 recordCount)
                   where TCompositeResult : CompositeResult, new();

        #endregion

        #endregion

        #region ComposeOne

        #region �Ƿ���

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, Sorter sorter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Sorter sorter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeSettings settings, Filter whereFilter, Sorter sorter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter);

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <param name="compositeResultType">����ʵ�����͡�</param>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        CompositeResult ComposeOne(Type compositeResultType, CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter);

        #endregion

        #region ����

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>()
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

        /// <summary>
        /// ����һ������ʵ�塣
        /// </summary>
        /// <typeparam name="TCompositeResult">����ʵ�����͡�</typeparam>
        /// <param name="loadStrategy">���ز��ԡ�</param>
        /// <param name="settings">����ʵ��������б�</param>
        /// <param name="whereFilter">WHERE ��������</param>
        /// <param name="sorter">��������</param>
        /// <returns>����ʵ�塣</returns>
        TCompositeResult ComposeOne<TCompositeResult>(CompositeBuilderStrategy loadStrategy, CompositeSettings settings, Filter whereFilter, Sorter sorter)
                   where TCompositeResult : CompositeResult, new();

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
        Boolean Exists(Type entityType, Filter filter);

        /// <summary>
        /// �жϸ���������ʵ���Ƿ���ڣ����������Ϊ�ձ�ʾ����ȫ������
        /// </summary>
        /// <typeparam name="TEntity">ʵ�����͡�</typeparam>
        /// <param name="filter">��������</param>
        /// <returns>���ʵ����ڣ��򷵻� true�����򷵻� false��</returns>
        Boolean Exists<TEntity>(Filter filter) where TEntity : class, new();

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ�������ĵ�ǰʱ�䡣
        /// </summary>
        /// <returns>�������ĵ�ǰʱ�䡣</returns>
        DateTime Now();

        #endregion

        #region ֱ�ӷ������ݿ�

        /// <summary>
        /// �������ݿ����ӡ�
        /// </summary>
        /// <returns>�����õ����ݿ����ӡ�</returns>
        DbConnection CreateConnection();

        /// <summary>
        /// ���� Database ʵ����
        /// </summary>
        /// <returns>�����õ� Database ʵ����</returns>
        Database CreateDatabase();

        #endregion
    }
}