using System;

namespace TalentManagement.Domain.Entities
{
    public partial class AcrsReleaseNotes
    {
        public string ArnSectionName { get; set; }
        public string ArnSectionContent { get; set; }
        public string ArnActiveInd { get; set; }
        public decimal? ArnOrderBy { get; set; }
        public decimal ArnCreateId { get; set; }
        public DateTime ArnCreateDate { get; set; }
        public decimal? ArnUpdateId { get; set; }
        public DateTime? ArnUpdateDate { get; set; }
    }
}
