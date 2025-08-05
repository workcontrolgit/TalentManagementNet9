using System;

namespace TalentManagement.Domain.Entities
{
    public partial class AcrsTracking
    {
        public decimal AtId { get; set; }
        public decimal? ActivityId { get; set; }
        public decimal? ActionId { get; set; }
        public byte[] Guid { get; set; }
        public int? HruIdCreatedBy { get; set; }
        public int? HruIdAssignedTo { get; set; }
        public byte? WtId { get; set; }
        public byte? UgId { get; set; }
        public DateTime AtCreatedDate { get; set; }
        public string AtComments { get; set; }
        public decimal? HruIdUpdatedBy { get; set; }
        public DateTime? AtUpdateDate { get; set; }
    }
}
