#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyProject.cs
// 文件功能描述：项目表。
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
    /// 项目表
    /// </summary>
    [Serializable, Table("T_Project")]
    public sealed  partial class EtyProject : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyProject()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 项目ID
        /// </summary>
        [Column("Project_Id", DbType.Int32, IsPrimaryKey = true), Native]
        public int ProjectId { get; private set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public const string PROP_PROJECT_ID = "ProjectId";

		/// <summary>
        /// 项目名称
        /// </summary>
        [Column("Project_Name", DbType.AnsiString)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public const string PROP_PROJECT_NAME = "ProjectName";

		/// <summary>
        /// 项目经理
        /// </summary>
        [ForeignReference, ColumnMapping("User_Id", "User_Id")]
        public EtyUser User { get; set; }

        /// <summary>
        /// 项目经理
        /// </summary>
        public const string PROP_USER = "User";

		/// <summary>
        /// 甲方单位名称
        /// </summary>
        [Column("Party_Name", DbType.AnsiString)]
        public string PartyName { get; set; }

        /// <summary>
        /// 甲方单位名称
        /// </summary>
        public const string PROP_PARTY_NAME = "PartyName";

		/// <summary>
        /// 甲方联系人
        /// </summary>
        [Column("Party_Contacts", DbType.AnsiString)]
        public string PartyContacts { get; set; }

        /// <summary>
        /// 甲方联系人
        /// </summary>
        public const string PROP_PARTY_CONTACTS = "PartyContacts";

		/// <summary>
        /// 甲方联系电话
        /// </summary>
        [Column("Party_Phone", DbType.AnsiString)]
        public string PartyPhone { get; set; }

        /// <summary>
        /// 甲方联系电话
        /// </summary>
        public const string PROP_PARTY_PHONE = "PartyPhone";

		/// <summary>
        /// 项目开始时间
        /// </summary>
        [Column("Create_Time", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 项目开始时间
        /// </summary>
        public const string PROP_CREATE_TIME = "CreateTime";

		/// <summary>
        /// 项目初验时间
        /// </summary>
        [Column("InitInspect_Time", DbType.DateTime)]
        public DateTime InitInspectTime { get; set; }

        /// <summary>
        /// 项目初验时间
        /// </summary>
        public const string PROP_INITINSPECT_TIME = "InitInspectTime";

		/// <summary>
        /// 项目终验时间
        /// </summary>
        [Column("FinalInspect_Time", DbType.DateTime)]
        public DateTime FinalInspectTime { get; set; }

        /// <summary>
        /// 项目终验时间
        /// </summary>
        public const string PROP_FINALINSPECT_TIME = "FinalInspectTime";

		/// <summary>
        /// 项目进展状态
        /// </summary>
        [Column("Project_Status", DbType.Int32)]
        public int ProjectStatus { get; set; }

        /// <summary>
        /// 项目进展状态
        /// </summary>
        public const string PROP_PROJECT_STATUS = "ProjectStatus";

		/// <summary>
        /// 备注
        /// </summary>
        [Column("Remark", DbType.AnsiString)]
        public string Remark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public const string PROP_REMARK = "Remark";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyProject))]
        public sealed  class ProjectPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 项目ID
            /// </summary>
            public IPropertyChain ProjectId { get { return LinkPrimitiveProperty(PROP_PROJECT_ID); } }
	
            /// <summary>
            /// 项目名称
            /// </summary>
            public IPropertyChain ProjectName { get { return LinkPrimitiveProperty(PROP_PROJECT_NAME); } }
	
            /// <summary>
            /// 项目经理
            /// </summary>
			public EtyUser.UserPropertyDescriptor User { get { return LinkForeignReferenceProperty<EtyUser.UserPropertyDescriptor>(PROP_USER); } }
	
            /// <summary>
            /// 甲方单位名称
            /// </summary>
            public IPropertyChain PartyName { get { return LinkPrimitiveProperty(PROP_PARTY_NAME); } }
	
            /// <summary>
            /// 甲方联系人
            /// </summary>
            public IPropertyChain PartyContacts { get { return LinkPrimitiveProperty(PROP_PARTY_CONTACTS); } }
	
            /// <summary>
            /// 甲方联系电话
            /// </summary>
            public IPropertyChain PartyPhone { get { return LinkPrimitiveProperty(PROP_PARTY_PHONE); } }
	
            /// <summary>
            /// 项目开始时间
            /// </summary>
            public IPropertyChain CreateTime { get { return LinkPrimitiveProperty(PROP_CREATE_TIME); } }
	
            /// <summary>
            /// 项目初验时间
            /// </summary>
            public IPropertyChain InitInspectTime { get { return LinkPrimitiveProperty(PROP_INITINSPECT_TIME); } }
	
            /// <summary>
            /// 项目终验时间
            /// </summary>
            public IPropertyChain FinalInspectTime { get { return LinkPrimitiveProperty(PROP_FINALINSPECT_TIME); } }
	
            /// <summary>
            /// 项目进展状态
            /// </summary>
            public IPropertyChain ProjectStatus { get { return LinkPrimitiveProperty(PROP_PROJECT_STATUS); } }
	
            /// <summary>
            /// 备注
            /// </summary>
            public IPropertyChain Remark { get { return LinkPrimitiveProperty(PROP_REMARK); } }

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
        /// 项目表
        /// </summary>
        public static EtyProject.ProjectPropertyDescriptor Project { get { return new EtyProject.ProjectPropertyDescriptor(); } }
    }
}