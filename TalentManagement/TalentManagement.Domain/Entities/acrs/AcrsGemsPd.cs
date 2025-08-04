using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class AcrsGemsPd
    {
        public decimal? AgpSeqNum { get; set; }
        public string AgpJobcode { get; set; }
        public DateTime AgpJobcodeEffectiveDate { get; set; }
        public string GvtOrgTitleCd { get; set; }
        public string GvtPosnTitleCd { get; set; }
        public string SalAdminPlan { get; set; }
        public string GradeCode { get; set; }
        public string BargUnit { get; set; }
        public decimal? StdHours { get; set; }
        public string RegTemp { get; set; }
        public string PdFlsaInd { get; set; }
        public string GvtPayPlan { get; set; }
        public string GvtOccSeries { get; set; }
        public string PdClassifierHruId { get; set; }
        public DateTime? PdClassifiedDate { get; set; }
        public string PdTargetGradeCode { get; set; }
        public string SkillCd1 { get; set; }
        public string PdCareerLadderInd { get; set; }
        public string PdManagerLevel { get; set; }
        public string PdLeoType { get; set; }
        public string AgpProcessedInd { get; set; }
        public DateTime? AgpProcessedDate { get; set; }
        public string AgpComments { get; set; }
        public string AgpGemsError { get; set; }
        public string PdFinDisclosureInd { get; set; }
        public decimal? PclgFamilySeqNum { get; set; }
        public string AgpTransType { get; set; }
        public string GemsUserid { get; set; }
        public DateTime? AgpGemsTransactionDate { get; set; }
        public string PdClassifierEmplid { get; set; }
        public decimal? PdSeqNum { get; set; }
        public string SkillCd2 { get; set; }
        public decimal AgpCreateId { get; set; }
        public DateTime AgpCreateDate { get; set; }
        public decimal? AgpUpdateId { get; set; }
        public DateTime? AgpUpdateDate { get; set; }
        public DateTime? AgpSupvAppvDate { get; set; }
        public string AgpSupvAppEmplid { get; set; }
        public string PdIaAction { get; set; }
        public decimal? PdIaLimit { get; set; }
        public string PdReasonForSubmission { get; set; }
        public string ActionReason { get; set; }
        public string GmIdpd { get; set; }
        public string PdIpop { get; set; }
        public string DeptId { get; set; }
        public string GvtPosnSensCd { get; set; }
        public string GmCybersecCd1 { get; set; }
        public string GmCybersecCd2 { get; set; }
        public string GmCybersecCd3 { get; set; }
        public string GvtOpmCertNbr { get; set; }
        public string GmPublicTrust { get; set; }
        public string JobFunction { get; set; }
    }
}
