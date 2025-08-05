using System;

namespace TalentManagement.Domain.Entities
{
    public partial class ValidPdNbr
    {
        public string PsSourceCode { get; set; }
        public string PdNbr { get; set; }
        public string AvailableFlag { get; set; }
        public decimal? VprCreateId { get; set; }
        public DateTime? VprCreateDate { get; set; }
        public decimal? VprUpdateId { get; set; }
        public DateTime? VprUpdateDate { get; set; }
    }
}
