using System;

namespace Enterprise.Domain.Entities.Acrs
{
    public partial class Bureau
    {
        public string BurAbbr { get; set; }
        public string BurCode { get; set; }
        public string BurName { get; set; }
        public string BurMailboxName { get; set; }
        public string BurActiveInd { get; set; }
        public decimal? BurCreateId { get; set; }
        public DateTime? BurCreateDate { get; set; }
        public decimal? BurUpdateId { get; set; }
        public DateTime? BurUpdateDate { get; set; }
        public string BurSspInd { get; set; }
    }
}
