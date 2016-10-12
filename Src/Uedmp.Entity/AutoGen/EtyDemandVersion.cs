#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyDemandVersion.cs
// 文件功能描述：需求。
//
//
// 创建标识：模板创建 2012/4/1 18:59:32
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Data;

using Useease.GeneralDataAccess;

namespace Uedmp.Entity
{
    /// <summary>
    /// 需求
    /// </summary>
    [Serializable, Table("T_DemandVersion")]
    public sealed  partial class EtyDemandVersion : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyDemandVersion()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 需求唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Demand_Id", "Demand_Id"), PrimaryKey]
        public EtyDemand Demand { get; set; }

        /// <summary>
        /// 需求唯一标识
        /// </summary>
        public const string PROP_DEMAND = "Demand";

		/// <summary>
        /// 需求版本
        /// </summary>
        [Column("Demand_Version", DbType.Int32, IsPrimaryKey = true)]
        public int DemandVersion { get; private set; }

        /// <summary>
        /// 需求版本
        /// </summary>
        public const string PROP_DEMAND_VERSION = "DemandVersion";

		/// <summary>
        /// 创建用户
        /// </summary>
        [ForeignReference, ColumnMapping("Creator_Id", "User_Id"), PermitNull]
        public EtyUser Creator { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public const string PROP_CREATOR = "Creator";

		/// <summary>
        /// 需求方
        /// </summary>
        [Column("Demand_Side", DbType.AnsiString)]
        public string DemandSide { get; set; }

        /// <summary>
        /// 需求方
        /// </summary>
        public const string PROP_DEMAND_SIDE = "DemandSide";

		/// <summary>
        /// 标题
        /// </summary>
        [Column("Demand_Title", DbType.AnsiString)]
        public string DemandTitle { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public const string PROP_DEMAND_TITLE = "DemandTitle";

		/// <summary>
        /// 内容
        /// </summary>
        [Column("Demand_Content", DbType.AnsiString)]
        public string DemandContent { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public const string PROP_DEMAND_CONTENT = "DemandContent";

		/// <summary>
        /// 创建时间
        /// </summary>
        [Column("TimeCreated", DbType.DateTime)]
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public const string PROP_TIMECREATED = "TimeCreated";

		/// <summary>
        /// 加锁用户
        /// </summary>
        [ForeignReference, ColumnMapping("Locker_Id", "User_Id"), PermitNull]
        public EtyUser Locker { get; set; }

        /// <summary>
        /// 加锁用户
        /// </summary>
        public const string PROP_LOCKER = "Locker";

		/// <summary>
        /// 需求优先级唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Demand_Priority_ID", "Demand_Priority_ID")]
        public EtyDemandPriority DemandPriority { get; set; }

        /// <summary>
        /// 需求优先级唯一标识
        /// </summary>
        public const string PROP_DEMAND_PRIORITY = "DemandPriority";

		/// <summary>
        /// 需求紧急度唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Demand_Importance_ID", "Demand_Importance_ID")]
        public EtyDemandImportance DemandImportance { get; set; }

        /// <summary>
        /// 需求紧急度唯一标识
        /// </summary>
        public const string PROP_DEMAND_IMPORTANCE = "DemandImportance";

		/// <summary>
        /// 加锁时间
        /// </summary>
        [Column("Lock_Time", DbType.DateTime)]
        public DateTime LockTime { get; set; }

        /// <summary>
        /// 加锁时间
        /// </summary>
        public const string PROP_LOCK_TIME = "LockTime";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyDemandVersion))]
        public sealed  class DemandVersionPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 需求唯一标识
            /// </summary>
			public EtyDemand.DemandPropertyDescriptor Demand { get { return LinkForeignReferenceProperty<EtyDemand.DemandPropertyDescriptor>(PROP_DEMAND); } }
	
            /// <summary>
            /// 需求版本
            /// </summary>
            public IPropertyChain DemandVersion { get { return LinkPrimitiveProperty(PROP_DEMAND_VERSION); } }
	
            /// <summary>
            /// 创建用户
            /// </summary>
			public EtyUser.UserPropertyDescriptor Creator { get { return LinkForeignReferenceProperty<EtyUser.UserPropertyDescriptor>(PROP_CREATOR); } }
	
            /// <summary>
            /// 需求方
            /// </summary>
            public IPropertyChain DemandSide { get { return LinkPrimitiveProperty(PROP_DEMAND_SIDE); } }
	
            /// <summary>
            /// 标题
            /// </summary>
            public IPropertyChain DemandTitle { get { return LinkPrimitiveProperty(PROP_DEMAND_TITLE); } }
	
            /// <summary>
            /// 内容
            /// </summary>
            public IPropertyChain DemandContent { get { return LinkPrimitiveProperty(PROP_DEMAND_CONTENT); } }
	
            /// <summary>
            /// 创建时间
            /// </summary>
            public IPropertyChain TimeCreated { get { return LinkPrimitiveProperty(PROP_TIMECREATED); } }
	
            /// <summary>
            /// 加锁用户
            /// </summary>
			public EtyUser.UserPropertyDescriptor Locker { get { return LinkForeignReferenceProperty<EtyUser.UserPropertyDescriptor>(PROP_LOCKER); } }
	
            /// <summary>
            /// 需求优先级唯一标识
            /// </summary>
			public EtyDemandPriority.DemandPriorityPropertyDescriptor DemandPriority { get { return LinkForeignReferenceProperty<EtyDemandPriority.DemandPriorityPropertyDescriptor>(PROP_DEMAND_PRIORITY); } }
	
            /// <summary>
            /// 需求紧急度唯一标识
            /// </summary>
			public EtyDemandImportance.DemandImportancePropertyDescriptor DemandImportance { get { return LinkForeignReferenceProperty<EtyDemandImportance.DemandImportancePropertyDescriptor>(PROP_DEMAND_IMPORTANCE); } }
	
            /// <summary>
            /// 加锁时间
            /// </summary>
            public IPropertyChain LockTime { get { return LinkPrimitiveProperty(PROP_LOCK_TIME); } }

            #endregion
        }

        #endregion
    }
    
    /// <summary>
    /// 实体
    /// </summary>
    partial class Entities
    {
		/// <summary>
        /// 需求
        /// </summary>
        public static EtyDemandVersion.DemandVersionPropertyDescriptor DemandVersion { get { return new EtyDemandVersion.DemandVersionPropertyDescriptor(); } }
    }
}