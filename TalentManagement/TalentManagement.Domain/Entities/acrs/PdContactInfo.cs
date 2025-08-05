using System;

namespace TalentManagement.Domain.Entities
{
    public partial class PdContactInfo
    {
        public decimal PdSeqNum { get; set; }
        public decimal? PdciPdReqHruId { get; set; }
        public decimal? PdciPdAuthHruId { get; set; }
        public decimal? PdciPdAddInfoHruId { get; set; }
        public string PdciIaAction { get; set; }
        public decimal PdciCreateId { get; set; }
        public DateTime PdciCreateDate { get; set; }
        public decimal? PdciUpdateId { get; set; }
        public DateTime? PdciUpdateDate { get; set; }
    }
}
