using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class AcrsPosnTitle
    {
        public decimal AptSeqNum { get; set; }
        public string AgpJobcode { get; set; }
        public decimal? PdSeqNum { get; set; }
        public string PdPositionTitleText { get; set; }
        public string GvtPosnTitleCd { get; set; }
        public string AptStatus { get; set; }
        public string AptGemsError { get; set; }
        public DateTime? AptGemsTransactionDate { get; set; }
        public string AptProcessedInd { get; set; }
        public decimal? AptCreateId { get; set; }
        public DateTime? AptCreateDate { get; set; }
        public decimal? AptUpdateId { get; set; }
        public DateTime? AptUpdateDate { get; set; }
        public string GvtOccSeries { get; set; }
        public DateTime? PositionTitleEffectiveDate { get; set; }
        public string GemsUserid { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
    }
}
