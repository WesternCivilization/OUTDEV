using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Oze.Models;
using Oze.Models.StoreModel;
using Oze.Services.StoreManagerService;

namespace Oze.Controllers
{
    public class MinibarManagerController : BaseController
    {
        // GET: MinibarManager

        private readonly IStoreManagerService _service;

        public MinibarManagerController()
        {
            _service = new StoreManagerService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int length, int start, string search)
        {
            int total;
            var data = _service.SearchMinibar(new PagingModel { offset = start, limit = length, search = search }, out total);
            int recordsTotal = total;
            int recordsFiltered = recordsTotal;
            int draw = 1;
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

        public PartialViewResult ShowModal(int id, string action)
        {
            var model = new MinibarModel
            {
                ListRooms = _service.GetRoomByHotel(),
                Action = action
            };
            if (id > 0 && action != "Add")
            {
                var store = _service.GetItem(id);
                model.ListProduct = _service.GetProductAllByStore(id);
                model.RoomId = store.roomid ?? 0;
                model.Name = store.title;
                model.Id = store.Id;
                return PartialView("_ptvDetail", model);
            }
            model.ListProductsAdd = _service.GetProductByStore();
            return PartialView("_ptvAdd", model);

        }

        public ActionResult Update(MinibarModel model, string values)
        {
            try
            {
                if (string.IsNullOrEmpty(values) || string.IsNullOrEmpty(model.Name) || model.RoomId == 0)
                {
                    return Json(new { result = -1 }, JsonRequestBehavior.AllowGet);
                }
                var lstPro = JsonConvert.DeserializeObject<List<Item>>(values);
                model.Item = lstPro.Where(x => x.ProductId != 0).ToList();
                var rs = _service.InsertOrUpdateStore(model);
                return Json(new { result = rs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Delete(string id)
        {
            var result = _service.DeleteStore(int.Parse(id));
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
    }
}