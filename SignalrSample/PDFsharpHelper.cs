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

namespace SignalrSample
{
    public class PDFsharpHelper 
    {
        //private Section section;
        //private SizeF PdfSize;
      
        public static void DefineStyles(Document document)
        {
            System.Drawing.Text.PrivateFontCollection pfcFonts = new System.Drawing.Text.PrivateFontCollection();
            string strFontPath = @"C:/Windows/Fonts/msyh.ttf";
            pfcFonts.AddFontFile(strFontPath);
            Style style = document.Styles["Normal"];
            style.Font = new MigraDoc.DocumentObjectModel.Font(pfcFonts.Families[0].Name, 12);
            style.Font.Color = Colors.Black;

            style = document.Styles["Heading1"];
            //style.Font.Name = "Tahoma";
            style.Font.Size = 20;
            style.Font.Bold = true;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            // style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 16;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;

            style = document.Styles["Heading3"];
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;

            style = document.Styles["Heading4"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 3;
            style.ParagraphFormat.SpaceAfter = 3;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            //TODO: Colors
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = pfcFonts.Families[0].Name;
            style.Font.Size = 12;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        public static Section AddTable<T>(Section section, IList<T> entity)
        {
            var ptable = section.AddTable();
            ptable.Style = "Table";
            ptable.Borders.Width = 0.25;
            ptable.Borders.Left.Width = 0.5;
            ptable.Borders.Right.Width = 0.5;
            ptable.Rows.LeftIndent = 0;
            var propertyList = TypeDescriptor.GetProperties(typeof(T), Attribute.GetCustomAttributes(typeof(T), false));

            for (var n = 0; n < propertyList.Count; n++)
            {
                ptable.AddColumn("3.4cm");
            }

            var rowHeader = ptable.AddRow();
            for (var i = 0; i < propertyList.Count; i++)
            {
                rowHeader.Cells[i].AddParagraph(propertyList[i].DisplayName);
            }

            for (var i = 0; i < entity.Count; i++)
            {
                var row = ptable.AddRow();
                for (int j = 0; j < propertyList.Count; j++)
                {
                    var valueTemp = propertyList[j].GetValue(entity[i]);
                    row.Cells[j].AddParagraph(valueTemp == null ? string.Empty : valueTemp.ToString());
                }
            }

            return section;
        }

        public ParagraphAlignment GetParagraphAlignment(int alignment)
        {
            ParagraphAlignment result = ParagraphAlignment.Left;
            if (alignment == 0)
                result = ParagraphAlignment.Center;
            if (alignment == 1)
                result = ParagraphAlignment.Right;
            return result;
        }

        //private Paragraph AddTextToCell(string instring, Cell cell, Unit fontsize, Document document)
        //{
        //    PdfDocument pdfd = new PdfDocument();
        //    PdfPage pg = pdfd.AddPage();
        //    XGraphics oGfx = XGraphics.FromPdfPage();
        //    Unit maxWidth = cell.Column.Width - (cell.Column.LeftPadding + cell.Column.RightPadding);
        //    Paragraph par;
        //    XFont font = new XFont(section.Styles["Table"].Font.Name, fontsize);
        //    if (oGfx.MeasureString(instring, font).Width < maxWidth.Value)
        //    {
        //        par = cell.AddParagraph(instring);
        //    }
        //    else // String does not fit - start the truncation process...
        //    {
        //        int stringlength = instring.Length;
        //        for (int i = 0; i < 3; i++)
        //        {
        //            if (oGfx.MeasureString(instring.Substring(0, stringlength) + '\u2026', font).Width > maxWidth.Value)
        //                stringlength -= (int)Math.Ceiling(instring.Length * Math.Pow(0.5f, i));
        //            else
        //                if (i < 2)
        //                    stringlength += (int)Math.Ceiling(instring.Length * Math.Pow(0.5f, i));
        //        }
        //        par = cell.AddParagraph(instring.Substring(0, stringlength) + '\u2026');
        //    }
        //    par.Format.Font.Size = fontsize;
        //    return par;
        //}
    }
}
