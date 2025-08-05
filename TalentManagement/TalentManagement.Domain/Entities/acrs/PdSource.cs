using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class PdSource
    {
        public PdSource()
        {
            PositionDescription = new HashSet<PositionDescription>();
        }

        public string PsSourceCode { get; set; }
        public string PsSourceDesc { get; set; }
        public string PsActiveInd { get; set; }
        public decimal PsCreateId { get; set; }
        public DateTime PsCreateDate { get; set; }
        public decimal? PsUpdateId { get; set; }
        public DateTime PsUpdateDate { get; set; }

        public virtual ICollection<PositionDescription> PositionDescription { get; set; }

        // Domain methods
        public bool IsActive() => PsActiveInd?.ToUpper() == "Y";
        public string GetCodeDescription() => $"{PsSourceCode}-{PsSourceDesc}";
    }
}
