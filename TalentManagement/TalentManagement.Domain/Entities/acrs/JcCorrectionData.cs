using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class JcCorrectionData
    {
        public string Jobcode { get; set; }
        public DateTime JcEffdt { get; set; }
        public decimal? PdSeqNum { get; set; }
        public string CreateId { get; set; }
        public DateTime? CreateDt { get; set; }
    }
}
