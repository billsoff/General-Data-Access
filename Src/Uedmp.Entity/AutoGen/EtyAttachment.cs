#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyAttachment.cs
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
    [Serializable, Table("T_Attachment")]
    public sealed  partial class EtyAttachment : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyAttachment()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 附件唯一标识
        /// </summary>
        [Column("Attachment_ID", DbType.Int32, IsPrimaryKey = true), Native]
        public int AttachmentID { get; private set; }

        /// <summary>
        /// 附件唯一标识
        /// </summary>
        public const string PROP_ATTACHMENT_ID = "AttachmentID";

		/// <summary>
        /// 附件上传者唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Upload_User_Id", "User_Id")]
        public EtyUser UploadUser { get; set; }

        /// <summary>
        /// 附件上传者唯一标识
        /// </summary>
        public const string PROP_UPLOAD_USER = "UploadUser";

		/// <summary>
        /// 附件文件名称
        /// </summary>
        [Column("File_Name", DbType.AnsiString)]
        public string FileName { get; set; }

        /// <summary>
        /// 附件文件名称
        /// </summary>
        public const string PROP_FILE_NAME = "FileName";

		/// <summary>
        /// 附件统一名称
        /// </summary>
        [Column("Unified_Name", DbType.AnsiString)]
        public string UnifiedName { get; set; }

        /// <summary>
        /// 附件统一名称
        /// </summary>
        public const string PROP_UNIFIED_NAME = "UnifiedName";

		/// <summary>
        /// 附件上传时间
        /// </summary>
        [Column("Upload_Time", DbType.DateTime)]
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 附件上传时间
        /// </summary>
        public const string PROP_UPLOAD_TIME = "UploadTime";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyAttachment))]
        public sealed  class AttachmentPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 附件唯一标识
            /// </summary>
            public IPropertyChain AttachmentID { get { return LinkPrimitiveProperty(PROP_ATTACHMENT_ID); } }
	
            /// <summary>
            /// 附件上传者唯一标识
            /// </summary>
			public EtyUser.UserPropertyDescriptor UploadUser { get { return LinkForeignReferenceProperty<EtyUser.UserPropertyDescriptor>(PROP_UPLOAD_USER); } }
	
            /// <summary>
            /// 附件文件名称
            /// </summary>
            public IPropertyChain FileName { get { return LinkPrimitiveProperty(PROP_FILE_NAME); } }
	
            /// <summary>
            /// 附件统一名称
            /// </summary>
            public IPropertyChain UnifiedName { get { return LinkPrimitiveProperty(PROP_UNIFIED_NAME); } }
	
            /// <summary>
            /// 附件上传时间
            /// </summary>
            public IPropertyChain UploadTime { get { return LinkPrimitiveProperty(PROP_UPLOAD_TIME); } }

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
        public static EtyAttachment.AttachmentPropertyDescriptor Attachment { get { return new EtyAttachment.AttachmentPropertyDescriptor(); } }
    }
}