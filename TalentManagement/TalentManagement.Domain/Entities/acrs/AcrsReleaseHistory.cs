using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class AcrsReleaseHistory
    {
        public string ArhVersionNumber { get; set; }
        public string ArhReleaseNotes { get; set; }
        public string ArhActiveInd { get; set; }
        public DateTime? ArhReleaseDate { get; set; }
        public decimal? ArhOrderBy { get; set; }
        public decimal ArhCreateId { get; set; }
        public DateTime ArhCreateDate { get; set; }
        public decimal? ArhUpdateId { get; set; }
        public DateTime? ArhUpdateDate { get; set; }
    }
}
