using System;

namespace TalentManagement.Domain.Entities
{
    public partial class PdRpa
    {
        public decimal PdSeqNum { get; set; }
        public decimal RpaSeqNum { get; set; }
        public decimal PdrCreateId { get; set; }
        public DateTime PdrCreateDate { get; set; }
        public decimal? PdrUpdateId { get; set; }
        public DateTime PdrUpdateDate { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
        public virtual RequestPersonnelAction RpaSeqNumNavigation { get; set; }
    }
}
