using System;
using System.Collections.Generic;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class PdDutySubjectRef
    {
        public PdDutySubjectRef()
        {
            PdDuties = new HashSet<PdDuties>();
        }

        public decimal PddsrSeqNum { get; set; }
        public string PddsrMajorDutyName { get; set; }
        public string PddsrActiveInd { get; set; }
        public decimal PddsrCreateId { get; set; }
        public DateTime PddsrCreateDate { get; set; }
        public decimal? PddsrUpdateId { get; set; }
        public DateTime? PddsrUpdateDate { get; set; }

        public virtual ICollection<PdDuties> PdDuties { get; set; }
    }
}
