using System;

namespace TalentManagement.Domain.Entities
{
    public partial class ContactDetail
    {
        public decimal HruId { get; set; }
        public string CdPhoneNbr { get; set; }
        public string CdFaxNbr { get; set; }
        public decimal CdCreateId { get; set; }
        public DateTime CdCreateDate { get; set; }
        public decimal? CdUpdateId { get; set; }
        public DateTime CdUpdateDate { get; set; }
    }
}
