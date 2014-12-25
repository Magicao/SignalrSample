using System.IO;
using System.Web.Script.Serialization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Root.Reports;
using SignalrSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.DynamicLinq;
using Font = iTextSharp.text.Font;

namespace SignalrSample.Controllers
{
    public class HomeController : ControllerBase
    {        
        private static List<CenterModel> centerList = new List<CenterModel>();
        private ExportPdf exportPdf;
        public HomeController()
        {
            if (centerList.Count == 0)
            {
                for (var i = 1; i <= 100; i++)
                {
                    centerList.Add(new CenterModel()
                    {
                        CenterId = i,
                        CenterName = "CenterName" + i,
                        CenterAddress = "Address" + i,
                        Quota = i + 100,
                        CreateOn = new DateTime(2014,11,10)
                    });
                }
            }
            exportPdf = new ExportPdf();
            //exportPdf = new ExportPdf<CenterModel>(centerList);

//            string json = @"{'CenterName':'ccc','Quota':1,'CreateOn':'\/Date(1299456000000)\/',
//                 'CenterAddress':'address'}";
//            var serializer = new JavaScriptSerializer();
//            var model = serializer.Deserialize<CenterModel>(json);
//            model.CenterAddress = "cc";
        }
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;            
            return View();
        }

        public ActionResult HubChat()
        {
            ViewBag.ClientName = "用户-" +  new Random().Next(1000, 9999);
            return View();
        }

        public ActionResult KendoGridDemo()
        {
            return View();
        }

        public ActionResult GetCenterList(SearchBase model)
        {            
            //aggregates = new List<Aggregator>()
            //{
            //    new Aggregator(){
            //        Field="Quota",
            //        Aggregate = "sum"
            //    }
            //};
            IQueryable<CenterModel> classesIQue = (from c in centerList
                                                   select new CenterModel()
                                                   {
                                                       CenterId = c.CenterId,
                                                       CenterName = c.CenterName,
                                                       CenterAddress = c.CenterAddress,
                                                       Quota = c.Quota,
                                                       CreateOn = c.CreateOn
                                                   }).AsQueryable();

            //var centerArray = new[]
            //{
            //    new CenterModel() {CenterId = 1, CenterName = "aa", Quota = 10,CreateOn = new DateTime(2014, 11, 9)},
            //    new CenterModel() {CenterId = 2, CenterName = "bb",Quota = 20, CreateOn = new DateTime(2014, 11, 10)}
            //};

            var result = classesIQue.ToDataSourceResult(model.Take, model.Skip, model.Sort, model.Filter, model.Aggregate);
            model.TotailRecords = result.Total;
            //return
            //    this.DataList<CenterModel>(
            //        () =>
            //            new ResultBase<CenterModel>()
            ////            {
            //                DataList = result.Data as IEnumerable<CenterModel>,
            //                IsSuccessful = true,
            //                //Aggregates = result.Aggregates,
            //                TotailRecords = result.Total
            //            });
            return this.DataList(() => result.Data as IEnumerable<CenterModel>, model);
        }

        public ActionResult CreateCenter(CenterModel model)
        {
            if(!string.IsNullOrEmpty(model.CenterName) && !string.IsNullOrEmpty(model.CenterAddress))
            {
                model.CenterId = centerList.Max(c => c.CenterId) + 1;
                model.CreateOn = DateTime.Now;
                centerList.Add(model);
                return this.DataView(() => new ResultBase() {IsSuccessful = true, Message = "Add Successful"});
            }

            return this.DataView(() => new ResultBase() { IsSuccessful = false, Message = "Parameters can not be empty" });
        }

        [HttpPost]
        public ActionResult GetList(List<CenterModel> list, CenterModel model)
        {
            return Content("aa");
        }

        public ActionResult ExportPdf(int flag = 0)
        {
            var pdfData = new byte[0];
            switch (flag)
            {
                case 0:
                    pdfData = this.exportPdf.GetITextShapData(centerList);
                    break;
                case 1:
                    pdfData = this.exportPdf.GetPdfSharpData(centerList);
                    break;
                case 2:
                    pdfData = PdFsharp.PdfResult("This is a title", centerList);
                    break;
            }

            return this.File(pdfData, "application/pdf", "examEventTimeSheet.pdf");
        }

        public void ReportNetExport()
        {
            Report report = new Report(new PdfFormatter());
            FontDef fd = new FontDef(report, FontDef.StandardFont.Helvetica);
            FontProp fp = new FontPropMM(fd, 25);
            Page page = new Page(report);
            page.AddCB_MM(80, new RepString(fp, "Hello World!"));
            RT.ResponsePDF(report, System.Web.HttpContext.Current.Response);
        }
    }
}