using System;
using System.Collections.Generic;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class CsFactorsRef
    {
        public CsFactorsRef()
        {
            CsDetailRef = new HashSet<CsDetailRef>();
        }

        public string CsCode { get; set; }
        public string CsfrFactorName { get; set; }
        public string CsfrActiveInd { get; set; }
        public decimal CsfrCreateId { get; set; }
        public DateTime CsfrCreateDate { get; set; }
        public decimal? CsfrUpdateId { get; set; }
        public DateTime? CsfrUpdateDate { get; set; }

        public virtual ClassificationStandardRef CsCodeNavigation { get; set; }
        public virtual ICollection<CsDetailRef> CsDetailRef { get; set; }
    }
}
