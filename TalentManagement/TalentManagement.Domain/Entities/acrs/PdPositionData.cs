using System;

namespace TalentManagement.Domain.Entities
{
    public partial class PdPositionData
    {
        public decimal PdSeqNum { get; set; }
        public string SecurityClearance { get; set; }
        public string PositionSensitivity { get; set; }
        public string GvtCompArea { get; set; }
        public string GvtCompLevel { get; set; }
        public string QrpPositionOccupiedCode { get; set; }
        public string QrpDutyStationCode { get; set; }
        public decimal PdpdCreateId { get; set; }
        public DateTime PdpdCreateDate { get; set; }
        public decimal? PdpdUpdateId { get; set; }
        public DateTime? PdpdUpdateDate { get; set; }
        public string GvtOpmCertNbr { get; set; }
        public string GmPublicTrust { get; set; }
    }
}
