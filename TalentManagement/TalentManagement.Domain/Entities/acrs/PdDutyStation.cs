using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class PdDutyStation
    {
        public decimal PdSeqNum { get; set; }
        public string PddsDutyStationCode { get; set; }
        public decimal PddsCreateId { get; set; }
        public DateTime PddsCreateDate { get; set; }
        public decimal? PddsUpdateId { get; set; }
        public DateTime? PddsUpdateDate { get; set; }
    }
}
