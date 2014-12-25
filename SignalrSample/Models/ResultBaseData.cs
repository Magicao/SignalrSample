namespace SignalrSample.Models
{
    using System.Collections.Generic;

    public class ResultBase<TSubModel> : ResultBase
    {
        public IEnumerable<TSubModel> DataList { get; set; }
        public TSubModel SubModel { get; set; }
    }
}