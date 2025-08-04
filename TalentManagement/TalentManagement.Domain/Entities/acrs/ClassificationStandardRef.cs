using System;
using System.Collections.Generic;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class ClassificationStandardRef
    {
        public ClassificationStandardRef()
        {
            CsFactorsRef = new HashSet<CsFactorsRef>();
            GradeConversionRef = new HashSet<GradeConversionRef>();
        }

        public string CsCode { get; set; }
        public string CsName { get; set; }
        public string CsActiveInd { get; set; }
        public int CsCreateId { get; set; }
        public DateTime CsCreateDate { get; set; }
        public int? CsUpdateId { get; set; }
        public DateTime? CsUpdateDate { get; set; }

        public virtual ICollection<CsFactorsRef> CsFactorsRef { get; set; }
        public virtual ICollection<GradeConversionRef> GradeConversionRef { get; set; }
    }
}
