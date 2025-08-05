using System;

namespace TalentManagement.Domain.Entities
{
    public partial class DeletePd
    {
        public string PdNbr { get; set; }
        public string DpdDeleteFlag { get; set; }
        public decimal? DpdCreateId { get; set; }
        public DateTime? DpdCreateDate { get; set; }
        public DateTime? DpdDeletedDate { get; set; }
        public decimal? DpdUpdateId { get; set; }
        public DateTime? DpdUpdateDate { get; set; }
        public decimal? PdSeqNum { get; set; }
    }
}
