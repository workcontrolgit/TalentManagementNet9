using System;

namespace TalentManagement.Domain.Entities
{
    public partial class AcrsOrgTitle
    {
        public decimal AotSeqNum { get; set; }
        public string AgpJobcode { get; set; }
        public decimal? PdSeqNum { get; set; }
        public string PdOrgTitleText { get; set; }
        public string GvtOrgTitleCd { get; set; }
        public string AotStatus { get; set; }
        public string AotGemsError { get; set; }
        public DateTime? AotGemsTransactionDate { get; set; }
        public string AotProcessedInd { get; set; }
        public decimal? AotCreateId { get; set; }
        public DateTime? AotCreateDate { get; set; }
        public decimal? AotUpdateId { get; set; }
        public DateTime? AotUpdateDate { get; set; }
        public string GvtOccSeries { get; set; }
        public DateTime? OrgTitleEffectiveDate { get; set; }
        public string GemsUserid { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
    }
}
