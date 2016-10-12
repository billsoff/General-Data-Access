#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyDemand.cs
// 文件功能描述：。
//
//
// 创建标识：模板创建 2012/4/1 18:59:31
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
    /// 
    /// </summary>
    [Serializable, Table("T_Demand")]
    public sealed  partial class EtyDemand : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyDemand()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 需求唯一标识
        /// </summary>
        [Column("Demand_Id", DbType.Int32, IsPrimaryKey = true), Native]
        public int DemandId { get; private set; }

        /// <summary>
        /// 需求唯一标识
        /// </summary>
        public const string PROP_DEMAND_ID = "DemandId";

		/// <summary>
        /// 项目
        /// </summary>
        [ForeignReference, ColumnMapping("Project_Id", "Project_Id")]
        public EtyProject Project { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public const string PROP_PROJECT = "Project";

		/// <summary>
        /// 父需求唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Parent_Demand_Id", "Demand_Id"), PermitNull]
        public EtyDemand ParentDemand { get; set; }

        /// <summary>
        /// 父需求唯一标识
        /// </summary>
        public const string PROP_PARENT_DEMAND = "ParentDemand";

		/// <summary>
        /// 当前版本号
        /// </summary>
        [Column("Current_Version", DbType.Int32)]
        public int CurrentVersion { get; set; }

        /// <summary>
        /// 当前版本号
        /// </summary>
        public const string PROP_CURRENT_VERSION = "CurrentVersion";

		/// <summary>
        /// 
        /// </summary>
        [Column("TimeCreated", DbType.DateTime)]
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public const string PROP_TIMECREATED = "TimeCreated";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyDemand))]
        public sealed  class DemandPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 需求唯一标识
            /// </summary>
            public IPropertyChain DemandId { get { return LinkPrimitiveProperty(PROP_DEMAND_ID); } }
	
            /// <summary>
            /// 项目
            /// </summary>
			public EtyProject.ProjectPropertyDescriptor Project { get { return LinkForeignReferenceProperty<EtyProject.ProjectPropertyDescriptor>(PROP_PROJECT); } }
	
            /// <summary>
            /// 父需求唯一标识
            /// </summary>
			public EtyDemand.DemandPropertyDescriptor ParentDemand { get { return LinkForeignReferenceProperty<EtyDemand.DemandPropertyDescriptor>(PROP_PARENT_DEMAND); } }
	
            /// <summary>
            /// 当前版本号
            /// </summary>
            public IPropertyChain CurrentVersion { get { return LinkPrimitiveProperty(PROP_CURRENT_VERSION); } }
	
            /// <summary>
            /// 
            /// </summary>
            public IPropertyChain TimeCreated { get { return LinkPrimitiveProperty(PROP_TIMECREATED); } }

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
        /// 
        /// </summary>
        public static EtyDemand.DemandPropertyDescriptor Demand { get { return new EtyDemand.DemandPropertyDescriptor(); } }
    }
}