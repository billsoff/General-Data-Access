#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyRole.cs
// 文件功能描述：用户角色表。
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
    /// 用户角色表
    /// </summary>
    [Serializable, Table("T_Role")]
    public sealed  partial class EtyRole : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyRole()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 岗位序列号
        /// </summary>
        [Column("Role_Id", DbType.Int32, IsPrimaryKey = true), Native]
        public int RoleId { get; private set; }

        /// <summary>
        /// 岗位序列号
        /// </summary>
        public const string PROP_ROLE_ID = "RoleId";

		/// <summary>
        /// 岗位名称：如开发工程师
        /// </summary>
        [Column("Role_Name", DbType.AnsiString)]
        public string RoleName { get; set; }

        /// <summary>
        /// 岗位名称：如开发工程师
        /// </summary>
        public const string PROP_ROLE_NAME = "RoleName";

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
        [Serializable, PropertyDescriptor(typeof(EtyRole))]
        public sealed  class RolePropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 岗位序列号
            /// </summary>
            public IPropertyChain RoleId { get { return LinkPrimitiveProperty(PROP_ROLE_ID); } }
	
            /// <summary>
            /// 岗位名称：如开发工程师
            /// </summary>
            public IPropertyChain RoleName { get { return LinkPrimitiveProperty(PROP_ROLE_NAME); } }
	
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
        /// 用户角色表
        /// </summary>
        public static EtyRole.RolePropertyDescriptor Role { get { return new EtyRole.RolePropertyDescriptor(); } }
    }
}