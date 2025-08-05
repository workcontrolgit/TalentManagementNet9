using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class PositionDescription
    {
        public PositionDescription()
        {
            PositionDescriptionAudit = new HashSet<PositionDescriptionAudit>();
        }

        public decimal PdSeqNum { get; set; }
        public string PdNbr { get; set; }
        public DateTime? PdEffectiveDate { get; set; }
        public string PdCareerLadderInd { get; set; }
        public string PdTargetGradeCode { get; set; }
        public string PdFlsaInd { get; set; }
        public string PdFinDisclosureInd { get; set; }
        public string PdClassifierCommentsText { get; set; }
        public string PdArchiveInd { get; set; }
        public string PdOldGemsPdNbr { get; set; }
        public DateTime? PdClassifiedDate { get; set; }
        public decimal? PdClassifierHruId { get; set; }
        public string GrdCode { get; set; }
        public string JobFunction { get; set; }
        public DateTime? JobFunctionEffDt { get; set; }
        public string PdsStateCd { get; set; }
        public string GvtOccSeries { get; set; }
        public string GvtPayPlan { get; set; }
        public string GvtOrgTitleCd { get; set; }
        public DateTime? GvtOrgTitleEffDt { get; set; }
        public string GvtPosnTitleCd { get; set; }
        public DateTime? GvtPosnTitleEffDt { get; set; }
        public string PdPositionTitleText { get; set; }
        public string PdOrgTitleText { get; set; }
        public string SkillCd1 { get; set; }
        public string PsSourceCode { get; set; }
        public string PdReplacePdNum { get; set; }
        public string PdManagerLevel { get; set; }
        public string PdAcquisitionPosInd { get; set; }
        public string PdLimitationsOfUse { get; set; }
        public string PdOriginOrgCode { get; set; }
        public decimal? PdthAssignedOwner { get; set; }
        public string PdFlsaUrlText { get; set; }
        public string PdLeoType { get; set; }
        public string PdBargUnitCode { get; set; }
        public string PdOldGemsJobcode { get; set; }
        public decimal PdCreateId { get; set; }
        public DateTime PdCreateDate { get; set; }
        public decimal? PdUpdateId { get; set; }
        public DateTime PdUpdateDate { get; set; }
        public string PdEvalStatement { get; set; }
        public string PdTransType { get; set; }
        public decimal PdEffectiveSeqNum { get; set; }
        public string PdReasonForSubmission { get; set; }
        public string PdIaAction { get; set; }
        public string PdClpdGradeInterval { get; set; }
        public decimal? PdIaLimit { get; set; }
        public decimal? CrmTicketNum { get; set; }
        public string PdInterdisciplinaryInd { get; set; }
        public string PdExplantnComments { get; set; }
        public string PdIpop { get; set; }
        public string PdIntro { get; set; }
        public DateTime? PdOriginalEffdt { get; set; }
        public byte[] Guid { get; set; }

        public virtual ICollection<PositionDescriptionAudit> PositionDescriptionAudit { get; set; }
    }
}
