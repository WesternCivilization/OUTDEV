using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using Aspose.Cells;

namespace Oze.Services.ExportService
{
    public class ExportService : IExportService
    {

        #region Style In Export

        /// <summary>
        /// Tạo 1 style
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public Style NewStyle(StyleOption option)
        {
            var style = new Style();
            style.Font.Name = "Times New Roman";

            // Size
            style.Font.Size = option.Size ?? 9;
            // Wrapped
            style.IsTextWrapped = option.Wrapped ?? true;
            // align
            var align = option.Align == "Center" ? TextAlignmentType.Center :
                        option.Align == "Right" ? TextAlignmentType.Right : TextAlignmentType.Left;
            style.HorizontalAlignment = align;
            // vAlign
            var vAlign = option.Align == "Bottom" ? TextAlignmentType.Bottom :
                        option.Align == "Top" ? TextAlignmentType.Top : TextAlignmentType.Center;
            style.VerticalAlignment = vAlign;
            // Bold
            style.Font.IsBold = option.Bold ?? false;
            // Border
            if (option.Border == null || option.Border == true)
            {
                style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.TopBorder].Color = Color.Black;
                style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.BottomBorder].Color = Color.Black;
                style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.LeftBorder].Color = Color.Black;
                style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.RightBorder].Color = Color.Black;
            }
            return style;
        }
        /// <summary>
        /// StyleTitleGrid - style chung của title trong grid
        /// </summary>
        /// <returns></returns>
        public Style StyleTitleGrid()
        {
            return NewStyle(new StyleOption
            {
                Align = "Center",
                VerticalAlign = "Center",
                Size = 9,
                Border = true,
                Wrapped = true,
                Bold = true
            });
        }
        /// <summary>
        /// StyleTitleFile - Style chung của phần title trong grid
        /// </summary>
        /// <returns></returns>
        public Style StyleTitleFile()
        {
            return NewStyle(new StyleOption
            {
                Align = "Center",
                VerticalAlign = "Center",
                Size = 14,
                Border = false,
                Wrapped = true,
                Bold = true
            });
        }
        /// <summary>
        /// Style trong grid căn giữa
        /// </summary>
        public Style StyleInGridAlignCenter()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Center",
                Bold = false,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style trong grid căn trái
        /// </summary>
        public Style StyleInGridAlignLeft()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Left",
                Bold = false,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style trong grid  căn phải
        /// </summary>
        public Style StyleInGridAlignRight()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Right",
                Bold = false,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style căn giữa - ko border
        /// </summary>
        public Style StyleAlignCenter()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Center",
                Bold = false,
                Border = false,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style căn trái - ko border
        /// </summary>
        public Style StyleAlignLeft()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Left",
                Bold = false,
                Border = false,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style căn phải - ko border
        /// </summary>
        public Style StyleAlignRight()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Right",
                Bold = false,
                Border = false,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style border, đậm, căn giữa
        /// </summary>
        public Style StyleTitleInGrid()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Center",
                Bold = true,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style border, đậm, căn giữa
        /// </summary>
        public Style StyleBoldInGridCenter()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Center",
                Bold = true,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style border, đậm, căn trái
        /// </summary>
        public Style StyleBoldInGridLeft()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Left",
                Bold = true,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style border, đậm, căn phải
        /// </summary>
        public Style StyleBoldInGridRight()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Right",
                Bold = true,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style ko border, đậm, căn phải
        /// </summary>
        public Style StyleBoldCenter()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Center",
                Bold = true,
                Border = false,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style border, đậm, căn trái
        /// </summary>
        public Style StyleBoldLeft()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Left",
                Bold = true,
                Border = false,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }
        /// <summary>
        /// Style border, đậm, căn phải
        /// </summary>
        public Style StyleBoldRight()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Right",
                Bold = true,
                Border = false,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }

        public Style StyleBindSearch()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Center",
                Bold = false,
                Border = false,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }

        public Style StyleTotalAmount()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Right",
                Bold = true,
                Border = true,
                Size = 9,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }

        public Style StyleLeftBold()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Left",
                Bold = true,
                Border = true,
                Size = 10,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }

        public Style StyleCenterBold()
        {
            return NewStyle(new StyleOption()
            {
                Align = "Center",
                Bold = true,
                Border = true,
                Size = 10,
                VerticalAlign = "Center",
                Wrapped = true
            });
        }

        public Style SetSizeStyle(Style style, int size)
        {
            style.Font.Size = size;
            return style;
        }
        #endregion

        #region New CellOption

        /// <summary>
        /// Tạo 1 cell
        /// </summary>
        /// <returns></returns>
        public CellOption NewCellOption(CellOption cell)
        {
            var newCell = new CellOption()
            {
                AutoFitRow = cell.AutoFitRow,
                Column = cell.Column,
                Merge = cell.Merge,
                RequiredType = cell.RequiredType,
                Row = cell.Row,
                Style = cell.Style,
                Text = cell.Text,
                TotalRow = cell.TotalRow,
                Type = cell.Type,
                TotalColumn = cell.TotalColumn
            };

            return newCell;
        }


        #endregion

        #region Bind Header

        /// <summary>
        /// Tạo 1 cell
        /// </summary>
        /// <returns></returns>
        public Workbook BindHeader(Workbook workbook, string title, int column)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            var list = new List<CellOption>();
            var cell1 = new CellOption()
            {
                Merge = true,
                Style = StyleTitleFile(),
                RequiredType = false,
                Type = "String",
                Row = 0,
                Column = 0,
                TotalRow = 1,
                TotalColumn = 3,
                AutoFitRow = true,
                Text = "SYSTEM LUMITEL"
            };
            list.Add(cell1);
            var cell2 = NewCellOption(cell1);
            cell2.Column = 3;
            cell2.Text = title.ToUpper();
            cell2.TotalColumn = column - 3;
            list.Add(cell2);
            workbook = BindList(workbook, list);
            worksheet.Cells.SetRowHeightPixel(0, 50);

            return workbook;
        }

        /// <summary>
        /// tạo hearder
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cellSystem"></param>
        /// <param name="cellTitle"></param>
        /// <returns></returns>
        public Workbook BindHeaderCustomer(Workbook workbook, CellOption cellSystem, CellOption cellTitle)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            var list = new List<CellOption>();
            cellSystem.Style = StyleTitleFile();
            cellSystem.Text = string.IsNullOrEmpty(cellSystem.Text) ? "Lumitel" : cellSystem.Text;
            list.Add(cellSystem);
            if (cellTitle != null)
            {
                cellTitle.Style = StyleTitleFile();
                list.Add(cellTitle);
                workbook = BindList(workbook, list);
            }
            worksheet.Cells.SetRowHeightPixel(cellSystem.Row, 50);
            return workbook;
        }
        public Workbook BindHeaderCustomer(Workbook workbook, CellOption cellSystem, CellOption cellTitle, int height)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            var list = new List<CellOption>();
            cellSystem.Style = StyleTitleFile();
            cellSystem.Text = string.IsNullOrEmpty(cellSystem.Text) ? "Lumitel" : cellSystem.Text;
            list.Add(cellSystem);
            if (cellTitle != null)
            {
                cellTitle.Style = StyleTitleFile();
                list.Add(cellTitle);
                workbook = BindList(workbook, list);
            }
            worksheet.Cells.SetRowHeightPixel(cellSystem.Row, height);
            return workbook;
        }
        public Workbook BindText(Workbook workbook, CellOption cellOption, int height)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            // Merge
            if (cellOption.Merge)
                worksheet.Cells.Merge(cellOption.Row, cellOption.Column, cellOption.TotalRow, cellOption.TotalColumn);
            worksheet.Cells.SetRowHeightPixel(cellOption.Row, height);
            // PutValue và Style
            worksheet = PutValueInType(worksheet, cellOption);

            // return
            return workbook;
        }

        /// <summary>
        /// Set width theo Cells
        /// </summary>
        /// width của PDF ~ 682 đối với chiều dọc - margin 1
        /// width của PDF ~ 740 đối với chiều dọc - margin left 0.2 - right 0.2
        /// width của PDF ~ 997 đối với chiều ngang - margin 1
        /// width của PDF ~ 1052 đối với chiều ngang - margin left 0.2 - right 0.2
        public Workbook SetWidth(Workbook workbook, string export, int[] widthExcel, int[] widthPfd)
        {
            Worksheet worksheet = workbook.Worksheets[0];

            if (!string.IsNullOrEmpty(export) && export.ToLower() == "excel")
                for (int i = 0; i < widthExcel.Length; i++)
                {
                    worksheet.Cells.SetColumnWidthPixel(i, widthExcel[i]);
                }
            else
                for (int i = 0; i < widthPfd.Length; i++)
                {
                    worksheet.Cells.SetColumnWidthPixel(i, widthPfd[i]);
                }

            return workbook;
        }

        #endregion

        #region Bind text

        /// <summary>
        /// Bind dữ liệu vào 1 cell trong grid
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cellOption"></param>
        /// <remarks>LongLD</remarks>
        public Workbook BindText(Workbook workbook, CellOption cellOption)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            // Merge
            if (cellOption.Merge)
                worksheet.Cells.Merge(cellOption.Row, cellOption.Column, cellOption.TotalRow, cellOption.TotalColumn);
            // PutValue và Style
            worksheet = PutValueInType(worksheet, cellOption);
            // return
            return workbook;
        }

        /// <summary>
        /// PutValue theo định dạng
        /// </summary>
        /// <remarks>LongLD</remarks>
        private Worksheet PutValueInType(Worksheet worksheet, CellOption cellOption)
        {
            switch (cellOption.Type.ToLower())
            {
                case "date":
                    var date = !string.IsNullOrEmpty(cellOption.Text) ?
                        Convert.ToDateTime(cellOption.Text, CultureInfo.GetCultureInfo("vi-vn")) :
                        new DateTime();
                    worksheet = PutValue(worksheet, cellOption, cellOption.Text, date);
                    cellOption.Style.Custom = "dd/MM/yyyy";
                    break;
                case "datetime":
                    var dateT = !string.IsNullOrEmpty(cellOption.Text) ?
                        Convert.ToDateTime(cellOption.Text, CultureInfo.GetCultureInfo("vi-vn")) :
                        new DateTime();
                    worksheet = PutValue(worksheet, cellOption, cellOption.Text, dateT);
                    cellOption.Style.Custom = "dd/MM/yyyy HH:ss";
                    break;
                case "datetimefull":
                    var dateTf = !string.IsNullOrEmpty(cellOption.Text) ?
                        Convert.ToDateTime(cellOption.Text, CultureInfo.GetCultureInfo("vi-vn")) :
                        new DateTime();
                    worksheet = PutValue(worksheet, cellOption, cellOption.Text, dateTf);
                    cellOption.Style.Custom = "dd/MM/yyyy  HH:mm:ss";
                    break;
                case "number":
                    var n = !string.IsNullOrEmpty(cellOption.Text) ?
                        decimal.Parse(cellOption.Text) : 0;
                    worksheet = PutValue(worksheet, cellOption, cellOption.Text, n);
                    cellOption.Style.Custom = "#,##0";
                    break;
                case "numbern2":
                    var n2 = !string.IsNullOrEmpty(cellOption.Text) ?
                        decimal.Parse(cellOption.Text, CultureInfo.GetCultureInfo("en-us")) : 0;
                    worksheet = PutValue(worksheet, cellOption, cellOption.Text, n2);
                    cellOption.Style.Custom = "#,##0.00";
                    break;
                default:
                    cellOption.Style.Number = '@';
                    worksheet.Cells[cellOption.Row, cellOption.Column].PutValue(cellOption.Text);
                    break;
            }
            if (cellOption.Merge)
            {
                if (cellOption.AutoFitRow)
                    worksheet.AutoFitRow(cellOption.Row, cellOption.Row + cellOption.TotalRow, cellOption.Column, cellOption.Column + cellOption.TotalColumn);
                for (int i = 0; i < cellOption.TotalRow; i++)
                {
                    for (int j = 0; j < cellOption.TotalColumn; j++)
                    {
                        worksheet.Cells[cellOption.Row + i, cellOption.Column + j].SetStyle(cellOption.Style);
                    }
                }
            }
            else
            {
                if (cellOption.AutoFitRow)
                    worksheet.AutoFitRow(cellOption.Row, cellOption.Row + 1, cellOption.Column, cellOption.Column + 1);
                worksheet.Cells[cellOption.Row, cellOption.Column].SetStyle(cellOption.Style);
            }

            return worksheet;
        }

        /// <summary>
        /// Putvalue theo kiểu hay ko 
        /// </summary>
        /// <remarks>LongLD</remarks>
        private Worksheet PutValue<T>(Worksheet worksheet, CellOption cellOption, string value, T data)
        {
            // ko rỗng => show
            if (!string.IsNullOrEmpty(value))
                worksheet.Cells[cellOption.Row, cellOption.Column].PutValue(data);
            else
            {
                // rỗng nhưng lại buộc theo kiểu dữ liệu => show
                if (cellOption.RequiredType)
                    worksheet.Cells[cellOption.Row, cellOption.Column].PutValue(data);
                // rỗng nhưng lại ko buộc theo kiểu dữ liệu => ko show
                else
                    worksheet.Cells[cellOption.Row, cellOption.Column].PutValue("");
            }
            return worksheet;
        }
        /// <summary>
        /// Putvalue theo 1 list cellOption
        /// </summary>
        /// <remarks>LongLD</remarks>
        public Workbook BindList(Workbook workbook, List<CellOption> listcellOption)
        {
            foreach (var item in listcellOption)
            {
                workbook = BindText(workbook, item);
            }
            return workbook;
        }

        /// <summary>
        /// Set column cho 1 list cellOption chưa có column
        /// </summary>
        /// <param name="cellOption"></param>
        /// <param name="row"></param>
        /// <param name="columnBegin"></param>
        /// <returns></returns>
        public List<CellOption> SetColumInRow(List<CellOption> cellOption, int row, int columnBegin)
        {
            var list = new List<CellOption>();
            var i = 0;
            foreach (var item in cellOption)
            {
                var c = NewCellOption(item);
                c.Row = row;
                c.Column = i + columnBegin;
                list.Add(c);
                i++;
            }
            return list;
        }

        #endregion

        #region End - xuất file

        /// <summary>
        /// cấu hình Xuất file - xuất ngang
        /// </summary>
        /// <remarks>LongLD</remarks>
        public void ExportFileLandscape(HttpResponseBase response, Workbook workbook, string export, string fileName)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            System.IO.Stream stream = response.OutputStream;
            if (export != null && export.ToLower() == "excel")
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".xls");
                response.Clear();
                response.BufferOutput = true;
                workbook.Save(stream, SaveFormat.Excel97To2003);
            }
            else
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".pdf");
                response.Clear();
                response.BufferOutput = true;
                worksheet.PageSetup.LeftMargin = 1;
                worksheet.PageSetup.RightMargin = 1;
                worksheet.PageSetup.TopMargin = 1;
                worksheet.PageSetup.BottomMargin = 1;
                worksheet.PageSetup.Orientation = PageOrientationType.Landscape;
                workbook.Save(stream, SaveFormat.Pdf);
            }
            stream.Close();
            response.Flush();
        }
        /// <summary>
        /// cấu hình Xuất file - xuất ngang
        /// config margin
        /// </summary>
        /// <remarks>LongLD</remarks>
        public void ExportFileLandscapeMargin(HttpResponseBase response, Workbook workbook, string export, string fileName, Double?[] margin)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            System.IO.Stream stream = response.OutputStream;
            if (export != null && export.ToLower() == "excel")
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".xls");
                response.Clear();
                response.BufferOutput = true;
                workbook.Save(stream, SaveFormat.Excel97To2003);
            }
            else
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".pdf");
                response.Clear();
                response.BufferOutput = true;
                worksheet.PageSetup.LeftMargin = margin[3] ?? 1;
                worksheet.PageSetup.RightMargin = margin[1] ?? 1;
                worksheet.PageSetup.TopMargin = margin[0] ?? 1;
                worksheet.PageSetup.BottomMargin = margin[2] ?? 1;
                worksheet.PageSetup.Orientation = PageOrientationType.Landscape;
                workbook.Save(stream, SaveFormat.Pdf);
            }
            stream.Close();
            response.Flush();
        }

        /// <summary>
        /// cấu hình Xuất file
        /// </summary>
        /// <remarks>LongLD</remarks>
        public void ExportFile(HttpResponseBase response, Workbook workbook, string export, string fileName)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            System.IO.Stream stream = response.OutputStream;
            if (export != null && export.ToLower() == "excel")
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".xls");
                response.Clear();
                response.BufferOutput = true;
                workbook.Save(stream, SaveFormat.Excel97To2003);
            }
            else
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".pdf");
                response.Clear();
                response.BufferOutput = true;
                worksheet.PageSetup.LeftMargin = 1;
                worksheet.PageSetup.RightMargin = 1;
                worksheet.PageSetup.TopMargin = 1;
                worksheet.PageSetup.BottomMargin = 1;
                workbook.Save(stream, SaveFormat.Pdf);
            }
            stream.Close();
            response.Flush();
        }

        /// <summary>
        /// cấu hình Xuất file
        /// </summary>
        /// <remarks>LongLD</remarks>
        public void ExportFileMargin(HttpResponseBase response, Workbook workbook, string export, string fileName, Double?[] margin)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            System.IO.Stream stream = response.OutputStream;
            if (export != null && export.ToLower() == "excel")
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".xls");
                response.Clear();
                response.BufferOutput = true;
                workbook.Save(stream, SaveFormat.Excel97To2003);
            }
            else
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-disposition", "attachment; filename=" + fileName + ".pdf");
                response.Clear();
                response.BufferOutput = true;
                worksheet.PageSetup.LeftMargin = margin[3] ?? 1;
                worksheet.PageSetup.RightMargin = margin[1] ?? 1;
                worksheet.PageSetup.TopMargin = margin[0] ?? 1;
                worksheet.PageSetup.BottomMargin = margin[2] ?? 1;
                //worksheet.PageSetup.Orientation = PageOrientationType.Landscape;
                workbook.Save(stream, SaveFormat.Pdf);
            }
            stream.Close();
            response.Flush();
        }

        #endregion

    }
    #region StyleOption - CellOption
    /// <summary>
    /// Option của cell
    /// </summary>
    public class CellOption
    {
        public CellOption() { }
        public CellOption(string text, Style style, int column, int row)
        {
            Merge = false;
            Style = style;
            RequiredType = false;
            Type = "String";
            AutoFitRow = true;
            Column = column;
            Row = row;
            Text = text;
            TotalRow = 1;
            TotalColumn = 1;
        }
        public CellOption(string text, string type, Style style, int column, int row)
        {
            Merge = false;
            Style = style;
            RequiredType = false;
            Type = type;
            Type = type;
            AutoFitRow = true;
            Column = column;
            Row = row;
            Text = text;
            TotalRow = 1;
            TotalColumn = 1;
        }
        public CellOption(string text, Style style, int column, int row, int totalColumn, int totalRow)
        {
            Merge = true;
            Style = style;
            RequiredType = false;
            Type = "String";
            AutoFitRow = true;
            Column = column;
            Row = row;
            Text = text;
            TotalRow = totalRow;
            TotalColumn = totalColumn;
        }
        public CellOption(string text, string type, Style style, int column, int row, int totalColumn, int totalRow)
        {
            Merge = true;
            Style = style;
            RequiredType = false;
            Type = type;
            AutoFitRow = true;
            Column = column;
            Row = row;
            Text = text;
            TotalRow = totalRow;
            TotalColumn = totalColumn;
        }
        /// <summary>
        /// Dữ liệu
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// buộc theo kiểu vd: number => null = 0
        /// </summary>
        public bool RequiredType { get; set; }
        /// <summary>
        /// Kiểu dữ liệu 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Style
        /// </summary>
        public Style Style { get; set; }
        /// <summary>
        /// dòng số
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// Cột số
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// Có Merge ko
        /// </summary>
        public bool Merge { get; set; }
        /// <summary>
        /// tổng số dòng Merge
        /// </summary>
        public int TotalRow { get; set; }
        /// <summary>
        /// tổng số cột Merge
        /// </summary>
        public int TotalColumn { get; set; }
        /// <summary>
        /// AutoFitRow
        /// </summary>
        public bool AutoFitRow { get; set; }

    }

    /// <summary>
    /// option của Style
    /// </summary>
    public class StyleOption
    {
        public string Align { get; set; } // Center, Right, Left
        public string VerticalAlign { get; set; } // Top, Bottom, Center
        public int? Size { get; set; }
        public bool? Bold { get; set; }
        public bool? Border { get; set; }
        public bool? Wrapped { get; set; }
    }


    #endregion
}