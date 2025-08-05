using System;

namespace TalentManagement.Domain.Entities
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
