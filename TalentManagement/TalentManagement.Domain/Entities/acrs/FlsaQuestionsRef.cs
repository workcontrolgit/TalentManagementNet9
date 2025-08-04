using System;
using System.Collections.Generic;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class FlsaQuestionsRef
    {
        public FlsaQuestionsRef()
        {
            FlsaEvaluation = new HashSet<FlsaEvaluation>();
        }

        public decimal FlsaqSeqNum { get; set; }
        public string FlsaqSection { get; set; }
        public string FlsaqSectionName { get; set; }
        public string FlsaqQuestionId { get; set; }
        public string FlsaqQuestionText { get; set; }
        public string FlsaqActiveInd { get; set; }
        public string FlsaqFormName { get; set; }
        public decimal FlsaqCreateId { get; set; }
        public DateTime FlsaqCreateDate { get; set; }
        public decimal? FlsaqUpdateId { get; set; }
        public DateTime FlsaqUpdateDate { get; set; }

        public virtual FlsaForm FlsaqFormNameNavigation { get; set; }
        public virtual ICollection<FlsaEvaluation> FlsaEvaluation { get; set; }
    }
}
