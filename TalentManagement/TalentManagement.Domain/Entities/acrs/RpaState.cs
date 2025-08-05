using System;
using System.Collections.Generic;

namespace TalentManagement.Domain.Entities
{
    public partial class RpaState
    {
        public RpaState()
        {
            QueTrackHistory = new HashSet<QueTrackHistory>();
        }

        public string RpasStateCd { get; set; }
        public string RpasStateDesc { get; set; }
        public string RpasActiveInd { get; set; }
        public string RpasType { get; set; }
        public decimal RpasCreateId { get; set; }
        public DateTime RpasCreateDate { get; set; }
        public decimal? RpasUpdateId { get; set; }
        public DateTime RpasUpdateDate { get; set; }

        public virtual ICollection<QueTrackHistory> QueTrackHistory { get; set; }
    }
}
