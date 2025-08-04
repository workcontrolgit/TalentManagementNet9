using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class OccSeriesFuncCdRef
    {
        public string OsfcOccSeries { get; set; }
        public string OsfcFuncCdFlag { get; set; }
        public decimal OsfcCreateId { get; set; }
        public DateTime OsfcCreateDate { get; set; }
        public decimal? OsfcUpdateId { get; set; }
        public DateTime? OsfcUpdateDate { get; set; }
    }
}
