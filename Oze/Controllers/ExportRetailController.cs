using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Oze.Models;
using Oze.Services;
using HtmlXml = NotesFor.HtmlToOpenXml;
using HtmlDocument = DocumentFormat.OpenXml.Wordprocessing;
using HiQPdf;
namespace Oze.Controllers
{
    public class ExportRetailController : Controller
    {
        //Xuất bán lẻ
        // GET: /ExportRetail/
        #region Khởi tao

        private readonly ProductService _productService;
        private readonly ExportRetailService _exportRetailService;
        public ExportRetailController()
        {
            _productService = new ProductService();
            _exportRetailService = new ExportRetailService();
        }
        #endregion
        public ActionResult Index()
        {
            var model = new ExportRetailModel
            {
                ListProducts = _productService.getAll(new PagingModel { limit = int.MaxValue, offset = 0, search = string.Empty }).ToList()
            };
            return View(model);
        }
        public ActionResult ExportWord()
        {
            ExportRetailModel model = Session["@ExportWord@"] as ExportRetailModel;
            //render view
            const string viewName = "ViewForWord";
            String html = RenderStringView(viewName, model);
            try
            {
                const string filename = "StreamTest.docx";
                const string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                using (MemoryStream generatedDocument = new MemoryStream())
                {
                    using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = package.MainDocumentPart;
                        if (mainPart == null)
                        {
                            mainPart = package.AddMainDocumentPart();
                            new HtmlDocument.Document(new HtmlDocument.Body()).Save(mainPart);
                        }
                        if (Request.Url != null)
                        {
                            HtmlXml.HtmlConverter converter = new HtmlXml.HtmlConverter(mainPart)
                            {
                                BaseImageUrl = new Uri(Request.Url.Scheme + "://" + Request.Url.Authority)
                            };
                            HtmlDocument.Body body = mainPart.Document.Body;
                            var paragraphs = converter.Parse(html);
                            foreach (OpenXmlCompositeElement t in paragraphs)
                            {
                                body.Append(t);
                            }
                        }
                        mainPart.Document.Save();
                    }
                    byte[] bytesInStream = generatedDocument.ToArray(); // simpler way of converting to array
                    generatedDocument.Close();
                    Response.Clear();
                    Response.ContentType = contentType;
                    Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                    //this will generate problems
                    Response.BinaryWrite(bytesInStream);
                    try
                    {
                        Response.End();
                    }
                    catch (Exception ex)
                    {
                        return new EmptyResult();
                        //Response.End(); generates an exception. if you don't use it, you get some errors when Word opens the file...
                    }
                    Session.Remove("@ExportWord@");
                    return new EmptyResult();
                }
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public string ExportPdf(ExportRetailModel model)
        {
            //render view
            const string viewName = "ViewForPdf";
            String html = RenderStringView(viewName, model);


            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(html, null);
            return Convert.ToBase64String(pdfBuffer);
        }

        public string RenderStringView(string viewName, ExportRetailModel model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult ViewForWord(ExportRetailModel model)
        {
            //var demo = new ExportRetailModel();
            return View(model);
        }
        public ActionResult ViewForPdf(ExportRetailModel model)
        {
            return View(model);
        }
        [HttpPost]
        public string SaveSession(ExportRetailModel model)
        {
            Session.Add("@ExportWord@", model);
            return string.Empty;
        }

        [HttpPost]
        public ActionResult SaveExRetail(ExportRetailModel model)
        {
           
            var result = _exportRetailService.InsertOrder(model);
            return Json(new {message = result != null ? "Lưu hoá đơn bán lẻ thành công" : "Lưu hoá đơn bán lẻ không thành công",status = result != null ? "00" : "01"});
        }
    }
}