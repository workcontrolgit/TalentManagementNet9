using System;

namespace TalentManagement.Domain.Entities
{
    public partial class RpaCyberSecurity
    {
        public decimal RpaSeqNum { get; set; }
        public string RpacsCybersecCode { get; set; }
        public decimal RpacsOrderNbr { get; set; }
        public decimal RpacsCreateId { get; set; }
        public DateTime RpacsCreateDate { get; set; }
        public decimal? RpacsUpdateId { get; set; }
        public DateTime? RpacsUpdateDate { get; set; }

        public virtual RequestPersonnelAction RpaSeqNumNavigation { get; set; }
    }
}
