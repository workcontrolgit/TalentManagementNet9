using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class QuestionnaireRma
    {
        public QuestionnaireRma()
        {
            QuestionnaireRmaAudit = new HashSet<QuestionnaireRmaAudit>();
        }

        public decimal RpaSeqNum { get; set; }
        public string QrmSalaryExpensePosInd { get; set; }
        public string QrmBureauFundPosInd { get; set; }
        public string QrmSepApprFundPosInd { get; set; }
        public string QrmAllocationNbr { get; set; }
        public string QrmAllotmentNbr { get; set; }
        public string QrmFunctionCode { get; set; }
        public string QrmObjectCode { get; set; }
        public string QrmIcassPosnInd { get; set; }
        public string QrmFundType { get; set; }
        public decimal QrmCreateId { get; set; }
        public DateTime QrmCreateDate { get; set; }
        public decimal? QrmUpdateId { get; set; }
        public DateTime QrmUpdateDate { get; set; }
        public string QrmWorkingCapFundPosInd { get; set; }
        public string QrmOpAllowance { get; set; }
        public string QrmProject { get; set; }
        public string QrmObligationNo { get; set; }
        public string DsBbfy { get; set; }
        public string DsEbfy { get; set; }
        public string DsDivBureau { get; set; }

        public virtual RequestPersonnelAction RpaSeqNumNavigation { get; set; }
        public virtual ICollection<QuestionnaireRmaAudit> QuestionnaireRmaAudit { get; set; }
    }
}
