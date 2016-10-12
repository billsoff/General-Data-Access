#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DemandUtility.cs
// 文件功能描述：项目需求工具方法。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120331
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
    /// 项目需求工具方法。
    /// </summary>
    [DataObject]
    public static class DemandUtility
    {
        /// <summary>
        /// 获取指定唯一标识的项目需求。
        /// </summary>
        /// <param name="demandId">项目需求的唯一标识。</param>
        /// <returns>具有该唯一标识的项目需求。</returns>
        public static EtyDemand GetDemandById(int demandId)
        {
            EtyDemand demand = DbSessions.Default.LoadFirst<EtyDemand>(
                    Where.Property(Entities.Demand.DemandId).Is.EqualTo(demandId)
                );

            if (demand != null)
            {
                EtyDemandVersion content = DbSessions.Default.LoadFirst<EtyDemandVersion>(
                        Where.Property(Entities.DemandVersion.Demand.DemandId).Is.EqualTo(demand.DemandId)
                            .And.Property(Entities.DemandVersion.DemandVersion).Is.EqualTo(demand.CurrentVersion)
                    );

                demand.DemandInfo = content;
            }

            return demand;
        }

        /// <summary>
        /// 获取指定项目的所有的需求。
        /// </summary>
        /// <param name="projectId">项目的唯一标识。</param>
        /// <returns>该项目的所有需求。</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static EtyDemand[] GetAllDemandsByProjectId(int projectId)
        {
            EtyDemand[] allDemands = DbSessions.Default.Load<EtyDemand>(
                    Where.Property(Entities.Demand.Project.ProjectId).Is.EqualTo(projectId)
                );

            if (allDemands.Length != 0)
            {
                EtyDemandVersion[] allVersions = DbSessions.Default.Load<EtyDemandVersion>(
                        Where.Property(Entities.DemandVersion.Demand.Project.ProjectId).Is.EqualTo(projectId),
                        OrderBy.Property(Entities.DemandVersion.Demand.DemandId)
                            .Then.Property(Entities.DemandVersion.DemandVersion).Descending
                    );

                foreach (var demand in allDemands)
                {
                    demand.DemandInfo = allVersions.Where(v => v.Demand.DemandId == demand.DemandId).FirstOrDefault();
                }
            }

            return allDemands;
        }

        /// <summary>
        /// 添加需求。
        /// </summary>
        /// <param name="demand">需求。</param>
        public static void AddDemand(EtyDemand demand)
        {
            DbSessions.Default.Add(demand);
        }

        /// <summary>
        /// 添加需求版本。
        /// </summary>
        /// <param name="version">需求版本。</param>
        public static void AddDemandVersion(EtyDemandVersion version)
        {
            DbSessions.Default.Add(version);
        }

        /// <summary>
        /// 修改需求。
        /// </summary>
        /// <param name="demand">需求。</param>
        public static void ModifyDemand(EtyDemand demand)
        {
            DbSessions.Default.Modify(demand);
        }
    }
}