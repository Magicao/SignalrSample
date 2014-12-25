using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Color = MigraDoc.DocumentObjectModel.Color;

namespace SignalrSample
{
    public class PdFsharp2<T>
    {
        // The MigraDoc document that represents the invoice.
        private Document document;
        private IList<T> list;
        // The text frame of the MigraDoc document that contains the address.
        private TextFrame addressFrame;

        /// The table of the MigraDoc document that contains the data
        private Table table;

        // RGB colors
        private static readonly Color TableBorder = new Color(81, 125, 192);
        private static readonly Color TableBlue = new Color(235, 240, 249);
        private static readonly Color TableGray = new Color(242, 242, 242);
        private readonly PropertyDescriptorCollection propertyList;
        public PdFsharp2(IList<T> dataList)
        {
            list = dataList;
            propertyList = TypeDescriptor.GetProperties(typeof(T), Attribute.GetCustomAttributes(typeof(T), false));
        }

// Creates the document.
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "";
            this.document.Info.Subject = "";
            this.document.Info.Author = "aaaa";
            DefineStyles();
            CreatePage();
            FillContent();

            return this.document;
        }

// Defines the styles used to format the MigraDoc document.
        public void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";
            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("20cm", TabAlignment.Center);
            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
// Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;
// Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

// Creates the static parts of the invoice.
        private void CreatePage()
        {
            // Sets the PDF Page Orientation
            this.document.DefaultPageSetup.Orientation = Orientation.Portrait;
// Each MigraDoc document needs at least one section.
            Section section = this.document.AddSection();

// Put a logo in the header
            //Image image = section.AddImage(path);
            //image.Top = ShapePosition.Top;
            //image.Left = ShapePosition.Left;
            //image.WrapFormat.Style = WrapStyle.Through;
// Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("This is the footer section");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
// Create the text frame for the address
            this.addressFrame = section.AddTextFrame();
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "7.0cm";
            this.addressFrame.Left = ShapePosition.Left;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "5.0cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;
// Put sender in address frame
            paragraph = this.addressFrame.AddParagraph("Karachi,Pakistan");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.SpaceAfter = 3;
// Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "6cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("Patients Detail", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Date, ");
            paragraph.AddDateField("dd.MM.yyyy");
// Create the item table
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Borders.Color = TableBorder;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;
// Before you can add a row, you must define the columns
            Column column;

            for (var n = 0; n < propertyList.Count; n++)
            {
               column = this.table.AddColumn(Unit.FromCentimeter(3));
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

            this.table.SetEdge(0, 0, propertyList.Count, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

// Creates the dynamic parts of the PDF
        private void FillContent()
        {
            // Fill address in address text frame
            Paragraph paragraph = this.addressFrame.AddParagraph();
            paragraph.AddText("Dr. Anwar Ali");
            paragraph.AddLineBreak();
            paragraph.AddText("Health And Social Services ");
            paragraph.AddLineBreak();
            paragraph.AddText("Karachi");
            Row newRow;
            for (var i = 0; i < list.Count; i++)
            {
                newRow = this.table.AddRow();
                newRow.TopPadding = 1.5;
                for (int j = 0; j < propertyList.Count; j++)
                {
                    var valueTemp = propertyList[j].GetValue(list[i]);

                    //newRow.Cells[j].Shading.Color = TableGray;
                    newRow.Cells[j].VerticalAlignment = VerticalAlignment.Center;
                    newRow.Cells[j].Format.Alignment = ParagraphAlignment.Left;
                    newRow.Cells[j].Format.FirstLineIndent = 1;
                    newRow.Cells[j].AddParagraph(valueTemp == null ? string.Empty : valueTemp.ToString());
                    this.table.SetEdge(0, this.table.Rows.Count - 2, propertyList.Count, 1, Edge.Box, BorderStyle.Single, 0.75);
                }
            }
        }
    }
}

