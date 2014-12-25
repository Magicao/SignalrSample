using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SignalrSample.Models;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Paragraph = iTextSharp.text.Paragraph;

namespace SignalrSample
{
    public class ExportPdf
    {
        //private Table table;
        //private readonly IList<T> list;
        //public ExportPdf(IList<T> dataList)
        //{
        //    list = dataList;
        //}
        public byte[] GetITextShapData<T>(IList<T> list)
        {
            using (var ms = new MemoryStream())
            {
                iTextSharp.text.io.StreamUtil.AddToResourceSearch(Assembly.Load("iTextAsian"));

                // font style
                var fontTitle = new Font(Font.FontFamily.TIMES_ROMAN, 20, 1, BaseColor.BLACK);
                var fontCell = FontFactory.GetFont("Arial", 8, 1);
                Font georgia = FontFactory.GetFont("georgia", 10f);
                Font brown = FontFactory.GetFont("Arial", 9f, BaseColor.GRAY);
                Font lightblue = FontFactory.GetFont("Arial", 9f, BaseColor.BLUE);
                Font courier = FontFactory.GetFont("Arial", 9f, BaseColor.ORANGE);

                var doc = new Document(PageSize.A4, 10, 10, 5, 5);
                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var pTitle = new Paragraph("Exam Event Staff Timesheet", fontTitle);
                pTitle.Alignment = Element.ALIGN_CENTER;
                pTitle.SpacingAfter = 9f;
                doc.Add(pTitle);
                var table = new PdfPTable(8);
                table.TotalWidth = 800f;

                //table.LockedWidth = true;
                //table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                // var cell = new PdfPCell();
                table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell("Venue Name, Address:");
                table.AddCell(new PdfPCell(new Phrase("BLCU, Beijing Chaoyang(中文测试)") { Font = courier }) { Colspan = 7 });
                table.AddCell("Exam Date:");
                table.AddCell("06/09/2014");
                table.AddCell("Product Type:");
                table.AddCell("IELTS");
                table.AddCell("Exam Name:");
                table.AddCell("BEC");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("Component:");
                table.AddCell("BEC Higher Listening, Reading, Writing");
                table.AddCell("Exam Start Time:");
                table.AddCell("09:30");
                table.AddCell("Exam End Time:");
                table.AddCell("11:30");
                table.AddCell("Duration:");
                table.AddCell("60 minutes");
                table.SpacingAfter = 10f;
                doc.Add(table);

                // datalist table
                doc.Add(TextSharpHelper.CreateTable(list));

                doc.Close();
                writer.Close();
                return ms.ToArray();
            }
        }

        public byte[] GetReportNetData<T>(IList<T> list)
        {
            //Report report = new Report(new PdfFormatter());
            //FontDef fd = new FontDef(report, FontDef.StandardFont.Helvetica);
            //FontProp fp = new FontPropMM(fd, 25);
            //Page page = new Page(report);
            //page.AddCB_MM(80, new RepString(fp, "Hello World!"));
            //RT.ResponsePDF(report, System.Web.HttpContext.Current.Response);
            return null;
        }

        public byte[] GetPdfSharpData<T>(IList<T> list)
        {
            using (var ms = new MemoryStream())
            {
                //var document = new PdfSharp.Pdf.PdfDocument();
                //var document = new MigraDoc.DocumentObjectModel.Document();
                //PDFsharpHelper.DefineStyles(document);
                //var section = document.AddSection();
                //section.AddParagraph("Exam Timetable Sheet", "Heading1");
                
                //PDFsharpHelper.AddTable(section, list);
                //var pDFsharp = new PdFsharp2<T>(list);
                //var pDFsharp = new PdFsharp();
                var document = PdFsharp.CreateDocument();
                var section = PdFsharp.CreatePage(document);

                // header footer test
                // pDFsharp.CreateFooterOrHeader(section, "this is a header message");
                PdFsharp.CreateFooterOrHeader(section, "this is a footer message", false);

                // img test
                PdFsharp.InsertImage(section, "~/Content/images/dog.jpg");
                //var img = PdFsharp.InsertImage(section, "~/Content/images/dog.jpg");
                //img.Top = ShapePosition.Top;
                //img.Left = ShapePosition.Right;
                //img.Height = 150;
                //img.Width = 150;
                //img.RelativeVertical = RelativeVertical.Line;

                // paragraph test 
                PdFsharp.CreateParagraph(section, "Event Timetable Sheet", "Header");

                section.AddParagraph();

                // table test 1 and chinese test
                var table = PdFsharp.CreateTable(section, 6);
                Row row = table.AddRow();
                row.Cells[0].AddParagraph("Venue Name, Address:");
                row.Cells[1].MergeRight = 4;
                var p = row.Cells[1].AddParagraph("BLCU, Beijing Haidianqu(中文测试)");
                p.Format.Font.Name = "微软雅黑";
                row = table.AddRow();
                row.Cells[0].AddParagraph("Exam Date:");
                row.Cells[1].AddParagraph("08/11/2014");
                row.Cells[2].AddParagraph("Product Type:");
                row.Cells[3].AddParagraph("IELTS");
                row.Cells[4].AddParagraph("Exam Name:");
                row.Cells[5].AddParagraph("BEC");

                section.AddParagraph();

                // table test 2
                PdFsharp.CreateTable(section, list);
                return PdFsharp.PdfRendererToBytes(document, ms);
            }
        }
    }
}