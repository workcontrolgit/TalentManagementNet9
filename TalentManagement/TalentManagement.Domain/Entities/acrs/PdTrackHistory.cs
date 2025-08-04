using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class PdTrackHistory
    {
        public decimal PdSeqNum { get; set; }
        public DateTime PdthDate { get; set; }
        public decimal PdthSeqNum { get; set; }
        public string PdsStateCd { get; set; }
        public decimal? PdthAssignedOwner { get; set; }
        public string PdthAssignedOwnerEmail { get; set; }
        public string PdthCommentsText { get; set; }
        public string RoleCode { get; set; }
        public string PdthActionType { get; set; }
        public string PdthReturnedFrom { get; set; }
        public decimal? PdthReturnedFromHruId { get; set; }
        public decimal PdthCreateId { get; set; }
        public DateTime PdthCreateDate { get; set; }
        public decimal? PdthUpdateId { get; set; }
        public DateTime PdthUpdateDate { get; set; }
        public string PdthLegacyName { get; set; }
        public DateTime? PdthLegacyDate { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
        public virtual PdState PdsStateCdNavigation { get; set; }
    }
}
