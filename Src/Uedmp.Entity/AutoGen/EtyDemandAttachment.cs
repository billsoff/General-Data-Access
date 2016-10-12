#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyDemandAttachment.cs
// 文件功能描述：。
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
    /// 
    /// </summary>
    [Serializable, Table("T_DemandAttachment")]
    public sealed partial class EtyDemandAttachment : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyDemandAttachment()
        {
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 需求唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Demand_ID", "Demand_Id"), ColumnMapping("Demand_Version", "Demand_Version"), PrimaryKey]
        public EtyDemandVersion Demand { get; set; }

        /// <summary>
        /// 需求唯一标识
        /// </summary>
        public const string PROP_DEMAND = "Demand";

        /// <summary>
        /// 附件唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Attachment_ID", "Attachment_ID"), PrimaryKey]
        public EtyAttachment Attachment { get; set; }

        /// <summary>
        /// 附件唯一标识
        /// </summary>
        public const string PROP_ATTACHMENT = "Attachment";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyDemandAttachment))]
        public sealed class DemandAttachmentPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性

            /// <summary>
            /// 需求唯一标识
            /// </summary>
            public EtyDemandVersion.DemandVersionPropertyDescriptor Demand { get { return LinkForeignReferenceProperty<EtyDemandVersion.DemandVersionPropertyDescriptor>(PROP_DEMAND); } }

            /// <summary>
            /// 附件唯一标识
            /// </summary>
            public EtyAttachment.AttachmentPropertyDescriptor Attachment { get { return LinkForeignReferenceProperty<EtyAttachment.AttachmentPropertyDescriptor>(PROP_ATTACHMENT); } }

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
        public static EtyDemandAttachment.DemandAttachmentPropertyDescriptor DemandAttachment { get { return new EtyDemandAttachment.DemandAttachmentPropertyDescriptor(); } }
    }
}