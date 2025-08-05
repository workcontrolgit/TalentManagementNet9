using System;

namespace TalentManagement.Domain.Entities
{
    public partial class AccessRequest
    {
        public decimal ArSeqNum { get; set; }
        public decimal? HruId { get; set; }
        public string RlCode { get; set; }
        public string ArReasonText { get; set; }
        public string ArCommentText { get; set; }
        public string AcsStatusCd { get; set; }
        public decimal ArCreateId { get; set; }
        public DateTime ArCreateDate { get; set; }
        public decimal? ArUpdateId { get; set; }
        public DateTime ArUpdateDate { get; set; }

        public virtual AccessStatus AcsStatusCdNavigation { get; set; }
    }
}
