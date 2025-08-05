using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class FlsaForm
    {
        public FlsaForm()
        {
            FlsaQuestionsRef = new HashSet<FlsaQuestionsRef>();
        }

        public string FlsafFormName { get; set; }
        public DateTime? FlsafFormEffdt { get; set; }
        public decimal FlsafCreateId { get; set; }
        public DateTime FlsafCreateDate { get; set; }
        public decimal? FlsafUpdateId { get; set; }
        public DateTime? FlsafUpdateDate { get; set; }

        public virtual ICollection<FlsaQuestionsRef> FlsaQuestionsRef { get; set; }
    }
}
