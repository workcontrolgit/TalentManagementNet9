using System;
using System.Collections.Generic;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class AccessStatus
    {
        public AccessStatus()
        {
            AccessRequest = new HashSet<AccessRequest>();
        }

        public string AcsStatusCd { get; set; }
        public string AcsStatusDescr { get; set; }
        public string AcsActiveInd { get; set; }
        public decimal AcsCreateId { get; set; }
        public DateTime AcsCreateDate { get; set; }
        public decimal? AcsUpdateId { get; set; }
        public DateTime AcsUpdateDate { get; set; }

        public virtual ICollection<AccessRequest> AccessRequest { get; set; }
    }
}
