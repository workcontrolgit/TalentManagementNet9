namespace TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions
{
    public class GetPositionDescriptionsViewModel
    {
        public decimal PdSeqNum { get; set; }
        public string PdNbr { get; set; }
        public DateTime? PdEffectiveDate { get; set; }
        public string PdPositionTitleText { get; set; }
        public string PdOrgTitleText { get; set; }
        public string GvtOccSeries { get; set; }
        public string GvtPayPlan { get; set; }
        public string PdsStateCd { get; set; }
        public string GrdCode { get; set; }
        public string JobFunction { get; set; }
        public DateTime? PdClassifiedDate { get; set; }
        public string PdArchiveInd { get; set; }
        public string PdTransType { get; set; }
        public decimal PdEffectiveSeqNum { get; set; }
        public string PdReasonForSubmission { get; set; }
        public DateTime PdCreateDate { get; set; }
        public DateTime PdUpdateDate { get; set; }
    }
}