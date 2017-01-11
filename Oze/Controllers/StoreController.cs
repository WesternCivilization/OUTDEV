using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Aspose.Cells;
using oze.data;
using Oze.Models;
using Oze.Models.StoreModel;
using Oze.Services.ExportService;
using Oze.Services.StoreManagerService;

namespace Oze.Controllers
{
    public class StoreController : BaseController
    {
        // GET: Store

        private readonly IStoreManagerService _service;
        private readonly string SessctionSearch = "StoreControllerSearch";
        private readonly IExportService _export;

        public StoreController()
        {
            _service = new StoreManagerService();
            _export = new ExportService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int length, int start, string search, SearchStoreModel model)
        {
            int total;
            var data = _service.Search(new PagingModel { offset = start, limit = length, search = search }, model, out total);
            var recordsTotal = total;
            var recordsFiltered = recordsTotal;
            var draw = 1;
            var obj = new SearchStoreModel
            {
                Lenght = length,
                Start = start
            };
            Session[SessctionSearch] = obj;
            try { draw = int.Parse(Request.Params["draw"]); }
            catch
            {
                // ignored
            }
            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data
            }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Edit(string id)
        {
            //var obj = _service.GetRoomTypeById(id);
            //return PartialView("_ptvUpdate", obj);
            return null;
        }

        
        public JsonResult Delete(string id)
        {
            // int result = _service.DeleteRoomLevel(int.Parse(id));
            //return Json(new { result }, JsonRequestBehavior.AllowGet);
            return null;
        }
        public PartialViewResult GetDetail(string id)
        {
            //var obj = _service.GetRoomTypeById(id);
            //return PartialView("_ptvDetail", obj);
            return null;
        }
        public PartialViewResult GetRoom(string id)
        {
            //var obj = new CommService().GetAllRoomByTypeId(int.Parse(id));
            //return PartialView("_ptvRoom", obj);
            return null;
        }

        public ActionResult Export(string type)
        {
            var gr = "!23456@#";
            int total;
            var search = Session[SessctionSearch] as SearchStoreModel;
            var data = _service.Search(new PagingModel { offset = search.Start, limit = search.Lenght, search = search.Search }, search, out total);

            // Tạo workbook
            var workbook = new Workbook();
            var styleInGridRight = _export.StyleInGridAlignRight();
            var styleInGridLeft = _export.StyleInGridAlignLeft();
            var centerBold = _export.StyleBoldInGridCenter();
            // set width
            var widthPdf = new[] { 40, 100, 200, 100, 100, 100 };
            var widthExcel = new[] { 40, 100, 200, 100, 100, 100 };
            workbook = _export.SetWidth(workbook, type, widthExcel, widthPdf);

            // header
            workbook = _export.BindText(workbook, new CellOption("KHO TỔNG", _export.StyleTitleFile(), 0, 0, 6, 2));

            var row = 3;
            #region bind title grid
            var styleTitleGrid = _export.StyleTitleInGrid();
            // bind title
            var listTitle = new List<CellOption>
            {
                new CellOption("ID", styleTitleGrid, 0, row),
                new CellOption("Mã sản phẩm", styleTitleGrid, 1, row),
                new CellOption("Tên sản phẩm/Nhóm dịch vụ", styleTitleGrid, 2, row),
                new CellOption("Đơn vị", styleTitleGrid, 3, row),
                new CellOption("Xuất kho", styleTitleGrid, 4, row),
                new CellOption("Tồn kho", styleTitleGrid, 5, row)
            };
            row += 1;
            workbook = _export.BindList(workbook, listTitle);
            #endregion
            #region - bind data
            var listDataGrid = new List<CellOption>();
            foreach (var item in data)
            {
                if (gr != item.GroupName)
                {
                    listDataGrid.Add(new CellOption(item.GroupName, "String", centerBold, 0, row, 6, 1));
                    row++;
                }
                listDataGrid.Add(new CellOption(item.ID.ToString("####"), "Number", styleInGridLeft, 0, row));
                listDataGrid.Add(new CellOption(item.Code, "String", styleInGridLeft, 1, row));
                listDataGrid.Add(new CellOption(item.Name, "String", styleInGridLeft, 2, row));
                listDataGrid.Add(new CellOption(item.UnitName, "String", styleInGridLeft, 3, row));
                listDataGrid.Add(new CellOption(item.XuatKho.ToString(), "Number", styleInGridRight, 4, row));
                listDataGrid.Add(new CellOption(item.TonKho.ToString(), "Number", styleInGridRight, 5, row));
                row++;
                gr = item.GroupName;
            }
            workbook = _export.BindList(workbook, listDataGrid);

            _export.ExportFileMargin(Response, workbook, type, "KhoTong", new Double?[] { 1, 0.2, 1, 0.2 });
            return new EmptyResult();
        }
        #endregion
    }
}