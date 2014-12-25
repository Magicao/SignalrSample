using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Ajax.Utilities;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Color = MigraDoc.DocumentObjectModel.Color;
using Image = MigraDoc.DocumentObjectModel.Shapes.Image;

namespace SignalrSample
{
    public class PdFsharp
    {
        // The MigraDoc document that represents the invoice.
        //private Document document;

        // RGB colors
        private static readonly Color TableBorder = new Color(81, 125, 192);
        private static readonly Color TableBlue = new Color(235, 240, 249);
        private static readonly Color TableGray = new Color(242, 242, 242);

        // Creates the document.
        public static Document CreateDocument()
        {
            // Create a new MigraDoc document
            var document = new Document {Info = {Title = "", Subject = "", Author = "BC"}};

            // set style
            DefineStyles(document);
            return document;
        }

        // Defines the styles used to format the MigraDoc document.
        private static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";
            style.Font.Size = 20;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            style.ParagraphFormat.SpaceAfter = 5;
            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);
            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;
            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        // Creates a page
        public static Section CreatePage(Document document,Orientation orientation = Orientation.Portrait)
        {
            // Sets the PDF Page Orientation
            document.DefaultPageSetup.Orientation = orientation;
            // Each MigraDoc document needs at least one section.
            return document.AddSection();
        }

        public static Image InsertImage(Section section, string imgPath)
        {
            imgPath = System.Web.HttpContext.Current.Server.MapPath(imgPath);
            var image = section.AddImage(imgPath);
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.TopBottom;
            return image;
        }

        public static void CreateFooterOrHeader(Section section, string msg, bool isHeader = true)
        {
            var paragraph = isHeader ? section.Headers.Primary.AddParagraph() : section.Footers.Primary.AddParagraph();
            paragraph.AddText(msg);
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
        }

        public static Table CreateTable<T>(Section section, IList<T> list)
        {
            // Create the item table
            var table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = TableBorder;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            var propertyList = TypeDescriptor.GetProperties(typeof(T), Attribute.GetCustomAttributes(typeof(T), false));
            // Before you can add a row, you must define the columns
            Column column;
            for (var n = 0; n < propertyList.Count; n++)
            {
                column = table.AddColumn(Unit.FromCentimeter(3));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = TableBlue;

            for (var i = 0; i < propertyList.Count; i++)
            {
                row.Cells[i].AddParagraph(propertyList[i].DisplayName);
                row.Cells[i].Format.Font.Bold = false;
                row.Cells[i].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[i].VerticalAlignment = VerticalAlignment.Bottom;
            }

            table.SetEdge(0, 0, propertyList.Count, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            Row newRow;
            for (var i = 0; i < list.Count; i++)
            {
                newRow = table.AddRow();
                newRow.TopPadding = 1.5;
                for (int j = 0; j < propertyList.Count; j++)
                {
                    var valueTemp = propertyList[j].GetValue(list[i]);

                    //newRow.Cells[j].Shading.Color = TableGray;
                    newRow.Cells[j].VerticalAlignment = VerticalAlignment.Center;
                    newRow.Cells[j].Format.Alignment = ParagraphAlignment.Left;
                    newRow.Cells[j].Format.FirstLineIndent = 1;
                    newRow.Cells[j].AddParagraph(valueTemp == null ? string.Empty : valueTemp.ToString());
                    table.SetEdge(0, table.Rows.Count - 2, propertyList.Count, 1, Edge.Box, BorderStyle.Single, 0.75);
                }
            }
            return table;
        }

        // Creates the table
        public static Table CreateTable(Section section, int colums, bool isPrimary = false)
        {
            var table = isPrimary ? section.Headers.Primary.AddTable() : section.AddTable();
            table.Style = "Table";
            table.Borders.Color = TableBorder;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            Column column;
            for (var n = 0; n < colums; n++)
            {
                column = table.AddColumn(Unit.FromCentimeter(2.5));
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            return table;
        }

        public static Paragraph CreateParagraph(Section section, string text, string styleName)
        {
            var paragraph = section.AddParagraph(text);
            paragraph.Style = styleName;
            return paragraph;
        }

        public static Paragraph CreatePrimaryParagraph(Section section, string text, string styleName)
        {
            var paragraph = section.Headers.Primary.AddParagraph(text);
            paragraph.Style = styleName;
            return paragraph;
        }

        public static byte[] PdfResult<T>(string title, IList<T> data)
        {
            using (var ms = new MemoryStream())
            {
                var document = CreateDocument();
                var section = CreatePage(document);
                CreateParagraph(section, title, "Header");
                CreateTable(section, data);

                return PdfRendererToBytes(document, ms);
            }
        }

        public static byte[] PdfRendererToBytes(Document document, MemoryStream ms)
        {
            var pdfRenderer = new PdfDocumentRenderer(true) {Document = document};
            pdfRenderer.RenderDocument();
            pdfRenderer.Save(ms, false);
            return ms.ToArray();
        }
    }
}

