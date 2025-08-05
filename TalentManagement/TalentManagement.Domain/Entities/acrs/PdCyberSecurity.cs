using System;

namespace TalentManagement.Domain.Entities
{
    public partial class PdCyberSecurity
    {
        public decimal PdSeqNum { get; set; }
        public string PdcsCybersecCode { get; set; }
        public decimal PdcsOrderNbr { get; set; }
        public decimal PdcsCreateId { get; set; }
        public DateTime PdcsCreateDate { get; set; }
        public decimal? PdcsUpdateId { get; set; }
        public DateTime? PdcsUpdateDate { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
    }
}
