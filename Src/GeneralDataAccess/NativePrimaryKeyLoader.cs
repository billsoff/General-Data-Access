#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NativePrimaryKeyLoader.cs
// �ļ���������������Ϊʵ��������ݿ����ɵ�������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110725
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// ����Ϊʵ��������ݿ����ɵ�������
    /// </summary>
    internal sealed class NativePrimaryKeyLoader
    {
        #region ˽���ֶ�

        private readonly Object m_entity;
        private readonly EntityDefinition m_definition;
        private readonly IDatabaseSession m_databaseSession;

        #endregion

        #region ���캯��

        /// <summary>
        /// ���캯��������ʵ�塢��������ݿ�Ự���档
        /// </summary>
        /// <param name="entity">ʵ�塣</param>
        /// <param name="databaseSession">���ݿ�Ự���档</param>
        internal NativePrimaryKeyLoader(Object entity, IDatabaseSession databaseSession)
        {
            m_entity = entity;
            m_definition = EntityDefinitionBuilder.Build(entity.GetType()); ;
            m_databaseSession = databaseSession;

            // TODO: ȡ���Զ�����ѡ������ֵ
            //m_definition.NativePrimaryKeyInfo.GenerateCandidateKeyValue(entity);
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��������ֵ��
        /// </summary>
        public void Load()
        {
            m_definition.NativePrimaryKeyInfo.LoadPrimaryKeyValue(m_entity, m_databaseSession);
        }

        #endregion
    }
}