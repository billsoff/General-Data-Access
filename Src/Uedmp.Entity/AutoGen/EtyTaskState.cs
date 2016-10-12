#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyTaskState.cs
// 文件功能描述：工作任务状态。
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
    /// 工作任务状态
    /// </summary>
    [Serializable, Table("T_Task_State")]
    public sealed  partial class EtyTaskState : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyTaskState()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 工作任务状态唯一标识
        /// </summary>
        [Column("Task_State_ID", DbType.Int32, IsPrimaryKey = true), Native]
        public int TaskStateID { get; private set; }

        /// <summary>
        /// 工作任务状态唯一标识
        /// </summary>
        public const string PROP_TASK_STATE_ID = "TaskStateID";

		/// <summary>
        /// 名称
        /// </summary>
        [Column("Task_State_Name", DbType.AnsiString)]
        public string TaskStateName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public const string PROP_TASK_STATE_NAME = "TaskStateName";

		/// <summary>
        /// 显示名称
        /// </summary>
        [Column("Task_State_Display_Name", DbType.AnsiString)]
        public string TaskStateDisplayName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public const string PROP_TASK_STATE_DISPLAY_NAME = "TaskStateDisplayName";

		/// <summary>
        /// 描述
        /// </summary>
        [Column("Task_State_Description", DbType.AnsiString)]
        public string TaskStateDescription { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public const string PROP_TASK_STATE_DESCRIPTION = "TaskStateDescription";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyTaskState))]
        public sealed  class TaskStatePropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 工作任务状态唯一标识
            /// </summary>
            public IPropertyChain TaskStateID { get { return LinkPrimitiveProperty(PROP_TASK_STATE_ID); } }
	
            /// <summary>
            /// 名称
            /// </summary>
            public IPropertyChain TaskStateName { get { return LinkPrimitiveProperty(PROP_TASK_STATE_NAME); } }
	
            /// <summary>
            /// 显示名称
            /// </summary>
            public IPropertyChain TaskStateDisplayName { get { return LinkPrimitiveProperty(PROP_TASK_STATE_DISPLAY_NAME); } }
	
            /// <summary>
            /// 描述
            /// </summary>
            public IPropertyChain TaskStateDescription { get { return LinkPrimitiveProperty(PROP_TASK_STATE_DESCRIPTION); } }

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
        /// 工作任务状态
        /// </summary>
        public static EtyTaskState.TaskStatePropertyDescriptor TaskState { get { return new EtyTaskState.TaskStatePropertyDescriptor(); } }
    }
}