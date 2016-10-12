#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：TaskUtility.cs
// 文件功能描述：任务工具类
//
//
// 创建标识：宋冰（13660481521@139.com） 20120401
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Uedmp.Entity;
using Useease.GeneralDataAccess;

namespace Uedmp.Business
{
    /// <summary>
    /// 任务工具类。
    /// </summary>
    [DataObject]
    public static class TaskUtility
    {
        /// <summary>
        /// 获取具有指定唯一标识的任务。
        /// </summary>
        /// <param name="taskId">任务的唯一标识。</param>
        /// <returns>任务，如果没有找到，则返回 null。</returns>
        public static EtyTask GetTaskById(string taskId)
        {
            EtyTask task = DbSessions.Default.LoadFirst<EtyTask>(Where.Property(Entities.Task.TaskId).Is.EqualTo(taskId));

            if (task != null)
            {
                EtyDemandVersion version = DbSessions.Default.LoadFirst<EtyDemandVersion>(
                        Where.Property(Entities.DemandVersion.Demand.DemandId).Is.EqualTo(task.Demand.DemandId)
                    );

                task.Demand.DemandInfo = version;
            }

            return task;
        }

        /// <summary>
        /// 获取所有的任务。
        /// </summary>
        /// <returns>所有的任务。</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static EtyTask[] GetAllTasks()
        {
            EtyTask[] allTasks = DbSessions.Default.Load<EtyTask>();

            if (allTasks.Length != 0)
            {
                EtyDemandVersion[] allVersions = DbSessions.Default.Load<EtyDemandVersion>(
                        OrderBy.Property(Entities.DemandVersion.Demand.DemandId)
                            .Then.Property(Entities.DemandVersion.DemandVersion).Descending
                    );

                foreach (EtyTask task in allTasks)
                {
                    task.Demand.DemandInfo = allVersions
                        .Where(version => version.Demand.DemandId == task.Demand.DemandId)
                        .FirstOrDefault();
                }
            }

            return allTasks;
        }

        /// <summary>
        /// 获取分配给我的所有任务。
        /// </summary>
        /// <returns>分配给我的所有任务。</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static EtyTask[] GetAllMyTasks()
        {
            if (Globals.CurrentUser == null)
            {
                throw new InvalidOperationException("当前用户为匿名用户，不能获取用户任务。");
            }

            EtyTask[] allMyTasks = DbSessions.Default.Load<EtyTask>(
                    Where.Property(Entities.Task.TaskWorker.UserId).Is.EqualTo(Globals.CurrentUser.UserId)
                );

            if (allMyTasks.Length != 0)
            {
                EtyDemandVersion[] allMyVersions = DbSessions.Default.Load<EtyDemandVersion>(
                        Where.Property(Entities.DemandVersion.Demand.DemandId)
                            .Is.InValues(
                                    Select.Property(Entities.Task.Demand.DemandId),
                                    Where.Property(Entities.Task.TaskWorker.UserId).Is.EqualTo(Globals.CurrentUser.UserId),
                                    true
                                )
                    );

                foreach (EtyTask task in allMyTasks)
                {
                    task.Demand.DemandInfo = allMyVersions.Where(version => version.Demand.DemandId == task.Demand.DemandId).FirstOrDefault();
                }
            }

            return allMyTasks;
        }

        /// <summary>
        /// 添加任务。
        /// </summary>
        /// <param name="task">要添加的任务。</param>
        public static void AddTask(EtyTask task)
        {
            DbSessions.Default.Add(task);
        }
    }
}