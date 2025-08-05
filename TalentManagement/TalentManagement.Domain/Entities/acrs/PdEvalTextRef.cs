using System;

namespace TalentManagement.Domain.Entities
{
    public partial class PdEvalTextRef
    {
        public decimal CsDetailSeqNum { get; set; }
        public string OccSeries { get; set; }
        public string PdetrText { get; set; }
        public decimal PdetrCreateId { get; set; }
        public DateTime PdetrCreateDate { get; set; }
        public decimal? PdetrUpdateId { get; set; }
        public DateTime? PdetrUpdateDate { get; set; }
    }
}
