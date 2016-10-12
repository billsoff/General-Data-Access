#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����TabularWriter.cs
// �ļ���������������д����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110510
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// ����д����
    /// </summary>
    public class TabularWriter
    {
        #region ˽���ֶ�

        private readonly Int32 m_columnCount;

        private Int32? m_cellPaddings;
        private Int32 m_cellTextMaxLength;
        private String m_indent;

        private readonly List<String[]> m_lines = new List<String[]>();
        private readonly Int32[] m_maxCellWidth;

        private const Int32 DEFAULT_CELL_MAX_LENGTH = 255;

        // Ĭ�ϵĵ�Ԫ���հף��˸��հ׷���
        private const Int32 DEFAULT_CELL_PADDINGS = 8;

        #endregion

        #region ���캯��

        /// <summary>
        /// ���캯��������������
        /// </summary>
        /// <param name="columnCount">������</param>
        public TabularWriter(Int32 columnCount)
            : this(columnCount, DEFAULT_CELL_MAX_LENGTH, null)
        {
        }

        /// <summary>
        /// ���캯������������������������
        /// </summary>
        /// <param name="columnCount">������</param>
        /// <param name="indent">����������</param>
        public TabularWriter(Int32 columnCount, String indent)
            : this(columnCount, DEFAULT_CELL_MAX_LENGTH, indent)
        {
        }

        /// <summary>
        /// ���캯��������������ÿ����󳤶ȡ�
        /// </summary>
        /// <param name="columnCount">������</param>
        /// <param name="cellTextMaxLength">ÿ����󳤶ȡ�</param>
        public TabularWriter(Int32 columnCount, Int32 cellTextMaxLength)
            : this(columnCount, cellTextMaxLength, null)
        {
        }

        /// <summary>
        /// ���캯��������������ÿ����󳤶�������������
        /// </summary>
        /// <param name="columnCount">������</param>
        /// <param name="cellTextMaxLength">ÿ����󳤶ȡ�</param>
        /// <param name="indent">����������</param>
        public TabularWriter(Int32 columnCount, Int32 cellTextMaxLength, String indent)
        {
            m_columnCount = columnCount;
            m_cellTextMaxLength = cellTextMaxLength;
            m_indent = indent;

            m_maxCellWidth = new Int32[columnCount];
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ�м�հס�
        /// </summary>
        public Int32 CellPaddings
        {
            get
            {
                if (m_cellPaddings == null)
                {
                    m_cellPaddings = DEFAULT_CELL_PADDINGS;
                }

                return m_cellPaddings.Value;
            }

            set { m_cellPaddings = value; }
        }

        /// <summary>
        /// ��ȡ��Ԫ���ı�����󳤶ȣ�������ı������ضϡ�
        /// </summary>
        public Int32 CellTextMaxLength
        {
            get { return m_cellTextMaxLength; }
            set { m_cellTextMaxLength = value; }
        }

        /// <summary>
        /// ��ȡ������
        /// </summary>
        public Int32 ColumnCount
        {
            get { return m_columnCount; }
        }

        /// <summary>
        /// ��ȡ����������
        /// </summary>
        public String Indent
        {
            get { return m_indent; }
            set { m_indent = value; }
        }

        #endregion

        #region ��������

        /// <summary>
        /// �������С�
        /// </summary>
        /// <returns>���С�</returns>
        public String[] NewLine()
        {
            return new String[m_columnCount];
        }

        /// <summary>
        /// д���С�
        /// </summary>
        /// <param name="line">�С�</param>
        public void WriteLine(String[] line)
        {
            for (Int32 i = 0; i < m_columnCount; i++)
            {
                String cellContent = line[i];
                Int32 cellTextWidth = GetWidth(cellContent);
                Int32 cellMaxWidth = m_maxCellWidth[i];

                Int32 width = Math.Min(cellTextWidth, CellTextMaxLength);
                m_maxCellWidth[i] = Math.Max(width, cellMaxWidth);
            }

            m_lines.Add(line);
        }

        /// <summary>
        /// ��ȡ�����
        /// </summary>
        /// <returns>�����</returns>
        public override String ToString()
        {
            if (m_lines.Count == 0)
            {
                return String.Empty;
            }

            StringBuilder output = new StringBuilder();

            // ��ȡ�ܿ��
            Int32 totalWidth = GetTotalWidth();

            // �������
            Print(output, m_lines[0]);

            String ruler = String.Empty.PadRight(totalWidth, '-');

            // ����ָ���
            if (m_indent != null)
            {
                ruler = m_indent + ruler;
            }

            output.AppendLine(ruler);

            const Int32 GROUP_SIZE = 3;

            // �������
            if (m_lines.Count > 1)
            {
                for (Int32 i = 1; i < m_lines.Count; i++)
                {
                    if ((i > 1) && ((i - 1) % GROUP_SIZE) == 0)
                    {
                        output.AppendLine(ruler);
                    }

                    Print(output, m_lines[i]);
                }

                output.AppendLine(ruler);

                if (m_indent != null)
                {
                    output.Append(m_indent);
                }

                output.AppendLine(String.Format("�� {0} ��", (m_lines.Count - 1).ToString("#,##0")));
            }

            return output.ToString();
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ�ܿ�ȡ�
        /// </summary>
        /// <returns>�ܿ�ȡ�</returns>
        private Int32 GetTotalWidth()
        {
            Int32 totalWidth = CellPaddings * (m_columnCount - 1);

            for (Int32 i = 0; i < m_maxCellWidth.Length; i++)
            {
                totalWidth += m_maxCellWidth[i];
            }

            return totalWidth;
        }

        /// <summary>
        /// ��ȡ�ı��Ŀ�ȣ�����Ϊ 2����ĸΪ 1����
        /// </summary>
        /// <param name="input">���롣</param>
        /// <returns>�ı��Ŀ�ȡ�</returns>
        private Int32 GetWidth(String input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return 0;
            }

            const Int32 MIN = 0x0100; // 0x4E00 ���ֵ�Ǻ��ֵ��ϱ߽�
            const Int32 MAX = 0xFF20; // 0x952F ���ֵ�Ǻ��ֵ��ϱ߽�

            Int32 width = 0;
            Char[] charValues = input.ToCharArray();

            for (Int32 i = 0; i < charValues.Length; i++)
            {
                UInt16 code = (UInt16)charValues[i];

                if ((MIN <= code) && (code <= MAX))
                {
                    width += 2;
                }
                else
                {
                    width += 1;
                }
            }

            return width;
        }

        /// <summary>
        /// ��ӡ�С�
        /// </summary>
        /// <param name="output">������塣</param>
        /// <param name="line">�С�</param>
        private void Print(StringBuilder output, String[] line)
        {
            for (Int32 i = 0; i < m_columnCount; i++)
            {
                if ((i == 0) && !String.IsNullOrEmpty(m_indent))
                {
                    output.Append(m_indent);
                }

                if (i != 0)
                {
                    output.Append(String.Empty.PadRight(CellPaddings));
                }

                String content = line[i];

                if (content == null)
                {
                    content = String.Empty;
                }
                else
                {
                    content = Regex.Replace(content, @"\r?\n", "  ");
                }

                while ((GetWidth(content) > CellTextMaxLength) && (content.Length > 0))
                {
                    content = content.Substring(0, (content.Length - 1));
                }

                Int32 width = GetWidth(content);

                output.Append(content + String.Empty.PadRight(Math.Max((m_maxCellWidth[i] - width), 0)));
            }

            output.AppendLine();
        }

        #endregion
    }
}