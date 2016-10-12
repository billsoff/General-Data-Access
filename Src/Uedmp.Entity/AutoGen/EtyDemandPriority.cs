#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyDemandPriority.cs
// 文件功能描述：需求优先级。
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
    /// 需求优先级
    /// </summary>
    [Serializable, Table("T_Demand_Priority")]
    public sealed  partial class EtyDemandPriority : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyDemandPriority()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 需求优先级唯一标识
        /// </summary>
        [Column("Demand_Priority_ID", DbType.Int32, IsPrimaryKey = true), Native]
        public int DemandPriorityID { get; private set; }

        /// <summary>
        /// 需求优先级唯一标识
        /// </summary>
        public const string PROP_DEMAND_PRIORITY_ID = "DemandPriorityID";

		/// <summary>
        /// 优先级名称
        /// </summary>
        [Column("Demand_Priority_Name", DbType.AnsiString)]
        public string DemandPriorityName { get; set; }

        /// <summary>
        /// 优先级名称
        /// </summary>
        public const string PROP_DEMAND_PRIORITY_NAME = "DemandPriorityName";

		/// <summary>
        /// 优先级显示名称
        /// </summary>
        [Column("Demand_Priority_Display_Name", DbType.AnsiString)]
        public string DemandPriorityDisplayName { get; set; }

        /// <summary>
        /// 优先级显示名称
        /// </summary>
        public const string PROP_DEMAND_PRIORITY_DISPLAY_NAME = "DemandPriorityDisplayName";

		/// <summary>
        /// 优先级描述
        /// </summary>
        [Column("Demand_Priority_Description", DbType.AnsiString)]
        public string DemandPriorityDescription { get; set; }

        /// <summary>
        /// 优先级描述
        /// </summary>
        public const string PROP_DEMAND_PRIORITY_DESCRIPTION = "DemandPriorityDescription";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyDemandPriority))]
        public sealed  class DemandPriorityPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 需求优先级唯一标识
            /// </summary>
            public IPropertyChain DemandPriorityID { get { return LinkPrimitiveProperty(PROP_DEMAND_PRIORITY_ID); } }
	
            /// <summary>
            /// 优先级名称
            /// </summary>
            public IPropertyChain DemandPriorityName { get { return LinkPrimitiveProperty(PROP_DEMAND_PRIORITY_NAME); } }
	
            /// <summary>
            /// 优先级显示名称
            /// </summary>
            public IPropertyChain DemandPriorityDisplayName { get { return LinkPrimitiveProperty(PROP_DEMAND_PRIORITY_DISPLAY_NAME); } }
	
            /// <summary>
            /// 优先级描述
            /// </summary>
            public IPropertyChain DemandPriorityDescription { get { return LinkPrimitiveProperty(PROP_DEMAND_PRIORITY_DESCRIPTION); } }

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
        /// 需求优先级
        /// </summary>
        public static EtyDemandPriority.DemandPriorityPropertyDescriptor DemandPriority { get { return new EtyDemandPriority.DemandPriorityPropertyDescriptor(); } }
    }
}