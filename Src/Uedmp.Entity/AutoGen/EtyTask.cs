#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyTask.cs
// 文件功能描述：工作任务表。
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
    /// 工作任务表
    /// </summary>
    [Serializable, Table("T_Task")]
    public sealed  partial class EtyTask : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyTask()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 需求唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Demand_ID", "Demand_Id")]
        public EtyDemand Demand { get; set; }

        /// <summary>
        /// 需求唯一标识
        /// </summary>
        public const string PROP_DEMAND = "Demand";

		/// <summary>
        /// 任务ID
        /// </summary>
        [Column("Task_id", DbType.Int32, IsPrimaryKey = true), Native]
        public int TaskId { get; private set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public const string PROP_TASK_ID = "TaskId";

		/// <summary>
        /// 任务分配者唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Task_Assigner_Id", "User_Id")]
        public EtyUser TaskAssigner { get; set; }

        /// <summary>
        /// 任务分配者唯一标识
        /// </summary>
        public const string PROP_TASK_ASSIGNER = "TaskAssigner";

		/// <summary>
        /// 任务接收者唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Task_Worker_Id", "User_Id")]
        public EtyUser TaskWorker { get; set; }

        /// <summary>
        /// 任务接收者唯一标识
        /// </summary>
        public const string PROP_TASK_WORKER = "TaskWorker";

		/// <summary>
        /// 父任务唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Parent_Task_id", "Task_id"), PermitNull]
        public EtyTask ParentTask { get; set; }

        /// <summary>
        /// 父任务唯一标识
        /// </summary>
        public const string PROP_PARENT_TASK = "ParentTask";

		/// <summary>
        /// 任务名称
        /// </summary>
        [Column("Task_Name", DbType.AnsiString)]
        public string TaskName { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public const string PROP_TASK_NAME = "TaskName";

		/// <summary>
        /// 任务内容
        /// </summary>
        [Column("Task_Content", DbType.AnsiString)]
        public string TaskContent { get; set; }

        /// <summary>
        /// 任务内容
        /// </summary>
        public const string PROP_TASK_CONTENT = "TaskContent";

		/// <summary>
        /// 开始时间
        /// </summary>
        [Column("Start_Time", DbType.DateTime)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public const string PROP_START_TIME = "StartTime";

		/// <summary>
        /// 结束时间
        /// </summary>
        [Column("End_Time", DbType.DateTime)]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public const string PROP_END_TIME = "EndTime";

		/// <summary>
        /// 工时
        /// </summary>
        [Column("Work_Time", DbType.Double)]
        public double WorkTime { get; set; }

        /// <summary>
        /// 工时
        /// </summary>
        public const string PROP_WORK_TIME = "WorkTime";

		/// <summary>
        /// 工作任务状态唯一标识
        /// </summary>
        [ForeignReference, ColumnMapping("Task_State_ID", "Task_State_ID")]
        public EtyTaskState TaskState { get; set; }

        /// <summary>
        /// 工作任务状态唯一标识
        /// </summary>
        public const string PROP_TASK_STATE = "TaskState";

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
        [Serializable, PropertyDescriptor(typeof(EtyTask))]
        public sealed  class TaskPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 需求唯一标识
            /// </summary>
			public EtyDemand.DemandPropertyDescriptor Demand { get { return LinkForeignReferenceProperty<EtyDemand.DemandPropertyDescriptor>(PROP_DEMAND); } }
	
            /// <summary>
            /// 任务ID
            /// </summary>
            public IPropertyChain TaskId { get { return LinkPrimitiveProperty(PROP_TASK_ID); } }
	
            /// <summary>
            /// 任务分配者唯一标识
            /// </summary>
			public EtyUser.UserPropertyDescriptor TaskAssigner { get { return LinkForeignReferenceProperty<EtyUser.UserPropertyDescriptor>(PROP_TASK_ASSIGNER); } }
	
            /// <summary>
            /// 任务接收者唯一标识
            /// </summary>
			public EtyUser.UserPropertyDescriptor TaskWorker { get { return LinkForeignReferenceProperty<EtyUser.UserPropertyDescriptor>(PROP_TASK_WORKER); } }
	
            /// <summary>
            /// 父任务唯一标识
            /// </summary>
			public EtyTask.TaskPropertyDescriptor ParentTask { get { return LinkForeignReferenceProperty<EtyTask.TaskPropertyDescriptor>(PROP_PARENT_TASK); } }
	
            /// <summary>
            /// 任务名称
            /// </summary>
            public IPropertyChain TaskName { get { return LinkPrimitiveProperty(PROP_TASK_NAME); } }
	
            /// <summary>
            /// 任务内容
            /// </summary>
            public IPropertyChain TaskContent { get { return LinkPrimitiveProperty(PROP_TASK_CONTENT); } }
	
            /// <summary>
            /// 开始时间
            /// </summary>
            public IPropertyChain StartTime { get { return LinkPrimitiveProperty(PROP_START_TIME); } }
	
            /// <summary>
            /// 结束时间
            /// </summary>
            public IPropertyChain EndTime { get { return LinkPrimitiveProperty(PROP_END_TIME); } }
	
            /// <summary>
            /// 工时
            /// </summary>
            public IPropertyChain WorkTime { get { return LinkPrimitiveProperty(PROP_WORK_TIME); } }
	
            /// <summary>
            /// 工作任务状态唯一标识
            /// </summary>
			public EtyTaskState.TaskStatePropertyDescriptor TaskState { get { return LinkForeignReferenceProperty<EtyTaskState.TaskStatePropertyDescriptor>(PROP_TASK_STATE); } }
	
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
        /// 工作任务表
        /// </summary>
        public static EtyTask.TaskPropertyDescriptor Task { get { return new EtyTask.TaskPropertyDescriptor(); } }
    }
}