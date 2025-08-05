using System;

namespace TalentManagement.Domain.Entities
{
    public partial class QueTrackHistory
    {
        public decimal RpaSeqNum { get; set; }
        public DateTime QthDate { get; set; }
        public decimal QthSeqNum { get; set; }
        public string RpasStateCd { get; set; }
        public string QthActionType { get; set; }
        public decimal? QthAssignedOwner { get; set; }
        public string QthAssignedOwnerEmail { get; set; }
        public string QthCommentsText { get; set; }
        public string RoleCode { get; set; }
        public string QthReturnedFrom { get; set; }
        public decimal? QthReturnedFromHruId { get; set; }
        public decimal QthCreateId { get; set; }
        public DateTime QthCreateDate { get; set; }
        public decimal? QthUpdateId { get; set; }
        public DateTime QthUpdateDate { get; set; }

        public virtual RequestPersonnelAction RpaSeqNumNavigation { get; set; }
        public virtual RpaState RpasStateCdNavigation { get; set; }
    }
}
