using System;

namespace TalentManagement.Domain.Entities
{
    public partial class GradeConversionRef
    {
        public string CsCode { get; set; }
        public string GrdCode { get; set; }
        public byte? GcrMinPoints { get; set; }
        public byte? GcrMaxPoints { get; set; }
        public string GcrActiveInd { get; set; }
        public decimal GcrCreateId { get; set; }
        public DateTime GcrCreateDate { get; set; }
        public decimal? GcrUpdateId { get; set; }
        public DateTime? GcrUpdateDate { get; set; }

        public virtual ClassificationStandardRef CsCodeNavigation { get; set; }
    }
}
