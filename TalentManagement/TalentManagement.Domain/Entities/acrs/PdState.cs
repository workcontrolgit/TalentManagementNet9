using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class PdState
    {
        public PdState()
        {
            PdTrackHistory = new HashSet<PdTrackHistory>();
            PositionDescription = new HashSet<PositionDescription>();
        }

        public string PdsStateCd { get; set; }
        public string PdsStateDesc { get; set; }
        public string PdsActiveInd { get; set; }
        public string PdsType { get; set; }
        public decimal PdsCreateId { get; set; }
        public DateTime PdsCreateDate { get; set; }
        public decimal? PdsUpdateId { get; set; }
        public DateTime PdsUpdateDate { get; set; }

        public virtual ICollection<PdTrackHistory> PdTrackHistory { get; set; }
        public virtual ICollection<PositionDescription> PositionDescription { get; set; }
    }
}
