#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NativeAttribute.cs
// �ļ�������������������������ϣ�ָʾ�����������ݿ����ɣ�ֻ��Ӧ����ֻ��һ�������õ�������ʵ�塣
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
    /// <summary>
    /// ��������������ϣ�ָʾ�����������ݿ����ɣ�ֻ��Ӧ����ֻ��һ�������õ�������ʵ�塣
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class NativeAttribute : Attribute
    {
        #region ˽���ֶ�

        private Boolean m_autoIncrement = true;
        private String m_retrieveIdentifierStatement;

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾ�����Ƿ�Ϊ�������ֶΣ�Ĭ��ֵΪ true��
        /// </summary>
        public Boolean AutoIncrement
        {
            get { return m_autoIncrement; }
            set { m_autoIncrement = value; }
        }

        /// <summary>
        /// ��ȡ���������ڻ�ȡ����������ʶ���ֶ�ֵ�� SQL ָ�
        /// </summary>
        public String RetrieveIdentifierStatement
        {
            get { return m_retrieveIdentifierStatement; }

            set
            {
                if (value != null)
                {
                    value = value.Trim();
                    value = value.TrimEnd(new Char[] { ';' });
                }

                m_retrieveIdentifierStatement = value;
            }
        }

        #endregion
    }
}