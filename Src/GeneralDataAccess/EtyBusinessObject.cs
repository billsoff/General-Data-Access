#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EtyBusinessObject.cs
// �ļ���������������ʵ��Ļ��ࡣ
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110406
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// ����ʵ��Ļ��ࡣ
    /// </summary>
    [Serializable]
    public abstract class EtyBusinessObject : IDbEntity, IDebugInfoProvider
    {
        #region ��̬��Ա

        #region ��������

        /// <summary>
        /// ���ʵ��ı���㡣
        /// </summary>
        /// <param name="entity">ʵ�塣</param>
        public static void ClearEntitySavePoint(Object entity)
        {
            EtyBusinessObject bo = entity as EtyBusinessObject;

            if (bo != null)
            {
                bo.ClearSavePoint();
            }
        }

        /// <summary>
        /// ����������ʵ�����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>����õ�ʵ�����</returns>
        public static TEntity CreateCompletely<TEntity>(Type entityType) where TEntity : EtyBusinessObject, new()
        {
            TEntity result = new TEntity();

            result.IsPartially = false;
            SetDbEnttityStatusOnCommitSuccess(result);

            return result;
        }

        /// <summary>
        /// ���첻������ʵ�����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>����õ�ʵ�����</returns>
        public static TEntity CreatePartially<TEntity>(Type entityType) where TEntity : EtyBusinessObject, new()
        {
            TEntity result = new TEntity();

            result.IsPartially = true;
            SetDbEnttityStatusOnCommitSuccess(result);

            return result;
        }

        /// <summary>
        /// ָʾ���������Ķ���
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>����õ�ʵ�壬���ڲ�״ָ̬ʾ��ȡ������ֵ��</returns>
        public static Object CreateCompletely(Type entityType)
        {
            return CreatePersistObject(entityType, false);
        }

        /// <summary>
        /// ָʾ���첿�֣�����ֵ�����ã���
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <returns>����õĶ������ڲ�״ָ̬ʾֻȡ���˼�ֵ��</returns>
        public static Object CreatePartially(Type entityType)
        {
            return CreatePersistObject(entityType, true);
        }

        /// <summary>
        /// �жϸ�����ʵ���Ƿ�Ϊ���ּ��ء�
        /// </summary>
        /// <param name="entity">Ҫ�жϵ�ʵ�塣</param>
        /// <returns>���ʵ��Ϊ���ּ��أ��򷵻� true�����򷵻� true��</returns>
        public static Boolean IsBusinessObjectPartially(Object entity)
        {
            Debug.Assert((entity != null), "ʵ����� entity ����Ϊ�ա�");

            EtyBusinessObject bo = entity as EtyBusinessObject;

            if (bo != null)
            {
                return bo.IsPartially;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ��ʵ��־û��ɹ�������ʵ���״̬��
        /// </summary>
        /// <param name="entity">Ҫ���õ�ʵ���״̬��</param>
        public static void SetDbEnttityStatusOnCommitSuccess(Object entity)
        {
            SetDbEnttityStatusOnCommitSuccess(entity, false);
        }

        /// <summary>
        /// ��ʵ��־û��ɹ�������ʵ���״̬��
        /// </summary>
        /// <param name="entity">Ҫ���õ�ʵ���״̬��</param>
        /// <param name="deleted">ָʾ�Ƿ���ɾ��ʵ�塣</param>
        public static void SetDbEnttityStatusOnCommitSuccess(Object entity, Boolean deleted)
        {
            IDbEntity e = entity as IDbEntity;

            if (e != null)
            {
                SetDbEnttityStatusOnCommitSuccess(e, deleted);
            }
        }

        /// <summary>
        /// ��ʵ��־û��ɹ�������ʵ���״̬��
        /// </summary>
        /// <param name="entity">Ҫ���õ�ʵ���״̬��</param>
        public static void SetDbEnttityStatusOnCommitSuccess(IDbEntity entity)
        {
            SetDbEnttityStatusOnCommitSuccess(entity, false);
        }

        /// <summary>
        /// ��ʵ��־û��ɹ�������ʵ���״̬��
        /// </summary>
        /// <param name="entity">Ҫ���õ�ʵ���״̬��</param>
        /// <param name="deleted">ָʾ�Ƿ���ɾ��ʵ�塣</param>
        public static void SetDbEnttityStatusOnCommitSuccess(IDbEntity entity, Boolean deleted)
        {
            if (entity == null)
            {
                return;
            }

            // ����Ϊ�־ö���
            entity.Transient = false;

            // �����Ǹɾ���
            entity.Dirty = false;

            if (deleted)
            {
                entity.RequireDelete = true;
            }

            if (entity.RequireDelete)
            {
                entity.Deleted = true;
            }

            // ��������
            EtyBusinessObject.ClearEntitySavePoint(entity);
        }

        /// <summary>
        /// ����ʵ������ԣ��ڱ��浽���ݺ���á�
        /// </summary>
        /// <param name="dbEntities">���ݿ�ʵ�弯�ϡ�</param>
        public static void SetDbEntitiesStatusOnCommitSuccess(IList<IDbEntity> dbEntities)
        {
            if (dbEntities == null)
            {
                return;
            }

            foreach (IDbEntity e in dbEntities)
            {
                EtyBusinessObject.SetDbEnttityStatusOnCommitSuccess(e);
            }
        }

        /// <summary>
        /// ��ʱ�����ӳټ��ء�
        /// <para>Ӧ�����ô˷��������صĽ������ using ���У��������뿪 using ��ʱ���Զ��ָ�ʵ��� SuppressLazyLoad ����ԭ�ȵ�ֵ��</para>
        /// </summary>
        /// <param name="entity">ʵ�塣</param>
        /// <returns>IDisposable ʵ�壬��������ʱʵ��� SuppressLazyLoad ֵ���ָ���</returns>
        public static IDisposable SuppressEntityLazyLoadTransient(Object entity)
        {
            return new EntitySuppressLazyLoadingTransient(entity);
        }

        /// <summary>
        /// ��ʱ���� EtyBusinessObject �ӳټ��ء�
        /// <para>Ӧ�����ô˷��������صĽ������ using ���У��������뿪 using ��ʱ���Զ��ָ�ʵ��� SuppressLazyLoad ����ԭ�ȵ�ֵ��</para>
        /// </summary>
        /// <param name="bo">ʵ�塣</param>
        /// <returns>IDisposable ʵ�壬��������ʱʵ��� SuppressLazyLoad ֵ���ָ���</returns>
        public static IDisposable SuppressEntityLazyLoadTransient(EtyBusinessObject bo)
        {
            return new EntitySuppressLazyLoadingTransient(bo);
        }

        #endregion

        #region ˽�з���

        /// <summary>
        /// �����־û�����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        /// <param name="partially">ָʾ�Ƿ�Ϊ���ּ��ء�</param>
        /// <returns>�����õĳ־û�����</returns>
        private static Object CreatePersistObject(Type entityType, Boolean partially)
        {
            Debug.Assert((entityType != null), "ʵ�����Ͳ���Ϊ�ա�");

            DbEntityPropertyInfo info = DbEntityPropertyInfoCache.GetProperty(entityType);

            Debug.Assert((info.Constructor != null), String.Format("ʵ������ {0} �����ṩ�޲ι��캯����", entityType.FullName));

            Object result = info.Constructor.Invoke(null);

            EtyBusinessObject bo = result as EtyBusinessObject;

            if (bo != null)
            {
                bo.IsPartially = partially;
                SetDbEnttityStatusOnCommitSuccess(bo);
            }

            return result;
        }

        #endregion

        #endregion

        #region ˽���ֶ�

        private Boolean m_deleted;
        private Boolean m_dirty;
        private Boolean m_requireDelete;
        private Boolean m_transient;

        private Boolean m_isPartially;
        private Boolean m_suppressLazyLoad;

        private EntityDataBag m_dataBag;

        // ���༯�ϣ���Ϊ�������ƣ���ת��Ϊȫ����д
        private Dictionary<String, Object[]> m_allChildren;

        [NonSerialized]
        private IDatabaseSession m_databaseSession;

        [NonSerialized]
        private DbEntityPropertyInfo m_dbEntityPropertyInfo;

        #endregion

        #region ���캯��

        /// <summary>
        /// Ĭ�Ϲ��캯����
        /// </summary>
        protected EtyBusinessObject()
        {
            InitializeStatus();
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ���ݿ�ִ�����档
        /// </summary>
        public IDatabaseSession DatabaseSession
        {
            get
            {
                return m_databaseSession;
            }

            internal protected set
            {
                m_databaseSession = value;
            }
        }

        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾʵ������Ƿ��Ѵ����ݿ�ɾ����Ĭ��ֵΪ false��
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">�� Transient Ϊ true��������Ϊ true��</exception>
        public Boolean Deleted
        {
            get { return m_deleted; }

            set
            {
                // ���Ϊ��������򲻿�����Ϊ true
                if (m_transient && value)
                {
                    throw new ArgumentOutOfRangeException("value", value, "����Ϊ����״̬��Transient ����Ϊ true������������ Deleted ����Ϊ true��");
                }

                // ��������־Ϊ��ɾ�����򲻿�����Ϊ false
                Boolean isCurrentStateDeleted = m_deleted;
                Boolean isNewStateNotDeleted = !value;

                if (isCurrentStateDeleted && isNewStateNotDeleted)
                {
                    throw new ArgumentOutOfRangeException("value", value, "�����ѱ��Ϊ��ɾ�������������� Deleted ����Ϊ true��");
                }

                m_deleted = value;
            }
        }

        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾ��ǰʵ���Ƿ��ѱ��޸ģ�Ĭ��ֵΪ false��
        /// </summary>
        public Boolean Dirty
        {
            get { return m_dirty; }
            set { m_dirty = value; }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ���е���ʵ�弯���Ƿ��Ѿ����ء�
        /// </summary>
        public Boolean IsAllChildrenLoaded
        {
            get
            {
                foreach (String name in DbEntityPropertyInfo.Children)
                {
                    if (!IsChildrenLoaded(name))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰʵ���Ƿ������ݿ�Ự�С�
        /// </summary>
        public Boolean IsInDbSession
        {
            get { return (DatabaseSession != null); }
        }

        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾ�����Ƿ񲿷ּ��أ���ֻ�������������֣���
        /// </summary>
        public Boolean IsPartially
        {
            get
            {
                return m_isPartially;
            }

            internal protected set
            {
                m_isPartially = value;
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�ɷ�����ӳټ��ز�����
        /// </summary>
        public Boolean MayLazyLoad
        {
            get { return !SuppressLazyLoad && IsInDbSession && !Transient && !Deleted; }
        }

        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾ�ͻ��Ƿ���ʾɾ��ʵ�壬Ĭ��ֵΪ false��
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">�� Transient Ϊ true��������Ϊ true��</exception>
        public Boolean RequireDelete
        {
            get { return m_requireDelete; }

            set
            {
                // ���Ϊ�����������������Ϊ true
                if (m_transient && value)
                {
                    throw new ArgumentOutOfRangeException("value", value, "����Ϊ����״̬��Transient ����Ϊ true���������������� RequireDelete ����Ϊ true��");
                }

                m_requireDelete = value;
            }
        }

        /// <summary>
        /// ��ȡ������һ��ֵ��ָʾ�Ƿ���ֹ�Զ�������δ���ص����ԣ�Ĭ��Ϊ false��
        /// </summary>
        public Boolean SuppressLazyLoad
        {
            get { return m_suppressLazyLoad; }
            set { m_suppressLazyLoad = value; }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ������󣨼���û�н��г־û�����Ĭ��ֵΪ true��
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">��ǰ��״̬Ϊ false��������Ϊ true��</exception>
        public Boolean Transient
        {
            get
            {
                return m_transient;
            }

            set
            {
                // ֻ�ܴ�����״̬����Ϊ�־�״̬����֮������
                Boolean isCurrentStatePersist = !m_transient;
                Boolean isNewStateTransient = value;

                if (isCurrentStatePersist && isNewStateTransient)
                {
                    throw new ArgumentOutOfRangeException("value", value, "ʵ��� Transient ����Ϊ false���־�״̬�������ܽ�������Ϊ true������״̬����");
                }

                m_transient = value;
            }
        }

        #endregion

        #region ��������

        /// <summary>
        /// ����ǰ���������������󣬸��� Transient �� ReqiureDelete ���Ծ���Ҫִ�еĲ����������� Dirty ���ԣ���
        /// </summary>
        /// <param name="scope">������</param>
        public void AddTo(TransactionScope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope", "��������Ϊ�ա�");
            }

            if (Deleted)
            {
                throw new InvalidOperationException("�����ѱ�ɾ�������ܵ��ô˷�����");
            }

            if (Transient)
            {
                scope.Add(ActionQueryType.Add, this);
            }
            else if (RequireDelete)
            {
                scope.Add(ActionQueryType.Delete, this);
            }
            else
            {
                scope.Add(ActionQueryType.Modify, this);
            }
        }

        /// <summary>
        /// ����Դ���������ֵ����ǰ����ͬʱ���������ⲿ����ʵ���е����ԣ������еĲ�������������������
        /// <para>ע�⣬���ô˷���Ҫ���أ���Ϊ������ϣ����һЩ���Ա���ԭ����ֵ��</para>
        /// <para>�����ǰ�����ĳ���ⲿ��������Ϊ�գ�����Դ�����ж�Ӧ���ⲿ�������ԡ�</para>
        /// <para>���Դ�����е�ĳ���ⲿ��������Ϊ�գ��򲻶Ը����Խ��и��Ʋ�����</para>
        /// </summary>
        /// <param name="source">Դ����</param>
        public void CopyFrom(EtyBusinessObject source)
        {
            Debug.Assert((source.GetType() == this.GetType()), "Ҫ���Ƶ�Դ����������뵱ǰ����ͬ���޷����ơ�");

            DbEntityPropertyInfo.Copy(source, this);
        }

        /// <summary>
        /// ɾ����ǰʵ�塣
        /// </summary>
        /// <returns>���ɾ���ɹ����򷵻� true�����򷵻� false��</returns>
        public Boolean Delete()
        {
            CheckDatabaseSessionExistence();

            if (Transient)
            {
                throw new InvalidOperationException("���ܵ��ô˷�������ʵ��Ϊ�������");
            }

            if (Deleted)
            {
                throw new InvalidOperationException("���ܵ��ô˷�������ʵ���ѱ�ɾ����");
            }

            Boolean success = DatabaseSession.Delete(this);

            return success;
        }

        /// <summary>
        /// ���Serializable����
        /// </summary>
        /// <returns></returns>
        public Object DuplicateSerializableObject()
        {
            MemoryStream _ms = new MemoryStream();
            BinaryFormatter _bf = new BinaryFormatter();

            _bf.Serialize(_ms, this);

            _ms.Seek(0, SeekOrigin.Begin);

            return _bf.Deserialize(_ms);
        }

        /// <summary>
        /// �ж���ʵ�弯���Ƿ��Ѿ����ء�
        /// </summary>
        /// <param name="propertyName">�������ơ�</param>
        /// <returns>�����ʵ�弯���Ѿ����أ��򷵻� true�����򷵻� false��</returns>
        public Boolean IsChildrenLoaded(String propertyName)
        {
            String name = DbEntityPropertyInfo.GetChildPropertyName(propertyName);

            if (!HasChildren)
            {
                return false;
            }

            Object[] children;

            AllChildren.TryGetValue(name, out children);

            return (children != null);
        }

        /// <summary>
        /// ���ص�ǰʵ��ĳ־û��汾���������ӳټ������ԡ�
        /// </summary>
        /// <returns>��ǰʵ��ĳ־û��汾������Ϊ�գ��ڼ��غ������û����ܻ�ɾ����ʵ�壩��</returns>
        public EtyBusinessObject Load()
        {
            return Load(false);
        }

        /// <summary>
        /// ���ص�ǰʵ��ĳ־û��汾����ָ���Ƿ�����ӳټ������ԡ�
        /// </summary>
        /// <param name="includingLazyLoads">ָʾ�Ƿ�����ӳټ������ԡ�</param>
        /// <returns>��ǰʵ��ĳ־û��汾������Ϊ�գ��ڼ��غ������û����ܻ�ɾ����ʵ�壩��</returns>
        public EtyBusinessObject Load(Boolean includingLazyLoads)
        {
            CheckDatabaseSessionExistence();

            if (Transient)
            {
                throw new InvalidOperationException("��ǰʵ��Ϊ������󣬲��ܵ��ô˷�����");
            }

            return (EtyBusinessObject)DbEntityPropertyInfo.Load(this, DatabaseSession, includingLazyLoads);
        }

        /// <summary>
        /// ����������ʵ�弯�ϣ���ָʾ�����ʵ�弯���Ѽ����Ƿ�ǿ��ˢ�¡�
        /// </summary>
        /// <param name="forceRefresh">ָʾ�Ƿ�ǿ��ˢ�¡�</param>
        public void LoadAllChildren(Boolean forceRefresh)
        {
            foreach (String name in DbEntityPropertyInfo.Children)
            {
                LoadChildren(name, forceRefresh);
            }
        }

        /// <summary>
        /// ����ָ������ʵ�弯�ϣ���ָʾ�����ʵ�弯���Ѽ����Ƿ�ǿ��ˢ�¡�
        /// </summary>
        /// <param name="propertyName">�����������ơ�</param>
        /// <param name="forceRefresh">ָʾ�Ƿ�ǿ��ˢ�¡�</param>
        public void LoadChildren(String propertyName, Boolean forceRefresh)
        {
            CheckDatabaseSessionExistence();

            String name = DbEntityPropertyInfo.GetChildPropertyName(propertyName);

            if (IsChildrenLoaded(name) && !forceRefresh)
            {
                return;
            }

            Object[] children = DbEntityPropertyInfo.LoadChildren(propertyName, this, DatabaseSession); ;

            AllChildren[name] = children;
        }

        /// <summary>
        /// ʹ�õ�ǰʵ��ĳ־û��汾ˢ�µ�ǰʵ�壬��ˢ���ӳټ������ԡ�
        /// <para>�����ǰʵ���Լ��غ������û�ɾ����������ˢ�»�ʧ�ܣ����� false�������ô˷����Ḳ�����е��޸ġ�</para>
        /// </summary>
        /// <returns>���ˢ�³ɹ����򷵻� true�����򷵻� false��</returns>
        public Boolean Refresh()
        {
            return Refresh(false);
        }

        /// <summary>
        /// ʹ�õ�ǰʵ��ĳ־û��汾ˢ�µ�ǰʵ�壬����ָ���Ƿ�ˢ���ӳټ������ԡ�
        /// <para>�����ǰʵ���Լ��غ������û�ɾ����������ˢ�»�ʧ�ܣ����� false�������ô˷����Ḳ�����е��޸ġ�</para>
        /// </summary>
        /// <param name="includingLazyLoads">���Ϊ true����ˢ���ӳټ������ԣ����򣬲�ˢ���ӳټ������ԡ�</param>
        /// <returns>���ˢ�³ɹ����򷵻� true�����򷵻� false��</returns>
        public Boolean Refresh(Boolean includingLazyLoads)
        {
            EtyBusinessObject persist = Load(includingLazyLoads);

            // �𼶸���
            if (persist == null)
            {
                return false;
            }

            DbEntityPropertyInfo.Copy(persist, this);

            return true;
        }

        /// <summary>
        /// ���浱ǰʵ�塣
        /// </summary>
        public void Save()
        {
            CheckDatabaseSessionExistence();

            if (Deleted)
            {
                throw new InvalidOperationException("���ܵ��ô˷�������ʵ���ѱ�ɾ����");
            }

            if (Transient)
            {
                DatabaseSession.Add(this);
            }
            else
            {
                DatabaseSession.Modify(this);
            }
        }

        /// <summary>
        /// ����ʵ���״̬Ϊ�־�״̬���˸ı��ǲ�����ģ���ʵ��ɹ����浽���ݿ��к���ã���
        /// </summary>
        [Obsolete("�벻Ҫʹ�ô˷�����ֱ������ Transient ����Ϊ true��")]
        public void SetPersistent()
        {
            m_transient = false;
        }

        #region �ӳټ�����ع�

        /// <summary>
        /// �������㡣
        /// </summary>
        public void ClearSavePoint()
        {
            if (m_dataBag != null)
            {
                m_dataBag.ClearSavePoint();
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�Ƿ������˱���㡣
        /// </summary>
        public Boolean HasSavePoint
        {
            get { return DataBag.HasSavePoint; }
        }

        /// <summary>
        /// ��ȡ���Ե�ֵ��������ʾ����ǿ��ת������
        /// </summary>
        /// <typeparam name="TValue">�������͡�</typeparam>
        /// <param name="propertyName">�������ơ�</param>
        /// <returns>����ֵ��</returns>
        public TValue GetLazyLoadValue<TValue>(String propertyName)
        {
            return DataBag.GetLazyLoadValue<TValue>(propertyName);
        }

        /// <summary>
        /// �����ӳټ������Ե�ֵ��
        /// </summary>
        /// <param name="propertyName">�������ơ�</param>
        /// <param name="propertyValue">����ֵ��</param>
        public void SetLazyLoadValue(String propertyName, Object propertyValue)
        {
            DataBag.SetLazyLoadValue(propertyName, propertyValue);
        }

        /// <summary>
        /// �ж�ָ�������Ե�ֵ�Ƿ�Ϊ DBEmpty ������δ���أ���
        /// </summary>
        /// <param name="chain">��������</param>
        /// <returns>�����������δ���أ��򷵻� true�����򷵻� false��</returns>
        public Boolean IsEmpty(IPropertyChain chain)
        {
            #region ǰ������

            Debug.Assert(chain != null, "���������� chain ����Ϊ�ա�");
            Debug.Assert(chain.Type == GetType(), "��������Ŀ��ʵ�������뵱ǰʵ������Ͳ�ͬ");

            #endregion

            if (chain.IsImmediateProperty)
            {
                return IsEmpty(chain.Name);
            }
            else
            {
                Object propertyValue = chain.GetPropertyValue(this);

                return IsEmpty(propertyValue, chain.Name);
            }
        }

        /// <summary>
        /// �ж�ָ�������Ե�ֵ�Ƿ�Ϊ DBEmpty������δ���أ���
        /// </summary>
        /// <param name="propertyName">�������ơ�</param>
        /// <returns>��������Ե�ֵ��δ��ʼ�����򷵻� true�����򷵻� false��</returns>
        public Boolean IsEmpty(String propertyName)
        {
            return DataBag.IsEmpty(propertyName);
        }

        /// <summary>
        /// �ж�ʵ�������ֵ�Ƿ�Ϊ DBEmpty������δ��ʼ������ֻ���ӳټ��ص�������Ч��
        /// </summary>
        /// <param name="entity">Ҫ�жϵ�ʵ�塣</param>
        /// <param name="propertyName">�������ơ�</param>
        /// <returns>���ʵ���� EtyBusinessObject��������Ϊ�ӳټ��ص������ҵ�ǰû�б����أ��򷵻� true�����򷵻� false��</returns>
        public static Boolean IsEmpty(Object entity, String propertyName)
        {
            EtyBusinessObject bo = entity as EtyBusinessObject;

            return (bo != null) && bo.IsEmpty(propertyName);
        }

        /// <summary>
        /// �ع��������һ������㡣
        /// </summary>
        /// <returns>����������ڣ���ع��ɹ������� true�����򷵻� false��</returns>
        public Boolean Rollback()
        {
            return DataBag.Rollback();
        }

        /// <summary>
        /// ���ӳټ���������Ϊ DBEmpty��
        /// </summary>
        /// <param name="propertyName">�ӳټ����������ơ�</param>
        public void SetEmpty(String propertyName)
        {
            DataBag.SetEmpty(propertyName);
        }

        /// <summary>
        /// ���ñ���㡣
        /// </summary>
        public void SetSavePoint()
        {
            DataBag.SetSavePoint();
        }

        #endregion

        #endregion

        #region ����������

        #endregion

        #region �������鷽��

        /// <summary>
        /// ��������������ִ�д˷���������ʵ������ȷ���ö����״̬�ͱ��Ϊ AutoGenerateOnNewAttribute ���������Ե�ֵ��
        /// </summary>
        internal protected virtual void OnCreateNew()
        {
            InitializeStatus();

            SetPrimaryKeyValue();
        }

        /// <summary>
        /// ��������ֵ����ǰ��ʵ��������һ�� GUID ����
        /// </summary>
        /// <returns>����ֵ��</returns>
        protected virtual Object GeneratePrimaryKeyValue()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        #region �����ķ���

        /// <summary>
        /// ȷ��ʵ���������ء�
        /// </summary>
        /// <returns>����ɹ�ִ�д˷���������ʵ���Ѿ����������ݿ�Ự���桢״̬��ȷ����������Ϊ��ɾ�������� SuppressNavigate Ϊ false������ true�����򷵻� fasle��</returns>
        protected Boolean EnsureCompletelyLoaded()
        {
            if ((DatabaseSession == null) || Transient || Deleted || SuppressLazyLoad)
            {
                return false;
            }
            else if (IsPartially)
            {
                Refresh();

                // ��һ����ʡ�ԣ����ڸ���ֵʱ���Զ�����
                IsPartially = false;
            }

            return true;
        }

        /// <summary>
        /// ��ȡ���༯�ϣ��� SuppressNavigate Ϊ true ʱ����������ȥ������ʵ�弯�ϡ�
        /// </summary>
        /// <typeparam name="TEntity">����ʵ�����͡�</typeparam>
        /// <param name="propertyName">�������ơ�</param>
        /// <returns>���༯�ϡ�</returns>
        protected TEntity[] GetChildren<TEntity>(String propertyName) where TEntity : EtyBusinessObject, new()
        {
            if (MayLazyLoad)
            {
                LoadChildren(propertyName, false);
            }

            String name = propertyName;
            Object[] children = null;

            if (HasChildren)
            {
                AllChildren.TryGetValue(name, out children);
            }

            if (children == null)
            {
                children = new TEntity[0];
            }

            return (TEntity[])children;
        }

        /// <summary>
        /// ������ʵ�弯�ϡ�
        /// </summary>
        /// <param name="propertyName">��ʵ�弯���������ơ�</param>
        /// <param name="children">Ҫ���õ���ʵ���б�</param>
        protected void SetChildren(String propertyName, Object[] children)
        {
            AllChildren[propertyName] = children;
        }

        /// <summary>
        /// ���־û��ɹ�������ʵ���״̬��
        /// </summary>
        internal protected void SetDbEntityStatusOnCommitSuccess()
        {
            // ����Ϊ�־ö���
            Transient = false;

            // �����Ǹɾ���
            Dirty = false;

            if (RequireDelete)
            {
                Deleted = true;
            }
        }

        #endregion

        #region ˽�е�����

        /// <summary>
        /// ��ȡ���е���ʵ�弯�ϡ�
        /// </summary>
        private Dictionary<String, Object[]> AllChildren
        {
            get
            {
                if (m_allChildren == null)
                {
                    m_allChildren = new Dictionary<String, Object[]>();
                }

                return m_allChildren;
            }
        }

        /// <summary>
        /// ��ȡ���ݰ���֧���ӳټ��غͻع���
        /// </summary>
        private EntityDataBag DataBag
        {
            get
            {
                if (m_dataBag == null)
                {
                    m_dataBag = new EntityDataBag(this);
                }

                return m_dataBag;
            }
        }

        /// <summary>
        /// ��ȡʵ��������Ϣ��
        /// </summary>
        private DbEntityPropertyInfo DbEntityPropertyInfo
        {
            get
            {
                if (m_dbEntityPropertyInfo == null)
                {
                    m_dbEntityPropertyInfo = DbEntityPropertyInfoCache.GetProperty(GetType());
                }

                return m_dbEntityPropertyInfo;
            }
        }

        /// <summary>
        /// �ж��Ƿ������༯�ϱ����ء�
        /// </summary>
        private Boolean HasChildren
        {
            get
            {
                return ((m_allChildren != null) && (m_allChildren.Count != 0));
            }

        }

        #endregion

        #region ��������

        /// <summary>
        /// ������ݿ�Ự�����Ƿ���ڣ���������ڣ����׳� InvalidOperationException �쳣��
        /// </summary>
        private void CheckDatabaseSessionExistence()
        {
            if (DatabaseSession == null)
            {
                throw new InvalidOperationException("���ܵ��õ�ǰ���������ʵ��û�и������ݿ�Ự���档");
            }
        }

        /// <summary>
        /// ��ʼ�������״̬��
        /// </summary>
        private void InitializeStatus()
        {
            m_deleted = false;
            m_requireDelete = false;
            m_dirty = false;

            // Ĭ������������
            m_isPartially = false;

            m_transient = true;
        }

        /// <summary>
        /// Ϊ��ǰʵ����������ֵ��
        /// </summary>
        private void SetPrimaryKeyValue()
        {
            PropertyInfo[] primaryKeys = DbEntityPropertyInfo.PrimaryKeys;

            if (primaryKeys.Length != 1)
            {
                return;
            }

            PropertyInfo pf = primaryKeys[0];

            if (!Attribute.IsDefined(pf, typeof(AutoGenerateOnNewAttribute)))
            {
                return;
            }

            if (!pf.CanWrite)
            {
                throw new InvalidOperationException(
                        String.Format("�޷�Ϊ���� {0}.{1} ��������ֵ����Ϊ�����Բ���д��",
                                DbEntityPropertyInfo.Type.FullName,
                                pf.Name
                            )
                    );
            }

            Object primaryKeyValue = GeneratePrimaryKeyValue();

            pf.SetValue(this, primaryKeyValue, null);
        }

        #endregion

        #region IDebugInfoProvider ��Ա

        /// <summary>
        /// ʵ�����ϸ��Ϣ�����ڵ��ԡ�
        /// </summary>
        public String Dump()
        {
            return DbEntityDebugger.Dump(this);
        }

        /// <summary>
        /// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public String Dump(String indent)
        {
            return DbEntityDebugger.IndentText(Dump(), indent);
        }

        /// <summary>
        /// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public String Dump(Int32 level)
        {
            return DbEntityDebugger.IndentText(Dump(), level);
        }

        /// <summary>
        /// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public String Dump(String indent, Int32 level)
        {
            return DbEntityDebugger.IndentText(Dump(), indent, level);
        }

        #endregion
    }
}