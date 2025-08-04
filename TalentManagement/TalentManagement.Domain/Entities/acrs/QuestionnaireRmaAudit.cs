using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class QuestionnaireRmaAudit
    {
        public decimal QrmaSeqNum { get; set; }
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
        public DateTime QrmaCreateDate { get; set; }
        public decimal QrmCreateId { get; set; }
        public DateTime QrmCreateDate { get; set; }
        public decimal? QrmUpdateId { get; set; }
        public DateTime QrmUpdateDate { get; set; }
        public string QrmWorkingCapFundPosInd { get; set; }
        public string QrmOpAllowance { get; set; }
        public string QrmProject { get; set; }
        public string QrmObligationNo { get; set; }

        public virtual QuestionnaireRma RpaSeqNumNavigation { get; set; }
    }
}
