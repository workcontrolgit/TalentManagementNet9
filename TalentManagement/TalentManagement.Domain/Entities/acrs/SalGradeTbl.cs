using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class SalGradeTbl
    {
        public string Grade { get; set; }
        public string GradeDescrShort { get; set; }
        public string GradeDescr { get; set; }
        public DateTime? GradeEffdt { get; set; }
        public string PayPlan { get; set; }
        public string SalAdminPlan { get; set; }
        public decimal? SgCreateId { get; set; }
        public DateTime? SgCreateDate { get; set; }
        public decimal? SgUpdateId { get; set; }
        public DateTime? SgUpdateDate { get; set; }
    }
}
