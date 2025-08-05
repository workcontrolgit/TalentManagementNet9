using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class QuestionnairePosition
    {
        public QuestionnairePosition()
        {
            QuestionnairePositionAudit = new HashSet<QuestionnairePositionAudit>();
        }

        public decimal RpaSeqNum { get; set; }
        public string QrpDutyStationCode { get; set; }
        public string QrpTitle38EligibilityInd { get; set; }
        public string QrpTitle38PremiumPayCd { get; set; }
        public string PositionSensitivity { get; set; }
        public DateTime? PositionSensitivityEffDt { get; set; }
        public string SecurityClearance { get; set; }
        public string QrpFirearmsAmmoInd { get; set; }
        public string QrpLatenbergAmendInd { get; set; }
        public string WorkSchedule { get; set; }
        public decimal? QrpNumOfPositions { get; set; }
        public string QrpPositionTypeCode { get; set; }
        public decimal? StdHours { get; set; }
        public decimal? QrpMaxHeadCountNbr { get; set; }
        public string SalAdminPlan { get; set; }
        public string QrpPositionOccupiedCode { get; set; }
        public string GvtCompLevel { get; set; }
        public DateTime? QrpNteDate { get; set; }
        public string GvtSubAgency { get; set; }
        public string QrpFullTimeInd { get; set; }
        public decimal QrpCreateId { get; set; }
        public DateTime QrpCreateDate { get; set; }
        public decimal? QrpUpdateId { get; set; }
        public DateTime QrpUpdateDate { get; set; }
        public string PositionNbr { get; set; }
        public string BargUnit { get; set; }
        public string QrpOpmCertNbr { get; set; }
        public decimal? MonHrs { get; set; }
        public decimal? TueHrs { get; set; }
        public decimal? WedHrs { get; set; }
        public decimal? ThursHrs { get; set; }
        public decimal? FriHrs { get; set; }
        public decimal? SatHrs { get; set; }
        public decimal? SunHrs { get; set; }
        public string GvtCompArea { get; set; }
        public string CyberSecurity { get; set; }
        public string GmPublicTrust { get; set; }

        public virtual RequestPersonnelAction RpaSeqNumNavigation { get; set; }
        public virtual ICollection<QuestionnairePositionAudit> QuestionnairePositionAudit { get; set; }
    }
}
