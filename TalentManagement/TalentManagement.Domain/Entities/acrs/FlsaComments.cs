using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class FlsaComments
    {
        public decimal PdSeqNum { get; set; }
        public string FlsaqFormName { get; set; }
        public string FlsacComments { get; set; }
        public decimal FlsacCreateId { get; set; }
        public DateTime FlsacCreateDate { get; set; }
        public decimal? FlsacUpdateId { get; set; }
        public DateTime? FlsacUpdateDate { get; set; }
    }
}
