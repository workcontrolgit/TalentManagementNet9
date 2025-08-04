using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class PdEvalFactors
    {
        public decimal PdSeqNum { get; set; }
        public decimal CsDetailSeqNum { get; set; }
        public string PdefEvaluationText { get; set; }
        public decimal PdefCreateId { get; set; }
        public DateTime PdefCreateDate { get; set; }
        public decimal? PdefUpdateId { get; set; }
        public DateTime PdefUpdateDate { get; set; }

        public virtual CsDetailRef CsDetailSeqNumNavigation { get; set; }
        public virtual PositionDescription PdSeqNumNavigation { get; set; }
    }
}
