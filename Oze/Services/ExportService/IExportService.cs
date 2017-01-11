using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Aspose.Cells;

namespace Oze.Services.ExportService
{
    interface IExportService
    {
        Style NewStyle(StyleOption option);
        Style StyleTitleGrid();
        Style StyleTitleFile();
        Style StyleInGridAlignCenter();
        Style StyleInGridAlignLeft();
        Style StyleInGridAlignRight();
        Style StyleAlignCenter();
        Style StyleAlignLeft();
        Style StyleAlignRight();
        Style StyleTitleInGrid();
        Style StyleBoldInGridCenter();
        Style StyleBoldInGridLeft();
        Style StyleBoldInGridRight();
        Style StyleBoldCenter();
        Style StyleBoldLeft();
        Style StyleBoldRight();
        Style StyleBindSearch();
        Style StyleTotalAmount();
        Style StyleLeftBold();
        Style StyleCenterBold();

        Style SetSizeStyle(Style style, int size);
        Workbook BindHeader(Workbook workbook, string title, int column);
        Workbook BindHeaderCustomer(Workbook workbook, CellOption cellSystem, CellOption cellTitle);
        Workbook BindHeaderCustomer(Workbook workbook, CellOption cellSystem, CellOption cellTitle, int height);
        Workbook BindText(Workbook workbook, CellOption cellOption, int height);
        Workbook BindText(Workbook workbook, CellOption cellOption);
        Workbook BindList(Workbook workbook, List<CellOption> cellOption);
        CellOption NewCellOption(CellOption cell);


        Workbook SetWidth(Workbook workbook, string export, int[] widthExcel, int[] widthPfd);
        List<CellOption> SetColumInRow(List<CellOption> cellOption, int row, int columnBegin);

        void ExportFileLandscape(HttpResponseBase response, Workbook workbook, string export, string fileName);

        void ExportFileLandscapeMargin(HttpResponseBase response, Workbook workbook, string export, string fileName,
            Double?[] margin);
        void ExportFile(HttpResponseBase response, Workbook workbook, string export, string fileName);

        void ExportFileMargin(HttpResponseBase response, Workbook workbook, string export, string fileName,
            Double?[] margin);
    }
}
