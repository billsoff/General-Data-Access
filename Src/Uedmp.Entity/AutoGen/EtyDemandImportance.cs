#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyDemandImportance.cs
// 文件功能描述：需求紧急度。
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
    /// 需求紧急度
    /// </summary>
    [Serializable, Table("T_Demand_Importance")]
    public sealed  partial class EtyDemandImportance : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyDemandImportance()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 需求重要性唯一标识
        /// </summary>
        [Column("Demand_Importance_ID", DbType.Int32, IsPrimaryKey = true), Native]
        public int DemandImportanceID { get; private set; }

        /// <summary>
        /// 需求重要性唯一标识
        /// </summary>
        public const string PROP_DEMAND_IMPORTANCE_ID = "DemandImportanceID";

		/// <summary>
        /// 重要性名称
        /// </summary>
        [Column("Demand_Importance_Name", DbType.AnsiString)]
        public string DemandImportanceName { get; set; }

        /// <summary>
        /// 重要性名称
        /// </summary>
        public const string PROP_DEMAND_IMPORTANCE_NAME = "DemandImportanceName";

		/// <summary>
        /// 重要性显示名称
        /// </summary>
        [Column("Demand_Importance_Display_Name", DbType.AnsiString)]
        public string DemandImportanceDisplayName { get; set; }

        /// <summary>
        /// 重要性显示名称
        /// </summary>
        public const string PROP_DEMAND_IMPORTANCE_DISPLAY_NAME = "DemandImportanceDisplayName";

		/// <summary>
        /// 重要性描述
        /// </summary>
        [Column("Demand_Importance_Description", DbType.AnsiString)]
        public string DemandImportanceDescription { get; set; }

        /// <summary>
        /// 重要性描述
        /// </summary>
        public const string PROP_DEMAND_IMPORTANCE_DESCRIPTION = "DemandImportanceDescription";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyDemandImportance))]
        public sealed  class DemandImportancePropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 需求重要性唯一标识
            /// </summary>
            public IPropertyChain DemandImportanceID { get { return LinkPrimitiveProperty(PROP_DEMAND_IMPORTANCE_ID); } }
	
            /// <summary>
            /// 重要性名称
            /// </summary>
            public IPropertyChain DemandImportanceName { get { return LinkPrimitiveProperty(PROP_DEMAND_IMPORTANCE_NAME); } }
	
            /// <summary>
            /// 重要性显示名称
            /// </summary>
            public IPropertyChain DemandImportanceDisplayName { get { return LinkPrimitiveProperty(PROP_DEMAND_IMPORTANCE_DISPLAY_NAME); } }
	
            /// <summary>
            /// 重要性描述
            /// </summary>
            public IPropertyChain DemandImportanceDescription { get { return LinkPrimitiveProperty(PROP_DEMAND_IMPORTANCE_DESCRIPTION); } }

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
        /// 需求紧急度
        /// </summary>
        public static EtyDemandImportance.DemandImportancePropertyDescriptor DemandImportance { get { return new EtyDemandImportance.DemandImportancePropertyDescriptor(); } }
    }
}