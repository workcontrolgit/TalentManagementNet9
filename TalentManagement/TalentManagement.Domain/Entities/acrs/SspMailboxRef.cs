using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class SspMailboxRef
    {
        public string CorSeqNum { get; set; }
        public string SmfMailboxName { get; set; }
        public string SmfSspName { get; set; }
        public decimal SmfCreateId { get; set; }
        public DateTime? SmfCreateDate { get; set; }
        public decimal? SmfUpdateId { get; set; }
        public DateTime? SmfUpdateDate { get; set; }
    }
}
