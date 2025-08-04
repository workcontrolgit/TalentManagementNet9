using System;
using System.Collections.Generic;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class QuestionnaireRecruit
    {
        public QuestionnaireRecruit()
        {
            QuestionnaireRecruitAudit = new HashSet<QuestionnaireRecruitAudit>();
        }

        public decimal RpaSeqNum { get; set; }
        public string QreObligatedPosInd { get; set; }
        public string QreIncWeatherInd { get; set; }
        public string QrePcsAuthorizedInd { get; set; }
        public string QrePreAppPhysicalReqInd { get; set; }
        public string QreAnnualPhysicalReqInd { get; set; }
        public string QreNewlyAuthPosnInd { get; set; }
        public DateTime? QreNewPosnAuthDate { get; set; }
        public string QreSurroundingLocInfoText { get; set; }
        public string QreMakeCommentInd { get; set; }
        public string QreCommentsText { get; set; }
        public string RecruitmentType { get; set; }
        public DateTime? RecruitmentTypeEffDt { get; set; }
        public string QreTeleworkEligibleInd { get; set; }
        public string QreReportToPosNbr { get; set; }
        public string QreUpwardMobilityInd { get; set; }
        public string QreRndDrugTestReqInd { get; set; }
        public string QreReasonForDrugTestCd { get; set; }
        public DateTime? QreReasonDrugTestEffDt { get; set; }
        public string QreUniformReqInd { get; set; }
        public decimal QreCreateId { get; set; }
        public DateTime QreCreateDate { get; set; }
        public decimal? QreUpdateId { get; set; }
        public DateTime QreUpdateDate { get; set; }

        public virtual RequestPersonnelAction RpaSeqNumNavigation { get; set; }
        public virtual ICollection<QuestionnaireRecruitAudit> QuestionnaireRecruitAudit { get; set; }
    }
}
