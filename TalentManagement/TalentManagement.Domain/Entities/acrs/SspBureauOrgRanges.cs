using System;

namespace TalentManagement.Domain.Entities
{
    public partial class SspBureauOrgRanges
    {
        public decimal SborSeqNum { get; set; }
        public string SborServicingBureauAbbr { get; set; }
        public string SborServicingBureauCode { get; set; }
        public string SborServicingBureauName { get; set; }
        public string SborServicedBureauAbbr { get; set; }
        public string SborServicedBureauCode { get; set; }
        public string SborServicedBureauName { get; set; }
        public string SborSspStartOrgCd { get; set; }
        public string SborSspEndOrgCd { get; set; }
        public string SborSspActiveInd { get; set; }
        public decimal? SborCreateId { get; set; }
        public DateTime? SborCreateDate { get; set; }
        public decimal? SborUpdateId { get; set; }
        public DateTime? SborUpdateDate { get; set; }
    }
}
