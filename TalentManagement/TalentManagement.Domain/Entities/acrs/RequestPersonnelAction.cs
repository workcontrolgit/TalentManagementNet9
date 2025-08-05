using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class RequestPersonnelAction
    {
        public RequestPersonnelAction()
        {
            PdRpa = new HashSet<PdRpa>();
            QueTrackHistory = new HashSet<QueTrackHistory>();
            RequestPersonnelActionAudit = new HashSet<RequestPersonnelActionAudit>();
            RpaCyberSecurity = new HashSet<RpaCyberSecurity>();
        }

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
        public string ProposedPositionNbr { get; set; }
        public string RpaTransType { get; set; }
        public decimal RpaEffectiveSeqNum { get; set; }
        public decimal? CrmTicketNbr { get; set; }
        public string PsSourceCode { get; set; }
        public DateTime? RpaOriginalEffdt { get; set; }
        public byte[] Guid { get; set; }

        public virtual RpaActionType RatCodeNavigation { get; set; }
        public virtual QuestionnairePosition QuestionnairePosition { get; set; }
        public virtual QuestionnaireRecruit QuestionnaireRecruit { get; set; }
        public virtual QuestionnaireRma QuestionnaireRma { get; set; }
        public virtual ICollection<PdRpa> PdRpa { get; set; }
        public virtual ICollection<QueTrackHistory> QueTrackHistory { get; set; }
        public virtual ICollection<RequestPersonnelActionAudit> RequestPersonnelActionAudit { get; set; }
        public virtual ICollection<RpaCyberSecurity> RpaCyberSecurity { get; set; }
    }
}
