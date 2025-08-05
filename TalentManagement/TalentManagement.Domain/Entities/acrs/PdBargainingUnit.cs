using System;

namespace TalentManagement.Domain.Entities
{
    public partial class PdBargainingUnit
    {
        public decimal PdSeqNum { get; set; }
        public string PdbuBargUnitCode { get; set; }
        public decimal PdbuCreateId { get; set; }
        public DateTime PdbuCreateDate { get; set; }
        public decimal? PdbuUpdateId { get; set; }
        public DateTime? PdbuUpdateDate { get; set; }
    }
}
