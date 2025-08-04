using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class AcrsGemsPdOccSeries
    {
        public decimal? AgpSeqNum { get; set; }
        public decimal PdSeqNum { get; set; }
        public string PdocOccSeries { get; set; }
        public decimal PdocCreateId { get; set; }
        public DateTime PdocCreateDate { get; set; }
        public decimal? PdocUpdateId { get; set; }
        public DateTime? PdocUpdateDate { get; set; }
    }
}
