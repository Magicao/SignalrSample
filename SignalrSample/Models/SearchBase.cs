using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalrSample.Models
{
    public class SearchBase
    {
        public int Take { get; set; }

        public int Skip { get; set; }

        public IEnumerable<Sort> Sort { get; set; }

        public Filter Filter { get; set; }

        public IEnumerable<Aggregator> Aggregate  { get; set; }

        public int TotailRecords { get; set; }
    }
}