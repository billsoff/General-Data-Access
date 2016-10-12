#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：TabularWriter.cs
// 文件功能描述：表格编写器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110510
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 表格编写器。
    /// </summary>
    public class TabularWriter
    {
        #region 私有字段

        private readonly Int32 m_columnCount;

        private Int32? m_cellPaddings;
        private Int32 m_cellTextMaxLength;
        private String m_indent;

        private readonly List<String[]> m_lines = new List<String[]>();
        private readonly Int32[] m_maxCellWidth;

        private const Int32 DEFAULT_CELL_MAX_LENGTH = 255;

        // 默认的单元格间空白（八个空白符）
        private const Int32 DEFAULT_CELL_PADDINGS = 8;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，设置列数。
        /// </summary>
        /// <param name="columnCount">列数。</param>
        public TabularWriter(Int32 columnCount)
            : this(columnCount, DEFAULT_CELL_MAX_LENGTH, null)
        {
        }

        /// <summary>
        /// 构造函数，设置列数，整体缩进。
        /// </summary>
        /// <param name="columnCount">列数。</param>
        /// <param name="indent">整体缩进。</param>
        public TabularWriter(Int32 columnCount, String indent)
            : this(columnCount, DEFAULT_CELL_MAX_LENGTH, indent)
        {
        }

        /// <summary>
        /// 构造函数，设置列数，每列最大长度。
        /// </summary>
        /// <param name="columnCount">列数。</param>
        /// <param name="cellTextMaxLength">每列最大长度。</param>
        public TabularWriter(Int32 columnCount, Int32 cellTextMaxLength)
            : this(columnCount, cellTextMaxLength, null)
        {
        }

        /// <summary>
        /// 构造函数，设置列数，每列最大长度与整体缩进。
        /// </summary>
        /// <param name="columnCount">列数。</param>
        /// <param name="cellTextMaxLength">每列最大长度。</param>
        /// <param name="indent">整体缩进。</param>
        public TabularWriter(Int32 columnCount, Int32 cellTextMaxLength, String indent)
        {
            m_columnCount = columnCount;
            m_cellTextMaxLength = cellTextMaxLength;
            m_indent = indent;

            m_maxCellWidth = new Int32[columnCount];
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取行间空白。
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
        /// 获取单元格文本的最大长度，其余的文本将被截断。
        /// </summary>
        public Int32 CellTextMaxLength
        {
            get { return m_cellTextMaxLength; }
            set { m_cellTextMaxLength = value; }
        }

        /// <summary>
        /// 获取列数。
        /// </summary>
        public Int32 ColumnCount
        {
            get { return m_columnCount; }
        }

        /// <summary>
        /// 获取整体缩进。
        /// </summary>
        public String Indent
        {
            get { return m_indent; }
            set { m_indent = value; }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 创建新行。
        /// </summary>
        /// <returns>新行。</returns>
        public String[] NewLine()
        {
            return new String[m_columnCount];
        }

        /// <summary>
        /// 写入行。
        /// </summary>
        /// <param name="line">行。</param>
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
        /// 获取输出。
        /// </summary>
        /// <returns>输出。</returns>
        public override String ToString()
        {
            if (m_lines.Count == 0)
            {
                return String.Empty;
            }

            StringBuilder output = new StringBuilder();

            // 获取总宽度
            Int32 totalWidth = GetTotalWidth();

            // 输出标题
            Print(output, m_lines[0]);

            String ruler = String.Empty.PadRight(totalWidth, '-');

            // 输出分隔线
            if (m_indent != null)
            {
                ruler = m_indent + ruler;
            }

            output.AppendLine(ruler);

            const Int32 GROUP_SIZE = 3;

            // 输出内容
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

                output.AppendLine(String.Format("共 {0} 行", (m_lines.Count - 1).ToString("#,##0")));
            }

            return output.ToString();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取总宽度。
        /// </summary>
        /// <returns>总宽度。</returns>
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
        /// 获取文本的宽度（汉字为 2，字母为 1）。
        /// </summary>
        /// <param name="input">输入。</param>
        /// <returns>文本的宽度。</returns>
        private Int32 GetWidth(String input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return 0;
            }

            const Int32 MIN = 0x0100; // 0x4E00 这个值是汉字的上边界
            const Int32 MAX = 0xFF20; // 0x952F 这个值是汉字的上边界

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
        /// 打印行。
        /// </summary>
        /// <param name="output">输出缓冲。</param>
        /// <param name="line">行。</param>
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