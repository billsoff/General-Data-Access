#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����UpdateAction.cs
// �ļ�����������ʵ����¶�����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110223
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
    /// ʵ����¶�����
    /// </summary>
    internal enum UpdateAction
    {
        /// <summary>
        /// �����¡�
        /// </summary>
        None,

        /// <summary>
        /// ���롣
        /// </summary>
        Add,

        /// <summary>
        /// �޸ġ�
        /// </summary>
        Modify,

        /// <summary>
        /// ɾ����
        /// </summary>
        Delete
    }
}