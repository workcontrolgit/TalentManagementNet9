using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class PdDuties
    {
        public decimal PddSeqNum { get; set; }
        public decimal? PdSeqNum { get; set; }
        public decimal PddsrSeqNum { get; set; }
        public decimal? PddPercentTimeSpent { get; set; }
        public string PddCriticalDutyInd { get; set; }
        public decimal PddCreateId { get; set; }
        public DateTime PddCreateDate { get; set; }
        public decimal? PddUpdateId { get; set; }
        public DateTime PddUpdateDate { get; set; }
        public string PddMajorDutiesText { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
        public virtual PdDutySubjectRef PddsrSeqNumNavigation { get; set; }
    }
}
