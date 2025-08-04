using System;
using System.Collections.Generic;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class CsDetailRef
    {
        public CsDetailRef()
        {
            PdEvalFactors = new HashSet<PdEvalFactors>();
        }

        public decimal CsDetailSeqNum { get; set; }
        public string CsCode { get; set; }
        public string CsfrFactorName { get; set; }
        public string CsdrLevel { get; set; }
        public byte? CsdrFactorScoreNbr { get; set; }
        public string CsdrActiveInd { get; set; }
        public decimal CsdrCreateId { get; set; }
        public DateTime CsdrCreateDate { get; set; }
        public decimal? CsdrUpdateId { get; set; }
        public DateTime? CsdrUpdateDate { get; set; }

        public virtual CsFactorsRef Cs { get; set; }
        public virtual ICollection<PdEvalFactors> PdEvalFactors { get; set; }
    }
}
