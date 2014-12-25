using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;

namespace SignalrSample
{
    public class TextSharpHelper
    {
        public static PdfPTable CreateTable<T>(IList<T> entity)
        {
            var propertyList = TypeDescriptor.GetProperties(typeof(T), Attribute.GetCustomAttributes(typeof(T), false));
            var table = new PdfPTable(propertyList.Count);

            for (int i = 0; i < propertyList.Count; i++)
            {
                table.AddCell(propertyList[i].DisplayName);
            }

            for (var i = 0; i < entity.Count; i++)
            {
                for (int j = 0; j < propertyList.Count; j++)
                {
                    var valueTemp = propertyList[j].GetValue(entity[i]);
                    table.AddCell(valueTemp == null ? string.Empty : valueTemp.ToString());
                }
            }

            return table;
        }
    }
}