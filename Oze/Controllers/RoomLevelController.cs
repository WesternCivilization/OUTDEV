using System.Collections.Generic;
using System.Web.Mvc;
using oze.data;
using Oze.Models;
using Oze.Services;
using Oze.Services.RoomLevelService;

namespace Oze.Controllers
{
    public class RoomLevelController : BaseController
    {
        // GET: RoomLevel

        private readonly IRoomLevelService _service;

        public RoomLevelController()
        {
            _service=new RoomLevelService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int length, int start, string search)
        {
            int total;
            var data = _service.Search(new PagingModel { offset = start, limit = length, search = search }, out total);
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

        public PartialViewResult Edit(string id)
        {
            var obj = _service.GetRoomTypeById(id);
            return PartialView("_ptvUpdate", obj);
        }

        public ActionResult Update(tbl_Room_Type obj)
        {
            var result = _service.UpdateOrInsertRoomType(obj);
            return Json(new {result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(string id)
        {
            int result = _service.DeleteRoomLevel(int.Parse(id));
            return Json(new {result }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetDetail(string id)
        {
            var obj = _service.GetRoomTypeById(id);
            return PartialView("_ptvDetail", obj);
        }
        public PartialViewResult GetRoom(string id)
        {
            var obj = new CommService().GetAllRoomByTypeId(int.Parse(id));
            return PartialView("_ptvRoom", obj);
        }
    }
}