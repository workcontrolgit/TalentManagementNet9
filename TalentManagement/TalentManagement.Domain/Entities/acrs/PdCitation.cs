using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class PdCitation
    {
        public decimal PdSeqNum { get; set; }
        public string CitationCd { get; set; }
        public DateTime CitationEffDt { get; set; }
        public decimal PdcCreateId { get; set; }
        public DateTime PdcCreateDate { get; set; }
        public decimal? PdcUpdateId { get; set; }
        public DateTime? PdcUpdateDate { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
    }
}
