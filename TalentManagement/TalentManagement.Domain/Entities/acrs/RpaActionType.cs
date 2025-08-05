using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class RpaActionType
    {
        public RpaActionType()
        {
            RequestPersonnelAction = new HashSet<RequestPersonnelAction>();
        }

        public string RatCode { get; set; }
        public string RatDesc { get; set; }
        public string RatActiveInd { get; set; }
        public decimal RatCreateId { get; set; }
        public DateTime RatCreateDate { get; set; }
        public decimal? RatUpdateId { get; set; }
        public DateTime RatUpdateDate { get; set; }

        public virtual ICollection<RequestPersonnelAction> RequestPersonnelAction { get; set; }
    }
}
