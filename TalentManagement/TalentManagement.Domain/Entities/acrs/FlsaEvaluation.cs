using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class FlsaEvaluation
    {
        public decimal PdSeqNum { get; set; }
        public decimal FlsaqSeqNum { get; set; }
        public string FlsaResponseInd { get; set; }
        public string FlsaProfExeOther { get; set; }
        public string FlsaAdminComments { get; set; }
        public string FlsaCommentsText { get; set; }
        public string FlsaqFormName { get; set; }
        public decimal FlsaCreateId { get; set; }
        public DateTime FlsaCreateDate { get; set; }
        public decimal? FlsaUpdateId { get; set; }
        public DateTime FlsaUpdateDate { get; set; }

        public virtual FlsaQuestionsRef Flsaq { get; set; }
    }
}
