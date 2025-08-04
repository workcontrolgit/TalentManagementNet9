using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class ErrMsgs
    {
        public string PdNbr { get; set; }
        public string ErrorLocation { get; set; }
        public string ErrorMsgs { get; set; }
        public decimal? ErrCreateId { get; set; }
        public DateTime? ErrCreateDate { get; set; }
        public decimal? ErrUpdateId { get; set; }
        public DateTime? ErrUpdateDate { get; set; }
        public decimal ErrSeqNum { get; set; }
        public DateTime? PdEffectiveDate { get; set; }
        public DateTime? GemsLastupddttm { get; set; }
    }
}
