#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NativeEmptyPrimaryKeyInfo.cs
// �ļ�������������ʾ��ԭ��������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110726
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
    /// ��ʾ��ԭ��������
    /// </summary>
    internal sealed class NativeEmptyPrimaryKeyInfo : NativePrimaryKeyInfo
    {
        #region ��̬��Ա

        /// <summary>
        /// ��һʵ����
        /// </summary>
        public static readonly NativeEmptyPrimaryKeyInfo Value = new NativeEmptyPrimaryKeyInfo();

        #endregion

        #region ���캯��

        /// <summary>
        /// ˽�л�����ά��һ��������
        /// </summary>
        private NativeEmptyPrimaryKeyInfo()
        {
        }

        #endregion

        /// <summary>
        /// ���Ƿ��� null��
        /// </summary>
        public override EntityDefinition Definition
        {
            get { return null; }
        }

        /// <summary>
        /// ���Ƿ��� false��
        /// </summary>
        public override Boolean IsNative
        {
            get { return false; }
        }

        /// <summary>
        /// ���Ƿ��� false��
        /// </summary>
        public override bool AutoIncrement
        {
            get { return false; }
        }

        /// <summary>
        /// ���Ƿ��� null��
        /// </summary>
        public override string RetrieveIdentifierStatement
        {
            get { return null; }
        }

        /// <summary>
        /// ���Ƿ��� null��
        /// </summary>
        public override EntityPropertyDefinition NativePrimaryKey
        {
            get { return null; }
        }

        /// <summary>
        /// ���Ƿ��ؾ������Ԫ�صĿ����顣
        /// </summary>
        public override EntityPropertyDefinition[] CandidateKeys
        {
            get { return new EntityPropertyDefinition[0]; }
        }
    }
}