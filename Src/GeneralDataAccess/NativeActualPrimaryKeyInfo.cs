#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NativeActualPrimaryKeyInfo.cs
// �ļ�������������ʵԭ��������Ϣ��
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
using System.Text;

namespace Useease.GeneralDataAccess
{
    internal sealed class NativeActualPrimaryKeyInfo : NativePrimaryKeyInfo
    {
        #region ˽���ֶ�

        private readonly EntityDefinition m_definition;
        private readonly EntityPropertyDefinition m_nativePrimaryKey;
        private readonly NativeAttribute m_nativeAttribute;
        private readonly EntityPropertyDefinition[] m_candidateKeys;

        #endregion

        #region ���캯��

        /// <summary>
        /// ���캯��������ʵ�嶨�塢ԭ���������Զ���ͺ�ѡ�����Զ��弯�ϡ�
        /// </summary>
        /// <param name="definition">ʵ�嶨�塣</param>
        /// <param name="nativePrimaryKey">ԭ���������Զ��塣</param>
        /// <param name="candidateKeys">��ѡ�����ϡ�</param>
        public NativeActualPrimaryKeyInfo(EntityDefinition definition, EntityPropertyDefinition nativePrimaryKey, EntityPropertyDefinition[] candidateKeys)
        {
            m_definition = definition;
            m_nativePrimaryKey = nativePrimaryKey;
            m_nativeAttribute = (NativeAttribute)Attribute.GetCustomAttribute(nativePrimaryKey.PropertyInfo, typeof(NativeAttribute));
            m_candidateKeys = candidateKeys;
        }

        #endregion

        /// <summary>
        /// ��ȡʵ�嶨�塣
        /// </summary>
        public override EntityDefinition Definition
        {
            get { return m_definition; }
        }

        /// <summary>
        /// ���Ƿ��� true��
        /// </summary>
        public override Boolean IsNative
        {
            get { return true; }
        }

        /// <summary>
        /// ����һ��ֵ����ֵָʾ�����Ƿ�Ϊ����������ʶ���ֶΡ�
        /// </summary>
        public override Boolean AutoIncrement
        {
            get { return m_nativeAttribute.AutoIncrement; }
        }

        /// <summary>
        /// ��ȡ���ڻ�ȡ����������ʶ���ֶε�ֵ�� SQL ָ�
        /// </summary>
        public override String RetrieveIdentifierStatement
        {
            get { return m_nativeAttribute.RetrieveIdentifierStatement; }
        }

        /// <summary>
        /// ��ȡԭ���������Զ��塣
        /// </summary>
        public override EntityPropertyDefinition NativePrimaryKey
        {
            get { return m_nativePrimaryKey; }
        }

        /// <summary>
        /// ��ȡ��ѡ�������Զ��弯�ϡ�
        /// </summary>
        public override EntityPropertyDefinition[] CandidateKeys
        {
            get { return m_candidateKeys; }
        }
    }
}