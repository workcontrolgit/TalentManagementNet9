using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class PdCareerLadderGrade
    {
        public string PclgGradeCode { get; set; }
        public decimal PclgFamilySeqNum { get; set; }
        public string PdNbr { get; set; }
        public decimal? PdSeqNum { get; set; }
        public string PclgUpdatedInd { get; set; }
        public string PclgStateCd { get; set; }
        public decimal PclgCreateId { get; set; }
        public DateTime PclgCreateDate { get; set; }
        public decimal? PclgUpdateId { get; set; }
        public DateTime PclgUpdateDate { get; set; }
        public string PclgHrSpecialistUpdatedInd { get; set; }
        public string PclgSupvUpdatedInd { get; set; }
        public string PclgClassifierUpdatedInd { get; set; }
        public decimal ClGroupId { get; set; }

        public virtual PositionDescription PdSeqNumNavigation { get; set; }
    }
}
