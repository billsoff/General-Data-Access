#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EtyBusinessObject.cs
// 文件功能描述：用作实体的基类。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110406
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

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
    /// 用作实体的基类。
    /// </summary>
    [Serializable]
    public abstract class EtyBusinessObject : IDbEntity, IDebugInfoProvider
    {
        #region 静态成员

        #region 公共方法

        /// <summary>
        /// 清除实体的保存点。
        /// </summary>
        /// <param name="entity">实体。</param>
        public static void ClearEntitySavePoint(Object entity)
        {
            EtyBusinessObject bo = entity as EtyBusinessObject;

            if (bo != null)
            {
                bo.ClearSavePoint();
            }
        }

        /// <summary>
        /// 构造完整的实体对象。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>构造好的实体对象。</returns>
        public static TEntity CreateCompletely<TEntity>(Type entityType) where TEntity : EtyBusinessObject, new()
        {
            TEntity result = new TEntity();

            result.IsPartially = false;
            SetDbEnttityStatusOnCommitSuccess(result);

            return result;
        }

        /// <summary>
        /// 构造不完整的实体对象。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>构造好的实体对象。</returns>
        public static TEntity CreatePartially<TEntity>(Type entityType) where TEntity : EtyBusinessObject, new()
        {
            TEntity result = new TEntity();

            result.IsPartially = true;
            SetDbEnttityStatusOnCommitSuccess(result);

            return result;
        }

        /// <summary>
        /// 指示构造完整的对象。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>构造好的实体，其内部状态指示已取得所有值。</returns>
        public static Object CreateCompletely(Type entityType)
        {
            return CreatePersistObject(entityType, false);
        }

        /// <summary>
        /// 指示构造部分（仅键值被设置）。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>构造好的对象，其内部状态指示只取得了键值。</returns>
        public static Object CreatePartially(Type entityType)
        {
            return CreatePersistObject(entityType, true);
        }

        /// <summary>
        /// 判断给定的实体是否为部分加载。
        /// </summary>
        /// <param name="entity">要判断的实体。</param>
        /// <returns>如果实体为部分加载，则返回 true；否则返回 true。</returns>
        public static Boolean IsBusinessObjectPartially(Object entity)
        {
            Debug.Assert((entity != null), "实体参数 entity 不能为空。");

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
        /// 当实体持久化成功后，设置实体的状态，
        /// </summary>
        /// <param name="entity">要设置的实体的状态。</param>
        public static void SetDbEnttityStatusOnCommitSuccess(Object entity)
        {
            SetDbEnttityStatusOnCommitSuccess(entity, false);
        }

        /// <summary>
        /// 当实体持久化成功后，设置实体的状态，
        /// </summary>
        /// <param name="entity">要设置的实体的状态。</param>
        /// <param name="deleted">指示是否已删除实体。</param>
        public static void SetDbEnttityStatusOnCommitSuccess(Object entity, Boolean deleted)
        {
            IDbEntity e = entity as IDbEntity;

            if (e != null)
            {
                SetDbEnttityStatusOnCommitSuccess(e, deleted);
            }
        }

        /// <summary>
        /// 当实体持久化成功后，设置实体的状态，
        /// </summary>
        /// <param name="entity">要设置的实体的状态。</param>
        public static void SetDbEnttityStatusOnCommitSuccess(IDbEntity entity)
        {
            SetDbEnttityStatusOnCommitSuccess(entity, false);
        }

        /// <summary>
        /// 当实体持久化成功后，设置实体的状态，
        /// </summary>
        /// <param name="entity">要设置的实体的状态。</param>
        /// <param name="deleted">指示是否已删除实体。</param>
        public static void SetDbEnttityStatusOnCommitSuccess(IDbEntity entity, Boolean deleted)
        {
            if (entity == null)
            {
                return;
            }

            // 对象为持久对象
            entity.Transient = false;

            // 对象是干净的
            entity.Dirty = false;

            if (deleted)
            {
                entity.RequireDelete = true;
            }

            if (entity.RequireDelete)
            {
                entity.Deleted = true;
            }

            // 清除保存点
            EtyBusinessObject.ClearEntitySavePoint(entity);
        }

        /// <summary>
        /// 重置实体的属性，在保存到数据后调用。
        /// </summary>
        /// <param name="dbEntities">数据库实体集合。</param>
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
        /// 暂时抑制延迟加载。
        /// <para>应将调用此方法所返回的结果放入 using 块中，当调用离开 using 块时会自动恢复实体的 SuppressLazyLoad 属性原先的值。</para>
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <returns>IDisposable 实体，当被处置时实体的 SuppressLazyLoad 值被恢复。</returns>
        public static IDisposable SuppressEntityLazyLoadTransient(Object entity)
        {
            return new EntitySuppressLazyLoadingTransient(entity);
        }

        /// <summary>
        /// 暂时抑制 EtyBusinessObject 延迟加载。
        /// <para>应将调用此方法所返回的结果放入 using 块中，当调用离开 using 块时会自动恢复实体的 SuppressLazyLoad 属性原先的值。</para>
        /// </summary>
        /// <param name="bo">实体。</param>
        /// <returns>IDisposable 实体，当被处置时实体的 SuppressLazyLoad 值被恢复。</returns>
        public static IDisposable SuppressEntityLazyLoadTransient(EtyBusinessObject bo)
        {
            return new EntitySuppressLazyLoadingTransient(bo);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 创建持久化对象。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="partially">指示是否为部分加载。</param>
        /// <returns>创建好的持久化对象。</returns>
        private static Object CreatePersistObject(Type entityType, Boolean partially)
        {
            Debug.Assert((entityType != null), "实体类型不能为空。");

            DbEntityPropertyInfo info = DbEntityPropertyInfoCache.GetProperty(entityType);

            Debug.Assert((info.Constructor != null), String.Format("实体类型 {0} 必须提供无参构造函数。", entityType.FullName));

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

        #region 私有字段

        private Boolean m_deleted;
        private Boolean m_dirty;
        private Boolean m_requireDelete;
        private Boolean m_transient;

        private Boolean m_isPartially;
        private Boolean m_suppressLazyLoad;

        private EntityDataBag m_dataBag;

        // 子类集合，键为属性名称，被转换为全部大写
        private Dictionary<String, Object[]> m_allChildren;

        [NonSerialized]
        private IDatabaseSession m_databaseSession;

        [NonSerialized]
        private DbEntityPropertyInfo m_dbEntityPropertyInfo;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        protected EtyBusinessObject()
        {
            InitializeStatus();
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取数据库执行引擎。
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
        /// 获取或设置一个值，该值指示实体对象是否已从数据库删除，默认值为 false。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当 Transient 为 true，设置其为 true。</exception>
        public Boolean Deleted
        {
            get { return m_deleted; }

            set
            {
                // 如果为游离对象，则不可设置为 true
                if (m_transient && value)
                {
                    throw new ArgumentOutOfRangeException("value", value, "对象为游离状态（Transient 属性为 true），不能设置 Deleted 属性为 true。");
                }

                // 如果对象标志为已删除，则不可以设为 false
                Boolean isCurrentStateDeleted = m_deleted;
                Boolean isNewStateNotDeleted = !value;

                if (isCurrentStateDeleted && isNewStateNotDeleted)
                {
                    throw new ArgumentOutOfRangeException("value", value, "对象已标记为被删除，不能再设置 Deleted 属性为 true。");
                }

                m_deleted = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示当前实体是否已被修改，默认值为 false。
        /// </summary>
        public Boolean Dirty
        {
            get { return m_dirty; }
            set { m_dirty = value; }
        }

        /// <summary>
        /// 获取一个值，该值指示所有的子实体集合是否已经加载。
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
        /// 获取一个值，该值指示当前实例是否处于数据库会话中。
        /// </summary>
        public Boolean IsInDbSession
        {
            get { return (DatabaseSession != null); }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示对象是否部分加载（即只加载了主键部分）。
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
        /// 获取一个值，该值指示可否进行延迟加载操作。
        /// </summary>
        public Boolean MayLazyLoad
        {
            get { return !SuppressLazyLoad && IsInDbSession && !Transient && !Deleted; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示客户是否请示删除实体，默认值为 false。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当 Transient 为 true，设置其为 true。</exception>
        public Boolean RequireDelete
        {
            get { return m_requireDelete; }

            set
            {
                // 如果为游离对象，则不能设置其为 true
                if (m_transient && value)
                {
                    throw new ArgumentOutOfRangeException("value", value, "对象为游离状态（Transient 属性为 true），不能设置属性 RequireDelete 属性为 true。");
                }

                m_requireDelete = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示是否阻止自动加载尚未加载的属性，默认为 false。
        /// </summary>
        public Boolean SuppressLazyLoad
        {
            get { return m_suppressLazyLoad; }
            set { m_suppressLazyLoad = value; }
        }

        /// <summary>
        /// 获取一个值，该值指示当前对象是否为游离对象（即还没有进行持久化），默认值为 true。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当前的状态为 false，设置其为 true。</exception>
        public Boolean Transient
        {
            get
            {
                return m_transient;
            }

            set
            {
                // 只能从游离状态设置为持久状态，反之则不允许
                Boolean isCurrentStatePersist = !m_transient;
                Boolean isNewStateTransient = value;

                if (isCurrentStatePersist && isNewStateTransient)
                {
                    throw new ArgumentOutOfRangeException("value", value, "实体的 Transient 属性为 false（持久状态），不能将它设置为 true（游离状态）。");
                }

                m_transient = value;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 将当前对象加入事务域对象，根据 Transient 和 ReqiureDelete 属性决定要执行的操作（不考虑 Dirty 属性）。
        /// </summary>
        /// <param name="scope">事务域。</param>
        public void AddTo(TransactionScope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope", "事务域不能为空。");
            }

            if (Deleted)
            {
                throw new InvalidOperationException("对象已被删除，不能调用此方法。");
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
        /// 复制源对象的属性值到当前对象，同时复制所有外部引用实体中的属性，（所有的操作都不包括主键）。
        /// <para>注意，调用此方法要慎重，因为您可能希望有一些属性保持原来的值。</para>
        /// <para>如果当前对象的某个外部引用属性为空，则复制源对象中对应的外部引用属性。</para>
        /// <para>如果源对象中的某个外部引用属性为空，则不对该属性进行复制操作。</para>
        /// </summary>
        /// <param name="source">源对象。</param>
        public void CopyFrom(EtyBusinessObject source)
        {
            Debug.Assert((source.GetType() == this.GetType()), "要复制的源对象的类型与当前对象不同，无法复制。");

            DbEntityPropertyInfo.Copy(source, this);
        }

        /// <summary>
        /// 删除当前实体。
        /// </summary>
        /// <returns>如果删除成功，则返回 true；否则返回 false。</returns>
        public Boolean Delete()
        {
            CheckDatabaseSessionExistence();

            if (Transient)
            {
                throw new InvalidOperationException("不能调用此方法，因实体为游离对象。");
            }

            if (Deleted)
            {
                throw new InvalidOperationException("不能调用此方法，因实体已被删除。");
            }

            Boolean success = DatabaseSession.Delete(this);

            return success;
        }

        /// <summary>
        /// 深拷贝Serializable对象
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
        /// 判断子实体集合是否已经加载。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>如果子实体集合已经加载，则返回 true；否则返回 false。</returns>
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
        /// 加载当前实体的持久化版本，不包含延迟加载属性。
        /// </summary>
        /// <returns>当前实体的持久化版本，可能为空（在加载后，其他用户可能会删除该实体）。</returns>
        public EtyBusinessObject Load()
        {
            return Load(false);
        }

        /// <summary>
        /// 加载当前实体的持久化版本，可指定是否包含延迟加载属性。
        /// </summary>
        /// <param name="includingLazyLoads">指示是否包含延迟加载属性。</param>
        /// <returns>当前实体的持久化版本，可能为空（在加载后，其他用户可能会删除该实体）。</returns>
        public EtyBusinessObject Load(Boolean includingLazyLoads)
        {
            CheckDatabaseSessionExistence();

            if (Transient)
            {
                throw new InvalidOperationException("当前实体为游离对象，不能调用此方法。");
            }

            return (EtyBusinessObject)DbEntityPropertyInfo.Load(this, DatabaseSession, includingLazyLoads);
        }

        /// <summary>
        /// 加载所有子实体集合，并指示如果子实体集合已加载是否强制刷新。
        /// </summary>
        /// <param name="forceRefresh">指示是否强制刷新。</param>
        public void LoadAllChildren(Boolean forceRefresh)
        {
            foreach (String name in DbEntityPropertyInfo.Children)
            {
                LoadChildren(name, forceRefresh);
            }
        }

        /// <summary>
        /// 加载指定的子实体集合，并指示如果子实体集合已加载是否强制刷新。
        /// </summary>
        /// <param name="propertyName">子类属性名称。</param>
        /// <param name="forceRefresh">指示是否强制刷新。</param>
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
        /// 使用当前实体的持久化版本刷新当前实体，不刷新延迟加载属性。
        /// <para>如果当前实体自加载后其他用户删除了它，则刷新会失败（返回 false），调用此方法会覆盖所有的修改。</para>
        /// </summary>
        /// <returns>如果刷新成功，则返回 true；否则返回 false。</returns>
        public Boolean Refresh()
        {
            return Refresh(false);
        }

        /// <summary>
        /// 使用当前实体的持久化版本刷新当前实体，可以指定是否刷新延迟加载属性。
        /// <para>如果当前实体自加载后其他用户删除了它，则刷新会失败（返回 false），调用此方法会覆盖所有的修改。</para>
        /// </summary>
        /// <param name="includingLazyLoads">如果为 true；则刷新延迟加载属性；否则，不刷新延迟加载属性。</param>
        /// <returns>如果刷新成功，则返回 true；否则返回 false。</returns>
        public Boolean Refresh(Boolean includingLazyLoads)
        {
            EtyBusinessObject persist = Load(includingLazyLoads);

            // 逐级复制
            if (persist == null)
            {
                return false;
            }

            DbEntityPropertyInfo.Copy(persist, this);

            return true;
        }

        /// <summary>
        /// 保存当前实体。
        /// </summary>
        public void Save()
        {
            CheckDatabaseSessionExistence();

            if (Deleted)
            {
                throw new InvalidOperationException("不能调用此方法，因实体已被删除。");
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
        /// 设置实体的状态为持久状态（此改变是不可逆的，在实体成功保存到数据库中后调用）。
        /// </summary>
        [Obsolete("请不要使用此方法，直接设置 Transient 属性为 true。")]
        public void SetPersistent()
        {
            m_transient = false;
        }

        #region 延迟加载与回滚

        /// <summary>
        /// 清除保存点。
        /// </summary>
        public void ClearSavePoint()
        {
            if (m_dataBag != null)
            {
                m_dataBag.ClearSavePoint();
            }
        }

        /// <summary>
        /// 获取一个值，该值指示是否设置了保存点。
        /// </summary>
        public Boolean HasSavePoint
        {
            get { return DataBag.HasSavePoint; }
        }

        /// <summary>
        /// 获取属性的值（采用显示类型强制转换）。
        /// </summary>
        /// <typeparam name="TValue">属性类型。</typeparam>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>属性值。</returns>
        public TValue GetLazyLoadValue<TValue>(String propertyName)
        {
            return DataBag.GetLazyLoadValue<TValue>(propertyName);
        }

        /// <summary>
        /// 设置延迟加载属性的值。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <param name="propertyValue">属性值。</param>
        public void SetLazyLoadValue(String propertyName, Object propertyValue)
        {
            DataBag.SetLazyLoadValue(propertyName, propertyValue);
        }

        /// <summary>
        /// 判断指定的属性的值是否为 DBEmpty （即尚未加载）。
        /// </summary>
        /// <param name="chain">属性链。</param>
        /// <returns>如果该属性尚未加载，则返回 true；否则返回 false。</returns>
        public Boolean IsEmpty(IPropertyChain chain)
        {
            #region 前置条件

            Debug.Assert(chain != null, "属性链参数 chain 不能为空。");
            Debug.Assert(chain.Type == GetType(), "属性链的目标实体类型与当前实体的类型不同");

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
        /// 判断指定的属性的值是否为 DBEmpty（即尚未加载）。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>如果该属性的值尚未初始化，则返回 true；否则返回 false。</returns>
        public Boolean IsEmpty(String propertyName)
        {
            return DataBag.IsEmpty(propertyName);
        }

        /// <summary>
        /// 判断实体的属性值是否为 DBEmpty（即尚未初始化），只对延迟加载的属性有效。
        /// </summary>
        /// <param name="entity">要判断的实体。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>如果实体是 EtyBusinessObject，该属性为延迟加载的属性且当前没有被加载，则返回 true；否则返回 false。</returns>
        public static Boolean IsEmpty(Object entity, String propertyName)
        {
            EtyBusinessObject bo = entity as EtyBusinessObject;

            return (bo != null) && bo.IsEmpty(propertyName);
        }

        /// <summary>
        /// 回滚到最近的一个保存点。
        /// </summary>
        /// <returns>如果保存点存在，则回滚成功，返回 true；否则返回 false。</returns>
        public Boolean Rollback()
        {
            return DataBag.Rollback();
        }

        /// <summary>
        /// 将延迟加载属性置为 DBEmpty。
        /// </summary>
        /// <param name="propertyName">延迟加载属性名称。</param>
        public void SetEmpty(String propertyName)
        {
            DataBag.SetEmpty(propertyName);
        }

        /// <summary>
        /// 设置保存点。
        /// </summary>
        public void SetSavePoint()
        {
            DataBag.SetSavePoint();
        }

        #endregion

        #endregion

        #region 保护的属性

        #endregion

        #region 保护的虚方法

        /// <summary>
        /// 当创建游离对象后，执行此方法，当的实现是正确设置对象的状态和标记为 AutoGenerateOnNewAttribute 的主键属性的值。
        /// </summary>
        internal protected virtual void OnCreateNew()
        {
            InitializeStatus();

            SetPrimaryKeyValue();
        }

        /// <summary>
        /// 生成主键值，当前的实现是生成一个 GUID 串。
        /// </summary>
        /// <returns>主键值。</returns>
        protected virtual Object GeneratePrimaryKeyValue()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        #region 保护的方法

        /// <summary>
        /// 确保实体完整加载。
        /// </summary>
        /// <returns>如果成功执行此方法，即此实体已经附着了数据库会话引擎、状态正确（非游离且为非删除），且 SuppressNavigate 为 false，返回 true；否则返回 fasle。</returns>
        protected Boolean EnsureCompletelyLoaded()
        {
            if ((DatabaseSession == null) || Transient || Deleted || SuppressLazyLoad)
            {
                return false;
            }
            else if (IsPartially)
            {
                Refresh();

                // 这一步可省略，因在复制值时会自动设置
                IsPartially = false;
            }

            return true;
        }

        /// <summary>
        /// 获取子类集合，当 SuppressNavigate 为 true 时，不会主动去加载子实体集合。
        /// </summary>
        /// <typeparam name="TEntity">子类实体类型。</typeparam>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>子类集合。</returns>
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
        /// 设置子实体集合。
        /// </summary>
        /// <param name="propertyName">子实体集合属性名称。</param>
        /// <param name="children">要设置的子实体列表。</param>
        protected void SetChildren(String propertyName, Object[] children)
        {
            AllChildren[propertyName] = children;
        }

        /// <summary>
        /// 当持久化成功后设置实体的状态。
        /// </summary>
        internal protected void SetDbEntityStatusOnCommitSuccess()
        {
            // 对象为持久对象
            Transient = false;

            // 对象是干净的
            Dirty = false;

            if (RequireDelete)
            {
                Deleted = true;
            }
        }

        #endregion

        #region 私有的属性

        /// <summary>
        /// 获取所有的子实体集合。
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
        /// 获取数据包，支持延迟加载和回滚。
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
        /// 获取实体属性信息。
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
        /// 判断是否有子类集合被加载。
        /// </summary>
        private Boolean HasChildren
        {
            get
            {
                return ((m_allChildren != null) && (m_allChildren.Count != 0));
            }

        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 检查数据库会话引擎是否存在，如果不存在，则抛出 InvalidOperationException 异常。
        /// </summary>
        private void CheckDatabaseSessionExistence()
        {
            if (DatabaseSession == null)
            {
                throw new InvalidOperationException("不能调用当前方法，因此实体没有附着数据库会话引擎。");
            }
        }

        /// <summary>
        /// 初始化对象的状态。
        /// </summary>
        private void InitializeStatus()
        {
            m_deleted = false;
            m_requireDelete = false;
            m_dirty = false;

            // 默认是完整加载
            m_isPartially = false;

            m_transient = true;
        }

        /// <summary>
        /// 为当前实例设置主键值。
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
                        String.Format("无法为属性 {0}.{1} 生成主键值，因为该属性不可写。",
                                DbEntityPropertyInfo.Type.FullName,
                                pf.Name
                            )
                    );
            }

            Object primaryKeyValue = GeneratePrimaryKeyValue();

            pf.SetValue(this, primaryKeyValue, null);
        }

        #endregion

        #region IDebugInfoProvider 成员

        /// <summary>
        /// 实体的详细信息，用于调试。
        /// </summary>
        public String Dump()
        {
            return DbEntityDebugger.Dump(this);
        }

        /// <summary>
        /// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public String Dump(String indent)
        {
            return DbEntityDebugger.IndentText(Dump(), indent);
        }

        /// <summary>
        /// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public String Dump(Int32 level)
        {
            return DbEntityDebugger.IndentText(Dump(), level);
        }

        /// <summary>
        /// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
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