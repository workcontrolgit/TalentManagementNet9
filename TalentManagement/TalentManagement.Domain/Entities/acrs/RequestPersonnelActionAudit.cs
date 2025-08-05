using System;

namespace TalentManagement.Domain.Entities
{
    public partial class RequestPersonnelActionAudit
    {
        public decimal RpaaSeqNum { get; set; }
        public decimal RpaSeqNum { get; set; }
        public decimal? RpaNbr { get; set; }
        public DateTime? RpaEffectiveDate { get; set; }
        public string RatCode { get; set; }
        public decimal? RpaActionRequestByHruId { get; set; }
        public decimal? RpaActionAuthByHruId { get; set; }
        public decimal? RpaPocHruId { get; set; }
        public DateTime? RpaProposedEffectiveDate { get; set; }
        public DateTime? RpaRequestDate { get; set; }
        public DateTime? RpaConcurrenceDate { get; set; }
        public string RpasStateCd { get; set; }
        public string RpaCommentsText { get; set; }
        public string RpaPosnOrgCode { get; set; }
        public string PositionNbr { get; set; }
        public decimal? QthAssignedOwner { get; set; }
        public string RpasStateDesc { get; set; }
        public decimal RpaCreateId { get; set; }
        public DateTime RpaCreateDate { get; set; }
        public decimal? RpaUpdateId { get; set; }
        public DateTime RpaUpdateDate { get; set; }
        public DateTime RpaaCreateDate { get; set; }
        public string ProposedPositionNbr { get; set; }
        public string RpaTransType { get; set; }
        public decimal RpaEffectiveSeqNum { get; set; }
        public decimal? CrmTicketNbr { get; set; }
        public string PsSourceCode { get; set; }

        public virtual RequestPersonnelAction RpaSeqNumNavigation { get; set; }
    }
}
